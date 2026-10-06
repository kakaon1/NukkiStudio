namespace NukkiStudio.App
{
    partial class ImageResizeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageResizeForm));
            tblMain = new TableLayoutPanel();
            lblCurrentCaption = new Label();
            lblCurrent = new Label();
            lblWidthCaption = new Label();
            numWidth = new NumericUpDown();
            lblHeightCaption = new Label();
            numHeight = new NumericUpDown();
            chkKeepRatio = new CheckBox();
            flpPresets = new FlowLayoutPanel();
            btnPreset50 = new Button();
            btnPresetOriginal = new Button();
            btnPreset150 = new Button();
            btnPreset200 = new Button();
            lblNote = new Label();
            pnlFooter = new Panel();
            btnOk = new Button();
            btnCancel = new Button();
            tblMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numHeight).BeginInit();
            flpPresets.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            //
            // tblMain
            //
            tblMain.ColumnCount = 2;
            tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblMain.Controls.Add(lblCurrentCaption, 0, 0);
            tblMain.Controls.Add(lblCurrent, 1, 0);
            tblMain.Controls.Add(lblWidthCaption, 0, 1);
            tblMain.Controls.Add(numWidth, 1, 1);
            tblMain.Controls.Add(lblHeightCaption, 0, 2);
            tblMain.Controls.Add(numHeight, 1, 2);
            tblMain.Controls.Add(chkKeepRatio, 1, 3);
            tblMain.Controls.Add(flpPresets, 1, 4);
            tblMain.Controls.Add(lblNote, 0, 5);
            tblMain.Dock = DockStyle.Fill;
            tblMain.Location = new Point(0, 0);
            tblMain.Name = "tblMain";
            tblMain.Padding = new Padding(20, 16, 20, 8);
            tblMain.RowCount = 6;
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblMain.Size = new Size(460, 266);
            tblMain.TabIndex = 0;
            //
            // lblCurrentCaption
            //
            lblCurrentCaption.Dock = DockStyle.Fill;
            lblCurrentCaption.ForeColor = Color.FromArgb(160, 160, 170);
            lblCurrentCaption.Location = new Point(23, 16);
            lblCurrentCaption.Name = "lblCurrentCaption";
            lblCurrentCaption.Size = new Size(104, 34);
            lblCurrentCaption.TabIndex = 0;
            lblCurrentCaption.Text = "현재 크기";
            lblCurrentCaption.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblCurrent
            //
            lblCurrent.Dock = DockStyle.Fill;
            lblCurrent.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            lblCurrent.ForeColor = Color.FromArgb(236, 236, 239);
            lblCurrent.Location = new Point(133, 16);
            lblCurrent.Name = "lblCurrent";
            lblCurrent.Size = new Size(304, 34);
            lblCurrent.TabIndex = 1;
            lblCurrent.Text = "0 × 0";
            lblCurrent.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblWidthCaption
            //
            lblWidthCaption.Dock = DockStyle.Fill;
            lblWidthCaption.ForeColor = Color.FromArgb(160, 160, 170);
            lblWidthCaption.Location = new Point(23, 50);
            lblWidthCaption.Name = "lblWidthCaption";
            lblWidthCaption.Size = new Size(104, 36);
            lblWidthCaption.TabIndex = 2;
            lblWidthCaption.Text = "너비 (px)";
            lblWidthCaption.TextAlign = ContentAlignment.MiddleLeft;
            //
            // numWidth
            //
            numWidth.Anchor = AnchorStyles.Left;
            numWidth.BackColor = Color.FromArgb(50, 50, 57);
            numWidth.BorderStyle = BorderStyle.FixedSingle;
            numWidth.ForeColor = Color.FromArgb(236, 236, 239);
            numWidth.Location = new Point(133, 56);
            numWidth.Maximum = new decimal(new int[] { 16000, 0, 0, 0 });
            numWidth.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
            numWidth.Name = "numWidth";
            numWidth.Size = new Size(120, 23);
            numWidth.TabIndex = 3;
            numWidth.Value = new decimal(new int[] { 16, 0, 0, 0 });
            numWidth.ValueChanged += numWidth_ValueChanged;
            //
            // lblHeightCaption
            //
            lblHeightCaption.Dock = DockStyle.Fill;
            lblHeightCaption.ForeColor = Color.FromArgb(160, 160, 170);
            lblHeightCaption.Location = new Point(23, 86);
            lblHeightCaption.Name = "lblHeightCaption";
            lblHeightCaption.Size = new Size(104, 36);
            lblHeightCaption.TabIndex = 4;
            lblHeightCaption.Text = "높이 (px)";
            lblHeightCaption.TextAlign = ContentAlignment.MiddleLeft;
            //
            // numHeight
            //
            numHeight.Anchor = AnchorStyles.Left;
            numHeight.BackColor = Color.FromArgb(50, 50, 57);
            numHeight.BorderStyle = BorderStyle.FixedSingle;
            numHeight.ForeColor = Color.FromArgb(236, 236, 239);
            numHeight.Location = new Point(133, 92);
            numHeight.Maximum = new decimal(new int[] { 16000, 0, 0, 0 });
            numHeight.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
            numHeight.Name = "numHeight";
            numHeight.Size = new Size(120, 23);
            numHeight.TabIndex = 5;
            numHeight.Value = new decimal(new int[] { 16, 0, 0, 0 });
            numHeight.ValueChanged += numHeight_ValueChanged;
            //
            // chkKeepRatio
            //
            chkKeepRatio.Checked = true;
            chkKeepRatio.CheckState = CheckState.Checked;
            chkKeepRatio.Dock = DockStyle.Fill;
            chkKeepRatio.ForeColor = Color.FromArgb(236, 236, 239);
            chkKeepRatio.Location = new Point(133, 125);
            chkKeepRatio.Name = "chkKeepRatio";
            chkKeepRatio.Size = new Size(304, 26);
            chkKeepRatio.TabIndex = 6;
            chkKeepRatio.Text = "가로세로 비율 유지";
            chkKeepRatio.UseVisualStyleBackColor = true;
            //
            // flpPresets
            //
            flpPresets.Controls.Add(btnPreset50);
            flpPresets.Controls.Add(btnPresetOriginal);
            flpPresets.Controls.Add(btnPreset150);
            flpPresets.Controls.Add(btnPreset200);
            flpPresets.Dock = DockStyle.Fill;
            flpPresets.Location = new Point(130, 154);
            flpPresets.Margin = new Padding(0);
            flpPresets.Name = "flpPresets";
            flpPresets.Size = new Size(310, 42);
            flpPresets.TabIndex = 7;
            //
            // btnPreset50
            //
            btnPreset50.BackColor = Color.FromArgb(50, 50, 57);
            btnPreset50.FlatAppearance.BorderSize = 0;
            btnPreset50.FlatStyle = FlatStyle.Flat;
            btnPreset50.ForeColor = Color.FromArgb(236, 236, 239);
            btnPreset50.Location = new Point(3, 6);
            btnPreset50.Margin = new Padding(3, 6, 3, 3);
            btnPreset50.Name = "btnPreset50";
            btnPreset50.Size = new Size(68, 30);
            btnPreset50.TabIndex = 0;
            btnPreset50.Tag = "50";
            btnPreset50.Text = "50%";
            btnPreset50.UseVisualStyleBackColor = false;
            btnPreset50.Click += Preset_Click;
            //
            // btnPresetOriginal
            //
            btnPresetOriginal.BackColor = Color.FromArgb(50, 50, 57);
            btnPresetOriginal.FlatAppearance.BorderSize = 0;
            btnPresetOriginal.FlatStyle = FlatStyle.Flat;
            btnPresetOriginal.ForeColor = Color.FromArgb(236, 236, 239);
            btnPresetOriginal.Location = new Point(77, 6);
            btnPresetOriginal.Margin = new Padding(3, 6, 3, 3);
            btnPresetOriginal.Name = "btnPresetOriginal";
            btnPresetOriginal.Size = new Size(68, 30);
            btnPresetOriginal.TabIndex = 1;
            btnPresetOriginal.Tag = "100";
            btnPresetOriginal.Text = "100%";
            btnPresetOriginal.UseVisualStyleBackColor = false;
            btnPresetOriginal.Click += Preset_Click;
            //
            // btnPreset150
            //
            btnPreset150.BackColor = Color.FromArgb(50, 50, 57);
            btnPreset150.FlatAppearance.BorderSize = 0;
            btnPreset150.FlatStyle = FlatStyle.Flat;
            btnPreset150.ForeColor = Color.FromArgb(236, 236, 239);
            btnPreset150.Location = new Point(151, 6);
            btnPreset150.Margin = new Padding(3, 6, 3, 3);
            btnPreset150.Name = "btnPreset150";
            btnPreset150.Size = new Size(68, 30);
            btnPreset150.TabIndex = 2;
            btnPreset150.Tag = "150";
            btnPreset150.Text = "150%";
            btnPreset150.UseVisualStyleBackColor = false;
            btnPreset150.Click += Preset_Click;
            //
            // btnPreset200
            //
            btnPreset200.BackColor = Color.FromArgb(50, 50, 57);
            btnPreset200.FlatAppearance.BorderSize = 0;
            btnPreset200.FlatStyle = FlatStyle.Flat;
            btnPreset200.ForeColor = Color.FromArgb(236, 236, 239);
            btnPreset200.Location = new Point(225, 6);
            btnPreset200.Margin = new Padding(3, 6, 3, 3);
            btnPreset200.Name = "btnPreset200";
            btnPreset200.Size = new Size(68, 30);
            btnPreset200.TabIndex = 3;
            btnPreset200.Tag = "200";
            btnPreset200.Text = "200%";
            btnPreset200.UseVisualStyleBackColor = false;
            btnPreset200.Click += Preset_Click;
            //
            // lblNote
            //
            tblMain.SetColumnSpan(lblNote, 2);
            lblNote.Dock = DockStyle.Fill;
            lblNote.ForeColor = Color.FromArgb(105, 105, 115);
            lblNote.Location = new Point(23, 196);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(414, 62);
            lblNote.TabIndex = 8;
            lblNote.Text = "키울 때는 고품질 보간(바이큐빅)과 선명화로 깔끔하게 늘립니다.\r\n확정한 객체도 같은 크기로 맞춰지고, 되돌리기(Ctrl+Z)로 원래 크기로 돌아갈 수 있습니다.";
            lblNote.TextAlign = ContentAlignment.MiddleLeft;
            //
            // pnlFooter
            //
            pnlFooter.BackColor = Color.FromArgb(30, 30, 34);
            pnlFooter.Controls.Add(btnOk);
            pnlFooter.Controls.Add(btnCancel);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 266);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(460, 60);
            pnlFooter.TabIndex = 1;
            //
            // btnOk
            //
            btnOk.Anchor = AnchorStyles.Right;
            btnOk.BackColor = Color.FromArgb(99, 115, 255);
            btnOk.Cursor = Cursors.Hand;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.FlatAppearance.MouseDownBackColor = Color.FromArgb(82, 96, 228);
            btnOk.FlatAppearance.MouseOverBackColor = Color.FromArgb(122, 136, 255);
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            btnOk.ForeColor = Color.White;
            btnOk.Location = new Point(208, 12);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(110, 36);
            btnOk.TabIndex = 0;
            btnOk.Text = "적용";
            btnOk.UseVisualStyleBackColor = false;
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Right;
            btnCancel.BackColor = Color.FromArgb(50, 50, 57);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.ForeColor = Color.FromArgb(236, 236, 239);
            btnCancel.Location = new Point(328, 12);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(110, 36);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "취소";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // ImageResizeForm
            //
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(24, 24, 27);
            CancelButton = btnCancel;
            ClientSize = new Size(460, 326);
            Controls.Add(tblMain);
            Controls.Add(pnlFooter);
            ForeColor = Color.FromArgb(236, 236, 239);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ImageResizeForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "이미지 크기 조절";
            tblMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)numHeight).EndInit();
            flpPresets.ResumeLayout(false);
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tblMain;
        private Label lblCurrentCaption;
        private Label lblCurrent;
        private Label lblWidthCaption;
        private NumericUpDown numWidth;
        private Label lblHeightCaption;
        private NumericUpDown numHeight;
        private CheckBox chkKeepRatio;
        private FlowLayoutPanel flpPresets;
        private Button btnPreset50;
        private Button btnPresetOriginal;
        private Button btnPreset150;
        private Button btnPreset200;
        private Label lblNote;
        private Panel pnlFooter;
        private Button btnOk;
        private Button btnCancel;
    }
}
