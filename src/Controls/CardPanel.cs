using System.ComponentModel;
using System.Drawing.Drawing2D;
using NukkiStudio.App.UI;

namespace NukkiStudio.App.Controls;

/// <summary>둥근 모서리 카드 + 제목. 자식 컨트롤은 Padding 안쪽(제목 아래)에 배치한다.</summary>
public sealed class CardPanel : Panel
{
    private string _title = "";

    public CardPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor = Color.FromArgb(38, 38, 43);
        ForeColor = Color.FromArgb(160, 160, 170);
        Padding = new Padding(12, 38, 12, 12);
    }

    [Category("Appearance"), DefaultValue("")]
    public string Title
    {
        get => _title;
        set { _title = value ?? ""; Invalidate(); }
    }

    [Category("Appearance"), DefaultValue(10)]
    public int CornerRadius { get; set; } = 10;

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // 모서리 바깥은 부모 배경색
        e.Graphics.Clear(Parent?.BackColor ?? Color.FromArgb(30, 30, 34));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
        using (var path = Theme.RoundRect(r, CornerRadius))
        using (var fill = new SolidBrush(BackColor))
        using (var border = new Pen(Theme.Border))
        {
            g.FillPath(fill, path);
            g.DrawPath(border, path);
        }

        if (_title.Length > 0)
        {
            using var font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            TextRenderer.DrawText(g, _title, font, new Point(Padding.Left - 1, 12), ForeColor, TextFormatFlags.NoPadding);
        }
    }
}
