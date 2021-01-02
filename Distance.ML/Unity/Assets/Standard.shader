Shader "Custom/Standard" {
    Properties {
        _ID ("ID", Int) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float depth : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.depth = 1 - COMPUTE_DEPTH_01;
                return o;
            }

            unsigned int _ID;
            unsigned int _NumIDs;

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(i.depth, (float)_ID / _NumIDs, 0, 1);
            }
            ENDCG
        }
    }
    Fallback "Standard"
}
