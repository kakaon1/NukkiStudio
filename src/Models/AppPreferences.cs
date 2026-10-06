using System.Text.Json;

namespace NukkiStudio.App.Models;

/// <summary>
/// 내보내기 외의 사용자 선택 (지우기 모델 등). %AppData%\NukkiStudio\preferences.json 에 저장한다.
/// </summary>
public sealed class AppPreferences
{
    /// <summary>객체 지우기 모델 Id (ModelPackages.MiGanId / LamaId).</summary>
    public string InpaintModel { get; set; } = ModelPackages.MiGanId;

    /// <summary>브러시 크기 (px, 지름).</summary>
    public int BrushSize { get; set; } = 30;

    /// <summary>true면 네모 브러시.</summary>
    public bool BrushSquare { get; set; }

    /// <summary>시작할 때 받지 않고 넘긴 선택 모델 Id. 같은 모델로는 다시 묻지 않는다 (필수 모델은 항상 묻는다).</summary>
    public List<string> SkippedModels { get; set; } = new();

    private static string FilePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NukkiStudio", "preferences.json");

    public static AppPreferences Load()
    {
        try
        {
            if (File.Exists(FilePath) && JsonSerializer.Deserialize<AppPreferences>(File.ReadAllText(FilePath)) is { } loaded)
            {
                if (loaded.InpaintModel is not (ModelPackages.MiGanId or ModelPackages.LamaId)) loaded.InpaintModel = ModelPackages.MiGanId;
                return loaded;
            }
        }
        catch (Exception)
        {
            // 손상된 파일은 무시하고 기본값 사용
        }
        return new AppPreferences();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception)
        {
            // 저장 실패는 치명적이지 않음
        }
    }
}
