Shader "DAP/Portal No Cull" {

    SubShader {
        Tags {"Queue" = "Background+1" "RenderType" = "Transparent" "IgnoreProjector" = "True"  }
	    Lighting On
	    ZTest LEqual
	    ZWrite On
	    ColorMask 0
	   
    	CGPROGRAM
    	#pragma surface surf Lambert exclude_path:forward nolightmap
    	 
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
	    //Pass {
	    //}
    }
    
    SubShader {
        Tags {"Queue" = "Background+1" "RenderType" = "Transparent" "IgnoreProjector" = "True"  }
	    Lighting Off
	    ZTest LEqual
	    ZWrite On
	    ColorMask 0

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

	    Pass {
	    }
    }
}