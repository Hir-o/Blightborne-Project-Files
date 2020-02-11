//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.2                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Pixel Dissolve"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_GrayScale_Fade_1("_GrayScale_Fade_1", Range(0, 1)) = 1
_Displacement_Plus_ValueX_1("_Displacement_Plus_ValueX_1", Range(-1, 1)) = 0
_Displacement_Plus_ValueY_1("_Displacement_Plus_ValueY_1", Range(-1, 1)) = 0
_Displacement_Plus_Size_1("_Displacement_Plus_Size_1", Range(-3, 3)) = 1
_ColorFilters_Fade_1("_ColorFilters_Fade_1", Range(0, 1)) = 1
_RGBA_Mul_Fade_1("_RGBA_Mul_Fade_1", Range(0, 2)) = 1.18
_Brightness_Fade_1("_Brightness_Fade_1", Range(0, 1)) = 0
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
float _GrayScale_Fade_1;
float _Displacement_Plus_ValueX_1;
float _Displacement_Plus_ValueY_1;
float _Displacement_Plus_Size_1;
float _ColorFilters_Fade_1;
float _RGBA_Mul_Fade_1;
float _Brightness_Fade_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}



float4 grayscale(float4 txt,float fade)
{
float3 gs = dot(txt.rgb, float3(0.3, 0.59, 0.11));
return lerp(txt,float4(gs, txt.a), fade);
}

float4 Brightness(float4 txt, float value)
{
txt.rgb += value;
return txt;
}
float4 DisplacementPlusUV(float2 uv,sampler2D source,float4 rgba ,float4 rgba2,float x, float y, float value)
{
float r=(rgba2.r+rgba2.g+rgba2.b)/3;
r*=rgba2.a;
return tex2D(source,lerp(uv,uv+float2(rgba.r*x,rgba.g*y),value*r));
}
float4 ColorFilters(float4 rgba, float4 red, float4 green, float4 blue, float fade)
{
float3 c_r = float3(red.r, red.g, red.b);
float3 c_g = float3(green.r, green.g, green.b);
float3 c_b = float3(blue.r, blue.g, blue.b);
float4 r = float4(dot(rgba.rgb, c_r) + red.a, dot(rgba.rgb, c_g) + green.a, dot(rgba.rgb, c_b) + blue.a, rgba.a);
return lerp(rgba, saturate(r), fade);

}
float4 frag (v2f i) : COLOR
{
float4 SourceRGBA_1 = tex2D(_MainTex, i.texcoord);
float4 GrayScale_1 = grayscale(SourceRGBA_1,_GrayScale_Fade_1);
float4 _Displacement_Plus_1 = DisplacementPlusUV(i.texcoord,_MainTex,GrayScale_1,float4(1,1,1,1),_Displacement_Plus_ValueX_1,_Displacement_Plus_ValueY_1,_Displacement_Plus_Size_1);
float4 _ColorFilters_1 = ColorFilters(_Displacement_Plus_1,float4(2,0.98,-1.16,-0.48),float4(0.24,1,0.02,-0.2),float4(0.3,0.52,0.2,0.12),_ColorFilters_Fade_1);
_ColorFilters_1.gb *= _RGBA_Mul_Fade_1;
float4 Brightness_1 = Brightness(_ColorFilters_1,_Brightness_Fade_1);
float4 FinalResult = Brightness_1;
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
