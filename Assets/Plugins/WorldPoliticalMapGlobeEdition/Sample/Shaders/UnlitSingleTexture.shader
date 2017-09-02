Shader "World Political Map/Unlit Single Texture" {

Properties {
	_MainTex ("Texture", 2D) = "white"  {}
}
SubShader {
	Tags { "Queue"="Geometry" "RenderType"="Opaque" }
	Offset 5,2
	
    CGPROGRAM
    #pragma surface surf Lambert nofog noforwardadd nolightmap noshadow noambient

    sampler2D _MainTex;
      
    struct Input {
        float2 uv_MainTex;
    };
                
    void surf (Input IN, inout SurfaceOutput o) {
        o.Emission = tex2D (_MainTex, IN.uv_MainTex).rgb;
    }
    ENDCG
   }
Fallback "Diffuse"
}
