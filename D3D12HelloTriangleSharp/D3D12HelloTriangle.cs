using System;

namespace D3D12HelloTriangleSharp
{
    public sealed class D3D12HelloTriangle : IDisposable
    {
        private const int FrameCount = 2;
        
        private readonly GraphicsDevice _device;
        private readonly Shader _shader = new Shader();
        private readonly ResourceSet[] _resourceSets = new ResourceSet[FrameCount];

        private readonly Display _display;
        
        public D3D12HelloTriangle(IntPtr windowHandle, int width, int height, bool useWarpDevice)
        {
            _device = new GraphicsDevice(useWarpDevice);
            for (int i = 0; i < FrameCount; i++)
            {
                _resourceSets[i] = new ResourceSet(_device, width, height, _shader);
            }
            
            _display = new Display(_device, windowHandle, width, height, FrameCount);
        }
        
        public void Dispose()
        {
            _display.Dispose();
            for (int i = 0; i < FrameCount; i++)
            {
                _resourceSets[i].Dispose();
            }
            _device.Dispose();
        }

        public float Ratio { get; set; } = 1.0f;

        public void OnRender()
        {
            var frameIndex = _display.SwapChain.CurrentBackBufferIndex;
            _resourceSets[frameIndex].Ratio = Ratio;
            _resourceSets[frameIndex].Render(_display.RenderTargets[frameIndex],
                _display.GetRtvCpuDescriptorHandle(frameIndex));
            _display.SwapChain.Present(1, 0);
            _resourceSets[frameIndex].Fence.WaitForPreviousFrame();
        }
        
        public void OnDestroy()
        {
            for (int i = 0; i < FrameCount; i++)
            {
                _resourceSets[i].Fence.WaitForPreviousFrame();
            }
        }
    }
}