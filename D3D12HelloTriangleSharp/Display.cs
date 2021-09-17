using System;
using DXGI = SharpDX.DXGI;
using D3D12 = SharpDX.Direct3D12;

namespace D3D12HelloTriangleSharp
{
    public sealed class Display : IDisposable
    {
        private int _rtvDescriptorSize;
        
        public Display(GraphicsDevice device, IntPtr windowHandle, int width, int height, int frameCount)
        {
            var swapChainDesc = new DXGI.SwapChainDescription1
            {
                Width = width,
                Height = height,
                Format = DXGI.Format.R8G8B8A8_UNorm,
                Stereo = default,
                SampleDescription = new DXGI.SampleDescription
                {
                    Count = 1,
                    Quality = 0
                },
                Usage = DXGI.Usage.RenderTargetOutput,
                BufferCount = frameCount,
                Scaling = DXGI.Scaling.Stretch,
                SwapEffect = DXGI.SwapEffect.FlipDiscard,
                AlphaMode = DXGI.AlphaMode.Unspecified,
                Flags = DXGI.SwapChainFlags.None
            };
            using var swapChain = new DXGI.SwapChain1(device.Factory, device.CommandQueue, windowHandle, ref swapChainDesc);
            SwapChain = swapChain.QueryInterface<DXGI.SwapChain3>();
            device.Factory.MakeWindowAssociation(windowHandle, DXGI.WindowAssociationFlags.IgnoreAltEnter);
            
            var rtvHeapDesc = new D3D12.DescriptorHeapDescription
            {
                Type = D3D12.DescriptorHeapType.RenderTargetView,
                DescriptorCount = frameCount,
                Flags = D3D12.DescriptorHeapFlags.None,
                NodeMask = 0
            };
            RtvHeap = device.Device.CreateDescriptorHeap(rtvHeapDesc);
            _rtvDescriptorSize =
                device.Device.GetDescriptorHandleIncrementSize(D3D12.DescriptorHeapType.RenderTargetView);
            
            RenderTargets = new D3D12.Resource[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                RenderTargets[i] = SwapChain.GetBackBuffer<D3D12.Resource>(i);
                device.Device.CreateRenderTargetView(RenderTargets[i], null, GetRtvCpuDescriptorHandle(i));
            }
        }
        
        public void Dispose()
        {
            foreach (var rt in RenderTargets)
            {
                rt.Dispose();
            }
            SwapChain?.Dispose();
            RtvHeap.Dispose();
        }
        
        public DXGI.SwapChain3 SwapChain { get; }
        public D3D12.Resource[] RenderTargets { get; }

        public D3D12.DescriptorHeap RtvHeap { get; }

        public D3D12.CpuDescriptorHandle GetRtvCpuDescriptorHandle(int index) =>
            RtvHeap.CPUDescriptorHandleForHeapStart + index * _rtvDescriptorSize;
    }
}