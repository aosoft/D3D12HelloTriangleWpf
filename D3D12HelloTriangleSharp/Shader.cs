using System;
using System.Runtime.InteropServices;
using SharpDX;
using D3D = SharpDX.Direct3D;
using DXGI = SharpDX.DXGI;
using D3D12 = SharpDX.Direct3D12;
using D3DCompiler = SharpDX.D3DCompiler;

namespace D3D12HelloTriangleSharp
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector4 Color;
    };
    
    public sealed class Shader
    {
        public Shader()
        {
            var compileFlags =
#if DEBUG
                D3DCompiler.ShaderFlags.Debug | D3DCompiler.ShaderFlags.SkipOptimization;
#else
                D3DCompiler.ShaderFlags.None;
#endif
            VertexShader = D3DCompiler.ShaderBytecode.Compile(Hlsl, "VSMain", "vs_5_0", compileFlags).Bytecode.Data;
            PixelShader = D3DCompiler.ShaderBytecode.Compile(Hlsl, "PSMain", "ps_5_0", compileFlags).Bytecode.Data;
            InputLayout = new D3D12.InputElement[]
            {
                new()
                {
                    SemanticName = "POSITION",
                    SemanticIndex = 0,
                    Format = DXGI.Format.R32G32B32_Float,
                    Slot = 0,
                    AlignedByteOffset = 0,
                    Classification = D3D12.InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                },
                new()
                {
                    SemanticName = "COLOR",
                    SemanticIndex = 0,
                    Format = DXGI.Format.R32G32B32_Float,
                    Slot = 0,
                    AlignedByteOffset = 12,
                    Classification = D3D12.InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                },
            };
        }
        
        public byte[] VertexShader { get; }
        public byte[] PixelShader { get; }
        public D3D12.InputElement[] InputLayout { get; }
        
        public Vertex[] GetVerticies(float aspectRatio) =>
            new Vertex[]
            {
                new()
                {
                    Position = new(0.0f, 0.25f * aspectRatio, 0.0f),
                    Color = new(1.0f, 0.0f, 0.0f, 1.0f)
                },
                new()
                {
                    Position = new(0.25f, -0.25f * aspectRatio, 0.0f),
                    Color = new(0.0f, 1.0f, 0.0f, 1.0f)
                },
                new()
                {
                    Position = new(-0.25f, -0.25f * aspectRatio, 0.0f),
                    Color = new(0.0f, 0.0f, 1.0f, 1.0f)
                },
            };
        
        private static readonly string Hlsl = @"
cbuffer cb : register(b0)
{
    float ratio;
};

struct PSInput
{
    float4 position : SV_POSITION;
    float4 color : COLOR;
};

PSInput VSMain(float4 position : POSITION, float4 color : COLOR)
{
    PSInput result;

    result.position = position;
    result.color = color;

    return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    return input.color * ratio;
}
";
        
    }
}