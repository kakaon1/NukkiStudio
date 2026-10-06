namespace NukkiStudio.App
{
    partial class BusyForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BusyForm));
            lblMessage = new Label();
            lblDetail = new Label();
            prgBusy = new ProgressBar();
            lblElapsed = new Label();
            tmrElapsed = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            //
            // lblMessage
            //
            lblMessage.AutoEllipsis = true;
            lblMessage.Font = new Font("Malgun Gothic", 11F, FontStyle.Bold);
            lblMessage.ForeColor = Color.FromArgb(236, 236, 239);
            lblMessage.Location = new Point(24, 20);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(392, 26);
            lblMessage.TabIndex = 0;
            lblMessage.Text = "지우는 중...";
            //
            // lblDetail
            //
            lblDetail.AutoEllipsis = true;
            lblDetail.ForeColor = Color.FromArgb(160, 160, 170);
            lblDetail.Location = new Point(25, 50);
            lblDetail.Name = "lblDetail";
            lblDetail.Size = new Size(391, 20);
            lblDetail.TabIndex = 1;
            lblDetail.Text = "잠시만 기다려 주세요.";
            //
            // prgBusy
            //
            prgBusy.Location = new Point(24, 82);
            prgBusy.MarqueeAnimationSpeed = 25;
            prgBusy.Name = "prgBusy";
            prgBusy.Size = new Size(392, 14);
            prgBusy.Style = ProgressBarStyle.Marquee;
            prgBusy.TabIndex = 2;
            //
            // lblElapsed
            //
            lblElapsed.ForeColor = Color.FromArgb(105, 105, 115);
            lblElapsed.Location = new Point(24, 102);
            lblElapsed.Name = "lblElapsed";
            lblElapsed.Size = new Size(392, 20);
            lblElapsed.TabIndex = 3;
            lblElapsed.Text = "0.0초";
            lblElapsed.TextAlign = ContentAlignment.MiddleRight;
            //
            // tmrElapsed
            //
            tmrElapsed.Interval = 100;
            tmrElapsed.Tick += tmrElapsed_Tick;
            //
            // BusyForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 34);
            ClientSize = new Size(440, 136);
            ControlBox = false;
            Controls.Add(lblElapsed);
            Controls.Add(prgBusy);
            Controls.Add(lblDetail);
            Controls.Add(lblMessage);
            ForeColor = Color.FromArgb(236, 236, 239);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BusyForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "작업 중";
            ResumeLayout(false);
        }

        #endregion

        private Label lblMessage;
        private Label lblDetail;
        private ProgressBar prgBusy;
        private Label lblElapsed;
        private System.Windows.Forms.Timer tmrElapsed;
    }
}
