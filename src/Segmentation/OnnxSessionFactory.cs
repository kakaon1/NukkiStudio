using Microsoft.ML.OnnxRuntime;

namespace NukkiStudio.App.Segmentation;

public enum DevicePreference
{
    /// <summary>GPU(DirectML)를 먼저 시도하고 실패하면 CPU.</summary>
    Auto,
    CpuOnly,
}

/// <summary>ONNX Runtime 세션 생성. DirectML → CPU 자동 전환.</summary>
public static class OnnxSessionFactory
{
    public static InferenceSession Create(string modelPath, DevicePreference preference, out string deviceDescription)
    {
        if (preference == DevicePreference.Auto)
        {
            var gpu = GpuAdapters.PickBest();
            if (gpu is not null)
            {
                SessionOptions? options = null;
                try
                {
                    options = new SessionOptions
                    {
                        // DirectML 권장 설정
                        EnableMemoryPattern = false,
                        ExecutionMode = ExecutionMode.ORT_SEQUENTIAL,
                        GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL,
                    };
                    options.AppendExecutionProvider_DML(gpu.Index);
                    var session = new InferenceSession(modelPath, options);
                    deviceDescription = $"GPU: {gpu.Name}";
                    return session;
                }
                catch (Exception)
                {
                    // GPU 세션 실패 → CPU로 진행
                }
                finally
                {
                    options?.Dispose();
                }
            }
        }

        return CreateCpu(modelPath, out deviceDescription);
    }

    public static InferenceSession CreateCpu(string modelPath, out string deviceDescription)
    {
        using var options = new SessionOptions
        {
            GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL,
            IntraOpNumThreads = Math.Max(1, Environment.ProcessorCount / 2),
        };
        deviceDescription = "CPU";
        return new InferenceSession(modelPath, options);
    }
}
