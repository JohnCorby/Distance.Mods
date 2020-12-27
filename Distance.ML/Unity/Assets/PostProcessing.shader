Shader "PostProcessing" {
    SubShader {
        Cull Off ZWrite Off ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.uv = v.uv;
                return o;
            }

            sampler2D _CameraDepthTexture;

            fixed4 frag(v2f i) : SV_Target
            {
                float depth = tex2D(_CameraDepthTexture, i.uv);
                depth = Linear01Depth(depth);
                depth = 1 - depth; // closer = higher
                return depth;
            }
            ENDCG
        }
    }
}
