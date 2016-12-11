/// <summary>The brightness offset.</summary>
/// <defaultValue>1,1,1,1</defaultValue>
float4 ClearColor : register(c0);

/// <summary>The implicit input sampler passed into the pixel shader by WPF.</summary>
sampler2D Input : register(s0);

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 pixelColor = tex2D(Input, uv);
    return ClearColor * ClearColor.a + pixelColor;
}