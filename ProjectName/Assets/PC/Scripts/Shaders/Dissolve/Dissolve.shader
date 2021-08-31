Shader "Shaders/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaText("Alpha Texture", 2D) = "white" {}
        _MergeClipValue("Merge Time Value", Range(0, 1)) = 0.1
        _MergeColor("Merge Color",  Color) = (1,1,1,1)
        _MergeColorValue("Merge Color Value", Range(0, 1)) = 0.01
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"
            uniform sampler2D _MainTex, _SecondTex, _AlphaText;
            uniform float _MergeClipValue, _MergeColorValue;
            uniform float4 _MergeColor;

           struct vertex_in{
               float4 position : POSITION;
               float2 uv : TEXCOORD0;
           };
           struct fragment_in{
               float4 position : SV_POSITION;
               float2 uv : TEXCOORD0;
           };

            fragment_in vert(vertex_in i){
                fragment_in o;

                o.position = UnityObjectToClipPos(i.position);
                o.uv = i.uv;

                return o;
            }
            float4 frag(fragment_in i) : COLOR {
                float alphaValue = tex2D(_AlphaText, i.uv);
                float4 textureValue = tex2D(_MainTex, i.uv);
                
                clip(alphaValue < _MergeClipValue? -1 : 1);

                if(alphaValue - _MergeColorValue < _MergeClipValue)
                    textureValue = _MergeColor;
                
                return float4(textureValue.xyz, 1);
            }
            ENDCG
        }
    }
}
