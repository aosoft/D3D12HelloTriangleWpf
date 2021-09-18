using System;

namespace D3D12HelloTriangleSharp
{
    public sealed class D3D12HelloTriangle : IDisposable
    {
        private const int FrameCount = 2;
        
        private readonly GraphicsDevice _device;
        private readonly ResourceSet[] _resourceSets = new ResourceSet[FrameCount];
        private readonly Display _display;
        
        public D3D12HelloTriangle(IntPtr windowHandle, int width, int height)
        {
            _device = new GraphicsDevice(false);
            var shader = new Shader();
            for (int i = 0; i < FrameCount; i++)
            {
                _resourceSets[i] = new ResourceSet(_device, width, height, shader);
            }
            
            _display = new Display(_device, windowHandle, width, height, FrameCount);
        }
        
        public void Dispose()
        {
            for (int i = 0; i < FrameCount; i++)
            {
                _resourceSets[i].Dispose();
            }
            _display.Dispose();
            _device.Dispose();
        }

        public void Render(float ratio)
        {
            var frameIndex = _display.SwapChain.CurrentBackBufferIndex;
            _resourceSets[frameIndex].Render(ratio, _display.RenderTargets[frameIndex],
                _display.GetRtvCpuDescriptorHandle(frameIndex));
            _display.SwapChain.Present(1, 0);
            _resourceSets[frameIndex].Fence.WaitForPreviousFrame();
        }
    }
}