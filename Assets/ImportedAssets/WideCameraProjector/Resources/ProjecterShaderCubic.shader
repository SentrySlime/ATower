
Shader "Hidden/WideCameraProjectorShaderCubic" {

    Properties {
        _MainTex ("Cube Tex", Cube) = "" {}
        _Rect ("Rect", Vector) = (1,1,0,0)
        _Aspect ("Aspect", Float) = 1
    }
    
    CGINCLUDE

    #pragma multi_compile MAP_STEREOGRAPHIC MAP_EQUIDISTANT MAP_EQUISOLID MAP_ORTHOGRAPHIC MAP_GEOGRAPHIC MAP_MILLER MAP_CASSINI
    #include "UnityCG.cginc"
    #include "ProjecterIncludes.cginc"

    struct v2f {
        half4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
        half fovRatio : TEXCOORD1;
    };

    samplerCUBE _MainTex;
    half4 _MainTex_ST; // We don't know what inside this
	fixed _EyeBlindSide;
  
    v2f vert( appdata_img v ) 
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = mapTransform(v.texcoord.xy);
        o.fovRatio = 1 / ( _FovExpansion * 2);
        return o;
    }

    fixed4 frag(v2f i) : SV_Target 
    {
    	fixed4 color = fixed4(0, 0, 0, 0);

    	if (_EyeBlindSide != unity_StereoEyeIndex + 1 && mask(i.uv))
    	{
	     	half3 dir = map(i.uv);
			dir = mul(_FaceMatrix, half4(dir, 0));
			color = texCUBE (_MainTex, dir);
		}

		return color;
    }

    ENDCG 
    
    Subshader {
    	Tags { "Queue" = "Overlay" }
        Pass {
        		// Alpha based blend so blits can be merged each other.
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Off Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
