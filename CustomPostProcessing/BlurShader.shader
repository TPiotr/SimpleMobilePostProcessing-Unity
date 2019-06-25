Shader "Custom/BlurShader"
{
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _Radius ("Radius", Float) = 2
        _Dir("Direction", Vector) = (1.0, 0.0, 0.0, 0.0)
	}
	SubShader 
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _Radius;
			float2 _Dir;

			float4 frag(v2f_img i) : COLOR
			{
				float4 color = float4(0.0, 0.0, 0.0, 0.0);
				float2 offset1 = float2(1.3846153846, 1.3846153846) * _Dir * _Radius;
				float2 offset2 = float2(3.2307692308, 3.2307692308) * _Dir * _Radius;
				
				float2 resolution = _MainTex_TexelSize.zw; //z = texture width, w = texture height

				color += tex2D(_MainTex, i.uv) * 0.2270270270;
				color += tex2D(_MainTex, i.uv + (offset1 / resolution)) * 0.3162162162;
				color += tex2D(_MainTex, i.uv - (offset1 / resolution)) * 0.3162162162;
				color += tex2D(_MainTex, i.uv + (offset2 / resolution)) * 0.0702702703;
				color += tex2D(_MainTex, i.uv - (offset2 / resolution)) * 0.0702702703;

				return color; 
			}

			ENDCG
		} 
	}
}    