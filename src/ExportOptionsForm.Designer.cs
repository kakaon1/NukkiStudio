namespace NukkiStudio.App
{
    partial class ExportOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportOptionsForm));
            tblMain = new TableLayoutPanel();
            lblFormatTitle = new Label();
            flpFormat = new FlowLayoutPanel();
            rbPng = new RadioButton();
            rbJpg = new RadioButton();
            rbBmp = new RadioButton();
            tblQuality = new TableLayoutPanel();
            lblQuality = new Label();
            lblQualityValue = new Label();
            trkQuality = new NukkiStudio.App.Controls.FlatSlider();
            lblSizeTitle = new Label();
            tblSize = new TableLayoutPanel();
            rbSizeOriginal = new RadioButton();
            rbSizePercent = new RadioButton();
            trkPercent = new NukkiStudio.App.Controls.FlatSlider();
            lblPercentValue = new Label();
            rbSizeMax = new RadioButton();
            numMaxSide = new NumericUpDown();
            lblPx = new Label();
            lblLocationTitle = new Label();
            tblLocation = new TableLayoutPanel();
            rbLocSource = new RadioButton();
            rbLocCustom = new RadioButton();
            txtFolder = new TextBox();
            btnBrowse = new Button();
            lblExample = new Label();
            pnlFooter = new Panel();
            btnCancel = new Button();
            btnOk = new Button();
            tblMain.SuspendLayout();
            flpFormat.SuspendLayout();
            tblQuality.SuspendLayout();
            tblSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMaxSide).BeginInit();
            tblLocation.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            //
            // tblMain
            //
            tblMain.BackColor = Color.FromArgb(24, 24, 27);
            tblMain.ColumnCount = 1;
            tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblMain.Controls.Add(lblFormatTitle, 0, 0);
            tblMain.Controls.Add(flpFormat, 0, 1);
            tblMain.Controls.Add(tblQuality, 0, 2);
            tblMain.Controls.Add(lblSizeTitle, 0, 3);
            tblMain.Controls.Add(tblSize, 0, 4);
            tblMain.Controls.Add(lblLocationTitle, 0, 5);
            tblMain.Controls.Add(tblLocation, 0, 6);
            tblMain.Controls.Add(lblExample, 0, 7);
            tblMain.Dock = DockStyle.Fill;
            tblMain.Location = new Point(0, 0);
            tblMain.Name = "tblMain";
            tblMain.Padding = new Padding(24, 16, 24, 8);
            tblMain.RowCount = 8;
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 108F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 106F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblMain.Size = new Size(640, 516);
            tblMain.TabIndex = 0;
            //
            // lblFormatTitle
            //
            lblFormatTitle.Dock = DockStyle.Fill;
            lblFormatTitle.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            lblFormatTitle.ForeColor = Color.FromArgb(160, 160, 170);
            lblFormatTitle.Location = new Point(24, 16);
            lblFormatTitle.Margin = new Padding(0);
            lblFormatTitle.Name = "lblFormatTitle";
            lblFormatTitle.Size = new Size(592, 30);
            lblFormatTitle.TabIndex = 0;
            lblFormatTitle.Text = "파일 형식";
            lblFormatTitle.TextAlign = ContentAlignment.BottomLeft;
            //
            // flpFormat
            //
            flpFormat.Controls.Add(rbPng);
            flpFormat.Controls.Add(rbJpg);
            flpFormat.Controls.Add(rbBmp);
            flpFormat.Dock = DockStyle.Fill;
            flpFormat.Location = new Point(24, 46);
            flpFormat.Margin = new Padding(0);
            flpFormat.Name = "flpFormat";
            flpFormat.Size = new Size(592, 36);
            flpFormat.TabIndex = 1;
            //
            // rbPng
            //
            rbPng.Checked = true;
            rbPng.ForeColor = Color.FromArgb(236, 236, 239);
            rbPng.Location = new Point(3, 6);
            rbPng.Margin = new Padding(3, 6, 12, 3);
            rbPng.Name = "rbPng";
            rbPng.Size = new Size(190, 24);
            rbPng.TabIndex = 0;
            rbPng.TabStop = true;
            rbPng.Text = "PNG (투명 지원 · 무손실)";
            rbPng.UseVisualStyleBackColor = true;
            rbPng.CheckedChanged += Option_Changed;
            //
            // rbJpg
            //
            rbJpg.ForeColor = Color.FromArgb(236, 236, 239);
            rbJpg.Location = new Point(208, 6);
            rbJpg.Margin = new Padding(3, 6, 12, 3);
            rbJpg.Name = "rbJpg";
            rbJpg.Size = new Size(170, 24);
            rbJpg.TabIndex = 1;
            rbJpg.Text = "JPG (용량 작음)";
            rbJpg.UseVisualStyleBackColor = true;
            rbJpg.CheckedChanged += Option_Changed;
            //
            // rbBmp
            //
            rbBmp.ForeColor = Color.FromArgb(236, 236, 239);
            rbBmp.Location = new Point(393, 6);
            rbBmp.Margin = new Padding(3, 6, 3, 3);
            rbBmp.Name = "rbBmp";
            rbBmp.Size = new Size(170, 24);
            rbBmp.TabIndex = 2;
            rbBmp.Text = "BMP (무압축 · 원본 화질)";
            rbBmp.UseVisualStyleBackColor = true;
            rbBmp.CheckedChanged += Option_Changed;
            //
            // tblQuality
            //
            tblQuality.ColumnCount = 2;
            tblQuality.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblQuality.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tblQuality.Controls.Add(lblQuality, 0, 0);
            tblQuality.Controls.Add(lblQualityValue, 1, 0);
            tblQuality.Controls.Add(trkQuality, 0, 1);
            tblQuality.Dock = DockStyle.Fill;
            tblQuality.Location = new Point(24, 82);
            tblQuality.Margin = new Padding(0);
            tblQuality.Name = "tblQuality";
            tblQuality.RowCount = 2;
            tblQuality.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tblQuality.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblQuality.Size = new Size(592, 60);
            tblQuality.TabIndex = 2;
            //
            // lblQuality
            //
            lblQuality.Dock = DockStyle.Fill;
            lblQuality.ForeColor = Color.FromArgb(160, 160, 170);
            lblQuality.Location = new Point(0, 0);
            lblQuality.Margin = new Padding(0);
            lblQuality.Name = "lblQuality";
            lblQuality.Size = new Size(512, 28);
            lblQuality.TabIndex = 0;
            lblQuality.Text = "JPG 품질 (높을수록 화질 좋고 용량 큼)";
            lblQuality.TextAlign = ContentAlignment.BottomLeft;
            //
            // lblQualityValue
            //
            lblQualityValue.Dock = DockStyle.Fill;
            lblQualityValue.ForeColor = Color.FromArgb(236, 236, 239);
            lblQualityValue.Location = new Point(512, 0);
            lblQualityValue.Margin = new Padding(0);
            lblQualityValue.Name = "lblQualityValue";
            lblQualityValue.Size = new Size(80, 28);
            lblQualityValue.TabIndex = 1;
            lblQualityValue.Text = "92";
            lblQualityValue.TextAlign = ContentAlignment.BottomRight;
            //
            // trkQuality
            //
            tblQuality.SetColumnSpan(trkQuality, 2);
            trkQuality.Dock = DockStyle.Fill;
            trkQuality.Location = new Point(0, 28);
            trkQuality.Margin = new Padding(0);
            trkQuality.Minimum = 10;
            trkQuality.Name = "trkQuality";
            trkQuality.Size = new Size(592, 32);
            trkQuality.TabIndex = 2;
            trkQuality.Value = 92;
            trkQuality.ValueChanged += Option_Changed;
            //
            // lblSizeTitle
            //
            lblSizeTitle.Dock = DockStyle.Fill;
            lblSizeTitle.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            lblSizeTitle.ForeColor = Color.FromArgb(160, 160, 170);
            lblSizeTitle.Location = new Point(24, 142);
            lblSizeTitle.Margin = new Padding(0);
            lblSizeTitle.Name = "lblSizeTitle";
            lblSizeTitle.Size = new Size(592, 34);
            lblSizeTitle.TabIndex = 3;
            lblSizeTitle.Text = "크기 (용량)";
            lblSizeTitle.TextAlign = ContentAlignment.BottomLeft;
            //
            // tblSize
            //
            tblSize.ColumnCount = 3;
            tblSize.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            tblSize.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblSize.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tblSize.Controls.Add(rbSizeOriginal, 0, 0);
            tblSize.Controls.Add(rbSizePercent, 0, 1);
            tblSize.Controls.Add(trkPercent, 1, 1);
            tblSize.Controls.Add(lblPercentValue, 2, 1);
            tblSize.Controls.Add(rbSizeMax, 0, 2);
            tblSize.Controls.Add(numMaxSide, 1, 2);
            tblSize.Controls.Add(lblPx, 2, 2);
            tblSize.Dock = DockStyle.Fill;
            tblSize.Location = new Point(24, 176);
            tblSize.Margin = new Padding(0);
            tblSize.Name = "tblSize";
            tblSize.RowCount = 3;
            tblSize.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tblSize.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tblSize.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tblSize.Size = new Size(592, 108);
            tblSize.TabIndex = 4;
            //
            // rbSizeOriginal
            //
            rbSizeOriginal.Checked = true;
            tblSize.SetColumnSpan(rbSizeOriginal, 3);
            rbSizeOriginal.Dock = DockStyle.Fill;
            rbSizeOriginal.ForeColor = Color.FromArgb(236, 236, 239);
            rbSizeOriginal.Location = new Point(3, 3);
            rbSizeOriginal.Name = "rbSizeOriginal";
            rbSizeOriginal.Size = new Size(586, 30);
            rbSizeOriginal.TabIndex = 0;
            rbSizeOriginal.TabStop = true;
            rbSizeOriginal.Text = "원본 크기 그대로";
            rbSizeOriginal.UseVisualStyleBackColor = true;
            rbSizeOriginal.CheckedChanged += Option_Changed;
            //
            // rbSizePercent
            //
            rbSizePercent.Dock = DockStyle.Fill;
            rbSizePercent.ForeColor = Color.FromArgb(236, 236, 239);
            rbSizePercent.Location = new Point(3, 39);
            rbSizePercent.Name = "rbSizePercent";
            rbSizePercent.Size = new Size(154, 30);
            rbSizePercent.TabIndex = 1;
            rbSizePercent.Text = "비율 (5~400%)";
            rbSizePercent.UseVisualStyleBackColor = true;
            rbSizePercent.CheckedChanged += Option_Changed;
            //
            // trkPercent
            //
            trkPercent.Dock = DockStyle.Fill;
            trkPercent.LargeChange = 10;
            trkPercent.Location = new Point(163, 39);
            trkPercent.Maximum = 400;
            trkPercent.Minimum = 5;
            trkPercent.Name = "trkPercent";
            trkPercent.Size = new Size(356, 30);
            trkPercent.SmallChange = 5;
            trkPercent.TabIndex = 2;
            trkPercent.Value = 50;
            trkPercent.ValueChanged += Option_Changed;
            //
            // lblPercentValue
            //
            lblPercentValue.Dock = DockStyle.Fill;
            lblPercentValue.ForeColor = Color.FromArgb(236, 236, 239);
            lblPercentValue.Location = new Point(525, 36);
            lblPercentValue.Name = "lblPercentValue";
            lblPercentValue.Size = new Size(64, 36);
            lblPercentValue.TabIndex = 3;
            lblPercentValue.Text = "50%";
            lblPercentValue.TextAlign = ContentAlignment.MiddleRight;
            //
            // rbSizeMax
            //
            rbSizeMax.Dock = DockStyle.Fill;
            rbSizeMax.ForeColor = Color.FromArgb(236, 236, 239);
            rbSizeMax.Location = new Point(3, 75);
            rbSizeMax.Name = "rbSizeMax";
            rbSizeMax.Size = new Size(154, 30);
            rbSizeMax.TabIndex = 4;
            rbSizeMax.Text = "긴 변 최대 크기";
            rbSizeMax.UseVisualStyleBackColor = true;
            rbSizeMax.CheckedChanged += Option_Changed;
            //
            // numMaxSide
            //
            numMaxSide.Anchor = AnchorStyles.Left;
            numMaxSide.BackColor = Color.FromArgb(50, 50, 57);
            numMaxSide.BorderStyle = BorderStyle.FixedSingle;
            numMaxSide.ForeColor = Color.FromArgb(236, 236, 239);
            numMaxSide.Increment = new decimal(new int[] { 64, 0, 0, 0 });
            numMaxSide.Location = new Point(163, 79);
            numMaxSide.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
            numMaxSide.Minimum = new decimal(new int[] { 64, 0, 0, 0 });
            numMaxSide.Name = "numMaxSide";
            numMaxSide.Size = new Size(120, 23);
            numMaxSide.TabIndex = 5;
            numMaxSide.Value = new decimal(new int[] { 2048, 0, 0, 0 });
            numMaxSide.ValueChanged += Option_Changed;
            //
            // lblPx
            //
            lblPx.Dock = DockStyle.Fill;
            lblPx.ForeColor = Color.FromArgb(160, 160, 170);
            lblPx.Location = new Point(525, 72);
            lblPx.Name = "lblPx";
            lblPx.Size = new Size(64, 36);
            lblPx.TabIndex = 6;
            lblPx.Text = "px";
            lblPx.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblLocationTitle
            //
            lblLocationTitle.Dock = DockStyle.Fill;
            lblLocationTitle.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            lblLocationTitle.ForeColor = Color.FromArgb(160, 160, 170);
            lblLocationTitle.Location = new Point(24, 284);
            lblLocationTitle.Margin = new Padding(0);
            lblLocationTitle.Name = "lblLocationTitle";
            lblLocationTitle.Size = new Size(592, 34);
            lblLocationTitle.TabIndex = 5;
            lblLocationTitle.Text = "저장 위치";
            lblLocationTitle.TextAlign = ContentAlignment.BottomLeft;
            //
            // tblLocation
            //
            tblLocation.ColumnCount = 2;
            tblLocation.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblLocation.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96F));
            tblLocation.Controls.Add(rbLocSource, 0, 0);
            tblLocation.Controls.Add(rbLocCustom, 0, 1);
            tblLocation.Controls.Add(txtFolder, 0, 2);
            tblLocation.Controls.Add(btnBrowse, 1, 2);
            tblLocation.Dock = DockStyle.Fill;
            tblLocation.Location = new Point(24, 318);
            tblLocation.Margin = new Padding(0);
            tblLocation.Name = "tblLocation";
            tblLocation.RowCount = 3;
            tblLocation.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblLocation.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblLocation.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tblLocation.Size = new Size(592, 106);
            tblLocation.TabIndex = 6;
            //
            // rbLocSource
            //
            rbLocSource.Checked = true;
            tblLocation.SetColumnSpan(rbLocSource, 2);
            rbLocSource.Dock = DockStyle.Fill;
            rbLocSource.ForeColor = Color.FromArgb(236, 236, 239);
            rbLocSource.Location = new Point(3, 3);
            rbLocSource.Name = "rbLocSource";
            rbLocSource.Size = new Size(586, 28);
            rbLocSource.TabIndex = 0;
            rbLocSource.TabStop = true;
            rbLocSource.Text = "원본 파일이 있는 폴더 안의 output 폴더 (자동으로 만들어짐)";
            rbLocSource.UseVisualStyleBackColor = true;
            rbLocSource.CheckedChanged += Option_Changed;
            //
            // rbLocCustom
            //
            tblLocation.SetColumnSpan(rbLocCustom, 2);
            rbLocCustom.Dock = DockStyle.Fill;
            rbLocCustom.ForeColor = Color.FromArgb(236, 236, 239);
            rbLocCustom.Location = new Point(3, 37);
            rbLocCustom.Name = "rbLocCustom";
            rbLocCustom.Size = new Size(586, 28);
            rbLocCustom.TabIndex = 1;
            rbLocCustom.Text = "지정한 폴더에 저장";
            rbLocCustom.UseVisualStyleBackColor = true;
            rbLocCustom.CheckedChanged += Option_Changed;
            //
            // txtFolder
            //
            txtFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtFolder.BackColor = Color.FromArgb(50, 50, 57);
            txtFolder.BorderStyle = BorderStyle.FixedSingle;
            txtFolder.ForeColor = Color.FromArgb(236, 236, 239);
            txtFolder.Location = new Point(3, 75);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(490, 23);
            txtFolder.TabIndex = 2;
            txtFolder.TextChanged += Option_Changed;
            //
            // btnBrowse
            //
            btnBrowse.BackColor = Color.FromArgb(50, 50, 57);
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.Dock = DockStyle.Fill;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnBrowse.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.ForeColor = Color.FromArgb(236, 236, 239);
            btnBrowse.Location = new Point(499, 71);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(90, 32);
            btnBrowse.TabIndex = 3;
            btnBrowse.Text = "찾아보기...";
            btnBrowse.UseVisualStyleBackColor = false;
            btnBrowse.Click += btnBrowse_Click;
            //
            // lblExample
            //
            lblExample.Dock = DockStyle.Fill;
            lblExample.ForeColor = Color.FromArgb(122, 136, 255);
            lblExample.Location = new Point(24, 424);
            lblExample.Margin = new Padding(0, 8, 0, 0);
            lblExample.Name = "lblExample";
            lblExample.Size = new Size(592, 84);
            lblExample.TabIndex = 7;
            lblExample.Text = "예: ...";
            //
            // pnlFooter
            //
            pnlFooter.BackColor = Color.FromArgb(30, 30, 34);
            pnlFooter.Controls.Add(btnCancel);
            pnlFooter.Controls.Add(btnOk);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 516);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(640, 64);
            pnlFooter.TabIndex = 1;
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Right;
            btnCancel.BackColor = Color.FromArgb(50, 50, 57);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.ForeColor = Color.FromArgb(236, 236, 239);
            btnCancel.Location = new Point(376, 14);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(112, 36);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "취소";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // btnOk
            //
            btnOk.Anchor = AnchorStyles.Right;
            btnOk.BackColor = Color.FromArgb(99, 115, 255);
            btnOk.Cursor = Cursors.Hand;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.FlatAppearance.MouseDownBackColor = Color.FromArgb(82, 96, 228);
            btnOk.FlatAppearance.MouseOverBackColor = Color.FromArgb(122, 136, 255);
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            btnOk.ForeColor = Color.White;
            btnOk.Location = new Point(504, 14);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(112, 36);
            btnOk.TabIndex = 0;
            btnOk.Text = "저장";
            btnOk.UseVisualStyleBackColor = false;
            btnOk.Click += btnOk_Click;
            //
            // ExportOptionsForm
            //
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(24, 24, 27);
            CancelButton = btnCancel;
            ClientSize = new Size(640, 580);
            Controls.Add(tblMain);
            Controls.Add(pnlFooter);
            ForeColor = Color.FromArgb(236, 236, 239);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExportOptionsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "내보내기 설정";
            tblMain.ResumeLayout(false);
            flpFormat.ResumeLayout(false);
            tblQuality.ResumeLayout(false);
            tblSize.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numMaxSide).EndInit();
            tblLocation.ResumeLayout(false);
            tblLocation.PerformLayout();
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tblMain;
        private Label lblFormatTitle;
        private FlowLayoutPanel flpFormat;
        private RadioButton rbPng;
        private RadioButton rbJpg;
        private RadioButton rbBmp;
        private TableLayoutPanel tblQuality;
        private Label lblQuality;
        private Label lblQualityValue;
        private NukkiStudio.App.Controls.FlatSlider trkQuality;
        private Label lblSizeTitle;
        private TableLayoutPanel tblSize;
        private RadioButton rbSizeOriginal;
        private RadioButton rbSizePercent;
        private NukkiStudio.App.Controls.FlatSlider trkPercent;
        private Label lblPercentValue;
        private RadioButton rbSizeMax;
        private NumericUpDown numMaxSide;
        private Label lblPx;
        private Label lblLocationTitle;
        private TableLayoutPanel tblLocation;
        private RadioButton rbLocSource;
        private RadioButton rbLocCustom;
        private TextBox txtFolder;
        private Button btnBrowse;
        private Label lblExample;
        private Panel pnlFooter;
        private Button btnCancel;
        private Button btnOk;
    }
}
