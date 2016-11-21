Shader "Custom/PortalFx" {
    Properties {
        _Offset ("Time", Range (0, 1)) = 0.0
        _Offset2 ("Time", Range (0, 1)) = 0.0
        _Color ("Tint (RGB)", Color) = (1,1,1,1)
        _Color2 ("Tint (RGB)", Color) = (1,1,1,1)
        _SurfaceTex ("Texture (RGB)", 2D) = "white" {}
        _RampTex ("Facing Ratio Ramp (RGB)", 2D) = "white" {}
        _Detail ("Detail", 2D) = "gray" {}
    }
    
    SubShader {
        ZWrite Off
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha One 
        Cull Off

        CGPROGRAM
	      #pragma surface surf Lambert
	      struct Input {
	          float4 screenPos;
	      };
	      
	      uniform float _Offset2;
	      fixed4 _Color2;
	      sampler2D _Detail;

	      void surf (Input IN, inout SurfaceOutput o) {
	          float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
	          screenUV = screenUV*float2(8,6) + _Offset2;
	          o.Albedo = tex2D (_Detail, screenUV) * _Color2;
	          o.Alpha = _Color2.a;
	      }
	      ENDCG

        Pass {  
            CGPROGRAM 
			// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
			#pragma exclude_renderers gles
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_fog_exp2
            #include "UnityCG.cginc" 
 
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            uniform float _Offset;
            
            v2f vert (appdata_base v) {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                
                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                v.texcoord.x = v.texcoord.x;
                v.texcoord.y = v.texcoord.y + _Offset;
                o.uv = TRANSFORM_UV (1);
                o.uv2 = float2( abs (dot (viewDir, v.normal)), 0.5);
                o.normal = v.normal;
                return o;
            }
            
            uniform float4 _Color;
            uniform sampler2D _RampTex : register(s0);
            uniform sampler2D _SurfaceTex : register(s1);
                
            half4 frag (v2f f) : COLOR 
            {
                f.normal = normalize (f.normal);
                
                half4 ramp = tex2D (_RampTex, f.uv2) * _Color.a;
                half4 thisTex = tex2D (_SurfaceTex, f.uv) * ramp * _Color * 2;
                
                return half4 (thisTex.r, thisTex.g, thisTex.b, ramp.r);
            }
            ENDCG 

            
 
            SetTexture [_RampTex] {combine texture}
            SetTexture [_SurfaceTex] {combine texture}
        }
        

    }
    Fallback "Transparent/VertexLit"
}