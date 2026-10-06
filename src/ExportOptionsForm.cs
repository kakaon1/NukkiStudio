using NukkiStudio.App.Export;
using NukkiStudio.App.UI;

namespace NukkiStudio.App;

/// <summary>내보내기 설정: 파일 형식 / JPG 품질 / 크기 / 저장 위치.</summary>
public partial class ExportOptionsForm : Form
{
    private bool _loading;

    /// <summary>확인을 누르면 바뀐 설정.</summary>
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public ExportSettings Settings { get; private set; } = new();

    /// <summary>예시 경로 표시에 쓰는 현재 이미지 경로 (없으면 null).</summary>
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public string? SampleSourcePath { get; set; }

    public ExportOptionsForm()
    {
        InitializeComponent();
        if (DesignMode) return;
    }

    public ExportOptionsForm(ExportSettings settings, string? sampleSourcePath) : this()
    {
        if (DesignMode) return;
        SampleSourcePath = sampleSourcePath;
        LoadSettings(settings);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!DesignMode) Theme.ApplyDarkTitleBar(Handle);
    }

    private void LoadSettings(ExportSettings s)
    {
        _loading = true;
        rbPng.Checked = s.Format == ExportFormat.Png;
        rbJpg.Checked = s.Format == ExportFormat.Jpeg;
        rbBmp.Checked = s.Format == ExportFormat.Bmp;
        trkQuality.Value = s.JpegQuality;
        rbSizeOriginal.Checked = s.Resize == ExportResize.Original;
        rbSizePercent.Checked = s.Resize == ExportResize.Percent;
        rbSizeMax.Checked = s.Resize == ExportResize.MaxSide;
        trkPercent.Value = s.Percent;
        numMaxSide.Value = Math.Clamp(s.MaxSide, (int)numMaxSide.Minimum, (int)numMaxSide.Maximum);
        rbLocSource.Checked = s.UseSourceFolder;
        rbLocCustom.Checked = !s.UseSourceFolder;
        txtFolder.Text = s.OutputFolder;
        _loading = false;
        UpdateState();
    }

    private ExportSettings ReadSettings() => new()
    {
        Format = rbJpg.Checked ? ExportFormat.Jpeg : rbBmp.Checked ? ExportFormat.Bmp : ExportFormat.Png,
        JpegQuality = trkQuality.Value,
        Resize = rbSizePercent.Checked ? ExportResize.Percent : rbSizeMax.Checked ? ExportResize.MaxSide : ExportResize.Original,
        Percent = trkPercent.Value,
        MaxSide = (int)numMaxSide.Value,
        UseSourceFolder = rbLocSource.Checked,
        OutputFolder = txtFolder.Text.Trim(),
    };

    private void Option_Changed(object? sender, EventArgs e)
    {
        if (!_loading) UpdateState();
    }

    /// <summary>선택에 따라 관련 입력만 활성화하고, 저장될 경로 예시를 보여 준다.</summary>
    private void UpdateState()
    {
        lblQualityValue.Text = trkQuality.Value.ToString();
        lblPercentValue.Text = $"{trkPercent.Value}%";
        trkQuality.Enabled = lblQuality.Enabled = rbJpg.Checked;
        trkPercent.Enabled = rbSizePercent.Checked;
        numMaxSide.Enabled = rbSizeMax.Checked;
        txtFolder.Enabled = btnBrowse.Enabled = rbLocCustom.Checked;

        var s = ReadSettings();
        string folder;
        if (!s.UseSourceFolder && !string.IsNullOrWhiteSpace(s.OutputFolder)) folder = s.OutputFolder;
        else if (SampleSourcePath is not null) folder = Path.Combine(Path.GetDirectoryName(SampleSourcePath) ?? "", ExportSettings.OutputFolderName);
        else folder = Path.Combine("(원본 파일 폴더)", ExportSettings.OutputFolderName);
        string name = SampleSourcePath is null ? "이미지" : Path.GetFileNameWithoutExtension(SampleSourcePath);

        string note = s.Format == ExportFormat.Png ? "" : "\r\nJPG / BMP는 투명을 지원하지 않아 투명한 부분은 배경색(기본 흰색)으로 채워집니다.";
        lblExample.Text = $"저장 예: {Path.Combine(folder, name + "_사람_1" + s.Extension)}{note}";
    }

    private void btnBrowse_Click(object? sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog { Description = "결과를 저장할 폴더를 선택하세요", UseDescriptionForTitle = true, InitialDirectory = txtFolder.Text };
        if (dlg.ShowDialog(this) == DialogResult.OK) txtFolder.Text = dlg.SelectedPath;
    }

    private void btnOk_Click(object? sender, EventArgs e)
    {
        var s = ReadSettings();
        if (!s.UseSourceFolder && string.IsNullOrWhiteSpace(s.OutputFolder))
        {
            MessageBox.Show(this, "저장할 폴더를 지정하세요.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        Settings = s;
        DialogResult = DialogResult.OK;
        Close();
    }
}
