namespace NukkiStudio.App.Segmentation;

/// <summary>
/// models 폴더 안의 모델 하나. config.yaml (AnyLabeling 형식)에서 읽는다.
/// CanvasWidth / CanvasHeight: 인코더가 내부적으로 고정해서 쓰는 입력 크기 (config의 max_width / max_height).
/// 이미지는 이 크기 안에 비율을 유지해 넣고 나머지는 0으로 채워야 좌표가 맞는다.
/// </summary>
public sealed record ModelInfo(string Id, string DisplayName, string Type, string EncoderPath, string DecoderPath,
                               int CanvasWidth, int CanvasHeight)
{
    /// <summary>현재 지원하는 형식: segment_anything (MobileSAM / SAM v1 ONNX).</summary>
    public bool IsSupported => Type == "segment_anything";
}

/// <summary>
/// models\&lt;모델 폴더&gt;\config.yaml 을 찾아 사용 가능한 모델 목록을 만든다.
/// config.yaml 예:
///   type: segment_anything
///   display_name: Segment Anything (MobileSAM)
///   encoder_model_path: mobile_sam.encoder.onnx
///   decoder_model_path: sam_vit_h_4b8939.decoder.onnx
/// </summary>
public static class ModelCatalog
{
    public static IReadOnlyList<ModelInfo> Scan(string modelsDirectory)
    {
        var list = new List<ModelInfo>();
        if (!Directory.Exists(modelsDirectory)) return list;

        foreach (var config in Directory.EnumerateFiles(modelsDirectory, "config.yaml", SearchOption.AllDirectories))
        {
            var dir = Path.GetDirectoryName(config)!;
            var map = ParseSimpleYaml(config);
            if (!map.TryGetValue("type", out var type)) continue;
            if (!map.TryGetValue("encoder_model_path", out var enc) || !map.TryGetValue("decoder_model_path", out var dec)) continue;

            var encPath = Path.Combine(dir, enc);
            var decPath = Path.Combine(dir, dec);
            if (!File.Exists(encPath) || !File.Exists(decPath)) continue;

            var id = Path.GetFileName(dir);
            var name = map.TryGetValue("display_name", out var dn) ? dn : id;
            int inputSize = ReadInt(map, "input_size", 1024);
            int canvasW = Math.Clamp(ReadInt(map, "max_width", inputSize), 1, inputSize);
            int canvasH = Math.Clamp(ReadInt(map, "max_height", inputSize), 1, inputSize);
            list.Add(new ModelInfo(id, name, type, encPath, decPath, canvasW, canvasH));
        }

        // 기본 모델(MobileSAM)을 맨 앞으로
        return list.OrderByDescending(m => m.IsSupported)
                   .ThenByDescending(m => m.Id.Contains("mobile", StringComparison.OrdinalIgnoreCase))
                   .ThenBy(m => m.DisplayName)
                   .ToList();
    }

    public static ISegmentationModel Load(ModelInfo info, DevicePreference preference)
    {
        if (!info.IsSupported) throw new NotSupportedException($"지원하지 않는 모델 형식입니다: {info.Type}");
        return new SamOnnxModel(info.DisplayName, info.EncoderPath, info.DecoderPath, info.CanvasWidth, info.CanvasHeight, preference);
    }

    private static int ReadInt(Dictionary<string, string> map, string key, int fallback) =>
        map.TryGetValue(key, out var s) && int.TryParse(s, out int v) && v > 0 ? v : fallback;

    /// <summary>"키: 값" 한 줄 형식만 읽는 최소 YAML 파서.</summary>
    private static Dictionary<string, string> ParseSimpleYaml(string path)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            int colon = line.IndexOf(':');
            if (colon <= 0) continue;
            var key = line[..colon].Trim();
            var value = line[(colon + 1)..].Trim().Trim('"', '\'');
            if (value.Length > 0) map[key] = value;
        }
        return map;
    }
}
