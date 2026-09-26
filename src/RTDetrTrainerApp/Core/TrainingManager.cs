using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RTDetrTrainerApp.Core
{
    /// <summary>
    /// auto_train.py を呼び出す非同期プロセス管理クラス
    /// PYTHONUNBUFFERED=1 を指定したリアルタイムログ取得、子プロセスツリーを含む確実なキャンセルをサポート
    /// </summary>
    public class TrainingManager : IDisposable
    {
        private Process? _currentProcess;
        private readonly object _lock = new();
        private bool _disposed;

        public bool IsRunning
        {
            get
            {
                lock (_lock)
                {
                    return _currentProcess != null && !_currentProcess.HasExited;
                }
            }
        }

        /// <summary>
        /// ハードウェアスペック診断モード (--mode diagnose) を実行
        /// </summary>
        public async Task<HardwareDiagnosisResult?> RunDiagnoseAsync(
            string pythonExe,
            string autoTrainScript,
            IProgress<string>? progress,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(pythonExe))
            {
                throw new ArgumentException("Python実行ファイルのパスが指定されていません。", nameof(pythonExe));
            }
            if (string.IsNullOrWhiteSpace(autoTrainScript) || !File.Exists(autoTrainScript))
            {
                throw new FileNotFoundException("auto_train.py スクリプトが見つかりません。", autoTrainScript);
            }

            string arguments = $"\"{autoTrainScript}\" --mode diagnose";
            string workingDir = Path.GetDirectoryName(autoTrainScript) ?? AppDomain.CurrentDomain.BaseDirectory;

            string? detectedSpecJson = null;
            var jsonRegex = new Regex(@"\[SPEC_JSON\](.*?)\[/SPEC_JSON\]", RegexOptions.Singleline);

            var customProgress = new Progress<string>(line =>
            {
                var match = jsonRegex.Match(line);
                if (match.Success)
                {
                    detectedSpecJson = match.Groups[1].Value;
                }
                progress?.Report(line);
            });

            int exitCode = await RunProcessAsync(pythonExe, arguments, workingDir, customProgress, ct);

            if (exitCode == 0 && !string.IsNullOrWhiteSpace(detectedSpecJson))
            {
                try
                {
                    return JsonSerializer.Deserialize<HardwareDiagnosisResult>(detectedSpecJson);
                }
                catch (Exception ex)
                {
                    progress?.Report($"[ERROR] 診断JSONのデシリアライズに失敗しました: {ex.Message}");
                }
            }

            return null;
        }

        /// <summary>
        /// 学習・エクスポートパイプライン (--mode train) を実行
        /// </summary>
        public async Task<int> RunTrainAsync(
            TrainParameters p,
            IProgress<string>? progress,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(p.PythonExecutablePath))
            {
                throw new ArgumentException("Python実行ファイルのパスが指定されていません。");
            }
            if (string.IsNullOrWhiteSpace(p.AutoTrainScriptPath) || !File.Exists(p.AutoTrainScriptPath))
            {
                throw new FileNotFoundException("auto_train.py スクリプトが見つかりません。", p.AutoTrainScriptPath);
            }

            var argsBuilder = new StringBuilder();
            argsBuilder.Append($"\"{p.AutoTrainScriptPath}\" --mode train ");
            argsBuilder.Append($"--rtdetr-root \"{p.RtdetrRoot}\" ");
            argsBuilder.Append($"--config \"{p.ConfigFile}\" ");
            argsBuilder.Append($"--dataset-dir \"{p.DatasetDir}\" ");
            if (!string.IsNullOrWhiteSpace(p.OutputDir))
            {
                argsBuilder.Append($"--output-dir \"{p.OutputDir}\" ");
            }
            argsBuilder.Append($"--deploy-dir \"{p.DeployDir}\" ");
            argsBuilder.Append($"--epochs {p.Epochs} ");
            argsBuilder.Append($"--batch-size {p.BatchSize} ");
            argsBuilder.Append($"--num-workers {p.NumWorkers} ");
            argsBuilder.Append($"--input-size {p.InputSize} ");

            if (p.UseAmp)
            {
                argsBuilder.Append("--use-amp ");
            }

            if (!string.IsNullOrWhiteSpace(p.TuningWeightPath))
            {
                argsBuilder.Append($"--tuning \"{p.TuningWeightPath}\" ");
            }
            else if (!string.IsNullOrWhiteSpace(p.ResumeWeightPath))
            {
                argsBuilder.Append($"--resume \"{p.ResumeWeightPath}\" ");
            }

            if (p.IsDummy)
            {
                argsBuilder.Append("--dummy ");
            }

            string workingDir = Path.GetDirectoryName(p.AutoTrainScriptPath) ?? AppDomain.CurrentDomain.BaseDirectory;

            return await RunProcessAsync(
                p.PythonExecutablePath,
                argsBuilder.ToString().Trim(),
                workingDir,
                progress,
                ct);
        }

        /// <summary>
        /// 内部共通の非同期プロセス実行ロジック
        /// </summary>
        private async Task<int> RunProcessAsync(
            string fileName,
            string arguments,
            string workingDirectory,
            IProgress<string>? progress,
            CancellationToken ct)
        {
            lock (_lock)
            {
                if (_currentProcess != null && !_currentProcess.HasExited)
                {
                    throw new InvalidOperationException("現在すでに別のプロセスが実行中です。");
                }
            }

            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            // 標準出力がバッファリングされずにリアルタイム出力されるよう設定
            psi.EnvironmentVariables["PYTHONUNBUFFERED"] = "1";
            psi.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";

            var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            lock (_lock)
            {
                _currentProcess = process;
            }

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data != null)
                {
                    progress?.Report(e.Data);
                }
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null)
                {
                    progress?.Report($"[STDERR] {e.Data}");
                }
            };

            using var reg = ct.Register(() =>
            {
                progress?.Report("[PROCESS] キャンセル要求を受信しました。プロセスツリーを強制終了します...");
                KillProcessTree(process);
            });

            try
            {
                progress?.Report($"[START] コマンド開始: {fileName} {arguments}");
                if (!process.Start())
                {
                    throw new InvalidOperationException("プロセスの開始に失敗しました。");
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync(ct);
                return process.ExitCode;
            }
            catch (OperationCanceledException)
            {
                progress?.Report("[PROCESS] プロセスがユーザーによって中断されました。");
                return -1;
            }
            catch (Exception ex)
            {
                progress?.Report($"[ERROR] プロセス実行時例外: {ex.Message}");
                throw;
            }
            finally
            {
                lock (_lock)
                {
                    if (_currentProcess == process)
                    {
                        _currentProcess = null;
                    }
                }
                process.Dispose();
            }
        }

        /// <summary>
        /// プロセスおよびその子・孫プロセスツリー全体を強制終了する
        /// </summary>
        public void KillCurrentProcessTree()
        {
            lock (_lock)
            {
                if (_currentProcess != null && !_currentProcess.HasExited)
                {
                    KillProcessTree(_currentProcess);
                }
            }
        }

        /// <summary>
        /// 指定プロセスのプロセスツリーを完全に終了 (.NET 8 の Kill(true) および taskkill フォールバック)
        /// </summary>
        private static void KillProcessTree(Process? process)
        {
            if (process == null || process.HasExited) return;

            try
            {
                int pid = process.Id;
                // .NET 8 標準の KillProcessTree
                process.Kill(entireProcessTree: true);

                // Windows環境での追加の強制終了フォールバック (確実に子プロセスを停止)
                if (OperatingSystem.IsWindows())
                {
                    try
                    {
                        using var killer = Process.Start(new ProcessStartInfo
                        {
                            FileName = "taskkill",
                            Arguments = $"/F /T /PID {pid}",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });
                        killer?.WaitForExit(2000);
                    }
                    catch
                    {
                        // フォールバック失敗時は無視
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // すでに終了している場合は何もしない
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KillProcessTree 例外: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            KillCurrentProcessTree();
        }
    }
}
