using System.Text;
using RTDetrTrainerApp.Core;

namespace RTDetrTrainerApp.Forms
{
    public partial class MainForm : Form
    {
        private readonly TrainingManager _manager = new();
        private CancellationTokenSource? _cts;

        public MainForm()
        {
            InitializeComponent();
            InitializeDefaultPaths();
        }

        /// <summary>
        /// 要件に沿った初期パスの設定
        /// </summary>
        private void InitializeDefaultPaths()
        {
            // 要件に基づく初期パス設定
            txtPythonPath.Text = @"C:\Users\naoko\miniconda3\envs\rtdetr\python.exe";
            txtRtdetrRoot.Text = @"D:\Deveropment\RT-DETR_Models\python\rtdetrv2";
            txtDatasetDir.Text = @"D:\Deveropment\RT-DETR_Models\datasets";
            txtOutputDir.Text = @"D:\Deveropment\RT-DETR_Models\outputs";
            txtDeployDir.Text = @"D:\Deveropment\RT-DETR_Models\deploy";
        }

        private static string DetectDefaultPython()
        {
            // PATHから探索
            string? pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrEmpty(pathEnv))
            {
                foreach (string dir in pathEnv.Split(Path.PathSeparator))
                {
                    string candidate = Path.Combine(dir.Trim(), "python.exe");
                    if (File.Exists(candidate))
                    {
                        // WindowsApps のエイリアスを除外
                        if (!candidate.Contains("WindowsApps", StringComparison.OrdinalIgnoreCase))
                        {
                            return candidate;
                        }
                    }
                }
            }
            return string.Empty;
        }

        private string ResolvePythonExecutable()
        {
            string raw = txtPythonPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(raw))
            {
                string detected = DetectDefaultPython();
                if (!string.IsNullOrEmpty(detected)) return detected;
                throw new InvalidOperationException("CondaまたはPython環境のパスを指定してください。");
            }

            if (File.Exists(raw))
            {
                return raw;
            }

            if (Directory.Exists(raw))
            {
                string p1 = Path.Combine(raw, "python.exe");
                if (File.Exists(p1)) return p1;

                string p2 = Path.Combine(raw, "Scripts", "python.exe");
                if (File.Exists(p2)) return p2;

                string p3 = Path.Combine(raw, "bin", "python.exe");
                if (File.Exists(p3)) return p3;
            }

