namespace NukkiStudio.App
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            pnlHeader = new Panel();
            picLogo = new PictureBox();
            lblTitle = new Label();
            lblVersion = new Label();
            lblDescription = new Label();
            pnlBody = new Panel();
            txtLicense = new RichTextBox();
            pnlListGap = new Panel();
            lstLicenses = new ListBox();
            lblLicenseCaption = new Label();
            pnlFooter = new Panel();
            lblNotice = new Label();
            btnClose = new Button();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            pnlBody.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            //
            // pnlHeader
            //
            pnlHeader.BackColor = Color.FromArgb(30, 30, 34);
            pnlHeader.Controls.Add(picLogo);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblVersion);
            pnlHeader.Controls.Add(lblDescription);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(820, 112);
            pnlHeader.TabIndex = 0;
            //
            // picLogo
            //
            picLogo.Location = new Point(24, 24);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(64, 64);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Malgun Gothic", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(236, 236, 239);
            lblTitle.Location = new Point(104, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(160, 30);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Nukki Studio";
            //
            // lblVersion
            //
            lblVersion.AutoSize = true;
            lblVersion.ForeColor = Color.FromArgb(122, 136, 255);
            lblVersion.Location = new Point(107, 54);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(60, 15);
            lblVersion.TabIndex = 2;
            lblVersion.Text = "버전 1.0.0";
            //
            // lblDescription
            //
            lblDescription.AutoSize = true;
            lblDescription.ForeColor = Color.FromArgb(160, 160, 170);
            lblDescription.Location = new Point(107, 76);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(420, 15);
            lblDescription.TabIndex = 3;
            lblDescription.Text = "AI로 이미지 속 객체를 선택해 분리(누끼)하고, 객체를 지우거나 배경을 지우는 프로그램";
            //
            // pnlBody
            //
            pnlBody.BackColor = Color.FromArgb(24, 24, 27);
            pnlBody.Controls.Add(txtLicense);
            pnlBody.Controls.Add(pnlListGap);
            pnlBody.Controls.Add(lstLicenses);
            pnlBody.Controls.Add(lblLicenseCaption);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 112);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(20, 8, 20, 0);
            pnlBody.Size = new Size(820, 392);
            pnlBody.TabIndex = 1;
            //
            // txtLicense
            //
            txtLicense.BackColor = Color.FromArgb(30, 30, 34);
            txtLicense.BorderStyle = BorderStyle.None;
            txtLicense.DetectUrls = false;
            txtLicense.Dock = DockStyle.Fill;
            txtLicense.Font = new Font("Consolas", 9F);
            txtLicense.ForeColor = Color.FromArgb(210, 210, 216);
            txtLicense.Location = new Point(260, 36);
            txtLicense.Name = "txtLicense";
            txtLicense.ReadOnly = true;
            txtLicense.Size = new Size(540, 356);
            txtLicense.TabIndex = 2;
            txtLicense.Text = "";
            txtLicense.WordWrap = false;
            //
            // pnlListGap
            //
            pnlListGap.Dock = DockStyle.Left;
            pnlListGap.Location = new Point(250, 36);
            pnlListGap.Name = "pnlListGap";
            pnlListGap.Size = new Size(10, 356);
            pnlListGap.TabIndex = 3;
            //
            // lstLicenses
            //
            lstLicenses.BackColor = Color.FromArgb(38, 38, 43);
            lstLicenses.BorderStyle = BorderStyle.None;
            lstLicenses.Dock = DockStyle.Left;
            lstLicenses.DrawMode = DrawMode.OwnerDrawFixed;
            lstLicenses.ForeColor = Color.FromArgb(236, 236, 239);
            lstLicenses.IntegralHeight = false;
            lstLicenses.ItemHeight = 44;
            lstLicenses.Location = new Point(20, 36);
            lstLicenses.Name = "lstLicenses";
            lstLicenses.Size = new Size(230, 356);
            lstLicenses.TabIndex = 1;
            lstLicenses.DrawItem += lstLicenses_DrawItem;
            lstLicenses.SelectedIndexChanged += lstLicenses_SelectedIndexChanged;
            //
            // lblLicenseCaption
            //
            lblLicenseCaption.Dock = DockStyle.Top;
            lblLicenseCaption.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            lblLicenseCaption.ForeColor = Color.FromArgb(160, 160, 170);
            lblLicenseCaption.Location = new Point(20, 8);
            lblLicenseCaption.Name = "lblLicenseCaption";
            lblLicenseCaption.Size = new Size(780, 28);
            lblLicenseCaption.TabIndex = 0;
            lblLicenseCaption.Text = "오픈소스 라이선스";
            //
            // pnlFooter
            //
            pnlFooter.BackColor = Color.FromArgb(24, 24, 27);
            pnlFooter.Controls.Add(lblNotice);
            pnlFooter.Controls.Add(btnClose);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 504);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(820, 64);
            pnlFooter.TabIndex = 2;
            //
            // lblNotice
            //
            lblNotice.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblNotice.ForeColor = Color.FromArgb(105, 105, 115);
            lblNotice.Location = new Point(20, 14);
            lblNotice.Name = "lblNotice";
            lblNotice.Size = new Size(640, 36);
            lblNotice.TabIndex = 0;
            lblNotice.Text = "이 프로그램은 위 오픈소스 구성요소와 AI 모델을 각 라이선스 조건에 따라 포함하고 있습니다.\r\n각 항목을 선택하면 저작권 고지와 라이선스 전문을 볼 수 있습니다.";
            lblNotice.TextAlign = ContentAlignment.MiddleLeft;
            //
            // btnClose
            //
            btnClose.Anchor = AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(99, 115, 255);
            btnClose.Cursor = Cursors.Hand;
            btnClose.DialogResult = DialogResult.OK;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(82, 96, 228);
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(122, 136, 255);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(680, 14);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(120, 36);
            btnClose.TabIndex = 1;
            btnClose.Text = "닫기";
            btnClose.UseVisualStyleBackColor = false;
            //
            // AboutForm
            //
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(24, 24, 27);
            CancelButton = btnClose;
            ClientSize = new Size(820, 568);
            Controls.Add(pnlBody);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            ForeColor = Color.FromArgb(236, 236, 239);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "프로그램 정보";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            pnlBody.ResumeLayout(false);
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private PictureBox picLogo;
        private Label lblTitle;
        private Label lblVersion;
        private Label lblDescription;
        private Panel pnlBody;
        private RichTextBox txtLicense;
        private Panel pnlListGap;
        private ListBox lstLicenses;
        private Label lblLicenseCaption;
        private Panel pnlFooter;
        private Label lblNotice;
        private Button btnClose;
    }
}
