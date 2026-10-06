using System.Drawing.Drawing2D;

namespace NukkiStudio.App.UI;

/// <summary>메뉴 / 도구 모음 / 상태 표시줄 다크 렌더러.</summary>
internal sealed class DarkToolStripRenderer : ToolStripProfessionalRenderer
{
    public DarkToolStripRenderer() : base(new DarkColorTable())
    {
        RoundedEdges = false;
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        Color back = e.ToolStrip switch
        {
            ToolStripDropDown => Theme.Card,
            StatusStrip => Theme.Window,
            MenuStrip => Theme.Window,
            _ => Theme.Surface,
        };
        using var brush = new SolidBrush(back);
        e.Graphics.FillRectangle(brush, e.AffectedBounds);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        using var pen = new Pen(Theme.Border);
        var r = e.AffectedBounds;
        switch (e.ToolStrip)
        {
            case ToolStripDropDown:
                e.Graphics.DrawRectangle(pen, 0, 0, r.Width - 1, r.Height - 1);
                break;
            case StatusStrip:
                e.Graphics.DrawLine(pen, 0, 0, r.Width, 0);
                break;
            case MenuStrip:
                break;
            default:
                e.Graphics.DrawLine(pen, 0, r.Height - 1, r.Width, r.Height - 1);
                break;
        }
    }

    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (e.Item is not ToolStripButton button) return;
        var r = new RectangleF(1, 1, e.Item.Width - 2, e.Item.Height - 2);
        Color? fill = null;
        if (button.Checked) fill = button.Selected ? Theme.AccentHover : Theme.Accent;
        else if (button.Pressed) fill = Theme.Raised;
        else if (button.Selected && button.Enabled) fill = Theme.RaisedHover;
        if (fill is null) return;

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = Theme.RoundRect(r, 6);
        using var brush = new SolidBrush(fill.Value);
        e.Graphics.FillPath(brush, path);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        var item = e.Item;
        if (!item.Selected && !item.Pressed) return;
        if (!item.Enabled && item.IsOnDropDown) return;

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var r = item.IsOnDropDown
            ? new RectangleF(4, 1, item.Width - 8, item.Height - 2)
            : new RectangleF(1, 2, item.Width - 2, item.Height - 4);
        using var path = Theme.RoundRect(r, 5);
        using var brush = new SolidBrush(item.Pressed && !item.IsOnDropDown ? Theme.Card : Theme.Raised);
        e.Graphics.FillPath(brush, path);
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        if (!e.Item.Enabled) e.TextColor = Theme.TextFaint;
        else if (e.Item is ToolStripButton { Checked: true }) e.TextColor = Color.White;
        else if (e.Item is ToolStripStatusLabel label && label.ForeColor != SystemColors.ControlText) e.TextColor = label.ForeColor;
        else e.TextColor = Theme.Text;
        base.OnRenderItemText(e);
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        using var pen = new Pen(Theme.Border);
        var r = e.Item.ContentRectangle;
        if (e.Vertical)
        {
            int x = r.Width / 2;
            e.Graphics.DrawLine(pen, x, 6, x, e.Item.Height - 6);
        }
        else
        {
            int y = e.Item.Height / 2;
            e.Graphics.DrawLine(pen, 8, y, e.Item.Width - 8, y);
        }
    }

    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
        // 드롭다운 왼쪽 이미지 영역도 배경과 같은 색
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = e.ImageRectangle;
        using var pen = new Pen(Theme.Accent, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
        g.DrawLines(pen, new[]
        {
            new PointF(r.X + r.Width * 0.2f, r.Y + r.Height * 0.52f),
            new PointF(r.X + r.Width * 0.42f, r.Y + r.Height * 0.74f),
            new PointF(r.X + r.Width * 0.8f, r.Y + r.Height * 0.3f),
        });
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        e.ArrowColor = e.Item?.Enabled == false ? Theme.TextFaint : Theme.TextDim;
        base.OnRenderArrow(e);
    }

    private sealed class DarkColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => Theme.Raised;
        public override Color MenuItemBorder => Color.Transparent;
        public override Color MenuBorder => Theme.Border;
        public override Color MenuItemSelectedGradientBegin => Theme.Raised;
        public override Color MenuItemSelectedGradientEnd => Theme.Raised;
        public override Color MenuItemPressedGradientBegin => Theme.Card;
        public override Color MenuItemPressedGradientEnd => Theme.Card;
        public override Color ToolStripDropDownBackground => Theme.Card;
        public override Color ImageMarginGradientBegin => Theme.Card;
        public override Color ImageMarginGradientMiddle => Theme.Card;
        public override Color ImageMarginGradientEnd => Theme.Card;
        public override Color SeparatorDark => Theme.Border;
        public override Color SeparatorLight => Theme.Border;
        public override Color StatusStripGradientBegin => Theme.Window;
        public override Color StatusStripGradientEnd => Theme.Window;
        public override Color ToolStripBorder => Theme.Border;
        public override Color ToolStripGradientBegin => Theme.Surface;
        public override Color ToolStripGradientMiddle => Theme.Surface;
        public override Color ToolStripGradientEnd => Theme.Surface;
        public override Color MenuStripGradientBegin => Theme.Window;
        public override Color MenuStripGradientEnd => Theme.Window;
        public override Color CheckBackground => Color.Transparent;
        public override Color CheckSelectedBackground => Color.Transparent;
        public override Color CheckPressedBackground => Color.Transparent;
        public override Color ButtonSelectedHighlight => Theme.RaisedHover;
        public override Color ButtonSelectedBorder => Color.Transparent;
        public override Color ButtonCheckedHighlight => Theme.Accent;
        public override Color ButtonPressedBorder => Color.Transparent;
    }
}
