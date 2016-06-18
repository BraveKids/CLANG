///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////


Shader "CameraFilterPack/Distortion_Flush" { 
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
uniform float4 _ScreenResolution;
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
float cross(float2 a, float2 b) { return a.x * b.y - a.y * b.x; }
float4 frag (v2f i) : COLOR
{
    float2 center = float2(0.5,0.5);
    float t = fmod(_TimeX, 10.);
    float2 vis = i.texcoord.xy - center;
    float4 col2;
    float dist = pow((center.x - length(vis))/length(center),.5) * t * 5.;
    float2 dir = float2(cos(dist), cos(dist));
    float4 col = float4(1.,1.,1.,1.) * step(0., cross(vis, dir));
    
    col = tex2D(_MainTex,i.texcoord.xy+float2(col.r,col.r)/16);
    return col;
}
ENDCG
}
}
}
