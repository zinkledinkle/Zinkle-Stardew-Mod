Texture2D Texture;
sampler2D Sampler = sampler_state
{
    Texture = <Texture>;
};
float uTime;
float uThreshold;

#if OPENGL
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define TAU 6.28318548f;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 Main(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TexCoord;
    float2 off = float2(0.5f, 0.5f) - coords;
    float dist = length(off) * 2;
    if (dist > 1)
        return float4(0, 0, 0, 0);
    float angle = atan2(off.y, off.x);
    angle += uTime * 0.1f + sin(dist + uTime * 0.09f) * 0.2f;
    coords.x = angle / TAU;
    coords.y = dist - uTime * 0.12f;
    coords.y /= 4;
    coords = frac(coords);
    
    float4 color = tex2D(Sampler, coords);
    
    color *= saturate(color.r * uThreshold - (uThreshold - 1)) * smoothstep(0, 1, 1 - dist);
    color = lerp(color, float4(input.Color.rgb, 1.f), pow(1 - dist, 1.6f));
    return color * input.Color;
}

technique Technique1
{
    pass MainPass
    {
        PixelShader = compile PS_SHADERMODEL Main();
    }
}