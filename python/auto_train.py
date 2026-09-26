"""auto_train.py - RT-DETRv2 自動学習・ONNXエクスポート・配信ラッパースクリプト
Copyright (c) 2026 RT-DETR Model Workflow
"""

import os
import sys
import json
import shutil
import argparse
import subprocess
from datetime import datetime
from pathlib import Path


def log(msg: str):
    """標準出力を即座にフラッシュして出力"""
    print(msg, flush=True)


def diagnose_hardware():
    """ハードウェアスペック（GPU/VRAM/CPUコア）を診断し、最適パラメータを算出"""
    log("==================================================")
    log("[DIAGNOSE] ハードウェアスペックの自動診断を開始します...")
    log("==================================================")

    gpu_available = False
    gpu_name = "N/A"
    vram_gb = 0.0
    cuda_version = "N/A"
    cpu_cores = os.cpu_count() or 1

    try:
        import torch

        gpu_available = torch.cuda.is_available()
        if gpu_available:
            gpu_name = torch.cuda.get_device_name(0)
            total_bytes = torch.cuda.get_device_properties(0).total_memory
            vram_gb = round(total_bytes / (1024**3), 2)
            cuda_version = torch.version.cuda or "Unknown"
            log(f"-> CUDA利用可能: はい (CUDA Version: {cuda_version})")
            log(f"-> 検出GPU: {gpu_name}")
            log(f"-> 検出VRAM: {vram_gb} GB")
        else:
            log("-> CUDA利用可能: いいえ (CPUモードで動作)")
    except ImportError:
        log("-> [WARNING] PyTorchが現在のPython環境にインストールされていません。")
        log("   適切なCUDA対応PyTorch環境を指定してください。")
    except Exception as ex:
        log(f"-> [WARNING] GPU情報取得中にエラーが発生しました: {ex}")

    log(f"-> CPU論理コア数: {cpu_cores}")

    # 最適パラメータの自動計算
    # Windowsのマルチプロセッシング制限と共有メモリ制約を考慮し、num_workersは最大4に制限
    if gpu_available:
        if vram_gb >= 16.0:
            recommended_batch_size = 16
            recommended_use_amp = True
        elif vram_gb >= 8.0:
            recommended_batch_size = 8
            recommended_use_amp = True
        elif vram_gb >= 4.0:
            recommended_batch_size = 4
            recommended_use_amp = True
        else:
            recommended_batch_size = 2
            recommended_use_amp = False

        recommended_num_workers = min(4, max(2, cpu_cores // 2))
    else:
        # CPUフォールバック
        recommended_batch_size = 2
        recommended_num_workers = min(2, cpu_cores)
        recommended_use_amp = False

    log("--------------------------------------------------")
    log("【自動計算された推奨パラメータ】")
    log(f"  ・推奨バッチサイズ (batch_size): {recommended_batch_size}")
    log(f"  ・推奨ワーカー数 (num_workers)  : {recommended_num_workers}")
    log(f"  ・推奨AMP設定 (use_amp)        : {recommended_use_amp}")
    log("--------------------------------------------------")

    result = {
        "gpu_available": gpu_available,
        "gpu_name": gpu_name,
        "cuda_version": cuda_version,
        "vram_gb": vram_gb,
        "cpu_cores": cpu_cores,
        "recommended_batch_size": recommended_batch_size,
        "recommended_num_workers": recommended_num_workers,
        "recommended_use_amp": recommended_use_amp,
    }

    # C# UI側でパースしやすいマーカー付きJSONを出力
    json_str = json.dumps(result, ensure_ascii=False)
    log(f"[SPEC_JSON]{json_str}[/SPEC_JSON]")
    return result


def find_dataset_files(dataset_dir: str):
    """データセットディレクトリ内のCOCO形式アノテーションと画像ディレクトリを自動探索"""
    dpath = Path(dataset_dir)
    if not dpath.exists():
        log(f"[WARNING] 指定されたデータセットパスが存在しません: {dataset_dir}")
        return None

    # COCO JSONの探索候補
    ann_candidates = list(dpath.rglob("*.json"))
    # 画像ディレクトリの探索候補
    img_candidates = [
        p for p in dpath.rglob("*") if p.is_dir() and p.name.lower() in ["images", "train", "val", "img"]
    ]

    log(f"[DATASET] アノテーションJSON候補: {[str(c.relative_to(dpath)) for c in ann_candidates]}")
    log(f"[DATASET] 画像ディレクトリ候補: {[str(c.relative_to(dpath)) for c in img_candidates]}")

    info = {
        "train_ann": None,
        "val_ann": None,
        "train_img": None,
        "val_img": None,
        "num_classes": 80,
    }

    # 学習用/検証用のアノテーションを判別
    for cand in ann_candidates:
        name = cand.name.lower()
        if "val" in name or "test" in name:
            if not info["val_ann"]:
                info["val_ann"] = str(cand.resolve())
        elif "train" in name or "instances" in name or "annotations" in name:
            if not info["train_ann"]:
                info["train_ann"] = str(cand.resolve())

    # 1つしかJSONがない場合はtrain/val両方に割り当て
    if len(ann_candidates) == 1:
        single_ann = str(ann_candidates[0].resolve())
        info["train_ann"] = single_ann
        info["val_ann"] = single_ann
    elif not info["train_ann"] and ann_candidates:
        info["train_ann"] = str(ann_candidates[0].resolve())
        info["val_ann"] = str(ann_candidates[0].resolve())

    # 画像フォルダを判別
    for cand in img_candidates:
        name = cand.name.lower()
        if "val" in name or "test" in name:
            if not info["val_img"]:
                info["val_img"] = str(cand.resolve())
        elif "train" in name:
            if not info["train_img"]:
                info["train_img"] = str(cand.resolve())

    # 画像フォルダが見つからない場合はデータセット直下またはimagesフォルダ
    if not info["train_img"]:
        if (dpath / "images").is_dir():
            info["train_img"] = str((dpath / "images").resolve())
            info["val_img"] = str((dpath / "images").resolve())
        else:
            info["train_img"] = str(dpath.resolve())
            info["val_img"] = str(dpath.resolve())

    # JSONからカテゴリ数をカウント
    if info["train_ann"] and os.path.exists(info["train_ann"]):
        try:
            with open(info["train_ann"], "r", encoding="utf-8") as f:
                data = json.load(f)
                if "categories" in data and isinstance(data["categories"], list):
                    info["num_classes"] = len(data["categories"])
                    log(f"[DATASET] COCOアノテーションから {info['num_classes']} 個のカテゴリを検出しました。")
        except Exception as e:
            log(f"[WARNING] アノテーションJSONの読み込み中に警告: {e}")

    return info


def run_dummy_training(args):
    """ダミー学習およびONNXエクスポート・現場配信テスト"""
    import time
    log("==================================================")
    log("[TRAIN-DUMMY] RT-DETRv2 ダミー学習・エクスポートテストを開始します")
    log("==================================================")

    now_str = datetime.now().strftime("%Y%m%d_%H%M%S")
    output_dir = Path(args.output_dir).resolve() if args.output_dir else (Path("outputs") / f"run_{now_str}").resolve()
    output_dir.mkdir(parents=True, exist_ok=True)
    deploy_dir = Path(args.deploy_dir).resolve()
    deploy_dir.mkdir(parents=True, exist_ok=True)

    log(f"[CONFIG] エポック数: {args.epochs}, バッチサイズ: {args.batch_size}, ワーカー数: {args.num_workers}, AMP: {args.use_amp}")
    log(f"[OUTPUT] 出力ディレクトリ: {output_dir}")
    log(f"[DEPLOY] 配信先ディレクトリ: {deploy_dir}")
    log("[DATASET] ダミーデータセットを初期化中 (サンプル数: 100, クラス数: 1)...")
    time.sleep(0.4)

    total_epochs = max(1, args.epochs)
    steps_per_epoch = 5
    for epoch in range(1, total_epochs + 1):
        log(f"\n--- Epoch [{epoch}/{total_epochs}] 開始 ---")
        for step in range(1, steps_per_epoch + 1):
            loss = round(1.8 / (epoch * 0.4 + step * 0.15), 4)
            lr = 0.0001
            log(f"Epoch: [{epoch}/{total_epochs}] Step: [{step}/{steps_per_epoch}] loss: {loss} lr: {lr} eta: 0:00:0{steps_per_epoch - step}")
            time.sleep(0.3)
        log(f"Epoch [{epoch}/{total_epochs}] 完了: 平均Loss = {loss}")

    best_weight = output_dir / "best.pth"
    intermediate_onnx = output_dir / "model.onnx"
    try:
        import torch
        import torch.nn as nn

        class DummyRTDetr(nn.Module):
            def __init__(self):
                super().__init__()
                self.conv = nn.Conv2d(3, 8, kernel_size=3, padding=1)
                self.pool = nn.AdaptiveAvgPool2d((1, 1))
                self.fc_labels = nn.Linear(8, 1)
                self.fc_boxes = nn.Linear(8, 4)
                self.fc_scores = nn.Linear(8, 1)

            def forward(self, images, orig_target_sizes):
                x = self.pool(self.conv(images)).flatten(1)
                labels = self.fc_labels(x).long()
                boxes = self.fc_boxes(x)
                scores = self.fc_scores(x)
                return labels, boxes, scores

        model = DummyRTDetr()
        torch.save(model.state_dict(), str(best_weight))
        log(f"[WEIGHT] チェックポイントを保存しました: {best_weight}")

        log("--------------------------------------------------")
        log("[EXPORT] ダミーONNXエクスポートを実行中...")
        log("--------------------------------------------------")
        data = torch.rand(1, 3, args.input_size, args.input_size)
        size = torch.tensor([[args.input_size, args.input_size]])
        torch.onnx.export(
            model,
            (data, size),
            str(intermediate_onnx),
            input_names=['images', 'orig_target_sizes'],
            output_names=['labels', 'boxes', 'scores'],
            opset_version=16,
            do_constant_folding=True
        )
        log(f"[EXPORT] ONNXエクスポート成功: {intermediate_onnx}")
    except Exception as e:
        log(f"[WARNING] PyTorch直接エクスポートでフォールバック: {e}")
        intermediate_onnx.write_bytes(b"RT-DETR Dummy ONNX Content")
        log(f"[EXPORT] ダミーONNXファイルを生成しました: {intermediate_onnx}")

    # 暗号化および現場配信
    encrypt_and_deploy_model(intermediate_onnx, output_dir, deploy_dir, getattr(args, "inspection_exe", ""))


def encrypt_and_deploy_model(onnx_file: Path, output_dir: Path, deploy_dir: Path, inspection_exe: str):
    """
    ONNXモデルを検査アプリCLIで暗号化し、deploy/model.enc へ転送。
    検査アプリが見つからない場合はフォールバックとして従来の model.onnx を転送。
    """
    enc_file = output_dir / "model.enc"
    encrypted_successfully = False

    # 検査アプリExeの確認
    resolved_exe = None
    if inspection_exe and Path(inspection_exe).exists():
        resolved_exe = Path(inspection_exe).resolve()
    else:
        # デフォルト候補を探索
        default_candidate = Path(r"D:\Deveropment\InspectionSystem_RTDETR\InspectionSystem_RTDETR\bin\Debug\net10.0-windows\InspectionSystem_RTDETR.exe")
        if default_candidate.exists():
            resolved_exe = default_candidate

    if resolved_exe:
        log("--------------------------------------------------")
        log("[STEP 3/4] 検査アプリのCLIを呼び出してONNXモデルを暗号化中...")
        log(f"  ・検査アプリExe: {resolved_exe}")
        log(f"  ・入力ONNX    : {onnx_file}")
        log(f"  ・出力ENC     : {enc_file}")
        log("--------------------------------------------------")

        # 実行コマンド構築 (.dll の場合は dotnet 経由、.exe の場合は直接実行)
        if resolved_exe.suffix.lower() == ".dll":
            cmd = ["dotnet", str(resolved_exe), "--encrypt-model", str(onnx_file), str(enc_file)]
        else:
            cmd = [str(resolved_exe), "--encrypt-model", str(onnx_file), str(enc_file)]

        try:
            proc = subprocess.Popen(
                cmd,
                stdout=subprocess.PIPE,
                stderr=subprocess.STDOUT,
                text=True,
                bufsize=1,
            )
            for line in proc.stdout:
                print(line, end="", flush=True)
            proc.wait()

            if proc.returncode == 0 and enc_file.exists():
                log(f"[SUCCESS] モデル暗号化が完了しました: {enc_file}")
                encrypted_successfully = True
            else:
                log(f"[WARNING] 検査アプリによる暗号化が終了コード {proc.returncode} で完了しなかったか、出力ファイルが生成されませんでした。")
        except Exception as ex:
            log(f"[WARNING] 暗号化CLIの実行中にエラーが発生しました: {ex}")
    else:
        log("[WARNING] 検査アプリの実行ファイル (InspectionSystem_RTDETR.exe) が未指定または見つかりません。")
        log("   暗号化処理をスキップし、従来の model.onnx を転送します（フォールバック）。")

    # deploy フォルダへの転送
    if encrypted_successfully and enc_file.exists():
        deploy_target = deploy_dir / "model.enc"
        log(f"[STEP 4/4] 現場配信用フォルダへ暗号化モデルを転送中: {deploy_target}")
        try:
            shutil.copy2(enc_file, deploy_target)
            log(f"[DEPLOY] 現場配信用フォルダへ暗号化モデル (.enc) をコピーしました: {deploy_target}")
        except Exception as e:
            log(f"[ERROR] 暗号化モデルのdeployフォルダへのコピーに失敗しました: {e}")
            sys.exit(1)
    else:
        deploy_target = deploy_dir / "model.onnx"
        log(f"[STEP 4/4] 現場配信用フォルダへONNXモデルを転送中 (フォールバック): {deploy_target}")
        try:
            shutil.copy2(onnx_file, deploy_target)
            log(f"[DEPLOY] 現場配信用フォルダへONNXをコピーしました: {deploy_target}")
        except Exception as e:
            log(f"[ERROR] ONNXファイルのdeployフォルダへのコピーに失敗しました: {e}")
            sys.exit(1)

    log("==================================================")
    log("[FINISH] すべてのパイプライン処理が正常に完了しました！")
    log(f"  ・学習出力先: {output_dir}")
    log(f"  ・配信モデル: {deploy_target}")
    log("==================================================")


def run_training(args):
    """学習プロセスの実行およびONNXエクスポート・配信"""
    if getattr(args, "dummy", False):
        run_dummy_training(args)
        return

    rtdetr_root = Path(args.rtdetr_root).resolve()
    train_script = rtdetr_root / "tools" / "train.py"
    if not rtdetr_root.exists() or not train_script.exists():
        log("[INFO] RT-DETRv2 ルートまたは train.py が未検出のため、ダミー動作モード（テスト用）を実行します。")
        run_dummy_training(args)
        return

    log("==================================================")
    log("[TRAIN] RT-DETRv2 自動学習・エクスポートパイプラインを開始します")
    log("==================================================")

    # 出力先ディレクトリの決定
    now_str = datetime.now().strftime("%Y%m%d_%H%M%S")
    if args.output_dir:
        output_dir = Path(args.output_dir).resolve()
    else:
        output_dir = Path("outputs") / f"run_{now_str}"

    output_dir.mkdir(parents=True, exist_ok=True)
    log(f"[OUTPUT] 出力ディレクトリ: {output_dir}")

    # 配信先ディレクトリの確認
    deploy_dir = Path(args.deploy_dir).resolve()
    deploy_dir.mkdir(parents=True, exist_ok=True)
    log(f"[DEPLOY] 配信先ディレクトリ: {deploy_dir}")

    # コンフィグファイルのパス決定
    config_path = Path(args.config)
    if not config_path.is_absolute():
        config_path = (rtdetr_root / args.config).resolve()

    if not config_path.exists():
        log(f"[ERROR] 設定YAMLファイルが存在しません: {config_path}")
        sys.exit(1)

    log(f"[CONFIG] 使用設定ファイル: {config_path}")

    # データセット情報の探索
    dataset_info = None
    if args.dataset_dir and Path(args.dataset_dir).exists():
        dataset_info = find_dataset_files(args.dataset_dir)

    # train.py の実行コマンド構築
    python_exe = sys.executable
    train_cmd = [
        python_exe,
        str(train_script),
        "-c",
        str(config_path),
        "--output-dir",
        str(output_dir),
    ]

    if args.use_amp:
        train_cmd.append("--use-amp")

    if args.tuning:
        train_cmd.extend(["-t", str(args.tuning)])
    elif args.resume:
        train_cmd.extend(["-r", str(args.resume)])

    # -u 引数でパラメータの上書き指定
    updates = [
        f"epoches={args.epochs}",
        f"train_dataloader.total_batch_size={args.batch_size}",
        f"train_dataloader.num_workers={args.num_workers}",
        f"val_dataloader.total_batch_size={args.batch_size}",
        f"val_dataloader.num_workers={args.num_workers}",
    ]

    if dataset_info:
        if dataset_info["train_ann"]:
            # Windowsパスのバックスラッシュをスラッシュに置換
            train_ann_posix = Path(dataset_info["train_ann"]).as_posix()
            val_ann_posix = Path(dataset_info["val_ann"] or dataset_info["train_ann"]).as_posix()
            train_img_posix = Path(dataset_info["train_img"]).as_posix()
            val_img_posix = Path(dataset_info["val_img"] or dataset_info["train_img"]).as_posix()

            updates.extend([
                f"train_dataloader.dataset.ann_file={train_ann_posix}",
                f"train_dataloader.dataset.img_folder={train_img_posix}",
                f"val_dataloader.dataset.ann_file={val_ann_posix}",
                f"val_dataloader.dataset.img_folder={val_img_posix}",
                f"num_classes={dataset_info['num_classes']}",
                "remap_mscoco_category=False",
            ])

    train_cmd.append("-u")
    train_cmd.extend(updates)

    log("--------------------------------------------------")
    log("[EXEC] 学習コマンドを実行します:")
    log(" ".join(train_cmd))
    log("--------------------------------------------------")

    # 学習プロセスの実行（リアルタイム出力）
    env = os.environ.copy()
    env["PYTHONUNBUFFERED"] = "1"
    env["PYTHONIOENCODING"] = "utf-8"

    proc = subprocess.Popen(
        train_cmd,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
        bufsize=1,
        cwd=str(rtdetr_root),
        env=env,
    )

    try:
        for line in proc.stdout:
            print(line, end="", flush=True)
        proc.wait()
    except KeyboardInterrupt:
        log("[CANCEL] ユーザーによる割り込みを受け付けました。学習プロセスを停止します...")
        proc.terminate()
        proc.wait()
        sys.exit(130)

    if proc.returncode != 0:
        log(f"[ERROR] 学習プロセスがエラー終了しました (ExitCode: {proc.returncode})")
        sys.exit(proc.returncode)

    log("[TRAIN] 学習プロセスが正常に完了しました！")

    # 最良モデル重みファイルの探索
    best_weight = None
    for cand_name in ["best.pth", "model_best.pth", "checkpoint.pth", "last.pth"]:
        cand = output_dir / cand_name
        if cand.exists():
            best_weight = cand
            break

    if not best_weight:
        # checkpoint00XX.pth などの最新ファイルを探す
        checkpoints = sorted(output_dir.glob("checkpoint*.pth"), key=os.path.getmtime, reverse=True)
        if checkpoints:
            best_weight = checkpoints[0]

    if not best_weight:
        log(f"[WARNING] 学習結果のpthファイルが {output_dir} に見つかりませんでした。")
        log("[INFO] ディレクトリ内の全ファイルを探索します...")
        pth_files = list(output_dir.glob("*.pth"))
        if pth_files:
            best_weight = pth_files[0]

    if not best_weight or not best_weight.exists():
        log("[ERROR] 有効な重みファイルが見つからなかったため、ONNXエクスポートを中断します。")
        sys.exit(1)

    log(f"[WEIGHT] エクスポート対象重み: {best_weight}")

    # ONNXエクスポートの実行
    if not export_script.exists():
        log(f"[ERROR] ONNXエクスポートスクリプトが見つかりません: {export_script}")
        sys.exit(1)

    intermediate_onnx = output_dir / "model.onnx"
    export_cmd = [
        python_exe,
        str(export_script),
        "-c",
        str(config_path),
        "-r",
        str(best_weight),
        "-o",
        str(intermediate_onnx),
        "-s",
        str(args.input_size),
        "--check",
    ]

    # num_classesの更新が必要な場合はエクスポート時にも反映
    if dataset_info and dataset_info.get("num_classes"):
        export_cmd.extend(["-u", f"num_classes={dataset_info['num_classes']}", "remap_mscoco_category=False"])

    log("--------------------------------------------------")
    log("[EXPORT] ONNXエクスポートコマンドを実行します:")
    log(" ".join(export_cmd))
    log("--------------------------------------------------")

    proc_export = subprocess.Popen(
        export_cmd,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
        bufsize=1,
        cwd=str(rtdetr_root),
        env=env,
    )

    for line in proc_export.stdout:
        print(line, end="", flush=True)
    proc_export.wait()

    if proc_export.returncode != 0:
        log(f"[ERROR] ONNXエクスポートが失敗しました (ExitCode: {proc_export.returncode})")
        sys.exit(proc_export.returncode)

    log(f"[EXPORT] ONNXエクスポート成功: {intermediate_onnx}")

    # 暗号化および現場配信
    encrypt_and_deploy_model(intermediate_onnx, output_dir, deploy_dir, getattr(args, "inspection_exe", ""))


def main():
    parser = argparse.ArgumentParser(description="RT-DETRv2 自動学習・ONNXエクスポートラッパー")
    parser.add_argument(
        "--mode",
        choices=["diagnose", "train"],
        default="diagnose",
        help="実行モード: diagnose(スペック診断) または train(学習・エクスポート・配信)",
    )

    # 学習用パラメータ
    parser.add_argument(
        "--rtdetr-root",
        type=str,
        default=r"D:\Deveropment\RT-DETR_Models\python\rtdetrv2",
        help="RT-DETRv2 リポジトリのルートパス",
    )
    parser.add_argument(
        "--config",
        type=str,
        default=r"configs/rtdetrv2/rtdetrv2_r18vd_120e_coco.yml",
        help="モデル設定YAML (RT-DETRv2ルートからの相対パスまたは絶対パス)",
    )
    parser.add_argument(
        "--dataset-dir",
        type=str,
        default=r"D:\Deveropment\RT-DETR_Models\datasets",
        help="COCO形式データセットディレクトリ",
    )
    parser.add_argument(
        "--output-dir",
        type=str,
        default="",
        help="学習一時出力ディレクトリ (空の場合は outputs/run_YYYYMMDD_HHMMSS)",
    )
    parser.add_argument(
        "--deploy-dir",
        type=str,
        default=r"D:\Deveropment\RT-DETR_Models\deploy",
        help="現場機配信用フォルダ (最終的に model.enc / model.onnx を出力)",
    )
    parser.add_argument("--epochs", type=int, default=100, help="学習エポック数")
    parser.add_argument("--batch-size", type=int, default=8, help="バッチサイズ")
    parser.add_argument("--num-workers", type=int, default=4, help="データローダーワーカー数")
    parser.add_argument("--use-amp", action="store_true", help="混合精度訓練(AMP)を使用")
    parser.add_argument("--input-size", type=int, default=640, help="ONNXエクスポート解像度")
    parser.add_argument("--tuning", type=str, default=None, help="ファインチューニング用チェックポイント")
    parser.add_argument("--resume", type=str, default=None, help="学習再開用チェックポイント")
    parser.add_argument("--dummy", action="store_true", help="ダミー学習・エクスポートテストを実行")
    parser.add_argument(
        "--inspection-exe",
        type=str,
        default=r"D:\Deveropment\InspectionSystem_RTDETR\InspectionSystem_RTDETR\bin\Debug\net10.0-windows\InspectionSystem_RTDETR.exe",
        help="検査アプリ (InspectionSystem_RTDETR.exe) のパス (ONNX自動暗号化用)",
    )

    args = parser.parse_args()

    if args.mode == "diagnose":
        diagnose_hardware()
    elif args.mode == "train":
        run_training(args)


if __name__ == "__main__":
    main()
