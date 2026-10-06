namespace NukkiStudio.App.Editing;

/// <summary>
/// 이미지 목록의 항목 하나 (여러 이미지 작업용).
/// Document는 처음 열 때 만들어지고, 작업(확정 객체 / 지우기)이 있는 동안 유지되어 다른 이미지로 갔다 와도 작업이 남는다.
/// </summary>
public sealed class ImageItem
{
    public ImageItem(string? path, string displayName)
    {
        Path = path;
        DisplayName = displayName;
    }

    /// <summary>원본 파일 경로. 붙여넣은 이미지는 null.</summary>
    public string? Path { get; }

    public string DisplayName { get; }

    public EditorDocument? Document { get; set; }

    /// <summary>목록에 표시할 작은 미리보기 (백그라운드에서 생성).</summary>
    public Bitmap? Thumbnail { get; set; }

    /// <summary>열기 실패 시 오류 메시지.</summary>
    public string? Error { get; set; }

    /// <summary>저장할 작업이 있는지 (확정된 객체 또는 이미지 편집).</summary>
    public bool HasWork => Document is { } d && (d.Objects.Count > 0 || d.IsImageEdited);

    /// <summary>저장 파일 이름의 기준 (확장자 제외).</summary>
    public string BaseName => Path is null ? DisplayName.Replace(' ', '_') : System.IO.Path.GetFileNameWithoutExtension(Path);

    public override string ToString() => DisplayName;
}
