using System.ComponentModel;
using NukkiStudio.App.UI;

namespace NukkiStudio.App;

/// <summary>이미지 해상도(픽셀 크기) 조절 창. 비율 유지 / 빠른 배율 버튼.</summary>
public partial class ImageResizeForm : Form
{
    private Size _original = new(16, 16);
    private bool _syncing;

    /// <summary>현재 이미지 크기 (창을 열기 전에 지정).</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Size OriginalSize
    {
        get => _original;
        set
        {
            _original = value;
            lblCurrent.Text = $"{value.Width} × {value.Height}";
            SetSize(value.Width, value.Height);
        }
    }

    /// <summary>선택한 새 크기.</summary>
    [Browsable(false)]
    public Size ResultSize => new((int)numWidth.Value, (int)numHeight.Value);

    public ImageResizeForm()
    {
        InitializeComponent();
        if (DesignMode) return;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!DesignMode) Theme.ApplyDarkTitleBar(Handle);
    }

    private void SetSize(int width, int height)
    {
        _syncing = true;
        numWidth.Value = Math.Clamp(width, (int)numWidth.Minimum, (int)numWidth.Maximum);
        numHeight.Value = Math.Clamp(height, (int)numHeight.Minimum, (int)numHeight.Maximum);
        _syncing = false;
    }

    private void numWidth_ValueChanged(object? sender, EventArgs e)
    {
        if (_syncing || !chkKeepRatio.Checked || _original.Width == 0) return;
        int width = (int)numWidth.Value;
        SetSize(width, Math.Max(1, (int)Math.Round(width * (double)_original.Height / _original.Width)));
    }

    private void numHeight_ValueChanged(object? sender, EventArgs e)
    {
        if (_syncing || !chkKeepRatio.Checked || _original.Height == 0) return;
        int height = (int)numHeight.Value;
        SetSize(Math.Max(1, (int)Math.Round(height * (double)_original.Width / _original.Height)), height);
    }

    private void Preset_Click(object? sender, EventArgs e)
    {
        if (sender is not Button { Tag: string tag } || !int.TryParse(tag, out int percent)) return;
        SetSize((int)Math.Round(_original.Width * percent / 100.0), (int)Math.Round(_original.Height * percent / 100.0));
    }
}
