Shader "DAP/Dimension" {

    SubShader {
        Tags {"Queue" = "Background-1" "RenderType" = "Transparent" "IgnoreProjector" = "False"  }
	    Lighting Off
	    ZTest LEqual
	    ZWrite On
	    ColorMask 0
	    Cull Off
	   	
	   	Pass {
	    }
	    
    	CGPROGRAM
    	#pragma surface surf Lambert exclude_path:forward nolightmap
    	 
    	struct Input {
    		float4 color : COLOR;
    	};
    	void surf(Input IN, inout SurfaceOutput o) {
    		o.Albedo = 0;
    		o.Normal = float3(0.0,0.0,1.0);
    		o.Emission = 0;
    		o.Specular = 0;
    		o.Gloss = 0;
    		o.Alpha = 0;
    		//o.Custom = 0.0;
    		
    	} 
    	ENDCG

    }
    
    SubShader {
        Tags {"Queue" = "Background-1" "RenderType" = "Transparent" "IgnoreProjector" = "False"  }
	    Lighting Off
	    ZTest LEqual
	    ZWrite On
	    ColorMask 0
	    Cull Front
	    
		Pass {
	  	}
	    
		CGPROGRAM
    	#pragma surface surf Lambert exclude_path:prepass nolightmap
    	struct Input {
    		float4 color : COLOR;
    	};
    	
    	void surf(Input IN, inout SurfaceOutput o) {
    		o.Albedo = 0;
    		o.Normal = 0;
    		o.Emission = 0;
    		o.Specular = 0;
    		o.Gloss = 0;
    		o.Alpha = 0;
    	} 
    	ENDCG

	    //
    }
}