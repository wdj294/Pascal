// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Used by cities material

Shader "World Political Map/Unlit Cities" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white"
    }

   	SubShader {
   		
       Tags {
	       "Queue"="Geometry+6" // Draw over highlight
       }
       ZWrite Off
       Blend SrcAlpha OneMinusSrcAlpha
       Offset -60,-60
       
       Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest

		sampler2D _MainTex;
		fixed4 _Color;

		struct appdata {
			float4 vertex : POSITION;
			half2 texcoord: TEXCOORD0;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv: TEXCOORD0;
		};
		
		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}
		
		fixed4 frag(v2f i) : SV_Target {
			fixed4 p = tex2D(_MainTex, i.uv);
			return p * _Color;					
		}
			
		ENDCG
    }
  }  
}