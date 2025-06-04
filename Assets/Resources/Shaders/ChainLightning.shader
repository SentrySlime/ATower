Shader "Custom/ChainLightning"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _FlickerSpeed ("Flicker Speed", Float) = 30.0
        _FlickerIntensity ("Flicker Intensity", Range(0,1)) = 1.0
        _DisplacementAmount ("Vertex Displacement", Float) = 0.05
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _FlickerSpeed;
            float _FlickerIntensity;
            float _DisplacementAmount;
            float4 _EmissionColor;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float t = _Time.y * _FlickerSpeed;

                float3 offset;
                offset.x = sin(v.vertex.y * 10 + t) * _DisplacementAmount;
                offset.y = sin(v.vertex.x * 10 + t * 1.2) * _DisplacementAmount;
                offset.z = cos(v.vertex.x * 10 + t * 0.8) * _DisplacementAmount;

                o.vertex = UnityObjectToClipPos(v.vertex + float4(offset, 0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = _Time.y * _FlickerSpeed;
                float flicker = abs(sin(t)) * _FlickerIntensity;

                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
                tex.a *= flicker;

                // Emission is added directly to color output
                fixed3 emission = _EmissionColor.rgb * flicker;
                tex.rgb += emission;

                return tex;
            }
            ENDCG
        }
    }
}
