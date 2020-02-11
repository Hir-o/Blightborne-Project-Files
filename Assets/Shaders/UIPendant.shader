//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.2                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/UIPendant"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_SuperGrayScale_Fade_1("_SuperGrayScale_Fade_1", Range(0, 1)) = 0
_PremadeGradients_Offset_1("_PremadeGradients_Offset_1", Range(-1, 1)) =0
_PremadeGradients_Fade_1("_PremadeGradients_Fade_1", Range(0, 1)) =0.096
_PremadeGradients_Speed_1("_PremadeGradients_Speed_1", Range(-2, 2)) =2
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
float _SuperGrayScale_Fade_1;
float _PremadeGradients_Offset_1;
float _PremadeGradients_Fade_1;
float _PremadeGradients_Speed_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
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
float4 SuperGrayScale(float4 rgba, float4 red, float4 green, float4 blue, float fade)
{
float3 c_r = float3(red.r, red.g, red.b);
float3 c_g = float3(green.r, green.g, green.b);
float3 c_b = float3(blue.r, blue.g, blue.b);
float4 r = float4(dot(rgba.rgb, c_r) + red.a, dot(rgba.rgb, c_g) + green.a, dot(rgba.rgb, c_b) + blue.a, rgba.a);
return lerp(rgba, r, fade);

}
float4 frag (v2f i) : COLOR
{
float4 _MainTex_1 = tex2D(_MainTex, i.texcoord);
float4 _SuperGrayScale_1 = SuperGrayScale(_MainTex_1,float4(0.5,0.5,0,0),float4(0.5,0.5,0,0),float4(0.5,0.5,0,0),_SuperGrayScale_Fade_1);
float4 _PremadeGradients_1 = Color_PreGradients(_SuperGrayScale_1,float4(0.55,0.55,0.55,1),float4(0.8,0.8,0.8,1),float4(0.29,0.29,0.29,1),float4(0.54,0.59,0.6900001,1),_PremadeGradients_Offset_1,_PremadeGradients_Fade_1,_PremadeGradients_Speed_1);
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
