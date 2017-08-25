// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "World Political Map/Unlit Single Color Cursor" {
 
Properties {
    _Color ("Color", Color) = (1,1,1)
}
 
SubShader {
    Tags {
        "Queue"="Transparent"
        "RenderType"="Transparent"
    }
 	Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag				
		
		#include "UnityCG.cginc"
		
		fixed4 _Color;

		struct AppData {
			float4 vertex : POSITION;
			float4 scrPos : TEXCOORD0;
		};
		
		void vert(inout AppData v) {
			v.vertex = UnityObjectToClipPos(v.vertex);
			#if UNITY_REVERSED_Z
			v.vertex.z += 0.0001;
			#else
			v.vertex.z -= 0.0001;
			#endif
			v.scrPos = ComputeScreenPos(v.vertex);
		}
		
		fixed4 frag(AppData i) : SV_Target {
			float2 wcoord = (i.scrPos.xy/i.scrPos.w);
			float grad = abs(ddy(wcoord.y) / ddx(wcoord.x));
			float xm = lerp (wcoord.x, wcoord.y, grad > 0.5);
			xm = fmod( ( (int)(xm * 1000.0)) / 4,2);
			clip(xm - 0.5);
			return _Color;					
		}
			
		ENDCG
    }

}
}
 
