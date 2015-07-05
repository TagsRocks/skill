Shader "Hidden/Skill/LineGL" 
{
	SubShader 
	{ 
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off Cull Off Fog { Mode Off } 
            BindChannels {
            Bind "vertex", vertex Bind "color", color }
        }
	}
}