            throw new FileNotFoundException($"指定されたパスに python.exe が見つかりませんでした: {raw}");
        }

        private string GetAutoTrainScriptPath()
        {
            // 優先: D:\Deveropment\RT-DETR_Models\python\auto_train.py
            string fixedPath = @"D:\Deveropment\RT-DETR_Models\python\auto_train.py";
            if (File.Exists(fixedPath)) return fixedPath;

            // アプリ基準の相対パス
            string relativeCandidate = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\python\auto_train.py"));
            if (File.Exists(relativeCandidate)) return relativeCandidate;

            string localCandidate = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auto_train.py");
            if (File.Exists(localCandidate)) return localCandidate;

            return fixedPath;
        }

        private void AppendLog(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(AppendLog), message);
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            rtbLog.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.ScrollToCaret();
        }

        private void SetUiRunningState(bool running, string statusText)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool, string>(SetUiRunningState), running, statusText);
                return;
            }

            grpPaths.Enabled = !running;
            grpParams.Enabled = !running;
            btnDiagnose.Enabled = !running;
            btnStartTrain.Enabled = !running;
            btnCancel.Enabled = running;
            lblStatus.Text = statusText;
        }

        #region イベントハンドラ (参照ボタン)

        private void BtnBrowsePython_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "python.exe を選択してください (または Conda 環境フォルダ内の python.exe)",
                Filter = "Python実行ファイル (python.exe)|python.exe|すべてのファイル (*.*)|*.*"
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                txtPythonPath.Text = ofd.FileName;
            }
        }

        private void BtnBrowseRtdetr_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "RT-DETRv2 ルートディレクトリを選択してください",
                SelectedPath = Directory.Exists(txtRtdetrRoot.Text) ? txtRtdetrRoot.Text : @"D:\Deveropment\RT-DETR_Models\python"
            };

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                txtRtdetrRoot.Text = fbd.SelectedPath;
            }
        }

        private void BtnBrowseDataset_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "データセットフォルダを選択してください",
                SelectedPath = Directory.Exists(txtDatasetDir.Text) ? txtDatasetDir.Text : @"D:\Deveropment\RT-DETR_Models\datasets"
            };

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                txtDatasetDir.Text = fbd.SelectedPath;
            }
        }

        private void BtnBrowseOutput_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "学習出力フォルダを選択してください",
                SelectedPath = Directory.Exists(txtOutputDir.Text) ? txtOutputDir.Text : @"D:\Deveropment\RT-DETR_Models\outputs"
            };

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                txtOutputDir.Text = fbd.SelectedPath;
            }
        }

        private void BtnBrowseDeploy_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "配信先フォルダを選択してください",
                SelectedPath = Directory.Exists(txtDeployDir.Text) ? txtDeployDir.Text : @"D:\Deveropment\RT-DETR_Models\deploy"
            };

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                txtDeployDir.Text = fbd.SelectedPath;
            }
        }

        #endregion

        #region イベントハンドラ (アクションボタン)

        public decimal BatchSizeValue => numBatchSize.Value;
        public decimal WorkersValue => numWorkers.Value;
        public bool UseAmpValue => chkUseAmp.Checked;
        public decimal EpochsValue { get => numEpochs.Value; set => numEpochs.Value = value; }
        public bool DummyModeValue { get => chkDummyMode.Checked; set => chkDummyMode.Checked = value; }
        public string LogText => rtbLog.Text;

        private async void BtnDiagnose_Click(object sender, EventArgs e)
        {
            await ExecuteDiagnoseAsync(showDialog: true);
        }

        public async Task<bool> ExecuteDiagnoseAsync(bool showDialog = true)
        {
            string pythonExe;
            try
            {
                pythonExe = ResolvePythonExecutable();
            }
            catch (Exception ex)
            {
                if (showDialog)
                {
                    MessageBox.Show(this, ex.Message, "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    AppendLog($"[ERROR] 設定エラー: {ex.Message}");
                }
                return false;
            }

            string scriptPath = GetAutoTrainScriptPath();
            if (!File.Exists(scriptPath))
            {
                if (showDialog)
                {
                    MessageBox.Show(this, $"auto_train.py が見つかりません:\n{scriptPath}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    AppendLog($"[ERROR] auto_train.py が見つかりません: {scriptPath}");
                }
                return false;
            }

            SetUiRunningState(true, "スペック診断中...");
            _cts = new CancellationTokenSource();
            var progress = new Progress<string>(AppendLog);

            AppendLog("=== スペック診断を開始します ===");

            try
            {
                var result = await _manager.RunDiagnoseAsync(pythonExe, scriptPath, progress, _cts.Token);
                if (result != null)
                {
                    // UIパラメータへ自動反映
                    numBatchSize.Value = Math.Clamp(result.RecommendedBatchSize, numBatchSize.Minimum, numBatchSize.Maximum);
                    numWorkers.Value = Math.Clamp(result.RecommendedNumWorkers, numWorkers.Minimum, numWorkers.Maximum);
                    chkUseAmp.Checked = result.RecommendedUseAmp;

                    AppendLog("【自動反映完了】診断結果をUI設定パラメータに反映しました！");
                    lblStatus.Text = $"診断完了: {result.GpuName} (VRAM: {result.VramGb}GB)";
                    
                    if (showDialog)
                    {
                        MessageBox.Show(
                            this,
                            $"スペック診断が完了しました。\n\n" +
                            $"検出GPU: {result.GpuName}\n" +
                            $"VRAM: {result.VramGb} GB\n" +
                            $"CPUコア: {result.CpuCores}\n\n" +
                            $"【推奨パラメータを自動設定しました】\n" +
                            $"・バッチサイズ: {result.RecommendedBatchSize}\n" +
                            $"・ワーカー数: {result.RecommendedNumWorkers}\n" +
                            $"・AMP: {(result.RecommendedUseAmp ? "有効" : "無効")}",
                            "スペック診断完了",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    return true;
                }
                else
                {
                    AppendLog("[WARNING] 診断情報のパースに失敗しました。ログを確認してください。");
                    lblStatus.Text = "診断完了 (情報取得不十分)";
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                AppendLog("スペック診断がキャンセルされました。");
                lblStatus.Text = "中断";
                return false;
            }
            catch (Exception ex)
            {
                AppendLog($"[ERROR] 診断中にエラーが発生しました: {ex.Message}");
                lblStatus.Text = "エラー";
                return false;
            }
            finally
            {
                SetUiRunningState(false, lblStatus.Text ?? "待機中");
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async void BtnStartTrain_Click(object sender, EventArgs e)
        {
            await ExecuteStartTrainAsync(showDialog: true);
        }

        public async Task<int> ExecuteStartTrainAsync(bool showDialog = true)
        {
            string pythonExe;
            try
            {
                pythonExe = ResolvePythonExecutable();
            }
            catch (Exception ex)
            {
                if (showDialog)
                {
                    MessageBox.Show(this, ex.Message, "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    AppendLog($"[ERROR] 設定エラー: {ex.Message}");
                }
                return -1;
            }

            string scriptPath = GetAutoTrainScriptPath();
            if (!File.Exists(scriptPath))
            {
                if (showDialog)
                {
                    MessageBox.Show(this, $"auto_train.py が見つかりません:\n{scriptPath}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    AppendLog($"[ERROR] auto_train.py が見つかりません: {scriptPath}");
                }
                return -1;
            }

            bool isDummy = chkDummyMode.Checked;

            if (!isDummy && !Directory.Exists(txtRtdetrRoot.Text))
            {
                if (showDialog)
                {
                    var dr = MessageBox.Show(
                        this,
                        $"RT-DETRv2 ルートディレクトリが存在しません:\n{txtRtdetrRoot.Text}\n\nダミー動作モード（テスト用）として実行しますか？",
                        "確認",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        isDummy = true;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    isDummy = true;
                }
            }

            var parameters = new TrainParameters
            {
                PythonExecutablePath = pythonExe,
                AutoTrainScriptPath = scriptPath,
                RtdetrRoot = txtRtdetrRoot.Text.Trim(),
                ConfigFile = txtConfigFile.Text.Trim(),
                DatasetDir = txtDatasetDir.Text.Trim(),
                OutputDir = txtOutputDir.Text.Trim(),
                DeployDir = txtDeployDir.Text.Trim(),
                Epochs = (int)numEpochs.Value,
                BatchSize = (int)numBatchSize.Value,
                NumWorkers = (int)numWorkers.Value,
                InputSize = (int)numInputSize.Value,
                UseAmp = chkUseAmp.Checked,
                IsDummy = isDummy
            };

            SetUiRunningState(true, "学習中...");
            _cts = new CancellationTokenSource();
            var progress = new Progress<string>(AppendLog);

            AppendLog("==================================================");
            AppendLog("=== RT-DETRv2 学習＆ONNXエクスポートパイプラインを開始 ===");
            AppendLog("==================================================");

            try
            {
                int exitCode = await _manager.RunTrainAsync(parameters, progress, _cts.Token);
                if (exitCode == 0)
                {
                    AppendLog("==================================================");
                    AppendLog("🎉 すべての工程が正常終了しました！");
                    AppendLog($"配信モデル: {Path.Combine(parameters.DeployDir, "model.onnx")}");
                    AppendLog("==================================================");
                    lblStatus.Text = "完了: 正常終了";

                    if (showDialog)
                    {
                        MessageBox.Show(
                            this,
                            $"学習およびONNXエクスポートが完了しました！\n\n配信先モデル:\n{Path.Combine(parameters.DeployDir, "model.onnx")}",
                            "完了",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    AppendLog($"[ERROR] 処理が終了コード {exitCode} で終了しました。ログを確認してください。");
                    lblStatus.Text = $"エラー終了 (ExitCode: {exitCode})";
                }
                return exitCode;
            }
            catch (OperationCanceledException)
            {
                AppendLog("ユーザーによって処理が中断されました。");
                lblStatus.Text = "ユーザー中断";
                return -1;
            }
            catch (Exception ex)
            {
                AppendLog($"[FATAL] 予期しない例外が発生しました: {ex.Message}");
                lblStatus.Text = "例外発生";
                return -1;
            }
            finally
            {
                SetUiRunningState(false, lblStatus.Text ?? "待機中");
                _cts?.Dispose();
                _cts = null;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                var dr = MessageBox.Show(this, "実行中のプロセスを中断しますか？\nPythonプロセスツリー全体が停止します。", "中断確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    AppendLog("[USER] 中断要求を発行しました...");
                    _cts.Cancel();
                    _manager.KillCurrentProcessTree();
                }
            }
        }

        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        private void BtnSaveLog_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Title = "ログファイルの保存",
                Filter = "テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
                FileName = $"rtdetr_train_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(sfd.FileName, rtbLog.Text, Encoding.UTF8);
                    MessageBox.Show(this, "ログを正常に保存しました。", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"保存に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_manager.IsRunning)
            {
                var dr = MessageBox.Show(
                    this,
                    "学習または診断プロセスが実行中です。終了するとプロセスが強制停止されます。\n終了しますか？",
                    "確認",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    _cts?.Cancel();
                    _manager.Dispose();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion
    }
}
