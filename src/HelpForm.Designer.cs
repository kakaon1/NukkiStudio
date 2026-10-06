namespace NukkiStudio.App
{
    partial class HelpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpForm));
            pnlBody = new Panel();
            txtContent = new RichTextBox();
            pnlNavGap = new Panel();
            lstSections = new ListBox();
            pnlFooter = new Panel();
            btnAbout = new Button();
            btnClose = new Button();
            lblHint = new Label();
            pnlBody.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            //
            // pnlBody
            //
            pnlBody.BackColor = Color.FromArgb(24, 24, 27);
            pnlBody.Controls.Add(txtContent);
            pnlBody.Controls.Add(pnlNavGap);
            pnlBody.Controls.Add(lstSections);
            pnlBody.Dock = DockStyle.Fill;
            pnlBody.Location = new Point(0, 0);
            pnlBody.Name = "pnlBody";
            pnlBody.Padding = new Padding(16, 16, 16, 0);
            pnlBody.Size = new Size(900, 596);
            pnlBody.TabIndex = 0;
            //
            // txtContent
            //
            txtContent.BackColor = Color.FromArgb(30, 30, 34);
            txtContent.BorderStyle = BorderStyle.None;
            txtContent.DetectUrls = false;
            txtContent.Dock = DockStyle.Fill;
            txtContent.ForeColor = Color.FromArgb(220, 220, 226);
            txtContent.Location = new Point(258, 16);
            txtContent.Name = "txtContent";
            txtContent.ReadOnly = true;
            txtContent.Size = new Size(626, 580);
            txtContent.TabIndex = 1;
            txtContent.Text = "";
            //
            // pnlNavGap
            //
            pnlNavGap.Dock = DockStyle.Left;
            pnlNavGap.Location = new Point(246, 16);
            pnlNavGap.Name = "pnlNavGap";
            pnlNavGap.Size = new Size(12, 580);
            pnlNavGap.TabIndex = 2;
            //
            // lstSections
            //
            lstSections.BackColor = Color.FromArgb(38, 38, 43);
            lstSections.BorderStyle = BorderStyle.None;
            lstSections.Dock = DockStyle.Left;
            lstSections.DrawMode = DrawMode.OwnerDrawFixed;
            lstSections.ForeColor = Color.FromArgb(236, 236, 239);
            lstSections.IntegralHeight = false;
            lstSections.ItemHeight = 38;
            lstSections.Location = new Point(16, 16);
            lstSections.Name = "lstSections";
            lstSections.Size = new Size(230, 580);
            lstSections.TabIndex = 0;
            lstSections.DrawItem += lstSections_DrawItem;
            lstSections.SelectedIndexChanged += lstSections_SelectedIndexChanged;
            //
            // pnlFooter
            //
            pnlFooter.BackColor = Color.FromArgb(24, 24, 27);
            pnlFooter.Controls.Add(btnAbout);
            pnlFooter.Controls.Add(btnClose);
            pnlFooter.Controls.Add(lblHint);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 596);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(900, 64);
            pnlFooter.TabIndex = 1;
            //
            // btnAbout
            //
            btnAbout.Anchor = AnchorStyles.Right;
            btnAbout.BackColor = Color.FromArgb(50, 50, 57);
            btnAbout.Cursor = Cursors.Hand;
            btnAbout.FlatAppearance.BorderSize = 0;
            btnAbout.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnAbout.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnAbout.FlatStyle = FlatStyle.Flat;
            btnAbout.ForeColor = Color.FromArgb(236, 236, 239);
            btnAbout.Location = new Point(628, 14);
            btnAbout.Name = "btnAbout";
            btnAbout.Size = new Size(128, 36);
            btnAbout.TabIndex = 0;
            btnAbout.Text = "프로그램 정보";
            btnAbout.UseVisualStyleBackColor = false;
            btnAbout.Click += btnAbout_Click;
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
            btnClose.Location = new Point(764, 14);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(120, 36);
            btnClose.TabIndex = 1;
            btnClose.Text = "닫기";
            btnClose.UseVisualStyleBackColor = false;
            //
            // lblHint
            //
            lblHint.Anchor = AnchorStyles.Left;
            lblHint.AutoSize = true;
            lblHint.ForeColor = Color.FromArgb(105, 105, 115);
            lblHint.Location = new Point(16, 24);
            lblHint.Name = "lblHint";
            lblHint.Size = new Size(260, 15);
            lblHint.TabIndex = 2;
            lblHint.Text = "언제든 F1을 누르면 이 도움말을 다시 볼 수 있습니다.";
            //
            // HelpForm
            //
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(24, 24, 27);
            CancelButton = btnClose;
            ClientSize = new Size(900, 660);
            Controls.Add(pnlBody);
            Controls.Add(pnlFooter);
            ForeColor = Color.FromArgb(236, 236, 239);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimizeBox = false;
            MinimumSize = new Size(720, 480);
            Name = "HelpForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "도움말 - Nukki Studio 사용 설명서";
            pnlBody.ResumeLayout(false);
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlBody;
        private RichTextBox txtContent;
        private Panel pnlNavGap;
        private ListBox lstSections;
        private Panel pnlFooter;
        private Button btnAbout;
        private Button btnClose;
        private Label lblHint;
    }
}
