Shader "Unlit/colorBlind"
{
		Properties{
			_MainTex("Texture", 2D) = "white" {}
		}

		SubShader{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

		    sampler2D _MainTex;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				float commonRed = col.r * 0.163;
				float commonGreen = col.g * 0.320;
				float commonBlue = col.b * 0.062;
				
				return float4(
					col.r * 0.618 + commonGreen + commonBlue,
					commonRed + col.g * 0.775 + commonBlue,
					commonRed + commonGreen + col.b * 0.616,
					col.a);
			}

			ENDCG
		}
	}
}
