using System.Text.Json.Serialization;

namespace RTDetrTrainerApp.Core
{
    /// <summary>
    /// ハードウェア診断結果モデル (auto_train.py の [SPEC_JSON] に対応)
    /// </summary>
    public class HardwareDiagnosisResult
    {
        [JsonPropertyName("gpu_available")]
        public bool GpuAvailable { get; set; }

        [JsonPropertyName("gpu_name")]
        public string GpuName { get; set; } = string.Empty;

        [JsonPropertyName("cuda_version")]
        public string CudaVersion { get; set; } = string.Empty;

        [JsonPropertyName("vram_gb")]
        public double VramGb { get; set; }

        [JsonPropertyName("cpu_cores")]
        public int CpuCores { get; set; }

        [JsonPropertyName("recommended_batch_size")]
        public int RecommendedBatchSize { get; set; } = 8;

        [JsonPropertyName("recommended_num_workers")]
        public int RecommendedNumWorkers { get; set; } = 4;

        [JsonPropertyName("recommended_use_amp")]
        public bool RecommendedUseAmp { get; set; } = true;
    }

    /// <summary>
    /// 学習・エクスポート実行パラメータ
    /// </summary>
    public class TrainParameters
    {
        public string PythonExecutablePath { get; set; } = string.Empty;
        public string AutoTrainScriptPath { get; set; } = string.Empty;
        public string RtdetrRoot { get; set; } = @"D:\Deveropment\RT-DETR_Models\python\rtdetrv2";
        public string ConfigFile { get; set; } = @"configs/rtdetrv2/rtdetrv2_r18vd_120e_coco.yml";
        public string DatasetDir { get; set; } = @"D:\Deveropment\RT-DETR_Models\datasets";
        public string OutputDir { get; set; } = @"D:\Deveropment\RT-DETR_Models\outputs";
        public string DeployDir { get; set; } = @"D:\Deveropment\RT-DETR_Models\deploy";
        public int Epochs { get; set; } = 100;
        public int BatchSize { get; set; } = 8;
        public int NumWorkers { get; set; } = 4;
        public bool UseAmp { get; set; } = true;
        public int InputSize { get; set; } = 640;
        public string? TuningWeightPath { get; set; }
        public string? ResumeWeightPath { get; set; }
        public bool IsDummy { get; set; } = false;
        public string InspectionExePath { get; set; } = @"D:\Deveropment\InspectionSystem_RTDETR\InspectionSystem_RTDETR\bin\Debug\net10.0-windows\InspectionSystem_RTDETR.exe";
    }
}
