using System.Text.Json;
using System.Text.Json.Serialization;

namespace NukkiStudio.App.Export;

public enum ExportFormat
{
    Png,
    Jpeg,
    Bmp,
}

public enum ExportResize
{
    /// <summary>원본 크기 그대로.</summary>
    Original,

    /// <summary>원본의 Percent% 크기 (5~400%, 100% 초과는 고품질 확대 + 선명화).</summary>
    Percent,

    /// <summary>긴 변이 MaxSide 픽셀을 넘지 않게 줄이기 (작은 이미지는 그대로).</summary>
    MaxSide,
}

/// <summary>
/// 내보내기 설정. %AppData%\NukkiStudio\settings.json 에 저장해 다음 실행 때도 유지한다.
/// </summary>
public sealed class ExportSettings
{
    public ExportFormat Format { get; set; } = ExportFormat.Png;

    /// <summary>JPG 품질 (10~100). 높을수록 화질이 좋고 용량이 크다.</summary>
    public int JpegQuality { get; set; } = 92;

    public ExportResize Resize { get; set; } = ExportResize.Original;

    public int Percent { get; set; } = 50;

    public int MaxSide { get; set; } = 2048;

    /// <summary>true: 원본 파일이 있는 폴더 안의 output 폴더에 저장. false: OutputFolder에 저장.</summary>
    public bool UseSourceFolder { get; set; } = true;

    public string OutputFolder { get; set; } = "";

    public const string OutputFolderName = "output";

    [JsonIgnore]
    public string Extension => Format switch
    {
        ExportFormat.Jpeg => ".jpg",
        ExportFormat.Bmp => ".bmp",
        _ => ".png",
    };

    /// <summary>화면 표시용 요약.</summary>
    [JsonIgnore]
    public string Summary
    {
        get
        {
            string format = Format switch
            {
                ExportFormat.Jpeg => $"JPG 품질 {JpegQuality}",
                ExportFormat.Bmp => "BMP",
                _ => "PNG (투명 지원)",
            };
            string size = Resize switch
            {
                ExportResize.Percent => Percent > 100 ? $"{Percent}% 확대" : $"{Percent}% 크기",
                ExportResize.MaxSide => $"긴 변 최대 {MaxSide}px",
                _ => "원본 크기",
            };
            string where = UseSourceFolder || string.IsNullOrWhiteSpace(OutputFolder) ? "원본 위치\\output" : OutputFolder;
            return $"{format} · {size} → {where}";
        }
    }

    public ExportSettings Clone() => (ExportSettings)MemberwiseClone();

    // ---------------- 저장 / 불러오기 ----------------

    private static string SettingsPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NukkiStudio", "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public static ExportSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var loaded = JsonSerializer.Deserialize<ExportSettings>(File.ReadAllText(SettingsPath), JsonOptions);
                if (loaded is not null)
                {
                    loaded.JpegQuality = Math.Clamp(loaded.JpegQuality, 10, 100);
                    loaded.Percent = Math.Clamp(loaded.Percent, 5, 400);
                    loaded.MaxSide = Math.Clamp(loaded.MaxSide, 64, 20000);
                    return loaded;
                }
            }
        }
        catch (Exception)
        {
            // 손상된 설정 파일은 무시하고 기본값 사용
        }
        return new ExportSettings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, JsonOptions));
        }
        catch (Exception)
        {
            // 설정 저장 실패는 치명적이지 않음
        }
    }
}
