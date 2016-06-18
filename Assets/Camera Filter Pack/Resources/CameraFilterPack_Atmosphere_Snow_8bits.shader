///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////

Shader "CameraFilterPack/Atmosphere_Snow_8bits" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
}
SubShader
{
Pass
{
ZTest Always
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#pragma glsl
#include "UnityCG.cginc"
uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Value;
uniform float _Value2;
uniform float4 _ScreenResolution;
uniform float2 _MainTex_TexelSize;
struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};
struct v2f
{
half2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
fixed4 color    : COLOR;
};
v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}

inline float2 mod(float2 x,float2 modu) {
  return x - floor(x * (1.0 / modu)) * modu;
} 


inline float rand(float2 co)
{
	float r;
	co = floor(co*_Value2);
 	r = frac(sin(dot(co.xy,float2(12.9898,78.233))) * 43758.5453);
   return r;
}

float4 frag (v2f i) : COLOR
{
	float2 uv = i.texcoord.xy;
	
    float3 col=tex2D(_MainTex,uv).rgb;
	#if UNITY_UV_STARTS_AT_TOP
if (_MainTex_TexelSize.y < 0)
uv.y = 1-uv.y;
#endif
   float ts=uv.y-.2+sin(uv.x*4.0+7.4*cos(uv.x*10.0))*0.005;
 
    uv*=2.0;
    
    float c=cos(8*0.01),si=sin(8*0.01);
	uv=(uv-0.5)*float2(c+si,-si+c);	
    
    float s=rand(mod(uv * 1.01 +float2(_TimeX,_TimeX)*float2(0.02,0.501),1.0)).r;
    col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.9,1.0, s * .9 * _Value));
    
 	s=rand(mod(uv * 1.07 +float2(_TimeX,_TimeX)*float2(0.02,0.501),1.0)).r;
 	col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.9,1.0, s * 1. * _Value));
    
	s=rand(mod(uv+float2(_TimeX,_TimeX)*float2(0.05,0.5),1.0)).r;
    col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.9,1.0, s * .98 * _Value));
	
	s=rand(mod(uv * .9 +float2(_TimeX,_TimeX)*float2(0.02,0.51),1.0)).r;
    col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.9,1.0, s * .99 * _Value));
	
	s=rand(mod(uv * .75 +float2(_TimeX,_TimeX)*float2(0.07,0.493),1.0)).r;
    col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.9,1.0, s * 1. * _Value));
	
	s=rand(mod(uv * .5 +float2(_TimeX,_TimeX)*float2(0.03,0.504),1.0)).r;
    col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.94,1.0, s * 1. * _Value));
	
	s=rand(mod(uv * .3 +float2(_TimeX,_TimeX)*float2(0.02,0.497),1.0)).r;
    col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.95,1.0, s * 1. * _Value));
	
	s=rand(mod(uv * .1 +float2(_TimeX,_TimeX)*float2(0.0,0.51),1.0)).r;
	col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.96,1.0, s * 1. * _Value));
	
	s=rand(mod(uv * .03 +float2(_TimeX,_TimeX)*float2(0.0,0.523),1.0)).r;
	col=lerp(col,float3(1.0,1.0,1.0),smoothstep(0.99,1.0, s * 1. * _Value));
    
	return  float4(col,1.0);
}
ENDCG
}
}
}
