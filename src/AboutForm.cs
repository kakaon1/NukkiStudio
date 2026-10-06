using System.Drawing.Drawing2D;
using System.Reflection;
using NukkiStudio.App.UI;

namespace NukkiStudio.App;

/// <summary>프로그램 정보 + 사용한 오픈소스 / AI 모델의 저작권 고지와 라이선스 전문.</summary>
public partial class AboutForm : Form
{
    private sealed record LicenseEntry(string Title, string License, string ResourceName)
    {
        public override string ToString() => Title;
    }

    // 리소스 이름: csproj의 EmbeddedResource LogicalName (Licenses.<파일명>.txt)
    private static readonly LicenseEntry[] Entries =
    {
        new("MobileSAM", "Apache License 2.0 · AI 모델", "Licenses.01_MobileSAM.txt"),
        new("Segment Anything (Meta)", "Apache License 2.0 · AI 모델", "Licenses.02_SegmentAnything.txt"),
        new("MI-GAN (Picsart)", "MIT License · AI 모델", "Licenses.03_MI-GAN.txt"),
        new("D-FINE-N", "Apache License 2.0 · AI 모델", "Licenses.07_D-FINE.txt"),
        new("LaMa (OpenCV Zoo)", "Apache License 2.0 · AI 모델", "Licenses.08_LaMa.txt"),
        new("ONNX Runtime", "MIT License · Microsoft", "Licenses.04_ONNXRuntime.txt"),
        new("DirectML", "Microsoft 소프트웨어 사용 조건", "Licenses.05_DirectML.txt"),
        new(".NET Runtime / WinForms", "MIT License · .NET Foundation", "Licenses.06_DotNet.txt"),
    };

    public AboutForm()
    {
        InitializeComponent();
        if (DesignMode) return;

        picLogo.Image = new Icon(Icon!, 64, 64).ToBitmap();
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        lblVersion.Text = version is null ? "" : $"버전 {version.Major}.{version.Minor}.{version.Build}";
        lstLicenses.Items.AddRange(Entries);
        if (lstLicenses.Items.Count > 0) lstLicenses.SelectedIndex = 0;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!DesignMode) Theme.ApplyDarkTitleBar(Handle);
    }

    private static string ReadLicense(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null) return "(라이선스 텍스트를 찾을 수 없습니다: " + resourceName + ")";
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private void lstLicenses_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lstLicenses.SelectedItem is not LicenseEntry entry) return;
        txtLicense.Text = ReadLicense(entry.ResourceName);
        txtLicense.SelectionStart = 0;
        txtLicense.ScrollToCaret();
    }

    private void lstLicenses_DrawItem(object? sender, DrawItemEventArgs e)
    {
        var g = e.Graphics;
        using (var back = new SolidBrush(lstLicenses.BackColor)) g.FillRectangle(back, e.Bounds);
        if (e.Index < 0 || lstLicenses.Items[e.Index] is not LicenseEntry entry) return;

        g.SmoothingMode = SmoothingMode.AntiAlias;
        if ((e.State & DrawItemState.Selected) != 0)
        {
            var row = new RectangleF(e.Bounds.X + 4, e.Bounds.Y + 3, e.Bounds.Width - 8, e.Bounds.Height - 6);
            using var path = Theme.RoundRect(row, 6);
            using var fill = new SolidBrush(Theme.Raised);
            g.FillPath(fill, path);
            using var bar = new SolidBrush(Theme.Accent);
            g.FillRectangle(bar, row.X, row.Y + 8, 3, row.Height - 16);
        }

        var titleRect = new Rectangle(e.Bounds.X + 16, e.Bounds.Y + 5, e.Bounds.Width - 24, 20);
        TextRenderer.DrawText(g, entry.Title, lstLicenses.Font, titleRect, Theme.Text, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        var subRect = new Rectangle(e.Bounds.X + 16, e.Bounds.Y + 23, e.Bounds.Width - 24, 18);
        TextRenderer.DrawText(g, entry.License, lstLicenses.Font, subRect, Theme.TextDim, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
    }
}
