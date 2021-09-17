using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Mathematics.Interop;
using D3D = SharpDX.Direct3D;
using D3D12 = SharpDX.Direct3D12;
using DXGI = SharpDX.DXGI;

namespace D3D12HelloTriangleSharp
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct GraphicsPipelineConstantBuffer
    {
        private readonly float _ratio;
        public float Ratio => _ratio;

        public GraphicsPipelineConstantBuffer(float ratio)
        {
            _ratio = ratio;
        }
    }

    public sealed class GraphicsPipeline : IDisposable
    {
        private RawViewportF _viewport;
        private Rectangle _scissorRect;
        private readonly D3D12.RootSignature _rootSignature;
        private readonly D3D12.PipelineState _pipelineState;
        private readonly D3D12.Resource _vertexBuffer;
        private readonly D3D12.VertexBufferView _vertexBufferView;

        public GraphicsPipeline(GraphicsDevice device, int width, int height, Shader shader)
        {
            _viewport = new()
            {
                X = 0,
                Y = 0,
                Width = width,
                Height = height,
                MinDepth = 0.0f,
                MaxDepth = 1.0f
            };

            _scissorRect = new()
            {
                Left = 0,
                Top = 0,
                Right = width,
                Bottom = height,
            };

            var rootSignatureDesc = new D3D12.RootSignatureDescription(
                D3D12.RootSignatureFlags.AllowInputAssemblerInputLayout, new[]
                {
                    new D3D12.RootParameter(D3D12.ShaderVisibility.Pixel, new D3D12.DescriptorRange
                    {
                        RangeType = D3D12.DescriptorRangeType.ConstantBufferView,
                        DescriptorCount = 1,
                        BaseShaderRegister = 0,
                        RegisterSpace = 0,
                        OffsetInDescriptorsFromTableStart = int.MinValue
                    })
                });

            using (var signature = rootSignatureDesc.Serialize())
            {
                _rootSignature = device.Device.CreateRootSignature(signature);
            }

            var psoDesc = new D3D12.GraphicsPipelineStateDescription
            {
                RootSignature = _rootSignature,
                VertexShader = shader.VertexShader,
                PixelShader = shader.PixelShader,
                StreamOutput = new D3D12.StreamOutputDescription(),
                BlendState = D3D12.BlendStateDescription.Default(),
                SampleMask = -1,
                RasterizerState = D3D12.RasterizerStateDescription.Default(),
                DepthStencilState = new D3D12.DepthStencilStateDescription
                {
                    IsDepthEnabled = false,
                    IsStencilEnabled = false,
                },
                InputLayout = shader.InputLayout,
                IBStripCutValue = D3D12.IndexBufferStripCutValue.Disabled,
                PrimitiveTopologyType = D3D12.PrimitiveTopologyType.Triangle,
                RenderTargetCount = 1,
                DepthStencilFormat = DXGI.Format.D32_Float,
                SampleDescription = new DXGI.SampleDescription
                {
                    Count = 1,
                    Quality = 0
                },
                NodeMask = 0,
                Flags = D3D12.PipelineStateFlags.None
            };
            psoDesc.RenderTargetFormats[0] = DXGI.Format.R8G8B8A8_UNorm;
            _pipelineState = device.Device.CreateGraphicsPipelineState(psoDesc);
            CommandList =
                device.Device.CreateCommandList(0, D3D12.CommandListType.Direct, device.CommandAllocator,
                    _pipelineState);
            CommandList.Close();

            var vertices = shader.GetVerticies((float)width / height);
            _vertexBuffer = device.Device.CreateCommittedResource(new D3D12.HeapProperties(D3D12.HeapType.Upload),
                D3D12.HeapFlags.None,
                D3D12.ResourceDescription.Buffer(Utilities.SizeOf(vertices)),
                D3D12.ResourceStates.GenericRead);
            var vertexDataBegin = _vertexBuffer.Map(0, new D3D12.Range
            {
                Begin = 0,
                End = 0
            });
            try
            {
                Utilities.Write(vertexDataBegin, vertices, 0, vertices.Length);
            }
            finally
            {
                _vertexBuffer.Unmap(0);
            }

            _vertexBufferView = new D3D12.VertexBufferView
            {
                BufferLocation = _vertexBuffer.GPUVirtualAddress,
                SizeInBytes = Utilities.SizeOf(vertices),
                StrideInBytes = Utilities.SizeOf<Vertex>()
            };
        }

        public void Dispose()
        {
            _rootSignature.Dispose();
            _pipelineState.Dispose();
            CommandList.Dispose();
            _vertexBuffer.Dispose();
        }

        public D3D12.GraphicsCommandList CommandList { get; }

        public void PopulateCommandList(D3D12.CommandAllocator commandAllocator, D3D12.Resource rt,
            D3D12.CpuDescriptorHandle rtvHandle, ShaderConstantBuffer cb)
        {
            commandAllocator.Reset();
            CommandList.Reset(commandAllocator, _pipelineState);
            CommandList.SetGraphicsRootSignature(_rootSignature);
            CommandList.SetDescriptorHeaps(cb.Heap);
            CommandList.SetGraphicsRootDescriptorTable(0, cb.CbvGpuDescriptorHandle);
            CommandList.SetViewport(_viewport);
            CommandList.SetScissorRectangles(_scissorRect);
            CommandList.ResourceBarrierTransition(rt, D3D12.ResourceStates.Present, D3D12.ResourceStates.RenderTarget);

            CommandList.SetRenderTargets(1, rtvHandle, null);

            CommandList.ClearRenderTargetView(rtvHandle, new Color4(0, 0.2F, 0.4f, 1), 0, null);

            CommandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            CommandList.SetVertexBuffer(0, _vertexBufferView);
            CommandList.DrawInstanced(3, 1, 0, 0);

            CommandList.ResourceBarrierTransition(rt, D3D12.ResourceStates.RenderTarget, D3D12.ResourceStates.Present);

            CommandList.Close();
        }
    }
}