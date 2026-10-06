namespace NukkiStudio.App
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip = new MenuStrip();
            mnuFile = new ToolStripMenuItem();
            mnuFileOpen = new ToolStripMenuItem();
            mnuFilePaste = new ToolStripMenuItem();
            mnuFileSep1 = new ToolStripSeparator();
            mnuFileSave = new ToolStripMenuItem();
            mnuFileSaveAll = new ToolStripMenuItem();
            mnuFileCopy = new ToolStripMenuItem();
            mnuFileSep2 = new ToolStripSeparator();
            mnuFileExit = new ToolStripMenuItem();
            mnuEdit = new ToolStripMenuItem();
            mnuEditUndo = new ToolStripMenuItem();
            mnuEditRedo = new ToolStripMenuItem();
            mnuEditSep1 = new ToolStripSeparator();
            mnuEditCommit = new ToolStripMenuItem();
            mnuEditClear = new ToolStripMenuItem();
            mnuView = new ToolStripMenuItem();
            mnuViewOverlay = new ToolStripMenuItem();
            mnuViewCutout = new ToolStripMenuItem();
            mnuViewMask = new ToolStripMenuItem();
            mnuViewSep1 = new ToolStripSeparator();
            mnuViewFit = new ToolStripMenuItem();
            mnuViewActual = new ToolStripMenuItem();
            btnAutoDetect = new ToolStripButton();
            mnuEditAutoDetect = new ToolStripMenuItem();
            mnuFileAddFolder = new ToolStripMenuItem();
            mnuEditSep3 = new ToolStripSeparator();
            mnuEditCut = new ToolStripMenuItem();
            mnuEditPaste = new ToolStripMenuItem();
            mnuFileSep3 = new ToolStripSeparator();
            mnuFileExportSettings = new ToolStripMenuItem();
            mnuFileOpenOutput = new ToolStripMenuItem();
            tblExportSettings = new TableLayoutPanel();
            lblExportSummary = new Label();
            btnExportSettings = new Button();
            mnuFileSaveBatch = new ToolStripMenuItem();
            pnlLeft = new Panel();
            cardImages = new NukkiStudio.App.Controls.CardPanel();
            lblImagesEmpty = new Label();
            lstImages = new ListBox();
            tblImageButtons = new TableLayoutPanel();
            btnAddFiles = new Button();
            btnAddFolder = new Button();
            btnRemoveImage = new Button();
            btnSaveBatch = new Button();
            mnuFileSaveImage = new ToolStripMenuItem();
            mnuEditSep2 = new ToolStripSeparator();
            mnuEditErase = new ToolStripMenuItem();
            mnuEditRemoveBg = new ToolStripMenuItem();
            mnuHelp = new ToolStripMenuItem();
            mnuHelpManual = new ToolStripMenuItem();
            mnuHelpShortcuts = new ToolStripMenuItem();
            mnuHelpSep1 = new ToolStripSeparator();
            mnuHelpAbout = new ToolStripMenuItem();
            toolSep3 = new ToolStripSeparator();
            btnEraseObject = new ToolStripButton();
            btnRemoveBackground = new ToolStripButton();
            btnHelp = new ToolStripButton();
            btnSaveImage = new Button();
            mnuSettings = new ToolStripMenuItem();
            mnuSettingsModel = new ToolStripMenuItem();
            mnuSettingsDevice = new ToolStripMenuItem();
            mnuDeviceAuto = new ToolStripMenuItem();
            mnuDeviceCpu = new ToolStripMenuItem();
            toolStrip = new ToolStrip();
            btnOpen = new ToolStripButton();
            btnSave = new ToolStripButton();
            toolSep1 = new ToolStripSeparator();
            btnToolPoint = new ToolStripButton();
            btnToolBox = new ToolStripButton();
            btnToolBrushAdd = new ToolStripButton();
            btnBrushSize = new NukkiStudio.App.Controls.ToolStripBrushSize();
            btnBrushShape = new ToolStripButton();
            toolSep2 = new ToolStripSeparator();
            btnUndo = new ToolStripButton();
            btnRedo = new ToolStripButton();
            btnActual = new ToolStripButton();
            btnFit = new ToolStripButton();
            toolSepModel = new ToolStripSeparator();
            cboInpaintModel = new ToolStripComboBox();
            lblInpaintModel = new ToolStripLabel();
            mnuSettingsDownload = new ToolStripMenuItem();
            mnuSettingsSep1 = new ToolStripSeparator();
            mnuEditSep4 = new ToolStripSeparator();
            mnuEditResize = new ToolStripMenuItem();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            lblImageInfo = new ToolStripStatusLabel();
            lblModel = new ToolStripStatusLabel();
            lblZoom = new ToolStripStatusLabel();
            splitMain = new SplitContainer();
            canvas = new NukkiStudio.App.Controls.ImageCanvas();
            cardObjects = new NukkiStudio.App.Controls.CardPanel();
            lblObjectsEmpty = new Label();
            lstObjects = new ListBox();
            tblObjectButtons = new TableLayoutPanel();
            btnCommit = new Button();
            btnClearSelection = new Button();
            btnDeleteObject = new Button();
            pnlGap1 = new Panel();
            cardRefine = new NukkiStudio.App.Controls.CardPanel();
            tblRefine = new TableLayoutPanel();
            lblBrush = new Label();
            lblBrushValue = new Label();
            trkBrush = new NukkiStudio.App.Controls.FlatSlider();
            lblFeather = new Label();
            lblFeatherValue = new Label();
            trkFeather = new NukkiStudio.App.Controls.FlatSlider();
            lblGrow = new Label();
            lblGrowValue = new Label();
            trkGrow = new NukkiStudio.App.Controls.FlatSlider();
            chkCleanup = new NukkiStudio.App.Controls.ToggleSwitch();
            pnlGap2 = new Panel();
            cardExport = new NukkiStudio.App.Controls.CardPanel();
            tblExport = new TableLayoutPanel();
            chkCrop = new NukkiStudio.App.Controls.ToggleSwitch();
            chkBackground = new NukkiStudio.App.Controls.ToggleSwitch();
            btnBackgroundColor = new Button();
            btnSavePng = new Button();
            btnSaveAll = new Button();
            btnCopy = new Button();
            menuStrip.SuspendLayout();
            toolStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            pnlLeft.SuspendLayout();
            cardImages.SuspendLayout();
            tblImageButtons.SuspendLayout();
            cardObjects.SuspendLayout();
            tblObjectButtons.SuspendLayout();
            cardRefine.SuspendLayout();
            tblRefine.SuspendLayout();
            cardExport.SuspendLayout();
            tblExport.SuspendLayout();
            tblExportSettings.SuspendLayout();
            SuspendLayout();
            //
            // menuStrip
            //
            menuStrip.BackColor = Color.FromArgb(24, 24, 27);
            menuStrip.ForeColor = Color.FromArgb(236, 236, 239);
            menuStrip.Items.AddRange(new ToolStripItem[] { mnuFile, mnuEdit, mnuView, mnuSettings, mnuHelp });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(8, 3, 0, 3);
            menuStrip.Size = new Size(1500, 26);
            menuStrip.TabIndex = 0;
            //
            // mnuFile
            //
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuFileOpen, mnuFileAddFolder, mnuFilePaste, mnuFileSep1, mnuFileSave, mnuFileSaveAll, mnuFileSaveImage, mnuFileCopy, mnuFileSaveBatch, mnuFileSep3, mnuFileExportSettings, mnuFileOpenOutput, mnuFileSep2, mnuFileExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(57, 20);
            mnuFile.Text = "파일(&F)";
            //
            // mnuFileOpen
            //
            mnuFileOpen.Name = "mnuFileOpen";
            mnuFileOpen.ShortcutKeys = Keys.Control | Keys.O;
            mnuFileOpen.Size = new Size(280, 24);
            mnuFileOpen.Text = "이미지 열기 / 추가... (여러 개 선택 가능)";
            mnuFileOpen.Click += mnuFileOpen_Click;
            //
            // mnuFileAddFolder
            //
            mnuFileAddFolder.Name = "mnuFileAddFolder";
            mnuFileAddFolder.ShortcutKeys = Keys.Control | Keys.Shift | Keys.O;
            mnuFileAddFolder.Size = new Size(280, 24);
            mnuFileAddFolder.Text = "폴더 추가... (하위 폴더 포함)";
            mnuFileAddFolder.Click += mnuFileAddFolder_Click;
            //
            // mnuFileSaveBatch
            //
            mnuFileSaveBatch.Name = "mnuFileSaveBatch";
            mnuFileSaveBatch.ShortcutKeys = Keys.Control | Keys.Shift | Keys.B;
            mnuFileSaveBatch.Size = new Size(280, 24);
            mnuFileSaveBatch.Text = "전체 이미지 일괄 저장...";
            mnuFileSaveBatch.Click += btnSaveBatch_Click;
            //
            // mnuFilePaste
            //
            mnuFilePaste.Name = "mnuFilePaste";
            mnuFilePaste.ShortcutKeys = Keys.Control | Keys.Shift | Keys.V;
            mnuFilePaste.Size = new Size(280, 24);
            mnuFilePaste.Text = "클립보드 이미지를 새 이미지로 열기";
            mnuFilePaste.Click += mnuFilePaste_Click;
            //
            // mnuFileSep1
            //
            mnuFileSep1.Name = "mnuFileSep1";
            mnuFileSep1.Size = new Size(277, 6);
            //
            // mnuFileSave
            //
            mnuFileSave.Name = "mnuFileSave";
            mnuFileSave.ShortcutKeys = Keys.Control | Keys.S;
            mnuFileSave.Size = new Size(280, 24);
            mnuFileSave.Text = "선택 객체 PNG 저장...";
            mnuFileSave.Click += btnSavePng_Click;
            //
            // mnuFileSaveAll
            //
            mnuFileSaveAll.Name = "mnuFileSaveAll";
            mnuFileSaveAll.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            mnuFileSaveAll.Size = new Size(280, 24);
            mnuFileSaveAll.Text = "모든 객체 저장...";
            mnuFileSaveAll.Click += btnSaveAll_Click;
            //
            // mnuFileSaveImage
            //
            mnuFileSaveImage.Name = "mnuFileSaveImage";
            mnuFileSaveImage.ShortcutKeys = Keys.Control | Keys.Alt | Keys.S;
            mnuFileSaveImage.Size = new Size(280, 24);
            mnuFileSaveImage.Text = "편집한 이미지 저장...";
            mnuFileSaveImage.Click += btnSaveImage_Click;
            //
            // mnuFileCopy
            //
            mnuFileCopy.Name = "mnuFileCopy";
            mnuFileCopy.ShortcutKeys = Keys.Control | Keys.C;
            mnuFileCopy.Size = new Size(280, 24);
            mnuFileCopy.Text = "선택 객체 복사";
            mnuFileCopy.Click += btnCopy_Click;
            //
            // mnuFileSep2
            //
            mnuFileSep2.Name = "mnuFileSep2";
            mnuFileSep2.Size = new Size(277, 6);
            //
            // mnuFileExit
            //
            mnuFileExit.Name = "mnuFileExit";
            mnuFileExit.Size = new Size(280, 24);
            mnuFileExit.Text = "종료";
            mnuFileExit.Click += mnuFileExit_Click;
            //
            // mnuEdit
            //
            mnuEdit.DropDownItems.AddRange(new ToolStripItem[] { mnuEditUndo, mnuEditRedo, mnuEditSep3, mnuEditCut, mnuEditPaste, mnuEditSep1, mnuEditCommit, mnuEditClear, mnuEditSep2, mnuEditAutoDetect, mnuEditErase, mnuEditRemoveBg, mnuEditSep4, mnuEditResize });
            mnuEdit.Name = "mnuEdit";
            mnuEdit.Size = new Size(57, 20);
            mnuEdit.Text = "편집(&E)";
            //
            // mnuEditUndo
            //
            mnuEditUndo.Name = "mnuEditUndo";
            mnuEditUndo.ShortcutKeys = Keys.Control | Keys.Z;
            mnuEditUndo.Size = new Size(240, 24);
            mnuEditUndo.Text = "실행 취소";
            mnuEditUndo.Click += btnUndo_Click;
            //
            // mnuEditRedo
            //
            mnuEditRedo.Name = "mnuEditRedo";
            mnuEditRedo.ShortcutKeys = Keys.Control | Keys.Y;
            mnuEditRedo.Size = new Size(240, 24);
            mnuEditRedo.Text = "다시 실행";
            mnuEditRedo.Click += btnRedo_Click;
            //
            // mnuEditSep3
            //
            mnuEditSep3.Name = "mnuEditSep3";
            mnuEditSep3.Size = new Size(237, 6);
            //
            // mnuEditCut
            //
            mnuEditCut.Name = "mnuEditCut";
            mnuEditCut.ShortcutKeys = Keys.Control | Keys.X;
            mnuEditCut.Size = new Size(240, 24);
            mnuEditCut.Text = "잘라내기 (복사 후 지우기)";
            mnuEditCut.Click += mnuEditCut_Click;
            //
            // mnuEditPaste
            //
            mnuEditPaste.Name = "mnuEditPaste";
            mnuEditPaste.ShortcutKeys = Keys.Control | Keys.V;
            mnuEditPaste.Size = new Size(240, 24);
            mnuEditPaste.Text = "붙여넣기 (현재 이미지 위에)";
            mnuEditPaste.Click += mnuEditPaste_Click;
            //
            // mnuEditSep1
            //
            mnuEditSep1.Name = "mnuEditSep1";
            mnuEditSep1.Size = new Size(237, 6);
            //
            // mnuEditCommit
            //
            mnuEditCommit.Name = "mnuEditCommit";
            mnuEditCommit.ShortcutKeyDisplayString = "Enter";
            mnuEditCommit.Size = new Size(240, 24);
            mnuEditCommit.Text = "현재 선택을 객체로 확정";
            mnuEditCommit.Click += btnCommit_Click;
            //
            // mnuEditClear
            //
            mnuEditClear.Name = "mnuEditClear";
            mnuEditClear.ShortcutKeyDisplayString = "Esc";
            mnuEditClear.Size = new Size(240, 24);
            mnuEditClear.Text = "현재 선택 초기화";
            mnuEditClear.Click += btnClearSelection_Click;
            //
            // mnuEditSep2
            //
            mnuEditSep2.Name = "mnuEditSep2";
            mnuEditSep2.Size = new Size(237, 6);
            //
            // mnuEditErase
            //
            mnuEditErase.Name = "mnuEditErase";
            mnuEditErase.ShortcutKeys = Keys.Delete;
            mnuEditErase.Size = new Size(240, 24);
            mnuEditErase.Text = "선택 객체 지우기 (배경으로 채우기)";
            mnuEditErase.Click += btnEraseObject_Click;
            //
            // mnuEditRemoveBg
            //
            mnuEditRemoveBg.Name = "mnuEditRemoveBg";
            mnuEditRemoveBg.ShortcutKeys = Keys.Control | Keys.B;
            mnuEditRemoveBg.Size = new Size(240, 24);
            mnuEditRemoveBg.Text = "배경 지우기 (선택 객체만 남기기)";
            mnuEditRemoveBg.Click += btnRemoveBackground_Click;
            //
            // mnuEditSep4
            //
            mnuEditSep4.Name = "mnuEditSep4";
            mnuEditSep4.Size = new Size(237, 6);
            //
            // mnuEditResize
            //
            mnuEditResize.Name = "mnuEditResize";
            mnuEditResize.ShortcutKeys = Keys.Control | Keys.R;
            mnuEditResize.Size = new Size(240, 24);
            mnuEditResize.Text = "이미지 크기(해상도) 조절...";
            mnuEditResize.Click += mnuEditResize_Click;
            //
            // mnuView
            //
            mnuView.DropDownItems.AddRange(new ToolStripItem[] { mnuViewOverlay, mnuViewCutout, mnuViewMask, mnuViewSep1, mnuViewFit, mnuViewActual });
            mnuView.Name = "mnuView";
            mnuView.Size = new Size(59, 20);
            mnuView.Text = "보기(&V)";
            //
            // mnuViewOverlay
            //
            mnuViewOverlay.Checked = true;
            mnuViewOverlay.CheckState = CheckState.Checked;
            mnuViewOverlay.Name = "mnuViewOverlay";
            mnuViewOverlay.ShortcutKeys = Keys.F2;
            mnuViewOverlay.Size = new Size(240, 24);
            mnuViewOverlay.Text = "원본 + 마스크";
            mnuViewOverlay.Click += mnuViewOverlay_Click;
            //
            // mnuViewCutout
            //
            mnuViewCutout.Name = "mnuViewCutout";
            mnuViewCutout.ShortcutKeys = Keys.F3;
            mnuViewCutout.Size = new Size(240, 24);
            mnuViewCutout.Text = "누끼 결과";
            mnuViewCutout.Click += mnuViewCutout_Click;
            //
            // mnuViewMask
            //
            mnuViewMask.Name = "mnuViewMask";
            mnuViewMask.ShortcutKeys = Keys.F4;
            mnuViewMask.Size = new Size(240, 24);
            mnuViewMask.Text = "마스크만";
            mnuViewMask.Click += mnuViewMask_Click;
            //
            // mnuViewSep1
            //
            mnuViewSep1.Name = "mnuViewSep1";
            mnuViewSep1.Size = new Size(237, 6);
            //
            // mnuViewFit
            //
            mnuViewFit.Name = "mnuViewFit";
            mnuViewFit.ShortcutKeys = Keys.Control | Keys.D0;
            mnuViewFit.Size = new Size(240, 24);
            mnuViewFit.Text = "화면에 맞춤";
            mnuViewFit.Click += btnFit_Click;
            //
            // mnuViewActual
            //
            mnuViewActual.Name = "mnuViewActual";
            mnuViewActual.ShortcutKeys = Keys.Control | Keys.D1;
            mnuViewActual.Size = new Size(240, 24);
            mnuViewActual.Text = "실제 크기 (100%)";
            mnuViewActual.Click += btnActual_Click;
            //
            // mnuSettings
            //
            mnuSettings.DropDownItems.AddRange(new ToolStripItem[] { mnuSettingsDownload, mnuSettingsSep1, mnuSettingsModel, mnuSettingsDevice });
            mnuSettings.Name = "mnuSettings";
            mnuSettings.Size = new Size(59, 20);
            mnuSettings.Text = "설정(&S)";
            //
            // mnuSettingsDownload
            //
            mnuSettingsDownload.Name = "mnuSettingsDownload";
            mnuSettingsDownload.Size = new Size(180, 24);
            mnuSettingsDownload.Text = "AI 모델 다운로드 / 관리...";
            mnuSettingsDownload.Click += mnuSettingsDownload_Click;
            //
            // mnuSettingsSep1
            //
            mnuSettingsSep1.Name = "mnuSettingsSep1";
            mnuSettingsSep1.Size = new Size(177, 6);
            //
            // mnuSettingsModel
            //
            mnuSettingsModel.Name = "mnuSettingsModel";
            mnuSettingsModel.Size = new Size(180, 24);
            mnuSettingsModel.Text = "AI 모델";
            //
            // mnuSettingsDevice
            //
            mnuSettingsDevice.DropDownItems.AddRange(new ToolStripItem[] { mnuDeviceAuto, mnuDeviceCpu });
            mnuSettingsDevice.Name = "mnuSettingsDevice";
            mnuSettingsDevice.Size = new Size(180, 24);
            mnuSettingsDevice.Text = "실행 장치";
            //
            // mnuDeviceAuto
            //
            mnuDeviceAuto.Checked = true;
            mnuDeviceAuto.CheckState = CheckState.Checked;
            mnuDeviceAuto.Name = "mnuDeviceAuto";
            mnuDeviceAuto.Size = new Size(200, 24);
            mnuDeviceAuto.Text = "자동 (GPU 우선)";
            mnuDeviceAuto.Click += mnuDeviceAuto_Click;
            //
            // mnuDeviceCpu
            //
            mnuDeviceCpu.Name = "mnuDeviceCpu";
            mnuDeviceCpu.Size = new Size(200, 24);
            mnuDeviceCpu.Text = "CPU만 사용";
            mnuDeviceCpu.Click += mnuDeviceCpu_Click;
            //
            // mnuHelp
            //
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuHelpManual, mnuHelpShortcuts, mnuHelpSep1, mnuHelpAbout });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(72, 20);
            mnuHelp.Text = "도움말(&H)";
            //
            // mnuHelpManual
            //
            mnuHelpManual.Name = "mnuHelpManual";
            mnuHelpManual.ShortcutKeys = Keys.F1;
            mnuHelpManual.Size = new Size(220, 24);
            mnuHelpManual.Text = "사용 설명서";
            mnuHelpManual.Click += mnuHelpManual_Click;
            //
            // mnuHelpShortcuts
            //
            mnuHelpShortcuts.Name = "mnuHelpShortcuts";
            mnuHelpShortcuts.Size = new Size(220, 24);
            mnuHelpShortcuts.Text = "단축키 목록";
            mnuHelpShortcuts.Click += mnuHelpShortcuts_Click;
            //
            // mnuHelpSep1
            //
            mnuHelpSep1.Name = "mnuHelpSep1";
            mnuHelpSep1.Size = new Size(217, 6);
            //
            // mnuHelpAbout
            //
            mnuHelpAbout.Name = "mnuHelpAbout";
            mnuHelpAbout.Size = new Size(220, 24);
            mnuHelpAbout.Text = "프로그램 정보 (라이선스)";
            mnuHelpAbout.Click += mnuHelpAbout_Click;
            //
            // toolStrip
            //
            toolStrip.AutoSize = false;
            toolStrip.BackColor = Color.FromArgb(30, 30, 34);
            toolStrip.ForeColor = Color.FromArgb(236, 236, 239);
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.ImageScalingSize = new Size(18, 18);
            toolStrip.Items.AddRange(new ToolStripItem[] { btnOpen, btnSave, toolSep1, btnAutoDetect, btnToolPoint, btnToolBox, btnToolBrushAdd, btnBrushSize, btnBrushShape, toolSep2, btnEraseObject, btnRemoveBackground, toolSep3, btnUndo, btnRedo, btnHelp, btnActual, btnFit, toolSepModel, cboInpaintModel, lblInpaintModel });
            toolStrip.Location = new Point(0, 26);
            toolStrip.Name = "toolStrip";
            toolStrip.Padding = new Padding(10, 6, 10, 6);
            toolStrip.Size = new Size(1500, 46);
            toolStrip.TabIndex = 1;
            //
            // btnOpen
            //
            btnOpen.ImageScaling = ToolStripItemImageScaling.None;
            btnOpen.Margin = new Padding(0, 0, 2, 0);
            btnOpen.Name = "btnOpen";
            btnOpen.Padding = new Padding(6, 0, 6, 0);
            btnOpen.Size = new Size(56, 34);
            btnOpen.Text = "열기";
            btnOpen.ToolTipText = "이미지 열기 (Ctrl+O)";
            btnOpen.Click += mnuFileOpen_Click;
            //
            // btnSave
            //
            btnSave.ImageScaling = ToolStripItemImageScaling.None;
            btnSave.Margin = new Padding(0, 0, 2, 0);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6, 0, 6, 0);
            btnSave.Size = new Size(56, 34);
            btnSave.Text = "저장";
            btnSave.ToolTipText = "선택 객체 PNG 저장 (Ctrl+S)";
            btnSave.Click += btnSavePng_Click;
            //
            // toolSep1
            //
            toolSep1.Margin = new Padding(8, 0, 8, 0);
            toolSep1.Name = "toolSep1";
            toolSep1.Size = new Size(6, 34);
            //
            // btnAutoDetect
            //
            btnAutoDetect.ImageScaling = ToolStripItemImageScaling.None;
            btnAutoDetect.Margin = new Padding(0, 0, 6, 0);
            btnAutoDetect.Name = "btnAutoDetect";
            btnAutoDetect.Padding = new Padding(6, 0, 6, 0);
            btnAutoDetect.Size = new Size(112, 34);
            btnAutoDetect.Text = "객체 자동 선택";
            btnAutoDetect.ToolTipText = "AI가 이미지 속 객체(사람, 동물, 물건 등)를 자동으로 찾아 목록에 추가 (A)";
            btnAutoDetect.Click += btnAutoDetect_Click;
            //
            // mnuEditAutoDetect
            //
            mnuEditAutoDetect.Name = "mnuEditAutoDetect";
            mnuEditAutoDetect.ShortcutKeyDisplayString = "A";
            mnuEditAutoDetect.Size = new Size(240, 24);
            mnuEditAutoDetect.Text = "객체 자동 선택";
            mnuEditAutoDetect.Click += btnAutoDetect_Click;
            //
            // btnToolPoint
            //
            btnToolPoint.Checked = true;
            btnToolPoint.CheckState = CheckState.Checked;
            btnToolPoint.ImageScaling = ToolStripItemImageScaling.None;
            btnToolPoint.Margin = new Padding(0, 0, 2, 0);
            btnToolPoint.Name = "btnToolPoint";
            btnToolPoint.Padding = new Padding(6, 0, 6, 0);
            btnToolPoint.Size = new Size(80, 34);
            btnToolPoint.Text = "점 선택";
            btnToolPoint.ToolTipText = "점 선택 (Q) — 좌클릭: 포함 / 우클릭: 제외";
            btnToolPoint.Click += btnToolPoint_Click;
            //
            // btnToolBox
            //
            btnToolBox.ImageScaling = ToolStripItemImageScaling.None;
            btnToolBox.Margin = new Padding(0, 0, 2, 0);
            btnToolBox.Name = "btnToolBox";
            btnToolBox.Padding = new Padding(6, 0, 6, 0);
            btnToolBox.Size = new Size(56, 34);
            btnToolBox.Text = "박스";
            btnToolBox.ToolTipText = "박스 (W) — 드래그해서 객체 범위 지정";
            btnToolBox.Click += btnToolBox_Click;
            //
            // btnToolBrushAdd
            //
            btnToolBrushAdd.ImageScaling = ToolStripItemImageScaling.None;
            btnToolBrushAdd.Margin = new Padding(0, 0, 2, 0);
            btnToolBrushAdd.Name = "btnToolBrushAdd";
            btnToolBrushAdd.Padding = new Padding(6, 0, 6, 0);
            btnToolBrushAdd.Size = new Size(70, 34);
            btnToolBrushAdd.Text = "브러시";
            btnToolBrushAdd.ToolTipText = "브러시 (E) — 원하는 영역을 직접 칠해서 선택합니다.\r\n칠한 뒤 Enter(확정) / Del(지우기) / Ctrl+B(배경 지우기) / Ctrl+S(저장)\r\n잘못 칠한 부분은 마우스 오른쪽 버튼으로 드래그해서 지웁니다.";
            btnToolBrushAdd.Click += btnToolBrushAdd_Click;
            //
            // btnBrushSize
            //
            btnBrushSize.AutoSize = false;
            btnBrushSize.Margin = new Padding(0, 2, 2, 2);
            btnBrushSize.Name = "btnBrushSize";
            btnBrushSize.Size = new Size(20, 30);
            btnBrushSize.ToolTipText = "브러시 크기 + / − (단축키 + / −)";
            //
            // btnBrushShape
            //
            btnBrushShape.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnBrushShape.ImageScaling = ToolStripItemImageScaling.None;
            btnBrushShape.Margin = new Padding(0, 0, 2, 0);
            btnBrushShape.Name = "btnBrushShape";
            btnBrushShape.Padding = new Padding(4, 0, 4, 0);
            btnBrushShape.Size = new Size(30, 34);
            btnBrushShape.Text = "브러시 모양";
            btnBrushShape.ToolTipText = "브러시 모양: 원형 / 네모 (클릭해서 바꾸기)";
            btnBrushShape.Click += btnBrushShape_Click;
            //
            // toolSep2
            //
            toolSep2.Margin = new Padding(8, 0, 8, 0);
            toolSep2.Name = "toolSep2";
            toolSep2.Size = new Size(6, 34);
            //
            // btnEraseObject
            //
            btnEraseObject.ImageScaling = ToolStripItemImageScaling.None;
            btnEraseObject.Margin = new Padding(0, 0, 2, 0);
            btnEraseObject.Name = "btnEraseObject";
            btnEraseObject.Padding = new Padding(6, 0, 6, 0);
            btnEraseObject.Size = new Size(96, 34);
            btnEraseObject.Text = "객체 지우기";
            btnEraseObject.ToolTipText = "선택한 객체를 지우고 배경으로 채우기 (Del)";
            btnEraseObject.Click += btnEraseObject_Click;
            //
            // btnRemoveBackground
            //
            btnRemoveBackground.ImageScaling = ToolStripItemImageScaling.None;
            btnRemoveBackground.Margin = new Padding(0, 0, 2, 0);
            btnRemoveBackground.Name = "btnRemoveBackground";
            btnRemoveBackground.Padding = new Padding(6, 0, 6, 0);
            btnRemoveBackground.Size = new Size(96, 34);
            btnRemoveBackground.Text = "배경 지우기";
            btnRemoveBackground.ToolTipText = "선택한 객체만 남기고 배경을 투명하게 (Ctrl+B)";
            btnRemoveBackground.Click += btnRemoveBackground_Click;
            //
            // toolSep3
            //
            toolSep3.Margin = new Padding(8, 0, 8, 0);
            toolSep3.Name = "toolSep3";
            toolSep3.Size = new Size(6, 34);
            //
            // btnHelp
            //
            btnHelp.Alignment = ToolStripItemAlignment.Right;
            btnHelp.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnHelp.ImageScaling = ToolStripItemImageScaling.None;
            btnHelp.Margin = new Padding(2, 0, 0, 0);
            btnHelp.Name = "btnHelp";
            btnHelp.Padding = new Padding(6, 0, 6, 0);
            btnHelp.Size = new Size(34, 34);
            btnHelp.Text = "도움말";
            btnHelp.ToolTipText = "도움말 (F1)";
            btnHelp.Click += mnuHelpManual_Click;
            //
            // btnUndo
            //
            btnUndo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnUndo.ImageScaling = ToolStripItemImageScaling.None;
            btnUndo.Margin = new Padding(0, 0, 2, 0);
            btnUndo.Name = "btnUndo";
            btnUndo.Padding = new Padding(6, 0, 6, 0);
            btnUndo.Size = new Size(34, 34);
            btnUndo.Text = "실행 취소";
            btnUndo.ToolTipText = "실행 취소 (Ctrl+Z)";
            btnUndo.Click += btnUndo_Click;
            //
            // btnRedo
            //
            btnRedo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnRedo.ImageScaling = ToolStripItemImageScaling.None;
            btnRedo.Margin = new Padding(0, 0, 2, 0);
            btnRedo.Name = "btnRedo";
            btnRedo.Padding = new Padding(6, 0, 6, 0);
            btnRedo.Size = new Size(34, 34);
            btnRedo.Text = "다시 실행";
            btnRedo.ToolTipText = "다시 실행 (Ctrl+Y)";
            btnRedo.Click += btnRedo_Click;
            //
            // btnActual
            //
            btnActual.Alignment = ToolStripItemAlignment.Right;
            btnActual.ImageScaling = ToolStripItemImageScaling.None;
            btnActual.Margin = new Padding(2, 0, 0, 0);
            btnActual.Name = "btnActual";
            btnActual.Padding = new Padding(6, 0, 6, 0);
            btnActual.Size = new Size(64, 34);
            btnActual.Text = "100%";
            btnActual.ToolTipText = "실제 크기 (Ctrl+1)";
            btnActual.Click += btnActual_Click;
            //
            // btnFit
            //
            btnFit.Alignment = ToolStripItemAlignment.Right;
            btnFit.ImageScaling = ToolStripItemImageScaling.None;
            btnFit.Margin = new Padding(2, 0, 0, 0);
            btnFit.Name = "btnFit";
            btnFit.Padding = new Padding(6, 0, 6, 0);
            btnFit.Size = new Size(64, 34);
            btnFit.Text = "맞춤";
            btnFit.ToolTipText = "화면에 맞춤 (Ctrl+0)";
            btnFit.Click += btnFit_Click;
            //
            // toolSepModel
            //
            toolSepModel.Alignment = ToolStripItemAlignment.Right;
            toolSepModel.Name = "toolSepModel";
            toolSepModel.Size = new Size(6, 34);
            //
            // cboInpaintModel
            //
            cboInpaintModel.Alignment = ToolStripItemAlignment.Right;
            cboInpaintModel.BackColor = Color.FromArgb(50, 50, 57);
            cboInpaintModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cboInpaintModel.FlatStyle = FlatStyle.Flat;
            cboInpaintModel.ForeColor = Color.FromArgb(236, 236, 239);
            cboInpaintModel.Items.AddRange(new object[] { "MI-GAN (빠름)", "LaMa (고품질)" });
            cboInpaintModel.Margin = new Padding(2, 0, 4, 0);
            cboInpaintModel.Name = "cboInpaintModel";
            cboInpaintModel.Size = new Size(150, 34);
            cboInpaintModel.ToolTipText = "객체 지우기에 쓸 AI 모델 (없으면 다운로드)";
            cboInpaintModel.SelectedIndexChanged += cboInpaintModel_SelectedIndexChanged;
            //
            // lblInpaintModel
            //
            lblInpaintModel.Alignment = ToolStripItemAlignment.Right;
            lblInpaintModel.ForeColor = Color.FromArgb(160, 160, 170);
            lblInpaintModel.Name = "lblInpaintModel";
            lblInpaintModel.Size = new Size(70, 34);
            lblInpaintModel.Text = "지우기 모델";
            //
            // statusStrip
            //
            statusStrip.BackColor = Color.FromArgb(24, 24, 27);
            statusStrip.ForeColor = Color.FromArgb(160, 160, 170);
            statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, lblImageInfo, lblModel, lblZoom });
            statusStrip.Location = new Point(0, 872);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(10, 2, 10, 2);
            statusStrip.SizingGrip = false;
            statusStrip.Size = new Size(1500, 28);
            statusStrip.TabIndex = 3;
            //
            // lblStatus
            //
            lblStatus.ForeColor = Color.FromArgb(160, 160, 170);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(1000, 19);
            lblStatus.Spring = true;
            lblStatus.Text = "이미지를 열거나 끌어다 놓으세요.";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblImageInfo
            //
            lblImageInfo.ForeColor = Color.FromArgb(160, 160, 170);
            lblImageInfo.Margin = new Padding(12, 3, 0, 2);
            lblImageInfo.Name = "lblImageInfo";
            lblImageInfo.Size = new Size(12, 19);
            lblImageInfo.Text = "-";
            //
            // lblModel
            //
            lblModel.ForeColor = Color.FromArgb(255, 196, 77);
            lblModel.Margin = new Padding(16, 3, 0, 2);
            lblModel.Name = "lblModel";
            lblModel.Size = new Size(70, 19);
            lblModel.Text = "● 모델 없음";
            //
            // lblZoom
            //
            lblZoom.ForeColor = Color.FromArgb(160, 160, 170);
            lblZoom.Margin = new Padding(16, 3, 0, 2);
            lblZoom.Name = "lblZoom";
            lblZoom.Size = new Size(39, 19);
            lblZoom.Text = "100%";
            //
            // splitMain
            //
            splitMain.BackColor = Color.FromArgb(52, 52, 59);
            splitMain.Dock = DockStyle.Fill;
            splitMain.FixedPanel = FixedPanel.Panel2;
            splitMain.Location = new Point(0, 72);
            splitMain.Name = "splitMain";
            //
            // splitMain.Panel1
            //
            splitMain.Panel1.BackColor = Color.FromArgb(17, 17, 19);
            splitMain.Panel1.Controls.Add(canvas);
            splitMain.Panel1.Controls.Add(pnlLeft);
            //
            // pnlLeft
            //
            pnlLeft.BackColor = Color.FromArgb(30, 30, 34);
            pnlLeft.Controls.Add(cardImages);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Padding = new Padding(10);
            pnlLeft.Size = new Size(260, 800);
            pnlLeft.TabIndex = 1;
            //
            // cardImages
            //
            cardImages.BackColor = Color.FromArgb(38, 38, 43);
            cardImages.Controls.Add(lblImagesEmpty);
            cardImages.Controls.Add(lstImages);
            cardImages.Controls.Add(tblImageButtons);
            cardImages.Dock = DockStyle.Fill;
            cardImages.ForeColor = Color.FromArgb(160, 160, 170);
            cardImages.Location = new Point(10, 10);
            cardImages.Name = "cardImages";
            cardImages.Padding = new Padding(8, 38, 8, 10);
            cardImages.Size = new Size(240, 760);
            cardImages.TabIndex = 0;
            cardImages.Title = "이미지";
            //
            // lblImagesEmpty
            //
            lblImagesEmpty.Cursor = Cursors.Hand;
            lblImagesEmpty.Click += mnuFileOpen_Click;
            lblImagesEmpty.BackColor = Color.FromArgb(38, 38, 43);
            lblImagesEmpty.Dock = DockStyle.Fill;
            lblImagesEmpty.ForeColor = Color.FromArgb(105, 105, 115);
            lblImagesEmpty.Location = new Point(8, 38);
            lblImagesEmpty.Name = "lblImagesEmpty";
            lblImagesEmpty.Size = new Size(224, 620);
            lblImagesEmpty.TabIndex = 2;
            lblImagesEmpty.Text = "이미지 파일이나 폴더를\r\n이곳에 끌어다 놓거나\r\n클릭해서 선택하세요.\r\n\r\n여러 장을 목록에 넣고\r\n클릭해 가며 작업한 뒤\r\n한 번에 저장할 수 있습니다.";
            lblImagesEmpty.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lstImages
            //
            lstImages.BackColor = Color.FromArgb(38, 38, 43);
            lstImages.BorderStyle = BorderStyle.None;
            lstImages.Dock = DockStyle.Fill;
            lstImages.DrawMode = DrawMode.OwnerDrawFixed;
            lstImages.ForeColor = Color.FromArgb(236, 236, 239);
            lstImages.IntegralHeight = false;
            lstImages.ItemHeight = 56;
            lstImages.Location = new Point(8, 38);
            lstImages.Name = "lstImages";
            lstImages.Size = new Size(224, 620);
            lstImages.TabIndex = 0;
            lstImages.DrawItem += lstImages_DrawItem;
            lstImages.SelectedIndexChanged += lstImages_SelectedIndexChanged;
            //
            // tblImageButtons
            //
            tblImageButtons.BackColor = Color.FromArgb(38, 38, 43);
            tblImageButtons.ColumnCount = 3;
            tblImageButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            tblImageButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            tblImageButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            tblImageButtons.Controls.Add(btnAddFiles, 0, 0);
            tblImageButtons.Controls.Add(btnAddFolder, 1, 0);
            tblImageButtons.Controls.Add(btnRemoveImage, 2, 0);
            tblImageButtons.Controls.Add(btnSaveBatch, 0, 1);
            tblImageButtons.Dock = DockStyle.Bottom;
            tblImageButtons.Location = new Point(8, 658);
            tblImageButtons.Name = "tblImageButtons";
            tblImageButtons.Padding = new Padding(0, 8, 0, 0);
            tblImageButtons.RowCount = 2;
            tblImageButtons.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tblImageButtons.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            tblImageButtons.Size = new Size(224, 92);
            tblImageButtons.TabIndex = 1;
            //
            // btnAddFiles
            //
            btnAddFiles.BackColor = Color.FromArgb(50, 50, 57);
            btnAddFiles.Cursor = Cursors.Hand;
            btnAddFiles.Dock = DockStyle.Fill;
            btnAddFiles.FlatAppearance.BorderSize = 0;
            btnAddFiles.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnAddFiles.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnAddFiles.FlatStyle = FlatStyle.Flat;
            btnAddFiles.ForeColor = Color.FromArgb(236, 236, 239);
            btnAddFiles.Location = new Point(0, 8);
            btnAddFiles.Margin = new Padding(0, 0, 3, 4);
            btnAddFiles.Name = "btnAddFiles";
            btnAddFiles.Size = new Size(73, 36);
            btnAddFiles.TabIndex = 0;
            btnAddFiles.Text = "파일 추가";
            btnAddFiles.UseVisualStyleBackColor = false;
            btnAddFiles.Click += mnuFileOpen_Click;
            //
            // btnAddFolder
            //
            btnAddFolder.BackColor = Color.FromArgb(50, 50, 57);
            btnAddFolder.Cursor = Cursors.Hand;
            btnAddFolder.Dock = DockStyle.Fill;
            btnAddFolder.FlatAppearance.BorderSize = 0;
            btnAddFolder.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnAddFolder.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnAddFolder.FlatStyle = FlatStyle.Flat;
            btnAddFolder.ForeColor = Color.FromArgb(236, 236, 239);
            btnAddFolder.Location = new Point(76, 8);
            btnAddFolder.Margin = new Padding(0, 0, 3, 4);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Size = new Size(73, 36);
            btnAddFolder.TabIndex = 1;
            btnAddFolder.Text = "폴더 추가";
            btnAddFolder.UseVisualStyleBackColor = false;
            btnAddFolder.Click += mnuFileAddFolder_Click;
            //
            // btnRemoveImage
            //
            btnRemoveImage.BackColor = Color.FromArgb(50, 50, 57);
            btnRemoveImage.Cursor = Cursors.Hand;
            btnRemoveImage.Dock = DockStyle.Fill;
            btnRemoveImage.FlatAppearance.BorderSize = 0;
            btnRemoveImage.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnRemoveImage.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnRemoveImage.FlatStyle = FlatStyle.Flat;
            btnRemoveImage.ForeColor = Color.FromArgb(236, 236, 239);
            btnRemoveImage.Location = new Point(152, 8);
            btnRemoveImage.Margin = new Padding(0, 0, 0, 4);
            btnRemoveImage.Name = "btnRemoveImage";
            btnRemoveImage.Size = new Size(72, 36);
            btnRemoveImage.TabIndex = 2;
            btnRemoveImage.Text = "빼기";
            btnRemoveImage.UseVisualStyleBackColor = false;
            btnRemoveImage.Click += btnRemoveImage_Click;
            //
            // btnSaveBatch
            //
            btnSaveBatch.BackColor = Color.FromArgb(99, 115, 255);
            btnSaveBatch.Cursor = Cursors.Hand;
            tblImageButtons.SetColumnSpan(btnSaveBatch, 3);
            btnSaveBatch.Dock = DockStyle.Fill;
            btnSaveBatch.FlatAppearance.BorderSize = 0;
            btnSaveBatch.FlatAppearance.MouseDownBackColor = Color.FromArgb(82, 96, 228);
            btnSaveBatch.FlatAppearance.MouseOverBackColor = Color.FromArgb(122, 136, 255);
            btnSaveBatch.FlatStyle = FlatStyle.Flat;
            btnSaveBatch.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            btnSaveBatch.ForeColor = Color.White;
            btnSaveBatch.Location = new Point(0, 48);
            btnSaveBatch.Margin = new Padding(0, 4, 0, 0);
            btnSaveBatch.Name = "btnSaveBatch";
            btnSaveBatch.Size = new Size(224, 40);
            btnSaveBatch.TabIndex = 3;
            btnSaveBatch.Text = "전체 일괄 저장";
            btnSaveBatch.UseVisualStyleBackColor = false;
            btnSaveBatch.Click += btnSaveBatch_Click;
            //
            // splitMain.Panel2
            //
            splitMain.Panel2.BackColor = Color.FromArgb(30, 30, 34);
            splitMain.Panel2.Controls.Add(cardObjects);
            splitMain.Panel2.Controls.Add(pnlGap1);
            splitMain.Panel2.Controls.Add(cardRefine);
            splitMain.Panel2.Controls.Add(pnlGap2);
            splitMain.Panel2.Controls.Add(cardExport);
            splitMain.Panel2.Padding = new Padding(10);
            splitMain.Panel2MinSize = 280;
            splitMain.Size = new Size(1500, 800);
            splitMain.SplitterDistance = 1189;
            splitMain.SplitterWidth = 1;
            splitMain.TabIndex = 2;
            //
            // canvas
            //
            canvas.BackColor = Color.FromArgb(17, 17, 19);
            canvas.Dock = DockStyle.Fill;
            canvas.ForeColor = Color.FromArgb(160, 160, 170);
            canvas.Location = new Point(0, 0);
            canvas.Name = "canvas";
            canvas.Size = new Size(929, 800);
            canvas.TabIndex = 0;
            //
            // cardObjects
            //
            cardObjects.BackColor = Color.FromArgb(38, 38, 43);
            cardObjects.Controls.Add(lblObjectsEmpty);
            cardObjects.Controls.Add(lstObjects);
            cardObjects.Controls.Add(tblObjectButtons);
            cardObjects.Dock = DockStyle.Fill;
            cardObjects.ForeColor = Color.FromArgb(160, 160, 170);
            cardObjects.Location = new Point(10, 10);
            cardObjects.Name = "cardObjects";
            cardObjects.Padding = new Padding(10, 38, 10, 10);
            cardObjects.Size = new Size(296, 286);
            cardObjects.TabIndex = 0;
            cardObjects.Title = "객체";
            //
            // lblObjectsEmpty
            //
            lblObjectsEmpty.BackColor = Color.FromArgb(38, 38, 43);
            lblObjectsEmpty.Dock = DockStyle.Fill;
            lblObjectsEmpty.ForeColor = Color.FromArgb(105, 105, 115);
            lblObjectsEmpty.Location = new Point(10, 38);
            lblObjectsEmpty.Name = "lblObjectsEmpty";
            lblObjectsEmpty.Size = new Size(276, 194);
            lblObjectsEmpty.TabIndex = 2;
            lblObjectsEmpty.Text = "아직 확정된 객체가 없습니다.\r\n\r\n이미지에서 객체를 클릭한 뒤\r\nEnter로 확정하세요.";
            lblObjectsEmpty.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lstObjects
            //
            lstObjects.BackColor = Color.FromArgb(38, 38, 43);
            lstObjects.BorderStyle = BorderStyle.None;
            lstObjects.Dock = DockStyle.Fill;
            lstObjects.DrawMode = DrawMode.OwnerDrawFixed;
            lstObjects.ForeColor = Color.FromArgb(236, 236, 239);
            lstObjects.IntegralHeight = false;
            lstObjects.ItemHeight = 34;
            lstObjects.Location = new Point(10, 38);
            lstObjects.Name = "lstObjects";
            lstObjects.Size = new Size(276, 194);
            lstObjects.TabIndex = 0;
            lstObjects.DrawItem += lstObjects_DrawItem;
            lstObjects.SelectedIndexChanged += lstObjects_SelectedIndexChanged;
            lstObjects.KeyDown += lstObjects_KeyDown;
            lstObjects.MouseDown += lstObjects_MouseDown;
            //
            // tblObjectButtons
            //
            tblObjectButtons.BackColor = Color.FromArgb(38, 38, 43);
            tblObjectButtons.ColumnCount = 3;
            tblObjectButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F));
            tblObjectButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26F));
            tblObjectButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tblObjectButtons.Controls.Add(btnCommit, 0, 0);
            tblObjectButtons.Controls.Add(btnClearSelection, 1, 0);
            tblObjectButtons.Controls.Add(btnDeleteObject, 2, 0);
            tblObjectButtons.Dock = DockStyle.Bottom;
            tblObjectButtons.Location = new Point(10, 232);
            tblObjectButtons.Name = "tblObjectButtons";
            tblObjectButtons.Padding = new Padding(0, 8, 0, 0);
            tblObjectButtons.RowCount = 1;
            tblObjectButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblObjectButtons.Size = new Size(276, 44);
            tblObjectButtons.TabIndex = 1;
            //
            // btnCommit
            //
            btnCommit.BackColor = Color.FromArgb(99, 115, 255);
            btnCommit.Cursor = Cursors.Hand;
            btnCommit.Dock = DockStyle.Fill;
            btnCommit.FlatAppearance.BorderSize = 0;
            btnCommit.FlatAppearance.MouseDownBackColor = Color.FromArgb(82, 96, 228);
            btnCommit.FlatAppearance.MouseOverBackColor = Color.FromArgb(122, 136, 255);
            btnCommit.FlatStyle = FlatStyle.Flat;
            btnCommit.Font = new Font("Malgun Gothic", 9F, FontStyle.Bold);
            btnCommit.ForeColor = Color.White;
            btnCommit.Location = new Point(0, 8);
            btnCommit.Margin = new Padding(0, 0, 4, 0);
            btnCommit.Name = "btnCommit";
            btnCommit.Size = new Size(106, 36);
            btnCommit.TabIndex = 0;
            btnCommit.Text = "확정  Enter";
            btnCommit.UseVisualStyleBackColor = false;
            btnCommit.Click += btnCommit_Click;
            //
            // btnClearSelection
            //
            btnClearSelection.BackColor = Color.FromArgb(50, 50, 57);
            btnClearSelection.Cursor = Cursors.Hand;
            btnClearSelection.Dock = DockStyle.Fill;
            btnClearSelection.FlatAppearance.BorderSize = 0;
            btnClearSelection.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnClearSelection.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnClearSelection.FlatStyle = FlatStyle.Flat;
            btnClearSelection.ForeColor = Color.FromArgb(236, 236, 239);
            btnClearSelection.Location = new Point(110, 8);
            btnClearSelection.Margin = new Padding(0, 0, 4, 0);
            btnClearSelection.Name = "btnClearSelection";
            btnClearSelection.Size = new Size(78, 36);
            btnClearSelection.TabIndex = 1;
            btnClearSelection.Text = "초기화";
            btnClearSelection.UseVisualStyleBackColor = false;
            btnClearSelection.Click += btnClearSelection_Click;
            //
            // btnDeleteObject
            //
            btnDeleteObject.BackColor = Color.FromArgb(50, 50, 57);
            btnDeleteObject.Cursor = Cursors.Hand;
            btnDeleteObject.Dock = DockStyle.Fill;
            btnDeleteObject.FlatAppearance.BorderSize = 0;
            btnDeleteObject.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnDeleteObject.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnDeleteObject.FlatStyle = FlatStyle.Flat;
            btnDeleteObject.ForeColor = Color.FromArgb(236, 236, 239);
            btnDeleteObject.Location = new Point(192, 8);
            btnDeleteObject.Margin = new Padding(0);
            btnDeleteObject.Name = "btnDeleteObject";
            btnDeleteObject.Size = new Size(84, 36);
            btnDeleteObject.TabIndex = 2;
            btnDeleteObject.Text = "목록에서 빼기";
            btnDeleteObject.UseVisualStyleBackColor = false;
            btnDeleteObject.Click += btnDeleteObject_Click;
            //
            // pnlGap1
            //
            pnlGap1.BackColor = Color.FromArgb(30, 30, 34);
            pnlGap1.Dock = DockStyle.Bottom;
            pnlGap1.Location = new Point(10, 296);
            pnlGap1.Name = "pnlGap1";
            pnlGap1.Size = new Size(296, 10);
            pnlGap1.TabIndex = 1;
            //
            // cardRefine
            //
            cardRefine.BackColor = Color.FromArgb(38, 38, 43);
            cardRefine.Controls.Add(tblRefine);
            cardRefine.Dock = DockStyle.Bottom;
            cardRefine.ForeColor = Color.FromArgb(160, 160, 170);
            cardRefine.Location = new Point(10, 306);
            cardRefine.Name = "cardRefine";
            cardRefine.Padding = new Padding(12, 38, 12, 10);
            cardRefine.Size = new Size(296, 232);
            cardRefine.TabIndex = 2;
            cardRefine.Title = "마스크 보정";
            //
            // tblRefine
            //
            tblRefine.BackColor = Color.FromArgb(38, 38, 43);
            tblRefine.ColumnCount = 2;
            tblRefine.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblRefine.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56F));
            tblRefine.Controls.Add(lblBrush, 0, 0);
            tblRefine.Controls.Add(lblBrushValue, 1, 0);
            tblRefine.Controls.Add(trkBrush, 0, 1);
            tblRefine.Controls.Add(lblFeather, 0, 2);
            tblRefine.Controls.Add(lblFeatherValue, 1, 2);
            tblRefine.Controls.Add(trkFeather, 0, 3);
            tblRefine.Controls.Add(lblGrow, 0, 4);
            tblRefine.Controls.Add(lblGrowValue, 1, 4);
            tblRefine.Controls.Add(trkGrow, 0, 5);
            tblRefine.Controls.Add(chkCleanup, 0, 6);
            tblRefine.Dock = DockStyle.Fill;
            tblRefine.Location = new Point(12, 38);
            tblRefine.Name = "tblRefine";
            tblRefine.RowCount = 7;
            tblRefine.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tblRefine.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tblRefine.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tblRefine.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tblRefine.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tblRefine.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            tblRefine.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblRefine.Size = new Size(272, 184);
            tblRefine.TabIndex = 0;
            //
            // lblBrush
            //
            lblBrush.Dock = DockStyle.Fill;
            lblBrush.ForeColor = Color.FromArgb(160, 160, 170);
            lblBrush.Location = new Point(0, 0);
            lblBrush.Margin = new Padding(0);
            lblBrush.Name = "lblBrush";
            lblBrush.Size = new Size(216, 22);
            lblBrush.TabIndex = 0;
            lblBrush.Text = "브러시 크기";
            lblBrush.TextAlign = ContentAlignment.BottomLeft;
            //
            // lblBrushValue
            //
            lblBrushValue.Dock = DockStyle.Fill;
            lblBrushValue.ForeColor = Color.FromArgb(236, 236, 239);
            lblBrushValue.Location = new Point(216, 0);
            lblBrushValue.Margin = new Padding(0);
            lblBrushValue.Name = "lblBrushValue";
            lblBrushValue.Size = new Size(56, 22);
            lblBrushValue.TabIndex = 1;
            lblBrushValue.Text = "30";
            lblBrushValue.TextAlign = ContentAlignment.BottomRight;
            //
            // trkBrush
            //
            tblRefine.SetColumnSpan(trkBrush, 2);
            trkBrush.Dock = DockStyle.Fill;
            trkBrush.LargeChange = 10;
            trkBrush.Location = new Point(0, 22);
            trkBrush.Margin = new Padding(0);
            trkBrush.Maximum = 300;
            trkBrush.Minimum = 2;
            trkBrush.Name = "trkBrush";
            trkBrush.Size = new Size(272, 28);
            trkBrush.SmallChange = 2;
            trkBrush.TabIndex = 2;
            trkBrush.Value = 30;
            trkBrush.ValueChanged += trkBrush_ValueChanged;
            //
            // lblFeather
            //
            lblFeather.Dock = DockStyle.Fill;
            lblFeather.ForeColor = Color.FromArgb(160, 160, 170);
            lblFeather.Location = new Point(0, 50);
            lblFeather.Margin = new Padding(0);
            lblFeather.Name = "lblFeather";
            lblFeather.Size = new Size(216, 22);
            lblFeather.TabIndex = 3;
            lblFeather.Text = "경계 부드럽게 (페더)";
            lblFeather.TextAlign = ContentAlignment.BottomLeft;
            //
            // lblFeatherValue
            //
            lblFeatherValue.Dock = DockStyle.Fill;
            lblFeatherValue.ForeColor = Color.FromArgb(236, 236, 239);
            lblFeatherValue.Location = new Point(216, 50);
            lblFeatherValue.Margin = new Padding(0);
            lblFeatherValue.Name = "lblFeatherValue";
            lblFeatherValue.Size = new Size(56, 22);
            lblFeatherValue.TabIndex = 4;
            lblFeatherValue.Text = "1";
            lblFeatherValue.TextAlign = ContentAlignment.BottomRight;
            //
            // trkFeather
            //
            tblRefine.SetColumnSpan(trkFeather, 2);
            trkFeather.Dock = DockStyle.Fill;
            trkFeather.Location = new Point(0, 72);
            trkFeather.Margin = new Padding(0);
            trkFeather.Maximum = 20;
            trkFeather.Name = "trkFeather";
            trkFeather.Size = new Size(272, 28);
            trkFeather.TabIndex = 5;
            trkFeather.Value = 1;
            trkFeather.ValueChanged += trkRefine_ValueChanged;
            //
            // lblGrow
            //
            lblGrow.Dock = DockStyle.Fill;
            lblGrow.ForeColor = Color.FromArgb(160, 160, 170);
            lblGrow.Location = new Point(0, 100);
            lblGrow.Margin = new Padding(0);
            lblGrow.Name = "lblGrow";
            lblGrow.Size = new Size(216, 22);
            lblGrow.TabIndex = 6;
            lblGrow.Text = "확장 / 축소";
            lblGrow.TextAlign = ContentAlignment.BottomLeft;
            //
            // lblGrowValue
            //
            lblGrowValue.Dock = DockStyle.Fill;
            lblGrowValue.ForeColor = Color.FromArgb(236, 236, 239);
            lblGrowValue.Location = new Point(216, 100);
            lblGrowValue.Margin = new Padding(0);
            lblGrowValue.Name = "lblGrowValue";
            lblGrowValue.Size = new Size(56, 22);
            lblGrowValue.TabIndex = 7;
            lblGrowValue.Text = "0";
            lblGrowValue.TextAlign = ContentAlignment.BottomRight;
            //
            // trkGrow
            //
            tblRefine.SetColumnSpan(trkGrow, 2);
            trkGrow.Dock = DockStyle.Fill;
            trkGrow.FillFromZero = true;
            trkGrow.Location = new Point(0, 122);
            trkGrow.Margin = new Padding(0);
            trkGrow.Maximum = 20;
            trkGrow.Minimum = -20;
            trkGrow.Name = "trkGrow";
            trkGrow.Size = new Size(272, 28);
            trkGrow.TabIndex = 8;
            trkGrow.ValueChanged += trkRefine_ValueChanged;
            //
            // chkCleanup
            //
            chkCleanup.Checked = true;
            chkCleanup.CheckState = CheckState.Checked;
            tblRefine.SetColumnSpan(chkCleanup, 2);
            chkCleanup.Dock = DockStyle.Fill;
            chkCleanup.ForeColor = Color.FromArgb(236, 236, 239);
            chkCleanup.Location = new Point(0, 150);
            chkCleanup.Margin = new Padding(0, 4, 0, 0);
            chkCleanup.Name = "chkCleanup";
            chkCleanup.Size = new Size(272, 30);
            chkCleanup.TabIndex = 9;
            chkCleanup.Text = "작은 조각 제거 · 구멍 메우기";
            chkCleanup.UseVisualStyleBackColor = false;
            chkCleanup.CheckedChanged += chkCleanup_CheckedChanged;
            //
            // pnlGap2
            //
            pnlGap2.BackColor = Color.FromArgb(30, 30, 34);
            pnlGap2.Dock = DockStyle.Bottom;
            pnlGap2.Location = new Point(10, 538);
            pnlGap2.Name = "pnlGap2";
            pnlGap2.Size = new Size(296, 10);
            pnlGap2.TabIndex = 3;
            //
            // cardExport
            //
            cardExport.BackColor = Color.FromArgb(38, 38, 43);
            cardExport.Controls.Add(tblExport);
            cardExport.Dock = DockStyle.Bottom;
            cardExport.ForeColor = Color.FromArgb(160, 160, 170);
            cardExport.Location = new Point(10, 548);
            cardExport.Name = "cardExport";
            cardExport.Padding = new Padding(12, 38, 12, 12);
            cardExport.Size = new Size(296, 292);
            cardExport.TabIndex = 4;
            cardExport.Title = "내보내기";
            //
            // tblExport
            //
            tblExport.BackColor = Color.FromArgb(38, 38, 43);
            tblExport.ColumnCount = 2;
            tblExport.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblExport.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblExport.Controls.Add(chkCrop, 0, 0);
            tblExport.Controls.Add(chkBackground, 0, 1);
            tblExport.Controls.Add(btnBackgroundColor, 1, 1);
            tblExport.Controls.Add(tblExportSettings, 0, 2);
            tblExport.Controls.Add(btnSavePng, 0, 3);
            tblExport.Controls.Add(btnSaveAll, 0, 4);
            tblExport.Controls.Add(btnCopy, 1, 4);
            tblExport.Controls.Add(btnSaveImage, 0, 5);
            tblExport.Dock = DockStyle.Fill;
            tblExport.Location = new Point(12, 38);
            tblExport.Name = "tblExport";
            tblExport.RowCount = 6;
            tblExport.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tblExport.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tblExport.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            tblExport.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tblExport.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tblExport.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            tblExport.Size = new Size(272, 242);
            tblExport.TabIndex = 0;
            //
            // chkCrop
            //
            chkCrop.Checked = true;
            chkCrop.CheckState = CheckState.Checked;
            tblExport.SetColumnSpan(chkCrop, 2);
            chkCrop.Dock = DockStyle.Fill;
            chkCrop.ForeColor = Color.FromArgb(236, 236, 239);
            chkCrop.Location = new Point(0, 0);
            chkCrop.Margin = new Padding(0);
            chkCrop.Name = "chkCrop";
            chkCrop.Size = new Size(272, 30);
            chkCrop.TabIndex = 0;
            chkCrop.Text = "객체 크기로 자르기";
            chkCrop.UseVisualStyleBackColor = false;
            //
            // chkBackground
            //
            chkBackground.Dock = DockStyle.Fill;
            chkBackground.ForeColor = Color.FromArgb(236, 236, 239);
            chkBackground.Location = new Point(0, 30);
            chkBackground.Margin = new Padding(0);
            chkBackground.Name = "chkBackground";
            chkBackground.Size = new Size(136, 30);
            chkBackground.TabIndex = 1;
            chkBackground.Text = "배경색 채우기";
            chkBackground.UseVisualStyleBackColor = false;
            chkBackground.CheckedChanged += chkBackground_CheckedChanged;
            //
            // btnBackgroundColor
            //
            btnBackgroundColor.Anchor = AnchorStyles.Right;
            btnBackgroundColor.BackColor = Color.White;
            btnBackgroundColor.Cursor = Cursors.Hand;
            btnBackgroundColor.FlatAppearance.BorderColor = Color.FromArgb(62, 62, 70);
            btnBackgroundColor.FlatStyle = FlatStyle.Flat;
            btnBackgroundColor.Location = new Point(220, 34);
            btnBackgroundColor.Margin = new Padding(0);
            btnBackgroundColor.Name = "btnBackgroundColor";
            btnBackgroundColor.Size = new Size(52, 22);
            btnBackgroundColor.TabIndex = 2;
            btnBackgroundColor.UseVisualStyleBackColor = false;
            btnBackgroundColor.Click += btnBackgroundColor_Click;
            //
            // btnSavePng
            //
            btnSavePng.BackColor = Color.FromArgb(99, 115, 255);
            btnSavePng.Cursor = Cursors.Hand;
            tblExport.SetColumnSpan(btnSavePng, 2);
            btnSavePng.Dock = DockStyle.Fill;
            btnSavePng.FlatAppearance.BorderSize = 0;
            btnSavePng.FlatAppearance.MouseDownBackColor = Color.FromArgb(82, 96, 228);
            btnSavePng.FlatAppearance.MouseOverBackColor = Color.FromArgb(122, 136, 255);
            btnSavePng.FlatStyle = FlatStyle.Flat;
            btnSavePng.Font = new Font("Malgun Gothic", 9.75F, FontStyle.Bold);
            btnSavePng.ForeColor = Color.White;
            btnSavePng.Location = new Point(0, 68);
            btnSavePng.Margin = new Padding(0, 8, 0, 4);
            btnSavePng.Name = "btnSavePng";
            btnSavePng.Size = new Size(272, 36);
            btnSavePng.TabIndex = 3;
            btnSavePng.Text = "선택 객체 저장   Ctrl+S";
            btnSavePng.UseVisualStyleBackColor = false;
            btnSavePng.Click += btnSavePng_Click;
            //
            // btnSaveAll
            //
            btnSaveAll.BackColor = Color.FromArgb(50, 50, 57);
            btnSaveAll.Cursor = Cursors.Hand;
            btnSaveAll.Dock = DockStyle.Fill;
            btnSaveAll.FlatAppearance.BorderSize = 0;
            btnSaveAll.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnSaveAll.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnSaveAll.FlatStyle = FlatStyle.Flat;
            btnSaveAll.ForeColor = Color.FromArgb(236, 236, 239);
            btnSaveAll.Location = new Point(0, 108);
            btnSaveAll.Margin = new Padding(0, 4, 2, 0);
            btnSaveAll.Name = "btnSaveAll";
            btnSaveAll.Size = new Size(134, 36);
            btnSaveAll.TabIndex = 4;
            btnSaveAll.Text = "모두 저장";
            btnSaveAll.UseVisualStyleBackColor = false;
            btnSaveAll.Click += btnSaveAll_Click;
            //
            // btnCopy
            //
            btnCopy.BackColor = Color.FromArgb(50, 50, 57);
            btnCopy.Cursor = Cursors.Hand;
            btnCopy.Dock = DockStyle.Fill;
            btnCopy.FlatAppearance.BorderSize = 0;
            btnCopy.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnCopy.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnCopy.FlatStyle = FlatStyle.Flat;
            btnCopy.ForeColor = Color.FromArgb(236, 236, 239);
            btnCopy.Location = new Point(138, 108);
            btnCopy.Margin = new Padding(2, 4, 0, 0);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(134, 36);
            btnCopy.TabIndex = 5;
            btnCopy.Text = "클립보드 복사";
            btnCopy.UseVisualStyleBackColor = false;
            btnCopy.Click += btnCopy_Click;
            //
            // mnuFileSep3
            //
            mnuFileSep3.Name = "mnuFileSep3";
            mnuFileSep3.Size = new Size(277, 6);
            //
            // mnuFileExportSettings
            //
            mnuFileExportSettings.Name = "mnuFileExportSettings";
            mnuFileExportSettings.Size = new Size(280, 24);
            mnuFileExportSettings.Text = "내보내기 설정... (형식 / 품질 / 크기 / 위치)";
            mnuFileExportSettings.Click += btnExportSettings_Click;
            //
            // mnuFileOpenOutput
            //
            mnuFileOpenOutput.Name = "mnuFileOpenOutput";
            mnuFileOpenOutput.Size = new Size(280, 24);
            mnuFileOpenOutput.Text = "저장 폴더 열기";
            mnuFileOpenOutput.Click += mnuFileOpenOutput_Click;
            //
            // tblExportSettings
            //
            tblExportSettings.ColumnCount = 2;
            tblExportSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblExportSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tblExportSettings.Controls.Add(lblExportSummary, 0, 0);
            tblExportSettings.Controls.Add(btnExportSettings, 1, 0);
            tblExport.SetColumnSpan(tblExportSettings, 2);
            tblExportSettings.Dock = DockStyle.Fill;
            tblExportSettings.Location = new Point(0, 60);
            tblExportSettings.Margin = new Padding(0, 4, 0, 0);
            tblExportSettings.Name = "tblExportSettings";
            tblExportSettings.RowCount = 1;
            tblExportSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblExportSettings.Size = new Size(272, 42);
            tblExportSettings.TabIndex = 7;
            //
            // lblExportSummary
            //
            lblExportSummary.Dock = DockStyle.Fill;
            lblExportSummary.ForeColor = Color.FromArgb(160, 160, 170);
            lblExportSummary.Location = new Point(0, 0);
            lblExportSummary.Margin = new Padding(0);
            lblExportSummary.Name = "lblExportSummary";
            lblExportSummary.Size = new Size(212, 42);
            lblExportSummary.TabIndex = 0;
            lblExportSummary.Text = "PNG (투명 지원) · 원본 크기 → 원본 위치\\output";
            lblExportSummary.TextAlign = ContentAlignment.MiddleLeft;
            //
            // btnExportSettings
            //
            btnExportSettings.BackColor = Color.FromArgb(50, 50, 57);
            btnExportSettings.Cursor = Cursors.Hand;
            btnExportSettings.Dock = DockStyle.Fill;
            btnExportSettings.FlatAppearance.BorderSize = 0;
            btnExportSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnExportSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnExportSettings.FlatStyle = FlatStyle.Flat;
            btnExportSettings.ForeColor = Color.FromArgb(236, 236, 239);
            btnExportSettings.Location = new Point(212, 4);
            btnExportSettings.Margin = new Padding(0, 4, 0, 4);
            btnExportSettings.Name = "btnExportSettings";
            btnExportSettings.Size = new Size(60, 34);
            btnExportSettings.TabIndex = 1;
            btnExportSettings.Text = "설정";
            btnExportSettings.UseVisualStyleBackColor = false;
            btnExportSettings.Click += btnExportSettings_Click;
            //
            // btnSaveImage
            //
            btnSaveImage.BackColor = Color.FromArgb(50, 50, 57);
            btnSaveImage.Cursor = Cursors.Hand;
            tblExport.SetColumnSpan(btnSaveImage, 2);
            btnSaveImage.Dock = DockStyle.Fill;
            btnSaveImage.FlatAppearance.BorderSize = 0;
            btnSaveImage.FlatAppearance.MouseDownBackColor = Color.FromArgb(38, 38, 43);
            btnSaveImage.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 70);
            btnSaveImage.FlatStyle = FlatStyle.Flat;
            btnSaveImage.ForeColor = Color.FromArgb(236, 236, 239);
            btnSaveImage.Location = new Point(0, 156);
            btnSaveImage.Margin = new Padding(0, 6, 0, 0);
            btnSaveImage.Name = "btnSaveImage";
            btnSaveImage.Size = new Size(272, 38);
            btnSaveImage.TabIndex = 6;
            btnSaveImage.Text = "편집한 이미지 저장   Ctrl+Alt+S";
            btnSaveImage.UseVisualStyleBackColor = false;
            btnSaveImage.Click += btnSaveImage_Click;
            //
            // MainForm
            //
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(24, 24, 27);
            ClientSize = new Size(1500, 900);
            Controls.Add(splitMain);
            Controls.Add(statusStrip);
            Controls.Add(toolStrip);
            Controls.Add(menuStrip);
            ForeColor = Color.FromArgb(236, 236, 239);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(1180, 720);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Nukki Studio";
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            KeyDown += MainForm_KeyDown;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            pnlLeft.ResumeLayout(false);
            cardImages.ResumeLayout(false);
            tblImageButtons.ResumeLayout(false);
            cardObjects.ResumeLayout(false);
            tblObjectButtons.ResumeLayout(false);
            cardRefine.ResumeLayout(false);
            tblRefine.ResumeLayout(false);
            cardExport.ResumeLayout(false);
            tblExportSettings.ResumeLayout(false);
            tblExport.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuFileOpen;
        private ToolStripMenuItem mnuFilePaste;
        private ToolStripSeparator mnuFileSep1;
        private ToolStripMenuItem mnuFileSave;
        private ToolStripMenuItem mnuFileSaveAll;
        private ToolStripMenuItem mnuFileCopy;
        private ToolStripSeparator mnuFileSep2;
        private ToolStripMenuItem mnuFileExit;
        private ToolStripMenuItem mnuEdit;
        private ToolStripMenuItem mnuEditUndo;
        private ToolStripMenuItem mnuEditRedo;
        private ToolStripSeparator mnuEditSep1;
        private ToolStripMenuItem mnuEditCommit;
        private ToolStripMenuItem mnuEditClear;
        private ToolStripMenuItem mnuView;
        private ToolStripMenuItem mnuViewOverlay;
        private ToolStripMenuItem mnuViewCutout;
        private ToolStripMenuItem mnuViewMask;
        private ToolStripSeparator mnuViewSep1;
        private ToolStripMenuItem mnuViewFit;
        private ToolStripMenuItem mnuViewActual;
        private ToolStripButton btnAutoDetect;
        private ToolStripMenuItem mnuEditAutoDetect;
        private ToolStripMenuItem mnuFileAddFolder;
        private ToolStripSeparator mnuEditSep3;
        private ToolStripMenuItem mnuEditCut;
        private ToolStripMenuItem mnuEditPaste;
        private ToolStripMenuItem mnuFileSaveBatch;
        private Panel pnlLeft;
        private NukkiStudio.App.Controls.CardPanel cardImages;
        private Label lblImagesEmpty;
        private ListBox lstImages;
        private TableLayoutPanel tblImageButtons;
        private Button btnAddFiles;
        private Button btnAddFolder;
        private Button btnRemoveImage;
        private Button btnSaveBatch;
        private ToolStripMenuItem mnuFileSaveImage;
        private ToolStripSeparator mnuEditSep2;
        private ToolStripMenuItem mnuEditErase;
        private ToolStripMenuItem mnuEditRemoveBg;
        private ToolStripMenuItem mnuHelp;
        private ToolStripMenuItem mnuHelpManual;
        private ToolStripMenuItem mnuHelpShortcuts;
        private ToolStripSeparator mnuHelpSep1;
        private ToolStripMenuItem mnuHelpAbout;
        private ToolStripSeparator toolSep3;
        private ToolStripButton btnEraseObject;
        private ToolStripButton btnRemoveBackground;
        private ToolStripButton btnHelp;
        private NukkiStudio.App.Controls.ToolStripBrushSize btnBrushSize;
        private ToolStripButton btnBrushShape;
        private ToolStripSeparator toolSepModel;
        private ToolStripComboBox cboInpaintModel;
        private ToolStripLabel lblInpaintModel;
        private ToolStripMenuItem mnuSettingsDownload;
        private ToolStripSeparator mnuSettingsSep1;
        private ToolStripSeparator mnuEditSep4;
        private ToolStripMenuItem mnuEditResize;
        private Button btnSaveImage;
        private ToolStripSeparator mnuFileSep3;
        private ToolStripMenuItem mnuFileExportSettings;
        private ToolStripMenuItem mnuFileOpenOutput;
        private TableLayoutPanel tblExportSettings;
        private Label lblExportSummary;
        private Button btnExportSettings;
        private ToolStripMenuItem mnuSettings;
        private ToolStripMenuItem mnuSettingsModel;
        private ToolStripMenuItem mnuSettingsDevice;
        private ToolStripMenuItem mnuDeviceAuto;
        private ToolStripMenuItem mnuDeviceCpu;
        private ToolStrip toolStrip;
        private ToolStripButton btnOpen;
        private ToolStripButton btnSave;
        private ToolStripSeparator toolSep1;
        private ToolStripButton btnToolPoint;
        private ToolStripButton btnToolBox;
        private ToolStripButton btnToolBrushAdd;
        private ToolStripSeparator toolSep2;
        private ToolStripButton btnUndo;
        private ToolStripButton btnRedo;
        private ToolStripButton btnActual;
        private ToolStripButton btnFit;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblImageInfo;
        private ToolStripStatusLabel lblModel;
        private ToolStripStatusLabel lblZoom;
        private SplitContainer splitMain;
        private NukkiStudio.App.Controls.ImageCanvas canvas;
        private NukkiStudio.App.Controls.CardPanel cardObjects;
        private Label lblObjectsEmpty;
        private ListBox lstObjects;
        private TableLayoutPanel tblObjectButtons;
        private Button btnCommit;
        private Button btnClearSelection;
        private Button btnDeleteObject;
        private Panel pnlGap1;
        private NukkiStudio.App.Controls.CardPanel cardRefine;
        private TableLayoutPanel tblRefine;
        private Label lblBrush;
        private Label lblBrushValue;
        private NukkiStudio.App.Controls.FlatSlider trkBrush;
        private Label lblFeather;
        private Label lblFeatherValue;
        private NukkiStudio.App.Controls.FlatSlider trkFeather;
        private Label lblGrow;
        private Label lblGrowValue;
        private NukkiStudio.App.Controls.FlatSlider trkGrow;
        private NukkiStudio.App.Controls.ToggleSwitch chkCleanup;
        private Panel pnlGap2;
        private NukkiStudio.App.Controls.CardPanel cardExport;
        private TableLayoutPanel tblExport;
        private NukkiStudio.App.Controls.ToggleSwitch chkCrop;
        private NukkiStudio.App.Controls.ToggleSwitch chkBackground;
        private Button btnBackgroundColor;
        private Button btnSavePng;
        private Button btnSaveAll;
        private Button btnCopy;
    }
}
