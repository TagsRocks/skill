Shader "Hidden/Skill/ImageEffext/Radial_Blur" {	
Properties { 
    _MainTex ("Base (RGB)", 2D) = "white" {}	
}

SubShader 
{ 
    Pass 
    { 
        ZTest Always Cull Off ZWrite Off 
        Fog { Mode off }

CGPROGRAM 

#pragma target 3.0 
#pragma vertex vert 
#pragma fragment frag 
#pragma fragmentoption ARB_precision_hint_fastest 

#include "UnityCG.cginc"

uniform sampler2D _MainTex; 
uniform float4 _Parameters;
uniform float4 _MainTex_ST; 
uniform float4 _MainTex_TexelSize; 

struct v2f { 
    float4 pos : POSITION; 
    float2 uv : TEXCOORD0; 
};

v2f vert (appdata_img v) 
{ 
    v2f o; 
    o.pos = mul(UNITY_MATRIX_MVP, v.vertex); 
    o.uv = v.texcoord.xy; 

//#if UNITY_UV_STARTS_AT_TOP        	
    //if (_MainTex_TexelSize.y < 0.0)
		//o.uv.y = 1.0 - o.uv.y;
//#endif

    return o; 
}

float4 frag (v2f i) : COLOR 
{ 			

	float sampleDist = _Parameters.x;
	float sampleStrength = _Parameters.y;
	float2 p = float2(_Parameters.z,_Parameters.w);

    float2 texCoord = i.uv;    
	float2 dir = p - texCoord;    
	float dist = length(dir);    
	dir /= dist; 
    float4 color = tex2D(_MainTex, texCoord);     
	float4 sum = color;  	
     
    sum += tex2D(_MainTex, texCoord + (dir * (-0.08 * sampleDist)));
	sum += tex2D(_MainTex, texCoord + (dir * (-0.05 * sampleDist)));
	sum += tex2D(_MainTex, texCoord + (dir * (-0.03 * sampleDist)));
	sum += tex2D(_MainTex, texCoord + (dir * (-0.02 * sampleDist)));
	sum += tex2D(_MainTex, texCoord + (dir * (-0.01 * sampleDist)));
	sum += tex2D(_MainTex, texCoord + (dir * (0.01 * sampleDist)));
	sum += tex2D(_MainTex, texCoord + (dir * (0.02 * sampleDist)));
	sum += tex2D(_MainTex, texCoord + (dir * (0.03 * sampleDist)));
	sum += tex2D(_MainTex, texCoord + (dir * (0.05 * sampleDist)));
	sum += tex2D(_MainTex, texCoord + (dir * (0.08 * sampleDist)));
    

    sum /= 11.0; 
    float t = saturate(dist * sampleStrength); 
    return lerp(color, sum, t);
} 
ENDCG

    } 
}

Fallback off

}