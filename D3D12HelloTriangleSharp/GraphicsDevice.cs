using System;
using SharpDX;
using D3D = SharpDX.Direct3D;
using D3D12 = SharpDX.Direct3D12;
using DXGI = SharpDX.DXGI;

namespace D3D12HelloTriangleSharp
{
    public sealed class GraphicsDevice : IDisposable
    {
        public GraphicsDevice(bool useWarpDevice)
        {
            bool debug = false;
#if DEBUG
            using var debugController = D3D12.DebugInterface.Get();
            if (debugController != null)
            {
                debugController.EnableDebugLayer();
                debug = true;
            }
#endif
            using var factory = new DXGI.Factory2(debug);
            Factory = factory.QueryInterface<DXGI.Factory4>();
            if (useWarpDevice)
            {
                using var warpAdapter = Factory.GetWarpAdapter();
                Device = new D3D12.Device(warpAdapter, D3D.FeatureLevel.Level_11_0);
            }
            else
            {
                using var hardwareAdapter = GetHardwareAdapter(Factory);
                Device = new D3D12.Device(hardwareAdapter, D3D.FeatureLevel.Level_11_0);
            }
            
            var queueDesc = new D3D12.CommandQueueDescription
            {
                Type = D3D12.CommandListType.Direct,
                Priority = 0,
                Flags = D3D12.CommandQueueFlags.None,
                NodeMask = 0
            };
            CommandQueue = Device.CreateCommandQueue(queueDesc);
            
            CommandAllocator = Device.CreateCommandAllocator(D3D12.CommandListType.Direct);
        }
        
        
        public void Dispose()
        {
            CommandQueue.Dispose();
            CommandAllocator.Dispose();
            Device.Dispose();
            Factory.Dispose();
        }
        
        public DXGI.Factory4 Factory { get; }
        public D3D12.Device Device { get; }
        public D3D12.CommandAllocator CommandAllocator { get; }
        public D3D12.CommandQueue CommandQueue { get; }
        /*
        */
    
        private static DXGI.Adapter1? GetHardwareAdapter(DXGI.Factory1 factory)
        {
            var count = factory.GetAdapterCount1();
            for (int i = 0; i < count; i++)
            {
                using var adapter = factory.GetAdapter1(i);
                var desc = adapter.Description1;
                if ((desc.Flags & DXGI.AdapterFlags.Software) != 0)
                {
                    continue;
                }

                using var device = new D3D12.Device(adapter, D3D.FeatureLevel.Level_11_0);
                return adapter.QueryInterface<DXGI.Adapter1>();
            }

            return null;
        }
        
    }
}