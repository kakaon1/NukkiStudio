using System.Drawing.Drawing2D;

namespace NukkiStudio.App.UI;

public enum AppIcon
{
    Open,
    Save,
    Point,
    Box,
    BrushAdd,
    BrushErase,
    Undo,
    Redo,
    Fit,
    Actual,
    Eye,
    EyeOff,
    Image,
    Eraser,
    RemoveBackground,
    Info,
    AutoDetect,
    Brush,
    ShapeCircle,
    ShapeSquare,
}

/// <summary>
/// 도구 모음 아이콘을 코드로 그린다 (외부 이미지 / 폰트 의존 없음). 선 굵기 기반의 단순한 선형 아이콘.
/// 좌표는 24×24 기준으로 작성하고 size에 맞게 확대한다.
/// </summary>
internal static class IconFactory
{
    public static Bitmap Create(AppIcon icon, int size, Color color)
    {
        var bmp = new Bitmap(size, size);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.ScaleTransform(size / 24f, size / 24f);
        Draw(g, icon, color);
        return bmp;
    }

    public static void Draw(Graphics g, AppIcon icon, Color color)
    {
        using var pen = new Pen(color, 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        using var brush = new SolidBrush(color);

        switch (icon)
        {
            case AppIcon.Open:
                using (var p = new GraphicsPath())
                {
                    p.AddLines(new[] { new PointF(3, 18.5f), new PointF(3, 6), new PointF(9, 6), new PointF(11, 8.5f), new PointF(19, 8.5f), new PointF(19, 11) });
                    g.DrawPath(pen, p);
                }
                g.DrawPolygon(pen, new[] { new PointF(3, 18.5f), new PointF(6.5f, 11.5f), new PointF(21.5f, 11.5f), new PointF(18, 18.5f) });
                break;

            case AppIcon.Save:
                g.DrawLine(pen, 12, 3.5f, 12, 14);
                g.DrawLines(pen, new[] { new PointF(7.5f, 9.5f), new PointF(12, 14), new PointF(16.5f, 9.5f) });
                g.DrawLines(pen, new[] { new PointF(4, 15), new PointF(4, 19.5f), new PointF(20, 19.5f), new PointF(20, 15) });
                break;

            case AppIcon.Point:
                g.DrawEllipse(pen, 4, 4, 16, 16);
                g.DrawLine(pen, 12, 8.5f, 12, 15.5f);
                g.DrawLine(pen, 8.5f, 12, 15.5f, 12);
                break;

            case AppIcon.Box:
                using (var dash = new Pen(color, 1.8f) { DashPattern = new[] { 1.6f, 1.4f }, LineJoin = LineJoin.Round })
                {
                    g.DrawRectangle(dash, 4, 5, 16, 14);
                }
                g.FillEllipse(brush, 2.4f, 3.4f, 3.2f, 3.2f);
                g.FillEllipse(brush, 18.4f, 17.4f, 3.2f, 3.2f);
                break;

            case AppIcon.Brush:
            case AppIcon.BrushAdd:
            case AppIcon.BrushErase:
                // 붓: 손잡이 + 붓끝
                g.DrawLine(pen, 19.5f, 3.5f, 11.5f, 11.5f);
                using (var tip = new GraphicsPath())
                {
                    tip.AddBezier(11.5f, 11.5f, 7.5f, 10.5f, 5, 14, 5.5f, 17.5f);
                    tip.AddLine(5.5f, 17.5f, 3.5f, 20.5f);
                    tip.AddBezier(3.5f, 20.5f, 9, 20.5f, 13.5f, 17.5f, 12.5f, 12.5f);
                    tip.CloseFigure();
                    g.FillPath(brush, tip);
                }
                // 오른쪽 아래 + / − (일반 브러시는 표시 없음)
                if (icon != AppIcon.Brush) g.DrawLine(pen, 15.5f, 18.5f, 21.5f, 18.5f);
                if (icon == AppIcon.BrushAdd) g.DrawLine(pen, 18.5f, 15.5f, 18.5f, 21.5f);
                break;

            case AppIcon.Undo:
            case AppIcon.Redo:
                var state = g.Save();
                if (icon == AppIcon.Redo)
                {
                    g.TranslateTransform(24, 0);
                    g.ScaleTransform(-1, 1);
                }
                g.DrawArc(pen, 5, 7, 15, 12, 270, 180);
                g.DrawLine(pen, 12.5f, 19, 8, 19);
                g.DrawLine(pen, 12.5f, 7, 5, 7);
                g.DrawLines(pen, new[] { new PointF(8.5f, 3.5f), new PointF(5, 7), new PointF(8.5f, 10.5f) });
                g.Restore(state);
                break;

            case AppIcon.Fit:
                g.DrawLines(pen, new[] { new PointF(4, 9), new PointF(4, 4), new PointF(9, 4) });
                g.DrawLines(pen, new[] { new PointF(15, 4), new PointF(20, 4), new PointF(20, 9) });
                g.DrawLines(pen, new[] { new PointF(20, 15), new PointF(20, 20), new PointF(15, 20) });
                g.DrawLines(pen, new[] { new PointF(9, 20), new PointF(4, 20), new PointF(4, 15) });
                g.DrawRectangle(pen, 8.5f, 8.5f, 7, 7);
                break;

            case AppIcon.Actual:
                g.DrawRectangle(pen, 3, 4, 18, 16);
                g.DrawLines(pen, new[] { new PointF(7, 9.5f), new PointF(8.5f, 8.5f), new PointF(8.5f, 15.5f) });
                g.DrawLines(pen, new[] { new PointF(15, 9.5f), new PointF(16.5f, 8.5f), new PointF(16.5f, 15.5f) });
                g.FillEllipse(brush, 11.2f, 10, 1.7f, 1.7f);
                g.FillEllipse(brush, 11.2f, 13.5f, 1.7f, 1.7f);
                break;

            case AppIcon.Eye:
            case AppIcon.EyeOff:
                using (var eye = new GraphicsPath())
                {
                    eye.AddBezier(2.5f, 12, 6, 5.5f, 18, 5.5f, 21.5f, 12);
                    eye.AddBezier(21.5f, 12, 18, 18.5f, 6, 18.5f, 2.5f, 12);
                    eye.CloseFigure();
                    g.DrawPath(pen, eye);
                }
                g.DrawEllipse(pen, 9, 9, 6, 6);
                if (icon == AppIcon.EyeOff) g.DrawLine(pen, 4, 20, 20, 4);
                break;

            case AppIcon.Image:
                g.DrawRectangle(pen, 3, 4.5f, 18, 15);
                g.DrawLines(pen, new[] { new PointF(3.5f, 17), new PointF(9, 11.5f), new PointF(13, 15.5f), new PointF(15.5f, 13), new PointF(20.5f, 18) });
                g.DrawEllipse(pen, 14.5f, 7.5f, 3, 3);
                break;

            case AppIcon.Eraser:
                // 기울어진 지우개 + 바닥선
                g.DrawPolygon(pen, new[] { new PointF(14, 4), new PointF(21, 11), new PointF(12, 20), new PointF(7.5f, 20), new PointF(3.5f, 16), new PointF(14, 4) });
                g.DrawLine(pen, 9, 9.5f, 15.5f, 16);
                g.DrawLine(pen, 12, 20, 20.5f, 20);
                break;

            case AppIcon.RemoveBackground:
                // 체커 무늬 배경 위의 인물 실루엣
                using (var faint = new SolidBrush(Color.FromArgb(110, color)))
                {
                    for (int cy = 0; cy < 4; cy++)
                    for (int cx = 0; cx < 4; cx++)
                        if ((cx + cy) % 2 == 0) g.FillRectangle(faint, 3 + cx * 4.5f, 3 + cy * 4.5f, 4.5f, 4.5f);
                }
                g.DrawRectangle(pen, 3, 3, 18, 18);
                g.FillEllipse(brush, 9.5f, 7, 5, 5);
                using (var body = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    body.AddArc(6.5f, 13, 11, 10, 180, 180);
                    body.AddLine(17.5f, 18, 17.5f, 21);
                    body.AddLine(17.5f, 21, 6.5f, 21);
                    body.CloseFigure();
                    g.FillPath(brush, body);
                }
                break;

            case AppIcon.AutoDetect:
                // 점선 상자 두 개 + 반짝임 (자동으로 찾기)
                using (var dash = new Pen(color, 1.6f) { DashPattern = new[] { 1.5f, 1.3f } })
                {
                    g.DrawRectangle(dash, 2.5f, 8, 9, 9);
                    g.DrawRectangle(dash, 9.5f, 12.5f, 10, 8);
                }
                using (var star = new GraphicsPath())
                {
                    star.AddPolygon(new[]
                    {
                        new PointF(17, 1.5f), new PointF(18.4f, 5.6f), new PointF(22.5f, 7), new PointF(18.4f, 8.4f),
                        new PointF(17, 12.5f), new PointF(15.6f, 8.4f), new PointF(11.5f, 7), new PointF(15.6f, 5.6f),
                    });
                    g.FillPath(brush, star);
                }
                break;

            case AppIcon.ShapeCircle:
                g.FillEllipse(brush, 5, 5, 14, 14);
                break;

            case AppIcon.ShapeSquare:
                g.FillRectangle(brush, 5.5f, 5.5f, 13, 13);
                break;

            case AppIcon.Info:
                g.DrawEllipse(pen, 3, 3, 18, 18);
                g.DrawLine(pen, 12, 11, 12, 16.5f);
                g.FillEllipse(brush, 10.9f, 6.8f, 2.2f, 2.2f);
                break;
        }
    }
}
