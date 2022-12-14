sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float progress;
float3 uColor;
matrix WorldViewProjection;
texture vineTexture;
texture vnoise;
sampler vineSampler = sampler_state
{
    Texture = (vineTexture);
};
struct VertexShaderInput
{
	float2 TextureCoordinates : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float2 TextureCoordinates : TEXCOORD0;
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    float4 pos = mul(input.Position, WorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;

	output.TextureCoordinates = input.TextureCoordinates;

    return output;
};
float4 White(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(vineSampler, float2((input.TextureCoordinates.x * (progress + 0.1f)) % 1.0f, input.TextureCoordinates.y));
	return color * input.Color;
}

technique BasicColorDrawing
{
    pass DefaultPass
	{
		VertexShader = compile vs_2_0 MainVS();
	}
    pass MainPS
    {
        PixelShader = compile ps_2_0 White();
    }
};