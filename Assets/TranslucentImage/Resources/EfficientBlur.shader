// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/EfficientBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		half4 _MainTex_TexelSize;
		half4 _MainTex_ST;

		uniform fixed size;

		struct v2f
		{
			half4 vertex : SV_POSITION;
			half2 uv1 : TEXCOORD1;
			half2 uv2 : TEXCOORD2;
			half2 uv3 : TEXCOORD3;
			half2 uv4 : TEXCOORD4;
		};

		/*struct appdata
		{
			half4 vertex : POSITION;
			half2 texcoord: TEXCOORD0;
		}*/

		v2f vert(appdata_img v)
		{
			v2f o;
			half4 offset = half2(0.5h, -0.5h).xxyy * _MainTex_TexelSize.xyxy * size;

			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv1 = UnityStereoScreenSpaceUVAdjust(v.texcoord + offset.xy, _MainTex_ST);
			o.uv2 = UnityStereoScreenSpaceUVAdjust(v.texcoord + offset.zy, _MainTex_ST);
			o.uv3 = UnityStereoScreenSpaceUVAdjust(v.texcoord + offset.xw, _MainTex_ST);
			o.uv4 = UnityStereoScreenSpaceUVAdjust(v.texcoord + offset.zw, _MainTex_ST);

			return o;
		}

		half4 frag(v2f i) : SV_Target
		{
			half4 o = tex2D(_MainTex, i.uv1);
			o += tex2D(_MainTex, i.uv2);
			o += tex2D(_MainTex, i.uv3);
			o += tex2D(_MainTex, i.uv4);
			o /= 4.0;
			return o;
		}
	ENDCG

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always Blend Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag		
			ENDCG
		}
	}

	FallBack Off
}
