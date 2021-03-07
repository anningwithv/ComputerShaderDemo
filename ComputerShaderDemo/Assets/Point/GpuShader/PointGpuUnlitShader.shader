
Shader "Unlit/PointGpuUnlitShader"
{
    Properties
    {
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float4 positionWS : TEXCOORD0;
            };

            //#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                StructuredBuffer<float3> _Positions;
            //#endif

            float _Step;

            void ConfigureProcedural (uint id) {
                //#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                    float3 position = _Positions[id];

                    unity_ObjectToWorld = 0.0;
                    unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
                    unity_ObjectToWorld._m00_m11_m22 = _Step;
                //#endif
            }

            v2f vert (appdata v, uint iId : SV_InstanceID)
            {
                ConfigureProcedural(iId);

                v2f o;
                o.positionCS = UnityObjectToClipPos(v.vertex);
                o.positionWS = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = saturate(i.positionWS * 0.5 + 0.5);
                return col;
            }
            ENDCG
        }
    }
}
