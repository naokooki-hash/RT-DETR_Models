# RT-DETR Model Workflow & Trainer App

RT-DETRv2 モデル作成・学習、ONNXエクスポート、および検査アプリ（`InspectionSystem_RTDETR.exe`）のCLIと連携した**自動モデル暗号化（.enc）**を行うための C# WinForms アプリケーションと Python ラッパースクリプト一式です。

## ディレクトリ構成

```text
D:\Deveropment\RT-DETR_Models\
├── src/                                # C# WinForms ソースコード (.NET 8.0)
│   ├── RTDetrTrainerApp.sln            # ソリューションファイル
│   └── RTDetrTrainerApp/
│       ├── Forms/
│       │   ├── MainForm.cs             # メイン画面処理・UIイベント・パス管理
│       │   └── MainForm.Designer.cs
│       ├── Core/
│       │   ├── TrainingManager.cs      # Pythonプロセス制御・非同期ログ・KillProcessTree
│       │   └── HardwareConfig.cs       # 診断結果・パラメータ構造体
│       ├── Program.cs                  # エントリポイント
│       └── RTDetrTrainerApp.csproj
├── python/                             # Pythonバックエンド
│   └── auto_train.py                   # 自動スペック診断・学習・ONNX出力・自動暗号化・配信転送ラッパー
├── datasets/                           # 学習用データセット（COCOフォーマット）
├── outputs/                            # 学習一時出力フォルダ（pth, onnx, enc）
└── deploy/                             # 現場配信用フォルダ（model.enc / model.onnx）
```

## 主な機能

1. **ハードウェアスペック自動診断 (`python/auto_train.py --mode diagnose`)**:
   - PyTorch (`torch.cuda`) により GPU/VRAM/CPUコア数を自動判定
   - 最適な `batch_size`、`num_workers`、`use_amp` を自動計算
   - GUI上の「スペック診断 & 自動設定」ボタンで各入力欄に自動反映

2. **学習〜ONNX変換〜自動モデル暗号化〜現場配信パイプライン (`python/auto_train.py --mode train`)**:
   - COCOデータセットのカテゴリ数を自動判定
   - RT-DETRv2 の `train.py` を呼び出して学習実行
   - 学習完了後、最良チェックポイント重みから `export_onnx.py` で ONNX エクスポート
   - **検査アプリCLI連携（自動モデル暗号化）**:
     - `InspectionSystem_RTDETR.exe --encrypt-model <onnx> <enc>` を自動実行
     - 共通の暗号鍵（AES-256-CBC）で `model.enc` を生成
     - 現場配信用ディレクトリ（`deploy/model.enc`）へ自動配置
   - 検査アプリ未指定・未検出時のフォールバック機能（従来の `model.onnx` を転送）
   - ダミー動作テストモード（`--dummy`）を完備

3. **リアルタイム非同期プロセス制御 (`TrainingManager.cs`)**:
   - `PYTHONUNBUFFERED=1` によるリアルタイムログストリーミング表示
   - .NET 8 `process.Kill(entireProcessTree: true)` による安全な中断機能
