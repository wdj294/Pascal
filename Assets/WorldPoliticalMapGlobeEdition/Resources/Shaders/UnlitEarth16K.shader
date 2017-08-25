// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "World Political Map/Unlit Texture 16K" {
 
Properties {
    _Color ("Color", Color) = (1,1,1)
	_TexTL ("Tex TL", 2D) = "white" {}
	_TexTR ("Tex TR", 2D) = "white" {}
	_TexBL ("Tex BL", 2D) = "white" {}
	_TexBR ("Tex BR", 2D) = "white" {}
}
 
SubShader {
  	Tags { "RenderType"="Opaque" }
		Lighting Off
		Offset 5,2
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma target 3.0
				
				#include "UnityCG.cginc"
				
				sampler2D _TexTL;
				sampler2D _TexTR;
				sampler2D _TexBL;
				sampler2D _TexBR;
				half3 _ColorBoost;
				
				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};
        
				v2f vert (appdata_base v) {
					v2f o;
					o.pos 				= UnityObjectToClipPos (v.vertex);
					o.uv				= v.texcoord;
					return o;
				 }
				
				half4 frag (v2f i) : SV_Target {
					half4 color;
					// compute Earth pixel color
					if (i.uv.x<0.5) {
						if (i.uv.y>0.5) {
							color = tex2Dlod (_TexTL, float4(i.uv.xy * 2.0.xx, 0, 0));
						} else {
							color = tex2Dlod (_TexBL, float4(i.uv.x * 2.0f, (i.uv.y-0.5) * 2.0, 0, 0));	
						}
					} else {
						if (i.uv.y>0.5) {
							color = tex2Dlod (_TexTR, float4((i.uv.x - 0.5) * 2.0f, i.uv.y * 2.0, 0, 0));
						} else {
							color = tex2Dlod (_TexBR, float4((i.uv.x - 0.5) * 2.0f, (i.uv.y-0.5) * 2.0, 0, 0));	
						}
					}
		  		   return color;
				}
			
			ENDCG
		}
	}
}