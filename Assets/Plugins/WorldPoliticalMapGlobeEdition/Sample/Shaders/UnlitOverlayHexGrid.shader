// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "World Political Map/Unlit Overlay Hex Grid" {
 
Properties
    {
       _MainTex ("Texture", 2D) = ""
    }
 
SubShader
    {
        Tags {
         "Queue" = "Transparent" 
         "RenderType"="Transparent"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass {
    	CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag				
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		
		struct AppData {
			float4 vertex   : POSITION;
			float2 texcoord : TEXCOORD0;
			float3 normal   : NORMAL;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv  : TEXCOORD0;
			float3 uvg : TEXCOORD1;
		};
		
		float hex(float3 p) {
        	p.x *= 0.57735*2.0;
        	p.y += fmod(floor(p.x), 2.0)*0.5;
        	p.xy = abs((frac(p) - 0.5));
        	float r = abs(max(p.x*1.5 + p.y, p.y*2.0) - 1.0);
        	return saturate(p.z / r);
		}

		v2f vert(AppData v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			const float2 scale = float2(4.0, 1.0) * 100.0;
			float aniso = 1.0 + abs(dot(WorldSpaceViewDir(v.vertex), v.normal)) * 0.5; 
			const float width = 0.05;
			float t1 = sin( (0.5 - abs(v.texcoord.y - 0.5)) * 3.1415927 * 0.5 );
			v.texcoord.x = (v.texcoord.x - 0.5) * t1;
			float2 t2 = float2(0.0, sin(0.5 - (abs(v.texcoord.x - 0.5)) * 3.1415927 * 0.5 ));
//			v.texcoord.y = (v.texcoord.y - 0.5) * t2;
			o.uvg = float3(v.texcoord * scale, width * aniso * aniso);
			return o;
		}
		
		float4 frag(v2f i) : SV_Target {
			float4 pixel = tex2D(_MainTex, i.uv);
			float tt = saturate(0.5 - abs(i.uv.y - 0.5));
			if (tt) {
				pixel += hex(i.uvg);
			}
			return pixel;
		}
			
		ENDCG
    }    
        
    }
}