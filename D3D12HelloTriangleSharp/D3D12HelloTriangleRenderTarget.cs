using System;
using SharpDX;
using DXGI = SharpDX.DXGI;
using D3D12 = SharpDX.Direct3D12;

namespace D3D12HelloTriangleSharp
{
    public sealed class D3D12HelloTriangleRenderTarget : IDisposable
    {
        private readonly GraphicsDevice _device;
        private readonly GraphicsDevice9 _device9;
        private readonly ResourceSet _resourceSet;
        private readonly D3D12.Resource _renderTarget;
        private readonly D3D12.DescriptorHeap _rtvHeap;

        public D3D12HelloTriangleRenderTarget(IntPtr handle, int width, int height)
        {
            _device9 = new GraphicsDevice9(width, height);
            _device = new GraphicsDevice(false);
            _resourceSet = new ResourceSet(_device, width, height, new Shader());

            IntPtr intf;
            _device.Device.OpenSharedHandle(_device9.RenderTargetSharedHanlde,
                Utilities.GetGuidFromType(typeof(D3D12.Resource)), out intf);
            _renderTarget = new D3D12.Resource(intf);

            var desc = _renderTarget.Description;
            
            var rtvHeapDesc = new D3D12.DescriptorHeapDescription
            {
                Type = D3D12.DescriptorHeapType.RenderTargetView,
                DescriptorCount = 1,
                Flags = D3D12.DescriptorHeapFlags.None,
                NodeMask = 0
            };
            _rtvHeap = _device.Device.CreateDescriptorHeap(rtvHeapDesc);
            _device.Device.CreateRenderTargetView(_renderTarget, null, _rtvHeap.CPUDescriptorHandleForHeapStart);
        }

        public void Dispose()
        {
            _rtvHeap.Dispose();
            _renderTarget.Dispose();
            _resourceSet.Dispose();
            _device.Dispose();
            _device9.Dispose();
        }

        public void Render(float ratio)
        {
            _resourceSet.Render(ratio, _renderTarget, _rtvHeap.CPUDescriptorHandleForHeapStart);
            _resourceSet.Fence.WaitForPreviousFrame();
        }
    }
}