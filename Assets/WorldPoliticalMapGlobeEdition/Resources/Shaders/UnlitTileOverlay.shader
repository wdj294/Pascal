// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "World Political Map/Unlit Tile Overlay" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture 1", 2D) = "white"
        _MainTex1 ("Texture 2", 2D) = "white"
        _MainTex2 ("Texture 3", 2D) = "white"
        _MainTex3 ("Texture 4", 2D) = "white"
    }

   	SubShader {
   		
       Tags {
	       "Queue"="Geometry" 
       }
       ZWrite On
       
       Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest

		sampler2D _MainTex;
		sampler2D _MainTex1;
		sampler2D _MainTex2;
		sampler2D _MainTex3;
//		float3 _SunLightDirection;

		struct appdata {
			float4 vertex : POSITION;
			float2 texcoord: TEXCOORD0;
			fixed4 color: COLOR;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv: TEXCOORD0;
//			float3 normal: TEXCOORD1;
			fixed4 color: COLOR;
		};
		
		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			o.color = v.color;
//			o.normal = normalize(o.pos - mul(UNITY_MATRIX_MVP, float4(0,0,0,1)));
			return o;
		}
		
		fixed4 frag(v2f i) : SV_Target {
			fixed4 p0 = tex2D(_MainTex, i.uv);
			fixed4 p1 = tex2D(_MainTex1, i.uv);
			fixed4 p2 = tex2D(_MainTex2, i.uv);
			fixed4 p3 = tex2D(_MainTex3, i.uv);
			fixed4 p = p0 * i.color.rrrr + p1 * i.color.gggg + p2 * i.color.bbbb + p3 * i.color.aaaa;
//			if (i.uv.x>0.99) return 1;
//			if (i.uv.y>0.99) return 1;
//			p.rgb *= saturate (dot(_SunLightDirection, i.normal));			
			return p;					
		}
			
		ENDCG
    }
  }  
}