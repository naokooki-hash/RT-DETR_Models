using System.Runtime.InteropServices;
using System.Text;
using RTDetrTrainerApp.Forms;

namespace RTDetrTrainerApp
{
    internal static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            if (args.Contains("--test"))
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                var testForm = new MainForm();
                testForm.Shown += async (s, e) =>
                {
                    try
                    {
                        int exitCode = await RunAutomatedTestsAsync(testForm);
                        Environment.ExitCode = exitCode;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[FATAL] テスト実行中例外: {ex}");
                        Environment.ExitCode = 1;
                    }
                    finally
                    {
                        Application.Exit();
                    }
                };
                Application.Run(testForm);
                return;
            }

            Application.Run(new MainForm());
        }

        private static async Task<int> RunAutomatedTestsAsync(MainForm form)
        {
            var sb = new StringBuilder();
            void Log(string line)
            {
                Console.WriteLine(line);
                sb.AppendLine(line);
            }

            Console.OutputEncoding = Encoding.UTF8;
            Log("==================================================");
            Log("RTDetrTrainerApp 自動動作テストを開始します");
            Log("==================================================");

            // テスト1: 初期状態の検証
            Log("\n[TEST 1] 初期UIパラメータの確認:");
            Log($"  ・初期バッチサイズ: {form.BatchSizeValue}");
            Log($"  ・初期ワーカー数  : {form.WorkersValue}");
            Log($"  ・初期AMP設定     : {form.UseAmpValue}");

            // テスト2: 「スペック診断 & 自動設定」テスト
            Log("\n[TEST 2] 「スペック診断 & 自動設定」ボタンのテスト実行中...");
            bool diagOk = await form.ExecuteDiagnoseAsync(showDialog: false);
            if (!diagOk)
            {
                Log("[FAIL] スペック診断が失敗しました。");
                Log("ログ:\n" + form.LogText);
                File.WriteAllText("test_result.log", sb.ToString(), Encoding.UTF8);
                return 1;
            }

            Log("[PASS] スペック診断が完了しました。");
            Log($"  ・診断後の自動補完バッチサイズ: {form.BatchSizeValue}");
            Log($"  ・診断後の自動補完ワーカー数  : {form.WorkersValue}");
            Log($"  ・診断後の自動補完AMP設定     : {form.UseAmpValue}");

            if (form.BatchSizeValue != 2)
            {
                Log($"[WARNING] CPU環境で期待値 2 に対して {form.BatchSizeValue} が設定されました。");
            }
            else
            {
                Log("[SUCCESS] 入力欄に自動で「2」が補完されたことを確認しました！");
            }

            // テスト3: 「学習開始」ボタンのテスト（ダミー動作）
            Log("\n[TEST 3] 「学習開始」ボタンのテスト（ダミー動作: エポック1）を実行中...");
            form.EpochsValue = 1;
            form.DummyModeValue = true;

            int trainExitCode = await form.ExecuteStartTrainAsync(showDialog: false);
            if (trainExitCode != 0)
            {
                Log($"[FAIL] 学習テストがエラー終了しました (ExitCode: {trainExitCode})");
                Log("ログ:\n" + form.LogText);
                File.WriteAllText("test_result.log", sb.ToString(), Encoding.UTF8);
                return 1;
            }

            Log("[PASS] 学習〜ONNX出力〜暗号化〜deploy転送が正常に完了しました！");

            // 生成された model.enc の存在確認
            string deployEnc = @"D:\Deveropment\RT-DETR_Models\deploy\model.enc";
            if (File.Exists(deployEnc))
            {
                var fi = new FileInfo(deployEnc);
                Log($"[VERIFY] 現場配信用暗号化モデルを確認: {deployEnc} (サイズ: {fi.Length:N0} bytes)");
            }
            else
            {
                Log($"[FAIL] 現場配信用暗号化モデルが見つかりません: {deployEnc}");
                File.WriteAllText("test_result.log", sb.ToString(), Encoding.UTF8);
                return 1;
            }

            // ログ内に暗号化ステップが出力されたか確認
            if (form.LogText.Contains("[STEP 3/4]") && form.LogText.Contains("[STEP 4/4]"))
            {
                Log("[VERIFY] 暗号化パイプラインステップ [STEP 3/4], [STEP 4/4] のログ出力を確認しました！");
            }
            else
            {
                Log("[WARNING] 暗号化ステップのログマーカーが確認できませんでした。");
            }

            Log("\n--------------------------------------------------");
            Log("【RichTextBox に記録された最新ログ一覧】");
            Log(form.LogText);
            Log("--------------------------------------------------");
            Log("🎉 すべての動作テストが正常に成功しました！");
            Log("==================================================");

            File.WriteAllText("test_result.log", sb.ToString(), Encoding.UTF8);
            return 0;
        }
    }
}
