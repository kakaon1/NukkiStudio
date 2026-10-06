using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using NukkiStudio.App.Imaging;

namespace NukkiStudio.App.Export;

/// <summary>
/// 내보내기 설정(형식 / 품질 / 크기 / 위치)에 맞춰 이미지를 파일로 저장한다. 여러 스레드에서 동시에 써도 된다.
/// </summary>
public static class ImageExporter
{
    /// <summary>
    /// 저장 폴더를 정하고 만든다.
    /// 원본 위치 옵션: 원본 파일 폴더\output / 지정 폴더 옵션: 그 폴더 / 원본이 없는 이미지(붙여넣기): 사진 폴더\Nukki Studio\output
    /// </summary>
    public static string ResolveFolder(ExportSettings settings, string? sourcePath)
    {
        string folder;
        if (!settings.UseSourceFolder && !string.IsNullOrWhiteSpace(settings.OutputFolder))
        {
            folder = settings.OutputFolder;
        }
        else if (sourcePath is not null && Path.GetDirectoryName(sourcePath) is { } dir)
        {
            folder = Path.Combine(dir, ExportSettings.OutputFolderName);
        }
        else
        {
            folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Nukki Studio", ExportSettings.OutputFolderName);
        }
        Directory.CreateDirectory(folder);
        return folder;
    }

    /// <summary>
    /// bitmap(32비트 ARGB)을 설정대로 저장하고 실제 파일 경로를 돌려준다.
    /// JPG / BMP는 투명을 지원하지 않으므로 투명 부분을 opaqueBackground(기본 흰색)로 채운다.
    /// </summary>
    public static string Save(Bitmap bitmap, string folder, string baseName, ExportSettings settings, Color? opaqueBackground = null)
    {
        using var resized = Resize(bitmap, settings);
        var source = resized ?? bitmap;
        var path = UniquePath(folder, SafeFileName(baseName) + settings.Extension);

        switch (settings.Format)
        {
            case ExportFormat.Jpeg:
            {
                using var flat = Flatten(source, opaqueBackground ?? Color.White);
                var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                using var parameters = new EncoderParameters(1);
                parameters.Param[0] = new EncoderParameter(Encoder.Quality, (long)Math.Clamp(settings.JpegQuality, 10, 100));
                flat.Save(path, encoder, parameters);
                break;
            }
            case ExportFormat.Bmp:
            {
                using var flat = Flatten(source, opaqueBackground ?? Color.White);
                flat.Save(path, ImageFormat.Bmp);
                break;
            }
            default:
                source.Save(path, ImageFormat.Png);
                break;
        }
        return path;
    }

    /// <summary>크기 옵션에 따라 줄이거나 키운 새 비트맵. 원본 크기 그대로면 null. 키울 때는 고품질 보간 + 선명화.</summary>
    private static Bitmap? Resize(Bitmap bitmap, ExportSettings settings)
    {
        double scale = settings.Resize switch
        {
            ExportResize.Percent => settings.Percent / 100.0,
            ExportResize.MaxSide => Math.Min(1.0, (double)settings.MaxSide / Math.Max(bitmap.Width, bitmap.Height)),
            _ => 1.0,
        };
        if (Math.Abs(scale - 1.0) < 0.001) return null;

        int w = Math.Max(1, (int)Math.Round(bitmap.Width * scale));
        int h = Math.Max(1, (int)Math.Round(bitmap.Height * scale));
        return ImageResampler.ResizeForExport(bitmap, w, h);
    }

    /// <summary>투명 부분을 배경색으로 채운 24비트 이미지.</summary>
    private static Bitmap Flatten(Bitmap source, Color background)
    {
        var dst = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
        dst.SetResolution(source.HorizontalResolution, source.VerticalResolution);
        using var g = Graphics.FromImage(dst);
        g.Clear(background);
        g.DrawImage(source, 0, 0, source.Width, source.Height);
        return dst;
    }

    public static string SafeFileName(string name) =>
        string.Concat(name.Select(c => Path.GetInvalidFileNameChars().Contains(c) || c == ' ' ? '_' : c));

    /// <summary>같은 이름의 파일이 있으면 (2), (3)... 을 붙인다.</summary>
    public static string UniquePath(string folder, string fileName)
    {
        var path = Path.Combine(folder, fileName);
        var name = Path.GetFileNameWithoutExtension(fileName);
        var ext = Path.GetExtension(fileName);
        for (int i = 2; File.Exists(path); i++) path = Path.Combine(folder, $"{name} ({i}){ext}");
        return path;
    }
}
