Shader "Standard" {
    Properties {
        _ID ("ID", Int) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 pos : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            int _ID;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float depth = i.pos.z;
                depth = Linear01Depth(depth);
                depth = 1 - depth; // closer = higher

                return fixed4(depth, _ID, 0, 1);
            }
            ENDCG
        }
    }
}
