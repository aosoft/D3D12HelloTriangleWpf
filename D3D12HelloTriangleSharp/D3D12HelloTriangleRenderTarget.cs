using System;
using DXGI = SharpDX.DXGI;
using D3D12 = SharpDX.Direct3D12;

namespace D3D12HelloTriangleSharp
{
    public sealed class D3D12HelloTriangleRenderTarget : IDisposable
    {
        private readonly GraphicsDevice _device;
        private readonly ResourceSet _resourceSet;
        private readonly D3D12.Resource _renderTarget;
        private readonly D3D12.DescriptorHeap _rtvHeap;

        public D3D12HelloTriangleRenderTarget(IntPtr handle, int width, int height)
        {
            _device = new GraphicsDevice(false);
            _resourceSet = new ResourceSet(_device, width, height, new Shader());

            _renderTarget = _device.Device.CreateCommittedResource(new D3D12.HeapProperties(D3D12.HeapType.Default),
                D3D12.HeapFlags.None,
                D3D12.ResourceDescription.Texture2D(DXGI.Format.B8G8R8A8_UNorm, width, height),
                D3D12.ResourceStates.PixelShaderResource);

            
            var rtvHeapDesc = new D3D12.DescriptorHeapDescription
            {
                Type = D3D12.DescriptorHeapType.RenderTargetView,
                DescriptorCount = 1,
                Flags = D3D12.DescriptorHeapFlags.None,
                NodeMask = 0
            };
            _rtvHeap = _device.Device.CreateDescriptorHeap(rtvHeapDesc);
        }

        public void Dispose()
        {
            _renderTarget.Dispose();
            _resourceSet.Dispose();
            _device.Dispose();
        }

        public void Render(float ratio)
        {
            _resourceSet.Render(ratio, _renderTarget, _rtvHeap.CPUDescriptorHandleForHeapStart);
            _resourceSet.Fence.WaitForPreviousFrame();
        }
    }
}