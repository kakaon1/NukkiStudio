namespace NukkiStudio.App
{
    partial class ModelDownloadForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelDownloadForm));
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblIntro = new Label();
            tblPackages = new TableLayoutPanel();
            chkSam = new CheckBox();
            lblSamSize = new Label();
            lblSamState = new Label();
            chkDetector = new CheckBox();
            lblDetectorSize = new Label();
            lblDetectorState = new Label();
            chkMiGan = new CheckBox();
            lblMiGanSize = new Label();
            lblMiGanState = new Label();
            chkLama = new CheckBox();
            lblLamaSize = new Label();
            lblLamaState = new Label();
            pnlProgress = new Panel();
            lblProgress = new Label();
            prgDownload = new ProgressBar();
            pnlFooter = new Panel();
            lblTotal = new Label();
            btnDownload = new Button();
            btnClose = new Button();
            pnlHeader.SuspendLayout();
            tblPackages.SuspendLayout();
            pnlProgress.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            //
            // pnlHeader
            //
            pnlHeader.BackColor = Color.FromArgb(30, 30, 34);
            pnlHeader.Controls.Add(lblIntro);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(24, 16, 24, 12);
            pnlHeader.Size = new Size(640, 104);
            pnlHeader.TabIndex = 0;
            //
            // lblTitle
            //
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Malgun Gothic", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(236, 236, 239);
            lblTitle.Location = new Point(24, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(592, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "AI 모델 다운로드";
            //
            // lblIntro
            //
            lblIntro.Dock = DockStyle.Fill;
            lblIntro.ForeColor = Color.FromArgb(160, 160, 170);
            lblIntro.Location = new Point(24, 46);
            lblIntro.Name = "lblIntro";
            lblIntro.Size = new Size(592, 46);
            lblIntro.TabIndex = 1;
            lblIntro.Text = "받을 모델을 고른 뒤 [다운로드]를 누르세요. 데이터 요금(핫스팟 등)에 주의하세요.\r\n받은 모델은 아래에 표시된 models 폴더에 저장되며 다시 받을 필요가 없습니다.";
            //
            // tblPackages
            //
            tblPackages.BackColor = Color.FromArgb(24, 24, 27);
            tblPackages.ColumnCount = 3;
            tblPackages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblPackages.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tblPackages.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tblPackages.Controls.Add(chkSam, 0, 0);
            tblPackages.Controls.Add(lblSamSize, 1, 0);
            tblPackages.Controls.Add(lblSamState, 2, 0);
            tblPackages.Controls.Add(chkDetector, 0, 1);
            tblPackages.Controls.Add(lblDetectorSize, 1, 1);
            tblPackages.Controls.Add(lblDetectorState, 2, 1);
            tblPackages.Controls.Add(chkMiGan, 0, 2);
            tblPackages.Controls.Add(lblMiGanSize, 1, 2);
            tblPackages.Controls.Add(lblMiGanState, 2, 2);
            tblPackages.Controls.Add(chkLama, 0, 3);
            tblPackages.Controls.Add(lblLamaSize, 1, 3);
            tblPackages.Controls.Add(lblLamaState, 2, 3);
            tblPackages.Dock = DockStyle.Fill;
            tblPackages.Location = new Point(0, 104);
            tblPackages.Name = "tblPackages";
            tblPackages.Padding = new Padding(20, 12, 20, 4);
            tblPackages.RowCount = 5;
            tblPackages.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tblPackages.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tblPackages.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tblPackages.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tblPackages.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblPackages.Size = new Size(640, 180);
            tblPackages.TabIndex = 1;
            //
            // chkSam
            //
            chkSam.Dock = DockStyle.Fill;
            chkSam.ForeColor = Color.FromArgb(236, 236, 239);
            chkSam.Location = new Point(23, 15);
            chkSam.Name = "chkSam";
            chkSam.Size = new Size(414, 34);
            chkSam.TabIndex = 0;
            chkSam.Text = "MobileSAM — 객체 선택";
            chkSam.UseVisualStyleBackColor = true;
            chkSam.CheckedChanged += Selection_Changed;
            //
            // lblSamSize
            //
            lblSamSize.Dock = DockStyle.Fill;
            lblSamSize.ForeColor = Color.FromArgb(160, 160, 170);
            lblSamSize.Location = new Point(443, 12);
            lblSamSize.Name = "lblSamSize";
            lblSamSize.Size = new Size(84, 40);
            lblSamSize.TabIndex = 1;
            lblSamSize.Text = "35.0 MB";
            lblSamSize.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblSamState
            //
            lblSamState.Dock = DockStyle.Fill;
            lblSamState.ForeColor = Color.FromArgb(160, 160, 170);
            lblSamState.Location = new Point(533, 12);
            lblSamState.Name = "lblSamState";
            lblSamState.Size = new Size(84, 40);
            lblSamState.TabIndex = 2;
            lblSamState.Text = "없음";
            lblSamState.TextAlign = ContentAlignment.MiddleRight;
            //
            // chkDetector
            //
            chkDetector.Dock = DockStyle.Fill;
            chkDetector.ForeColor = Color.FromArgb(236, 236, 239);
            chkDetector.Location = new Point(23, 55);
            chkDetector.Name = "chkDetector";
            chkDetector.Size = new Size(414, 34);
            chkDetector.TabIndex = 3;
            chkDetector.Text = "D-FINE-N — 객체 자동 선택";
            chkDetector.UseVisualStyleBackColor = true;
            chkDetector.CheckedChanged += Selection_Changed;
            //
            // lblDetectorSize
            //
            lblDetectorSize.Dock = DockStyle.Fill;
            lblDetectorSize.ForeColor = Color.FromArgb(160, 160, 170);
            lblDetectorSize.Location = new Point(443, 52);
            lblDetectorSize.Name = "lblDetectorSize";
            lblDetectorSize.Size = new Size(84, 40);
            lblDetectorSize.TabIndex = 4;
            lblDetectorSize.Text = "13.3 MB";
            lblDetectorSize.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblDetectorState
            //
            lblDetectorState.Dock = DockStyle.Fill;
            lblDetectorState.ForeColor = Color.FromArgb(160, 160, 170);
            lblDetectorState.Location = new Point(533, 52);
            lblDetectorState.Name = "lblDetectorState";
            lblDetectorState.Size = new Size(84, 40);
            lblDetectorState.TabIndex = 5;
            lblDetectorState.Text = "없음";
            lblDetectorState.TextAlign = ContentAlignment.MiddleRight;
            //
            // chkMiGan
            //
            chkMiGan.Dock = DockStyle.Fill;
            chkMiGan.ForeColor = Color.FromArgb(236, 236, 239);
            chkMiGan.Location = new Point(23, 95);
            chkMiGan.Name = "chkMiGan";
            chkMiGan.Size = new Size(414, 34);
            chkMiGan.TabIndex = 6;
            chkMiGan.Text = "MI-GAN — 객체 지우기 (빠름)";
            chkMiGan.UseVisualStyleBackColor = true;
            chkMiGan.CheckedChanged += Selection_Changed;
            //
            // lblMiGanSize
            //
            lblMiGanSize.Dock = DockStyle.Fill;
            lblMiGanSize.ForeColor = Color.FromArgb(160, 160, 170);
            lblMiGanSize.Location = new Point(443, 92);
            lblMiGanSize.Name = "lblMiGanSize";
            lblMiGanSize.Size = new Size(84, 40);
            lblMiGanSize.TabIndex = 7;
            lblMiGanSize.Text = "26.8 MB";
            lblMiGanSize.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblMiGanState
            //
            lblMiGanState.Dock = DockStyle.Fill;
            lblMiGanState.ForeColor = Color.FromArgb(160, 160, 170);
            lblMiGanState.Location = new Point(533, 92);
            lblMiGanState.Name = "lblMiGanState";
            lblMiGanState.Size = new Size(84, 40);
            lblMiGanState.TabIndex = 8;
            lblMiGanState.Text = "없음";
            lblMiGanState.TextAlign = ContentAlignment.MiddleRight;
            //
            // chkLama
            //
            chkLama.Dock = DockStyle.Fill;
            chkLama.ForeColor = Color.FromArgb(236, 236, 239);
            chkLama.Location = new Point(23, 135);
            chkLama.Name = "chkLama";
            chkLama.Size = new Size(414, 34);
            chkLama.TabIndex = 9;
            chkLama.Text = "LaMa — 객체 지우기 (고품질)";
            chkLama.UseVisualStyleBackColor = true;
            chkLama.CheckedChanged += Selection_Changed;
            //
            // lblLamaSize
            //
            lblLamaSize.Dock = DockStyle.Fill;
            lblLamaSize.ForeColor = Color.FromArgb(160, 160, 170);
            lblLamaSize.Location = new Point(443, 132);
            lblLamaSize.Name = "lblLamaSize";
            lblLamaSize.Size = new Size(84, 40);
            lblLamaSize.TabIndex = 10;
            lblLamaSize.Text = "88.3 MB";
            lblLamaSize.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblLamaState
            //
            lblLamaState.Dock = DockStyle.Fill;
            lblLamaState.ForeColor = Color.FromArgb(160, 160, 170);
            lblLamaState.Location = new Point(533, 132);
            lblLamaState.Name = "lblLamaState";
            lblLamaState.Size = new Size(84, 40);
            lblLamaState.TabIndex = 11;
            lblLamaState.Text = "없음";
            lblLamaState.TextAlign = ContentAlignment.MiddleRight;
            //
            // pnlProgress
            //
            pnlProgress.BackColor = Color.FromArgb(24, 24, 27);
            pnlProgress.Controls.Add(lblProgress);
            pnlProgress.Controls.Add(prgDownload);
            pnlProgress.Dock = DockStyle.Bottom;
            pnlProgress.Location = new Point(0, 284);
            pnlProgress.Name = "pnlProgress";
            pnlProgress.Padding = new Padding(24, 4, 24, 4);
            pnlProgress.Size = new Size(640, 60);
            pnlProgress.TabIndex = 2;
            //
            // lblProgress
            //
            lblProgress.Dock = DockStyle.Fill;
            lblProgress.ForeColor = Color.FromArgb(160, 160, 170);
            lblProgress.Location = new Point(24, 26);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(592, 30);
            lblProgress.TabIndex = 1;
            lblProgress.Text = "대기 중";
            lblProgress.TextAlign = ContentAlignment.MiddleLeft;
            //
            // prgDownload
            //
            prgDownload.Dock = DockStyle.Top;
            prgDownload.Location = new Point(24, 4);
            prgDownload.Maximum = 1000;
            prgDownload.Name = "prgDownload";
            prgDownload.Size = new Size(592, 22);
            prgDownload.TabIndex = 0;
            //
            // pnlFooter
            //
            pnlFooter.BackColor = Color.FromArgb(30, 30, 34);
            pnlFooter.Controls.Add(lblTotal);
            pnlFooter.Controls.Add(btnDownload);
            pnlFooter.Controls.Add(btnClose);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 344);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(640, 64);
            pnlFooter.TabIndex = 3;
            //
            // lblTotal
            //
            lblTotal.Anchor = AnchorStyles.Left;
            lblTotal.ForeColor = Color.FromArgb(236, 236, 239);
            lblTotal.Location = new Point(24, 14);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(320, 36);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "선택한 모델 합계: 0 MB";
            lblTotal.TextAlign = ContentAlignment.MiddleLeft;
            //
            // btnDownload
            //
            btnDownload.Anchor = AnchorStyles.Right;
            btnDownload.BackColor = Color.FromArgb(99, 115, 255);
            btnDownload.Cursor = Cursors.Hand;
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.FlatAppearance.MouseDownBackColor = Color.FromArgb(82, 96, 228);
            btnDownload.FlatAppearance.MouseOverBackColor = Color.FromArgb(122, 136, 255);
            btnDownload.FlatStyle = FlatStyle.Flat;
            btnDownload.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            btnDownload.ForeColor = Color.White;
            btnDownload.Location = new Point(368, 14);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(120, 36);
            btnDownload.TabIndex = 1;
            btnDownload.Text = "다운로드";
            btnDownload.UseVisualStyleBackColor = false;
            btnDownload.Click += btnDownload_Click;
            //
            // btnClose
            //
            btnClose.Anchor = AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(50, 50, 57);
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(44, 44, 49);
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.ForeColor = Color.FromArgb(236, 236, 239);
            btnClose.Location = new Point(496, 14);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(120, 36);
            btnClose.TabIndex = 2;
            btnClose.Text = "나중에";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            //
            // ModelDownloadForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(24, 24, 27);
            ClientSize = new Size(640, 408);
            Controls.Add(tblPackages);
            Controls.Add(pnlProgress);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            ForeColor = Color.FromArgb(236, 236, 239);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ModelDownloadForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "AI 모델 다운로드";
            FormClosing += ModelDownloadForm_FormClosing;
            pnlHeader.ResumeLayout(false);
            tblPackages.ResumeLayout(false);
            pnlProgress.ResumeLayout(false);
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblIntro;
        private TableLayoutPanel tblPackages;
        private CheckBox chkSam;
        private Label lblSamSize;
        private Label lblSamState;
        private CheckBox chkDetector;
        private Label lblDetectorSize;
        private Label lblDetectorState;
        private CheckBox chkMiGan;
        private Label lblMiGanSize;
        private Label lblMiGanState;
        private CheckBox chkLama;
        private Label lblLamaSize;
        private Label lblLamaState;
        private Panel pnlProgress;
        private Label lblProgress;
        private ProgressBar prgDownload;
        private Panel pnlFooter;
        private Label lblTotal;
        private Button btnDownload;
        private Button btnClose;
    }
}
