//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.2                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Previews/PreviewXATXQ2"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float DSFXr (float2 c, float seed)
{
return frac(43.*sin(c.x+7.*c.y)*seed);
}

float DSFXn (float2 p, float seed)
{
float2 i = floor(p), w = p-i, j = float2 (1.,0.);
w = w*w*(3.-w-w);
return lerp(lerp(DSFXr(i, seed), DSFXr(i+j, seed), w.x), lerp(DSFXr(i+j.yx, seed), DSFXr(i+1., seed), w.x), w.y);
}

float DSFXa (float2 p, float seed)
{
float m = 0., f = 2.;
for ( int i=0; i<9; i++ ){ m += DSFXn(f*p, seed)/f; f+=f; }
return m;
}

float4 DestroyerFX(float4 txt, float2 uv, float value, float seed, float HDR)
{
float t = frac(value*0.9999);
float4 c = smoothstep(t / 1.2, t + .1, DSFXa(3.5*uv, seed));
c = txt*c;
c.r = lerp(c.r, c.r*120.0*(1 - c.a), value);
c.g = lerp(c.g, c.g*40.0*(1 - c.a), value);
c.b = lerp(c.b, c.b*5.0*(1 - c.a) , value);
c.rgb = lerp(saturate(c.rgb),c.rgb,HDR);
return c;
}
float Generate_Fire_hash2D(float2 x)
{
return frac(sin(dot(x, float2(13.454, 7.405)))*12.3043);
}

float Generate_Fire_voronoi2D(float2 uv, float precision)
{
float2 fl = floor(uv);
float2 fr = frac(uv);
float res = 1.0;
for (int j = -1; j <= 1; j++)
{
for (int i = -1; i <= 1; i++)
{
float2 p = float2(i, j);
float h = Generate_Fire_hash2D(fl + p);
float2 vp = p - fr + h;
float d = dot(vp, vp);
res += 1.0 / pow(d, 8.0);
}
}
return pow(1.0 / res, precision);
}

float4 Generate_Fire(float2 uv, float posX, float posY, float precision, float smooth, float speed, float black)
{
uv += float2(posX, posY);
float t = _Time*60*speed;
float up0 = Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(0, -t), precision);
float up1 = 0.5 + Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(42, -t ) + 30.0, precision);
float finalMask = up0 * up1  + (1.0 - uv.y);
finalMask += (1.0 - uv.y)* 0.5;
finalMask *= 0.7 - abs(uv.x - 0.5);
float4 result = smoothstep(smooth, 0.95, finalMask);
result.a = saturate(result.a + black);
return result;
}
float4 Color_PreGradients(float4 rgba, float4 a, float4 b, float4 c, float4 d, float offset, float fade, float speed)
{
float gray = (rgba.r + rgba.g + rgba.b) / 3;
gray += offset+(speed*_Time*20);
float4 result = a + b * cos(6.28318 * (c * gray + d));
result.a = rgba.a;
result.rgb = lerp(rgba.rgb, result.rgb, fade);
return result;
}
float2 PixelUV(float2 uv, float x)
{
uv = floor(uv * x + 0.5) / x;
return uv;
}
float4 frag (v2f i) : COLOR
{
float4 _PremadeGradients_1 = Color_PreGradients(float4(0,0,0,1),float4(1,0,0.13,1),float4(0.42,0.95,0,1),float4(0.99,0.68,0.99,1),float4(0.39,0.39,1,1),-0.41,1,0);
float4 FinalResult = _PremadeGradients_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
FinalResult.rgb *= FinalResult.a;
FinalResult.a = saturate(FinalResult.a);
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
