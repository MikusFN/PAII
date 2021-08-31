Shader "Shaders/BlobShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold("Line Cleaness", float ) = 0.5
        _BoarderColor("Boarder Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        
        ZTest Always

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float _Threshold;
            uniform float4 _BoarderColor;

            struct vert_in{
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct frag_in{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            frag_in vert(vert_in i){
                frag_in o;
                o.pos = UnityObjectToClipPos(i.pos);
                o.uv = i.uv;
                return o;
            }

            float4 frag(frag_in i) : COLOR {
                float4 color = tex2D(_MainTex, i.uv);
                clip( color.x < _Threshold ? -1 : 1);
                return _BoarderColor;
            }

            ENDCG
        }
    }
}
