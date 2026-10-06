using System.Drawing.Drawing2D;
using NukkiStudio.App.UI;

namespace NukkiStudio.App.Controls;

/// <summary>토글 스위치 모양 체크박스. CheckBox를 상속하므로 Checked / CheckedChanged를 그대로 쓴다.</summary>
public sealed class ToggleSwitch : CheckBox
{
    private bool _hover;

    public ToggleSwitch()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        Cursor = Cursors.Hand;
        AutoSize = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Parent?.BackColor ?? BackColor);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        const float w = 34, h = 18;
        float y = (Height - h) / 2f;
        var track = new RectangleF(1, y, w, h);
        Color trackColor = !Enabled ? Theme.Raised
            : Checked ? (_hover ? Theme.AccentHover : Theme.Accent)
            : (_hover ? Theme.RaisedHover : Theme.Raised);
        using (var path = Theme.RoundRect(track, h / 2))
        using (var brush = new SolidBrush(trackColor))
            g.FillPath(brush, path);

        float knob = h - 6;
        float kx = Checked ? track.Right - knob - 3 : track.X + 3;
        using (var brush = new SolidBrush(Enabled ? Color.White : Theme.TextFaint))
            g.FillEllipse(brush, kx, y + 3, knob, knob);

        var textRect = new Rectangle((int)(w + 10), 0, Width - (int)(w + 10), Height);
        TextRenderer.DrawText(g, Text, Font, textRect, Enabled ? ForeColor : Theme.TextFaint,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

        if (Focused && ShowFocusCues)
        {
            using var pen = new Pen(Theme.AccentHover, 1f) { DashStyle = DashStyle.Dot };
            g.DrawRectangle(pen, textRect.X - 2, 1, Math.Min(textRect.Width, TextRenderer.MeasureText(Text, Font).Width + 4), Height - 3);
        }
    }

    protected override void OnMouseEnter(EventArgs eventargs)
    {
        base.OnMouseEnter(eventargs);
        _hover = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs eventargs)
    {
        base.OnMouseLeave(eventargs);
        _hover = false;
        Invalidate();
    }
}
