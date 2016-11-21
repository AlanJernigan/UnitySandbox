Shader "DAP/Dimension Pro" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
    }
	SubShader {
		Tags {"Queue" = "Geometry-1" "RenderType" = "Transparent" "IgnoreProjector" = "False"  }
		LOD 200
		Lighting Off
		Ztest LEqual
		Zwrite On
		Cull Off
		
		Pass {  
		Offset -1,-1
			CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            #pragma target 3.0

	            #include "UnityCG.cginc"
	            
	            sampler2D _MainTex;

	            float4 vert(appdata_base v) : POSITION {
	                return mul (UNITY_MATRIX_MVP, v.vertex);
	            }

	            fixed4 frag(float4 sp:WPOS) : SV_Target {
	            	float2 wcoord = sp.xy/_ScreenParams.xy;
	            	fixed4 col = tex2D(_MainTex, wcoord);

	            	return col;
	            }

	            ENDCG
		}
		
		Pass {
       		ZWrite On
        	ColorMask 0
   		}
	}
}