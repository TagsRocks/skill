Shader "Hidden/Skill/BrushPreview"      
    {
        Properties
        {
            _Color ("Main Color", Color) = (.2,.7,1,.5)
            _MainTex ("Brush", 2D) = "white" {}
			_CutoutTex ("Cutout", 2D) = "black"
        }
     
        Subshader
        {    
            Tags {"Queue"="Transparent"}
            Pass
            {
                ZWrite Off
                Fog { Color (0, 0, 0) }
                ColorMask RGB
                Blend SrcAlpha OneMinusSrcAlpha
                Offset -1, -1
     
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #include "UnityCG.cginc"
             
                struct v2f
                {
                    float4 uv : TEXCOORD0;
                    float4 pos : SV_POSITION;
                };
             
                float4x4 _Projector;
                fixed4 _Color;
             
                v2f vert (float4 vertex : POSITION)
                {
                    v2f o;
                    o.pos = mul (UNITY_MATRIX_MVP, vertex);
                    o.uv = mul (_Projector, vertex);
                    return o;
                }
             
                sampler2D _MainTex;
				sampler2D _CutoutTex;
             
                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 texS = tex2Dproj (_MainTex, UNITY_PROJ_COORD(i.uv));
					fixed4 texC = tex2Dproj (_CutoutTex, UNITY_PROJ_COORD(i.uv));
					texS.a *= texC.a;
                    return texS  * _Color;
                }
                ENDCG
            }      
          }
    }
