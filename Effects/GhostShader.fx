sampler uImage0 : register(s0);
float progress;

float4 GhostShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);

    float luminosity = (color.r + color.g + color.b) / 3;
    color.rgb = luminosity;

    float ydistfromcenter = clamp((coords.y % (1 / progress)) * progress, 0, 1);

    return color * sampleColor * max(0.8 - ydistfromcenter, 0);
}

technique BasicColorDrawing
{
    pass GhostShaderPass
    {
        PixelShader = compile ps_2_0 GhostShader();
    }
};