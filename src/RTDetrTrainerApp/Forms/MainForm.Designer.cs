namespace RTDetrTrainerApp.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpPaths = new System.Windows.Forms.GroupBox();
            this.lblPythonPath = new System.Windows.Forms.Label();
            this.txtPythonPath = new System.Windows.Forms.TextBox();
            this.btnBrowsePython = new System.Windows.Forms.Button();
            this.lblRtdetrRoot = new System.Windows.Forms.Label();
            this.txtRtdetrRoot = new System.Windows.Forms.TextBox();
            this.btnBrowseRtdetr = new System.Windows.Forms.Button();
            this.lblDataset = new System.Windows.Forms.Label();
            this.txtDatasetDir = new System.Windows.Forms.TextBox();
            this.btnBrowseDataset = new System.Windows.Forms.Button();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.lblDeploy = new System.Windows.Forms.Label();
            this.txtDeployDir = new System.Windows.Forms.TextBox();
            this.btnBrowseDeploy = new System.Windows.Forms.Button();

            this.grpParams = new System.Windows.Forms.GroupBox();
            this.lblConfig = new System.Windows.Forms.Label();
            this.txtConfigFile = new System.Windows.Forms.TextBox();
            this.lblEpochs = new System.Windows.Forms.Label();
            this.numEpochs = new System.Windows.Forms.NumericUpDown();
            this.lblBatchSize = new System.Windows.Forms.Label();
            this.numBatchSize = new System.Windows.Forms.NumericUpDown();
            this.lblWorkers = new System.Windows.Forms.Label();
            this.numWorkers = new System.Windows.Forms.NumericUpDown();
            this.lblInputSize = new System.Windows.Forms.Label();
            this.numInputSize = new System.Windows.Forms.NumericUpDown();
            this.chkUseAmp = new System.Windows.Forms.CheckBox();
            this.chkDummyMode = new System.Windows.Forms.CheckBox();

            this.grpAction = new System.Windows.Forms.GroupBox();
            this.btnDiagnose = new System.Windows.Forms.Button();
            this.btnStartTrain = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.grpLog = new System.Windows.Forms.GroupBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.pnlLogToolbar = new System.Windows.Forms.Panel();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnSaveLog = new System.Windows.Forms.Button();

            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();

            this.grpPaths.SuspendLayout();
            this.grpParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEpochs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBatchSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWorkers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInputSize)).BeginInit();
            this.grpAction.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.pnlLogToolbar.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            // 
            // grpPaths
            // 
            this.grpPaths.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPaths.Controls.Add(this.lblPythonPath);
            this.grpPaths.Controls.Add(this.txtPythonPath);
            this.grpPaths.Controls.Add(this.btnBrowsePython);
            this.grpPaths.Controls.Add(this.lblRtdetrRoot);
            this.grpPaths.Controls.Add(this.txtRtdetrRoot);
            this.grpPaths.Controls.Add(this.btnBrowseRtdetr);
            this.grpPaths.Controls.Add(this.lblDataset);
            this.grpPaths.Controls.Add(this.txtDatasetDir);
            this.grpPaths.Controls.Add(this.btnBrowseDataset);
            this.grpPaths.Controls.Add(this.lblOutput);
            this.grpPaths.Controls.Add(this.txtOutputDir);
            this.grpPaths.Controls.Add(this.btnBrowseOutput);
            this.grpPaths.Controls.Add(this.lblDeploy);
            this.grpPaths.Controls.Add(this.txtDeployDir);
            this.grpPaths.Controls.Add(this.btnBrowseDeploy);
            this.grpPaths.Location = new System.Drawing.Point(12, 12);
            this.grpPaths.Name = "grpPaths";
            this.grpPaths.Size = new System.Drawing.Size(984, 185);
            this.grpPaths.TabIndex = 0;
            this.grpPaths.TabStop = false;
            this.grpPaths.Text = "環境・パス設定";

            // lblPythonPath / txtPythonPath / btnBrowsePython
            this.lblPythonPath.AutoSize = true;
            this.lblPythonPath.Location = new System.Drawing.Point(15, 25);
            this.lblPythonPath.Name = "lblPythonPath";
            this.lblPythonPath.Size = new System.Drawing.Size(120, 15);
            this.lblPythonPath.Text = "Conda/Python環境:";
            this.txtPythonPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPythonPath.Location = new System.Drawing.Point(145, 22);
            this.txtPythonPath.Name = "txtPythonPath";
            this.txtPythonPath.PlaceholderText = "python.exe のパス または Conda環境フォルダ (未指定時はシステム既定)";
            this.txtPythonPath.Size = new System.Drawing.Size(745, 23);
            this.txtPythonPath.TabIndex = 1;
            this.btnBrowsePython.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowsePython.Location = new System.Drawing.Point(896, 21);
            this.btnBrowsePython.Name = "btnBrowsePython";
            this.btnBrowsePython.Size = new System.Drawing.Size(75, 25);
            this.btnBrowsePython.TabIndex = 2;
            this.btnBrowsePython.Text = "参照...";
            this.btnBrowsePython.UseVisualStyleBackColor = true;
            this.btnBrowsePython.Click += new System.EventHandler(this.BtnBrowsePython_Click);

            // lblRtdetrRoot / txtRtdetrRoot / btnBrowseRtdetr
            this.lblRtdetrRoot.AutoSize = true;
            this.lblRtdetrRoot.Location = new System.Drawing.Point(15, 56);
            this.lblRtdetrRoot.Name = "lblRtdetrRoot";
            this.lblRtdetrRoot.Size = new System.Drawing.Size(117, 15);
            this.lblRtdetrRoot.Text = "RT-DETRv2 ルート:";
            this.txtRtdetrRoot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRtdetrRoot.Location = new System.Drawing.Point(145, 53);
            this.txtRtdetrRoot.Name = "txtRtdetrRoot";
            this.txtRtdetrRoot.Size = new System.Drawing.Size(745, 23);
            this.txtRtdetrRoot.TabIndex = 3;
            this.btnBrowseRtdetr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseRtdetr.Location = new System.Drawing.Point(896, 52);
            this.btnBrowseRtdetr.Name = "btnBrowseRtdetr";
            this.btnBrowseRtdetr.Size = new System.Drawing.Size(75, 25);
            this.btnBrowseRtdetr.TabIndex = 4;
            this.btnBrowseRtdetr.Text = "参照...";
            this.btnBrowseRtdetr.UseVisualStyleBackColor = true;
            this.btnBrowseRtdetr.Click += new System.EventHandler(this.BtnBrowseRtdetr_Click);

            // lblDataset / txtDatasetDir / btnBrowseDataset
            this.lblDataset.AutoSize = true;
            this.lblDataset.Location = new System.Drawing.Point(15, 87);
            this.lblDataset.Name = "lblDataset";
            this.lblDataset.Size = new System.Drawing.Size(107, 15);
            this.lblDataset.Text = "データセットフォルダ:";
            this.txtDatasetDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatasetDir.Location = new System.Drawing.Point(145, 84);
            this.txtDatasetDir.Name = "txtDatasetDir";
            this.txtDatasetDir.Size = new System.Drawing.Size(745, 23);
            this.txtDatasetDir.TabIndex = 5;
            this.btnBrowseDataset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseDataset.Location = new System.Drawing.Point(896, 83);
            this.btnBrowseDataset.Name = "btnBrowseDataset";
            this.btnBrowseDataset.Size = new System.Drawing.Size(75, 25);
            this.btnBrowseDataset.TabIndex = 6;
            this.btnBrowseDataset.Text = "参照...";
            this.btnBrowseDataset.UseVisualStyleBackColor = true;
            this.btnBrowseDataset.Click += new System.EventHandler(this.BtnBrowseDataset_Click);

            // lblOutput / txtOutputDir / btnBrowseOutput
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(15, 118);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(107, 15);
            this.lblOutput.Text = "学習出力フォルダ:";
            this.txtOutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputDir.Location = new System.Drawing.Point(145, 115);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(745, 23);
            this.txtOutputDir.TabIndex = 7;
            this.btnBrowseOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutput.Location = new System.Drawing.Point(896, 114);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(75, 25);
            this.btnBrowseOutput.TabIndex = 8;
            this.btnBrowseOutput.Text = "参照...";
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            this.btnBrowseOutput.Click += new System.EventHandler(this.BtnBrowseOutput_Click);

            // lblDeploy / txtDeployDir / btnBrowseDeploy
            this.lblDeploy.AutoSize = true;
            this.lblDeploy.Location = new System.Drawing.Point(15, 149);
            this.lblDeploy.Name = "lblDeploy";
            this.lblDeploy.Size = new System.Drawing.Size(107, 15);
            this.lblDeploy.Text = "配信先フォルダ:";
            this.txtDeployDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeployDir.Location = new System.Drawing.Point(145, 146);
            this.txtDeployDir.Name = "txtDeployDir";
            this.txtDeployDir.Size = new System.Drawing.Size(745, 23);
            this.txtDeployDir.TabIndex = 9;
            this.btnBrowseDeploy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseDeploy.Location = new System.Drawing.Point(896, 145);
            this.btnBrowseDeploy.Name = "btnBrowseDeploy";
            this.btnBrowseDeploy.Size = new System.Drawing.Size(75, 25);
            this.btnBrowseDeploy.TabIndex = 10;
            this.btnBrowseDeploy.Text = "参照...";
            this.btnBrowseDeploy.UseVisualStyleBackColor = true;
            this.btnBrowseDeploy.Click += new System.EventHandler(this.BtnBrowseDeploy_Click);

            // 
            // grpParams
            // 
            this.grpParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpParams.Controls.Add(this.lblConfig);
            this.grpParams.Controls.Add(this.txtConfigFile);
            this.grpParams.Controls.Add(this.lblEpochs);
            this.grpParams.Controls.Add(this.numEpochs);
            this.grpParams.Controls.Add(this.lblBatchSize);
            this.grpParams.Controls.Add(this.numBatchSize);
            this.grpParams.Controls.Add(this.lblWorkers);
            this.grpParams.Controls.Add(this.numWorkers);
            this.grpParams.Controls.Add(this.lblInputSize);
            this.grpParams.Controls.Add(this.numInputSize);
            this.grpParams.Controls.Add(this.chkUseAmp);
            this.grpParams.Location = new System.Drawing.Point(12, 203);
            this.grpParams.Name = "grpParams";
            this.grpParams.Size = new System.Drawing.Size(984, 95);
            this.grpParams.TabIndex = 1;
            this.grpParams.TabStop = false;
            this.grpParams.Text = "学習パラメータ設定";

            // lblConfig / txtConfigFile
            this.lblConfig.AutoSize = true;
            this.lblConfig.Location = new System.Drawing.Point(15, 26);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(74, 15);
            this.lblConfig.Text = "設定YAML:";
            this.txtConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConfigFile.Location = new System.Drawing.Point(95, 23);
            this.txtConfigFile.Name = "txtConfigFile";
            this.txtConfigFile.Size = new System.Drawing.Size(876, 23);
            this.txtConfigFile.TabIndex = 1;
            this.txtConfigFile.Text = "configs/rtdetrv2/rtdetrv2_r18vd_120e_coco.yml";

            // lblEpochs / numEpochs
            this.lblEpochs.AutoSize = true;
            this.lblEpochs.Location = new System.Drawing.Point(15, 60);
            this.lblEpochs.Name = "lblEpochs";
            this.lblEpochs.Size = new System.Drawing.Size(56, 15);
            this.lblEpochs.Text = "エポック数:";
            this.numEpochs.Location = new System.Drawing.Point(77, 58);
            this.numEpochs.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numEpochs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numEpochs.Name = "numEpochs";
            this.numEpochs.Size = new System.Drawing.Size(65, 23);
            this.numEpochs.TabIndex = 2;
            this.numEpochs.Value = new decimal(new int[] { 100, 0, 0, 0 });

            // lblBatchSize / numBatchSize
            this.lblBatchSize.AutoSize = true;
            this.lblBatchSize.Location = new System.Drawing.Point(165, 60);
            this.lblBatchSize.Name = "lblBatchSize";
            this.lblBatchSize.Size = new System.Drawing.Size(69, 15);
            this.lblBatchSize.Text = "バッチサイズ:";
            this.numBatchSize.Location = new System.Drawing.Point(240, 58);
            this.numBatchSize.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            this.numBatchSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numBatchSize.Name = "numBatchSize";
            this.numBatchSize.Size = new System.Drawing.Size(65, 23);
            this.numBatchSize.TabIndex = 3;
            this.numBatchSize.Value = new decimal(new int[] { 8, 0, 0, 0 });

            // lblWorkers / numWorkers
            this.lblWorkers.AutoSize = true;
            this.lblWorkers.Location = new System.Drawing.Point(330, 60);
            this.lblWorkers.Name = "lblWorkers";
            this.lblWorkers.Size = new System.Drawing.Size(57, 15);
            this.lblWorkers.Text = "ワーカー数:";
            this.numWorkers.Location = new System.Drawing.Point(393, 58);
            this.numWorkers.Maximum = new decimal(new int[] { 32, 0, 0, 0 });
            this.numWorkers.Name = "numWorkers";
            this.numWorkers.Size = new System.Drawing.Size(65, 23);
            this.numWorkers.TabIndex = 4;
            this.numWorkers.Value = new decimal(new int[] { 4, 0, 0, 0 });

            // lblInputSize / numInputSize
            this.lblInputSize.AutoSize = true;
            this.lblInputSize.Location = new System.Drawing.Point(485, 60);
            this.lblInputSize.Name = "lblInputSize";
            this.lblInputSize.Size = new System.Drawing.Size(72, 15);
            this.lblInputSize.Text = "入力解像度:";
            this.numInputSize.Location = new System.Drawing.Point(563, 58);
            this.numInputSize.Maximum = new decimal(new int[] { 1920, 0, 0, 0 });
            this.numInputSize.Minimum = new decimal(new int[] { 320, 0, 0, 0 });
            this.numInputSize.Name = "numInputSize";
            this.numInputSize.Size = new System.Drawing.Size(70, 23);
            this.numInputSize.TabIndex = 5;
            this.numInputSize.Value = new decimal(new int[] { 640, 0, 0, 0 });

            // chkUseAmp
            this.chkUseAmp.AutoSize = true;
            this.chkUseAmp.Checked = true;
            this.chkUseAmp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseAmp.Location = new System.Drawing.Point(670, 59);
            this.chkUseAmp.Name = "chkUseAmp";
            this.chkUseAmp.Size = new System.Drawing.Size(126, 19);
            this.chkUseAmp.TabIndex = 6;
            this.chkUseAmp.Text = "AMP (混合精度訓練)";
            this.chkUseAmp.UseVisualStyleBackColor = true;

            // chkDummyMode
            this.chkDummyMode.AutoSize = true;
            this.chkDummyMode.Location = new System.Drawing.Point(815, 59);
            this.chkDummyMode.Name = "chkDummyMode";
            this.chkDummyMode.Size = new System.Drawing.Size(145, 19);
            this.chkDummyMode.TabIndex = 7;
            this.chkDummyMode.Text = "ダミー動作テストモード";
            this.chkDummyMode.UseVisualStyleBackColor = true;

            this.grpParams.Controls.Add(this.chkDummyMode);

            // 
            // grpAction
            // 
            this.grpAction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAction.Controls.Add(this.btnDiagnose);
            this.grpAction.Controls.Add(this.btnStartTrain);
            this.grpAction.Controls.Add(this.btnCancel);
            this.grpAction.Location = new System.Drawing.Point(12, 304);
            this.grpAction.Name = "grpAction";
            this.grpAction.Size = new System.Drawing.Size(984, 65);
            this.grpAction.TabIndex = 2;
            this.grpAction.TabStop = false;
            this.grpAction.Text = "操作";

            // btnDiagnose
            this.btnDiagnose.Location = new System.Drawing.Point(15, 22);
            this.btnDiagnose.Name = "btnDiagnose";
            this.btnDiagnose.Size = new System.Drawing.Size(200, 32);
            this.btnDiagnose.TabIndex = 0;
            this.btnDiagnose.Text = "🔍 スペック診断 ＆ 自動設定";
            this.btnDiagnose.UseVisualStyleBackColor = true;
            this.btnDiagnose.Click += new System.EventHandler(this.BtnDiagnose_Click);

            // btnStartTrain
            this.btnStartTrain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            this.btnStartTrain.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStartTrain.Location = new System.Drawing.Point(230, 22);
            this.btnStartTrain.Name = "btnStartTrain";
            this.btnStartTrain.Size = new System.Drawing.Size(220, 32);
            this.btnStartTrain.TabIndex = 1;
            this.btnStartTrain.Text = "🚀 学習＆ONNXエクスポート開始";
            this.btnStartTrain.UseVisualStyleBackColor = false;
            this.btnStartTrain.Click += new System.EventHandler(this.BtnStartTrain_Click);

            // btnCancel
            this.btnCancel.Enabled = false;
            this.btnCancel.ForeColor = System.Drawing.Color.DarkRed;
            this.btnCancel.Location = new System.Drawing.Point(465, 22);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 32);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "🛑 中断";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);

            // 
            // grpLog
            // 
            this.grpLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLog.Controls.Add(this.rtbLog);
            this.grpLog.Controls.Add(this.pnlLogToolbar);
            this.grpLog.Location = new System.Drawing.Point(12, 375);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(984, 305);
            this.grpLog.TabIndex = 3;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "実行ログ";

            // pnlLogToolbar
            this.pnlLogToolbar.Controls.Add(this.btnClearLog);
            this.pnlLogToolbar.Controls.Add(this.btnSaveLog);
            this.pnlLogToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogToolbar.Location = new System.Drawing.Point(3, 19);
            this.pnlLogToolbar.Name = "pnlLogToolbar";
            this.pnlLogToolbar.Size = new System.Drawing.Size(978, 30);
            this.pnlLogToolbar.TabIndex = 1;

            // btnClearLog
            this.btnClearLog.Location = new System.Drawing.Point(3, 3);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(75, 23);
            this.btnClearLog.TabIndex = 0;
            this.btnClearLog.Text = "ログ消去";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.BtnClearLog_Click);

            // btnSaveLog
            this.btnSaveLog.Location = new System.Drawing.Point(84, 3);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(85, 23);
            this.btnSaveLog.TabIndex = 1;
            this.btnSaveLog.Text = "ログを保存...";
            this.btnSaveLog.UseVisualStyleBackColor = true;
            this.btnSaveLog.Click += new System.EventHandler(this.BtnSaveLog_Click);

            // rtbLog
            this.rtbLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.rtbLog.ForeColor = System.Drawing.Color.Gainsboro;
            this.rtbLog.Location = new System.Drawing.Point(3, 49);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(978, 253);
            this.rtbLog.TabIndex = 0;
            this.rtbLog.Text = "";

            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 687);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip.TabIndex = 4;

            // lblStatus
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(43, 17);
            this.lblStatus.Text = "待機中";

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 709);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpAction);
            this.Controls.Add(this.grpParams);
            this.Controls.Add(this.grpPaths);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RT-DETRv2 モデル作成 ＆ ONNXエクスポートマネージャー";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

            this.grpPaths.ResumeLayout(false);
            this.grpPaths.PerformLayout();
            this.grpParams.ResumeLayout(false);
            this.grpParams.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEpochs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBatchSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWorkers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numInputSize)).EndInit();
            this.grpAction.ResumeLayout(false);
            this.grpLog.ResumeLayout(false);
            this.pnlLogToolbar.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpPaths;
        private System.Windows.Forms.Label lblPythonPath;
        private System.Windows.Forms.TextBox txtPythonPath;
        private System.Windows.Forms.Button btnBrowsePython;
        private System.Windows.Forms.Label lblRtdetrRoot;
        private System.Windows.Forms.TextBox txtRtdetrRoot;
        private System.Windows.Forms.Button btnBrowseRtdetr;
        private System.Windows.Forms.Label lblDataset;
        private System.Windows.Forms.TextBox txtDatasetDir;
        private System.Windows.Forms.Button btnBrowseDataset;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.Label lblDeploy;
        private System.Windows.Forms.TextBox txtDeployDir;
        private System.Windows.Forms.Button btnBrowseDeploy;

        private System.Windows.Forms.GroupBox grpParams;
        private System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.TextBox txtConfigFile;
        private System.Windows.Forms.Label lblEpochs;
        private System.Windows.Forms.NumericUpDown numEpochs;
        private System.Windows.Forms.Label lblBatchSize;
        private System.Windows.Forms.NumericUpDown numBatchSize;
        private System.Windows.Forms.Label lblWorkers;
        private System.Windows.Forms.NumericUpDown numWorkers;
        private System.Windows.Forms.Label lblInputSize;
        private System.Windows.Forms.NumericUpDown numInputSize;
        private System.Windows.Forms.CheckBox chkUseAmp;
        private System.Windows.Forms.CheckBox chkDummyMode;

        private System.Windows.Forms.GroupBox grpAction;
        private System.Windows.Forms.Button btnDiagnose;
        private System.Windows.Forms.Button btnStartTrain;
        private System.Windows.Forms.Button btnCancel;

        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.Panel pnlLogToolbar;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnSaveLog;
        private System.Windows.Forms.RichTextBox rtbLog;

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}
