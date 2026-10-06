using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace NukkiStudio.App.UI;

/// <summary>
/// 다크 테마 색상표. Designer.cs에는 같은 값이 Color.FromArgb 리터럴로 들어 있다 (디자이너는 이 클래스를 참조하지 않음).
/// </summary>
internal static class Theme
{
    public static readonly Color Window = Color.FromArgb(24, 24, 27);
    public static readonly Color Surface = Color.FromArgb(30, 30, 34);
    public static readonly Color Card = Color.FromArgb(38, 38, 43);
    public static readonly Color Raised = Color.FromArgb(50, 50, 57);
    public static readonly Color RaisedHover = Color.FromArgb(62, 62, 70);
    public static readonly Color Border = Color.FromArgb(52, 52, 59);
    public static readonly Color Canvas = Color.FromArgb(17, 17, 19);

    public static readonly Color Text = Color.FromArgb(236, 236, 239);
    public static readonly Color TextDim = Color.FromArgb(160, 160, 170);
    public static readonly Color TextFaint = Color.FromArgb(105, 105, 115);

    public static readonly Color Accent = Color.FromArgb(99, 115, 255);
    public static readonly Color AccentHover = Color.FromArgb(122, 136, 255);
    public static readonly Color AccentPressed = Color.FromArgb(82, 96, 228);
    public static readonly Color Success = Color.FromArgb(61, 214, 140);
    public static readonly Color Warning = Color.FromArgb(255, 196, 77);
    public static readonly Color Danger = Color.FromArgb(255, 99, 99);

    public static GraphicsPath RoundRect(RectangleF r, float radius)
    {
        var p = new GraphicsPath();
        float d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
        if (d <= 0.5f)
        {
            p.AddRectangle(r);
            return p;
        }
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    /// <summary>Windows 10/11 제목 표시줄을 어두운 색으로 (Windows 11은 캡션 색까지 지정).</summary>
    public static void ApplyDarkTitleBar(IntPtr handle)
    {
        try
        {
            int on = 1;
            DwmSetWindowAttribute(handle, 20, ref on, sizeof(int)); // DWMWA_USE_IMMERSIVE_DARK_MODE
            int caption = ColorTranslator.ToWin32(Window);
            DwmSetWindowAttribute(handle, 35, ref caption, sizeof(int)); // DWMWA_CAPTION_COLOR (Windows 11)
            int border = ColorTranslator.ToWin32(Border);
            DwmSetWindowAttribute(handle, 34, ref border, sizeof(int)); // DWMWA_BORDER_COLOR (Windows 11)
        }
        catch (Exception)
        {
            // 지원하지 않는 Windows 버전: 기본 제목 표시줄 유지
        }
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
}
