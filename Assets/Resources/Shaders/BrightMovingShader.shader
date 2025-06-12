Shader "Custom/BrightMovingShader"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Gloss ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionColor ("Emission Color", Color) = (0,0,0,0)
        _EmissionMap ("Emission Map", 2D) = "black" {}
        _ScrollSpeed ("UV Scroll Speed", Vector) = (0.1, 0.0, 0, 0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            Lighting On
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            sampler2D _EmissionMap;
            float4 _EmissionColor;

            float _Gloss;
            float _Metallic;
            float4 _ScrollSpeed;

            uniform float4 _LightColor0;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float2 animatedUV = TRANSFORM_TEX(v.uv, _MainTex) + _Time.y * _ScrollSpeed.xy;
                o.uv = animatedUV;

                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 normal = normalize(i.worldNormal);
                fixed NdotL = max(0, dot(normal, lightDir));

                fixed4 albedoTex = tex2D(_MainTex, i.uv) * _Color;
                fixed3 albedo = albedoTex.rgb;

                fixed3 diffuse = albedo * (1.0 - _Metallic) * _LightColor0.rgb * NdotL;

                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(max(0, dot(normal, halfDir)), lerp(8.0, 128.0, _Gloss));
                fixed3 specular = lerp(0, _LightColor0.rgb * spec, _Metallic + 0.1);

                fixed3 emission = tex2D(_EmissionMap, i.uv).rgb * _EmissionColor.rgb;

                // Output final color with proper alpha
                return fixed4(diffuse + specular + emission, albedoTex.a);
            }
            ENDCG
        }
    }

    FallBack "VertexLit"
}
