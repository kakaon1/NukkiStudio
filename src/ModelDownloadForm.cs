using System.ComponentModel;
using NukkiStudio.App.Models;
using NukkiStudio.App.UI;

namespace NukkiStudio.App;

/// <summary>
/// AI 모델 확인 / 다운로드 창. 설치 상태를 보여 주고, 사용자가 고른 모델만 진행률과 함께 내려받는다.
/// (데이터 요금 때문에 사용자가 [다운로드]를 누르기 전에는 아무것도 받지 않는다)
/// </summary>
public partial class ModelDownloadForm : Form
{
    private sealed record Row(ModelPackage Package, CheckBox Check, Label Size, Label State);

    private Row[] _rows = Array.Empty<Row>();
    private CancellationTokenSource? _download;

    /// <summary>models 폴더 경로.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ModelsDirectory { get; set; } = "";

    /// <summary>미리 체크할 모델 Id (비어 있으면 설치되지 않은 모델 전부).</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string[] PreselectIds { get; set; } = Array.Empty<string>();

    /// <summary>창 위쪽 안내 문구를 바꿀 때.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? IntroText { get; set; }

    /// <summary>이번에 새로 설치된 모델이 있는지.</summary>
    [Browsable(false)]
    public bool Installed { get; private set; }

    public ModelDownloadForm()
    {
        InitializeComponent();
        if (DesignMode) return;

        _rows = new[]
        {
            new Row(ModelPackages.Sam, chkSam, lblSamSize, lblSamState),
            new Row(ModelPackages.Detector, chkDetector, lblDetectorSize, lblDetectorState),
            new Row(ModelPackages.MiGan, chkMiGan, lblMiGanSize, lblMiGanState),
            new Row(ModelPackages.Lama, chkLama, lblLamaSize, lblLamaState),
        };
        Load += (_, _) => RefreshRows(initial: true);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!DesignMode) Theme.ApplyDarkTitleBar(Handle);
    }

    /// <summary>설치되지 않은 모델 목록.</summary>
    public static IReadOnlyList<ModelPackage> Missing(string modelsDirectory) =>
        ModelPackages.All.Where(p => !p.IsInstalled(modelsDirectory)).ToList();

    private void RefreshRows(bool initial)
    {
        if (IntroText is not null) lblIntro.Text = IntroText;
        foreach (var row in _rows)
        {
            var p = row.Package;
            bool installed = p.IsInstalled(ModelsDirectory);
            row.Check.Text = $"{p.Title} — {p.Purpose}";
            row.Size.Text = p.SizeText;
            row.State.Text = installed ? "설치됨" : "없음";
            row.State.ForeColor = installed ? Theme.Success : (p.Required ? Theme.Danger : Theme.TextDim);
            // 설치된 모델은 체크할 수 없게 (비활성 회색 글자는 어두운 배경에서 읽기 어려워 AutoCheck만 끈다)
            row.Check.Enabled = true;
            row.Check.AutoCheck = !installed;
            row.Check.Cursor = installed ? Cursors.Default : Cursors.Hand;
            row.Check.ForeColor = installed ? Theme.TextDim : Theme.Text;
            if (installed) row.Check.Checked = false;
            else if (initial) row.Check.Checked = PreselectIds.Length == 0 || PreselectIds.Contains(p.Id);
        }
        UpdateTotal();
    }

    private IReadOnlyList<ModelPackage> Selected() =>
        _rows.Where(r => r.Check.Checked && r.Check.AutoCheck && !r.Package.IsInstalled(ModelsDirectory)).Select(r => r.Package).ToList();

    private void Selection_Changed(object? sender, EventArgs e) => UpdateTotal();

    private void UpdateTotal()
    {
        var selected = Selected();
        long total = selected.Sum(p => p.ApproxBytes);
        lblTotal.Text = selected.Count == 0
            ? (_rows.All(r => r.Package.IsInstalled(ModelsDirectory)) ? "모든 모델이 설치되어 있습니다." : "받을 모델을 선택하세요.")
            : $"선택한 모델 {selected.Count}개 · 합계 약 {ModelPackage.FormatSize(total)}";
        btnDownload.Enabled = selected.Count > 0 && _download is null;
        btnDownload.BackColor = btnDownload.Enabled ? Theme.Accent : Theme.Raised;
    }

    private async void btnDownload_Click(object? sender, EventArgs e)
    {
        var selected = Selected();
        if (selected.Count == 0 || _download is not null) return;

        _download = new CancellationTokenSource();
        foreach (var row in _rows) row.Check.AutoCheck = false;
        btnDownload.Enabled = false;
        btnDownload.BackColor = Theme.Raised;
        btnClose.Text = "취소";
        prgDownload.Value = 0;

        long grandTotal = selected.Sum(p => p.ApproxBytes);
        long finishedBytes = 0;
        int lastIndex = 0;
        bool finished = false;
        var progress = new Progress<DownloadProgress>(p =>
        {
            // Progress는 보고를 UI 스레드에 나중에 전달하므로, 끝난 뒤 늦게 도착한 보고가
            // "설치됨 / 완료" 표시를 "설치 중"으로 덮어쓰지 않게 버린다
            if (finished) return;
            if (p.Index != lastIndex)
            {
                finishedBytes += selected[lastIndex].ApproxBytes;
                lastIndex = p.Index;
            }
            long total = p.Total ?? p.Package.ApproxBytes;
            double fraction = total > 0 ? Math.Clamp((double)p.Received / total, 0, 1) : 0;
            double overall = (finishedBytes + fraction * p.Package.ApproxBytes) / grandTotal;
            prgDownload.Value = (int)Math.Round(Math.Clamp(overall, 0, 1) * 1000);
            lblProgress.Text = p.Stage == "설치 중"
                ? $"[{p.Index + 1}/{p.Count}] {p.Package.Title} 설치 중... · 전체 {overall * 100:0}%"
                : $"[{p.Index + 1}/{p.Count}] {p.Package.Title} {fraction * 100:0}% ({ModelPackage.FormatSize(p.Received)} / {ModelPackage.FormatSize(total)}) · 전체 {overall * 100:0}%";
            var row = _rows.First(r => r.Package == p.Package);
            row.State.Text = p.Stage == "설치 중" ? "설치 중" : $"{fraction * 100:0}%";
            row.State.ForeColor = Theme.AccentHover;
        });

        try
        {
            await ModelDownloader.InstallAsync(selected, ModelsDirectory, progress, _download.Token);
            Installed = true;
            prgDownload.Value = prgDownload.Maximum;
            lblProgress.Text = $"완료 — 모델 {selected.Count}개를 설치했습니다.";
            lblProgress.ForeColor = Theme.Success;
        }
        catch (OperationCanceledException)
        {
            lblProgress.Text = "다운로드를 취소했습니다. 이미 끝난 모델은 설치되어 있습니다.";
            lblProgress.ForeColor = Theme.TextDim;
        }
        catch (Exception ex)
        {
            lblProgress.Text = "다운로드 실패: " + ex.Message;
            lblProgress.ForeColor = Theme.Danger;
            MessageBox.Show(this, $"모델을 받지 못했습니다.\n인터넷 연결을 확인한 뒤 다시 시도하세요.\n\n{ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            finished = true;
            _download.Dispose();
            _download = null;
            // 일부만 설치되었을 수도 있으므로 다시 확인
            if (_rows.Any(r => r.Package.IsInstalled(ModelsDirectory) && selected.Contains(r.Package))) Installed = true;
            btnClose.Text = "닫기";
            RefreshRows(initial: false);
        }
    }

    private void btnClose_Click(object? sender, EventArgs e)
    {
        if (_download is not null)
        {
            _download.Cancel();
            return;
        }
        DialogResult = Installed ? DialogResult.OK : DialogResult.Cancel;
        Close();
    }

    private void ModelDownloadForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // 받는 중에 창을 닫으면 먼저 취소하고, 정리가 끝난 뒤 닫는다
        if (_download is not null)
        {
            _download.Cancel();
            e.Cancel = true;
            return;
        }
        if (Installed) DialogResult = DialogResult.OK;
    }
}
