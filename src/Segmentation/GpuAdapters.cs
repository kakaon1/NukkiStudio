using System.Runtime.InteropServices;

namespace NukkiStudio.App.Segmentation;

/// <summary>DXGI 그래픽 어댑터 정보. Index는 DirectML device_id와 같다.</summary>
public sealed record GpuAdapter(int Index, string Name, long DedicatedVideoMemory);

/// <summary>
/// DXGI로 GPU 어댑터를 나열한다. 노트북에서는 0번이 내장 그래픽인 경우가 많으므로
/// 전용 메모리가 가장 큰 하드웨어 어댑터를 DirectML 장치로 고른다.
/// </summary>
public static class GpuAdapters
{
    public static IReadOnlyList<GpuAdapter> Enumerate()
    {
        var result = new List<GpuAdapter>();
        try
        {
            var iid = typeof(IDXGIFactory1).GUID;
            if (CreateDXGIFactory1(ref iid, out var obj) != 0) return result;
            var factory = (IDXGIFactory1)obj;
            try
            {
                for (uint i = 0; factory.EnumAdapters1(i, out var adapter) == 0; i++)
                {
                    try
                    {
                        adapter.GetDesc1(out var desc);
                        const uint DXGI_ADAPTER_FLAG_SOFTWARE = 2;
                        if ((desc.Flags & DXGI_ADAPTER_FLAG_SOFTWARE) != 0) continue;
                        result.Add(new GpuAdapter((int)i, desc.Description.TrimEnd('\0'), (long)desc.DedicatedVideoMemory));
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(adapter);
                    }
                }
            }
            finally
            {
                Marshal.ReleaseComObject(factory);
            }
        }
        catch (Exception)
        {
            // DXGI를 사용할 수 없는 환경: 빈 목록 반환 → CPU 사용
        }
        return result;
    }

    public static GpuAdapter? PickBest()
    {
        return Enumerate().OrderByDescending(a => a.DedicatedVideoMemory).FirstOrDefault();
    }

    [DllImport("dxgi.dll")]
    private static extern int CreateDXGIFactory1(ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object factory);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DXGI_ADAPTER_DESC1
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Description;
        public uint VendorId;
        public uint DeviceId;
        public uint SubSysId;
        public uint Revision;
        public nuint DedicatedVideoMemory;
        public nuint DedicatedSystemMemory;
        public nuint SharedSystemMemory;
        public uint AdapterLuidLow;
        public int AdapterLuidHigh;
        public uint Flags;
    }

    // vtable 순서를 맞추기 위한 선언. 호출하지 않는 메서드는 자리만 차지한다.
    [ComImport, Guid("770aae78-f26f-4dba-a829-253c83d1b387"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IDXGIFactory1
    {
        void SetPrivateData();
        void SetPrivateDataInterface();
        void GetPrivateData();
        void GetParent();
        void EnumAdapters();
        void MakeWindowAssociation();
        void GetWindowAssociation();
        void CreateSwapChain();
        void CreateSoftwareAdapter();
        [PreserveSig] int EnumAdapters1(uint index, out IDXGIAdapter1 adapter);
        [PreserveSig] int IsCurrent();
    }

    [ComImport, Guid("29038f61-3839-4626-91fd-086879011a05"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IDXGIAdapter1
    {
        void SetPrivateData();
        void SetPrivateDataInterface();
        void GetPrivateData();
        void GetParent();
        void EnumOutputs();
        void GetDesc();
        void CheckInterfaceSupport();
        [PreserveSig] int GetDesc1(out DXGI_ADAPTER_DESC1 desc);
    }
}
