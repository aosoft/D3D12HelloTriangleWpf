using System;
using System.Runtime.InteropServices;
using SharpDX;
using D3D12 = SharpDX.Direct3D12;

namespace D3D12HelloTriangleSharp
{
    public sealed class ShaderConstantBuffer : IDisposable
    {
        private BufferLayout _buffer;
        private readonly D3D12.Resource _constantBuffer;
        private IntPtr _mapped = IntPtr.Zero;

        private static readonly int _bufferSize = (Marshal.SizeOf<BufferLayout>() + 255) & ~255;
        
        public float Ratio
        {
            get => _buffer.Ratio;
            set => _buffer.Ratio = value;
        }

        public ShaderConstantBuffer(D3D12.Device device)
        {
            _constantBuffer = device.CreateCommittedResource(new D3D12.HeapProperties(D3D12.HeapType.Upload),
                D3D12.HeapFlags.None,
                D3D12.ResourceDescription.Buffer(_bufferSize),
                D3D12.ResourceStates.GenericRead);
            Heap = device.CreateDescriptorHeap(new D3D12.DescriptorHeapDescription
            {
                Type = D3D12.DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
                DescriptorCount = 1,
                Flags = D3D12.DescriptorHeapFlags.ShaderVisible,
                NodeMask = 0
            });
            device.CreateConstantBufferView(new D3D12.ConstantBufferViewDescription
            {
                BufferLocation = _constantBuffer.GPUVirtualAddress,
                SizeInBytes = _bufferSize
            }, Heap.CPUDescriptorHandleForHeapStart);
            _mapped = _constantBuffer.Map(0, new D3D12.Range());

            Ratio = 1.0f;
            Update();
        }

        public void Dispose()
        {
            if (_mapped != IntPtr.Zero)
            {
                _constantBuffer.Unmap(0);
                _mapped = IntPtr.Zero;
            }
            _constantBuffer.Dispose();
            Heap.Dispose();
        }

        public void Update()
        {
            Utilities.Write(_mapped, ref _buffer);
        }

        public D3D12.DescriptorHeap Heap { get; }
        public D3D12.GpuDescriptorHandle CbvGpuDescriptorHandle => Heap.GPUDescriptorHandleForHeapStart;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct BufferLayout
        {
            public float Ratio;
        }
    }
}