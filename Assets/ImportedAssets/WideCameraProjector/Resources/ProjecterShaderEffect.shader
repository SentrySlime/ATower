
Shader "Hidden/WideCameraProjectorShaderEffect" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "" {}
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
    
    sampler2D _MainTex;
    half4 _MainTex_ST; // We don't know what inside this

  
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

    		fixed4 color = fixed4(0,0,0,0);

     		if (mask(i.uv))
     		{

	     		half3 dir = map(i.uv);
	      
	     		dir = mul(_FaceMatrix, half4(dir, 0));

	     		half2 angles = getCoordinate(dir);

	     		// Now we know angle, so normalize that to 0..1 (UV coord)
				half2 uv = (angles.xy + _FovExpansion) * i.fovRatio;

				if (isDirectionSync(dir))
				{
					if (isUVAcceptable(uv))
					{
						#if UNITY_VERSION >= 540
							color = tex2D (_MainTex, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST));
						#else
							color = tex2D (_MainTex, uv);
						#endif
					}
				}

			}

			return color;
    }

    ENDCG 
    
    Subshader {
    	Tags {"Queue" = "Overlay" }
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
