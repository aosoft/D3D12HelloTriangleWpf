using System;
using D3D12 = SharpDX.Direct3D12;

namespace D3D12HelloTriangleSharp
{
    public sealed class ResourceSet : IDisposable
    {
        private readonly GraphicsDevice _device;
        private readonly GraphicsPipeline _pipeline;
        private readonly ShaderConstantBuffer _cb;

        public ResourceSet(GraphicsDevice device, int width, int height, Shader shader)
        {
            _device = device;
            _pipeline = new GraphicsPipeline(device, width, height, shader);
            _cb = new ShaderConstantBuffer(device.Device);
            Fence = new Fence(device);
        }
        
        public void Dispose()
        {
            Fence.WaitForPreviousFrame();
            _pipeline.Dispose();
            _cb.Dispose();
            Fence.Dispose();
        }
        
        public Fence Fence { get; }
        
        public float Ratio
        {
            get => _cb.Ratio;
            set => _cb.Ratio = value;
        }
        
        public void Render(D3D12.Resource rt, D3D12.CpuDescriptorHandle rtvHandle)
        {
            _cb.Update();
            _pipeline.PopulateCommandList(_device.CommandAllocator, rt, rtvHandle, _cb);
            _device.CommandQueue.ExecuteCommandList(_pipeline.CommandList);
        }
        
    }
}