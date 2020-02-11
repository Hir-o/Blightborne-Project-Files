//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.2                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
__CompressionFX_Value_1("__CompressionFX_Value_1", Range(1, 16)) = 0
_HdrCreate_Value_1("_HdrCreate_Value_1", Range(-2, 2)) = 1.5
_Destroyer_Value_1("_Destroyer_Value_1", Range(0, 1)) = 0
_Destroyer_Speed_1("_Destroyer_Speed_1", Range(0, 1)) =  0.5
_ColorHSV_Hue_1("_ColorHSV_Hue_1", Range(0, 360)) = 180
_ColorHSV_Saturation_1("_ColorHSV_Saturation_1", Range(0, 2)) = 1
_ColorHSV_Brightness_1("_ColorHSV_Brightness_1", Range(0, 2)) = 1
_TintRGBA_Color_1("_TintRGBA_Color_1", COLOR) = (0.1882353,0.572549,0.9245283,0)
_HDRControlValue_Value_1("_HDRControlValue_Value_1", Range(-2, 2)) = 1
_RGBA_Mul_Fade_1("_RGBA_Mul_Fade_1", Range(0, 2)) = 1
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
float __CompressionFX_Value_1;
float _HdrCreate_Value_1;
float _Destroyer_Value_1;
float _Destroyer_Speed_1;
float _ColorHSV_Hue_1;
float _ColorHSV_Saturation_1;
float _ColorHSV_Brightness_1;
float4 _TintRGBA_Color_1;
float _HDRControlValue_Value_1;
float _RGBA_Mul_Fade_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float CMPFXrng2(float2 seed)
{
return frac(sin(dot(seed * floor(50 + (_Time + 0.1) * 12.), float2(127.1, 311.7))) * 43758.5453123);
}

float CMPFXrng(float seed)
{
return CMPFXrng2(float2(seed, 1.0));
}

float4 CompressionFX(float2 uv, sampler2D source,float Value)
{
float2 blockS = floor(uv * float2(24., 19.))*4.0;
float2 blockL = floor(uv * float2(38., 14.))*4.0;
float r = CMPFXrng2(uv);
float lineNoise = pow(CMPFXrng2(blockS), 3.0) *Value* pow(CMPFXrng2(blockL), 3.0);
float4 col1 = tex2D(source, uv + float2(lineNoise * 0.02 * CMPFXrng(2.0), 0));
float4 result = float4(float3(col1.x, col1.y, col1.z), 1.0);
result.a = col1.a;
return result;
}
float4 TintRGBA(float4 txt, float4 color)
{
float3 tint = dot(txt.rgb, float3(.222, .707, .071));
tint.rgb *= color.rgb;
txt.rgb = lerp(txt.rgb,tint.rgb,color.a);
return txt;
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
float4 ColorHSV(float4 RGBA, float HueShift, float Sat, float Val)
{

float4 RESULT = float4(RGBA);
float a1 = Val*Sat;
float a2 = HueShift*3.14159265 / 180;
float VSU = a1*cos(a2);
float VSW = a1*sin(a2);

RESULT.x = (.299*Val + .701*VSU + .168*VSW)*RGBA.x
+ (.587*Val - .587*VSU + .330*VSW)*RGBA.y
+ (.114*Val - .114*VSU - .497*VSW)*RGBA.z;

RESULT.y = (.299*Val - .299*VSU - .328*VSW)*RGBA.x
+ (.587*Val + .413*VSU + .035*VSW)*RGBA.y
+ (.114*Val - .114*VSU + .292*VSW)*RGBA.z;

RESULT.z = (.299*Val - .3*VSU + 1.25*VSW)*RGBA.x
+ (.587*Val - .588*VSU - 1.05*VSW)*RGBA.y
+ (.114*Val + .886*VSU - .203*VSW)*RGBA.z;

return RESULT;
}
float4 HDRControlValue(float4 txt,float value)
{
return lerp(saturate(txt),txt, value);
}
float4 HdrCreate(float4 txt,float value)
{
if (txt.r>0.98) txt.r=2;
if (txt.g>0.98) txt.g=2;
if (txt.b>0.98) txt.b=2;
return lerp(saturate(txt),txt, value);
}
float4 frag (v2f i) : COLOR
{
float4 _CompressionFX_1 = CompressionFX(i.texcoord,_MainTex,__CompressionFX_Value_1);
float4 HdrCreate_1 = HdrCreate(_CompressionFX_1,_HdrCreate_Value_1);
float4 _Destroyer_1 = DestroyerFX(HdrCreate_1,i.texcoord,_Destroyer_Value_1,_Destroyer_Speed_1,2);
float4 _ColorHSV_1 = ColorHSV(_Destroyer_1,_ColorHSV_Hue_1,_ColorHSV_Saturation_1,_ColorHSV_Brightness_1);
float4 TintRGBA_1 = TintRGBA(_ColorHSV_1,_TintRGBA_Color_1);
float4 HDRControlValue_1 = HDRControlValue(TintRGBA_1,_HDRControlValue_Value_1);
HDRControlValue_1.rgb *= _RGBA_Mul_Fade_1;
float4 FinalResult = HDRControlValue_1;
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
