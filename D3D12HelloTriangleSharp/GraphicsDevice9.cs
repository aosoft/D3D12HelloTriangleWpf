using System;
using D3D9 = SharpDX.Direct3D9;

namespace D3D12HelloTriangleSharp
{
    public sealed class GraphicsDevice9 : IDisposable
    {
        public GraphicsDevice9(int width, int height)
        {
            D3DEx = new D3D9.Direct3DEx();
            Device = new D3D9.DeviceEx(D3DEx, 0, D3D9.DeviceType.Hardware, IntPtr.Zero,
                D3D9.CreateFlags.Multithreaded | D3D9.CreateFlags.HardwareVertexProcessing,
                new D3D9.PresentParameters
                {
                    BackBufferWidth = 1,
                    BackBufferHeight = 1,
                    BackBufferFormat = D3D9.Format.A8R8G8B8,
                    BackBufferCount = 1,
                    MultiSampleType = D3D9.MultisampleType.None,
                    MultiSampleQuality = 0,
                    SwapEffect = D3D9.SwapEffect.Discard,
                    DeviceWindowHandle = IntPtr.Zero,
                    Windowed = true,
                    EnableAutoDepthStencil = false,
                    AutoDepthStencilFormat = D3D9.Format.D16,
                    PresentFlags = D3D9.PresentFlags.None,
                    FullScreenRefreshRateInHz = 0,
                    PresentationInterval = D3D9.PresentInterval.Default
                });

            IntPtr sharedHandle = IntPtr.Zero;
            RenderTarget = D3D9.Surface.CreateRenderTarget(Device, width, height, D3D9.Format.A8R8G8B8,
                D3D9.MultisampleType.None, 0, false, ref sharedHandle);
            RenderTargetSharedHanlde = sharedHandle;
        }

        public void Dispose()
        {
            RenderTarget.Dispose();
            Device.Dispose();
            D3DEx.Dispose();
        }

        public D3D9.Direct3DEx D3DEx { get; }
        public D3D9.DeviceEx Device { get; }
        public D3D9.Surface RenderTarget { get; }
        private IntPtr RenderTargetSharedHanlde { get; }
    }
}