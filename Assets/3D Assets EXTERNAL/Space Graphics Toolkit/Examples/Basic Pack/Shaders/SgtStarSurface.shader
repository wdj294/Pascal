Shader "Space Graphics Toolkit/SgtStarSurface"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Brightness("Brightness", Float) = 1.0
		_TimeScale("Time Scale", Float) = 1.0

		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimPower("Rim Power", Float) = 2.0

		[NoScaleOffset]_NoiseTex("Noise Tex", 3D) = "black" {}
		_NoiseStep("Noise Step", Vector) = (23, 29, 31)
		_NoiseColor("Noise Color", Color) = (1,1,1,1)
		_NoiseStrength("Noise Strength", Float) = 0.1
		_NoiseTile("Noise Tile", Float) = 1.0
		_NoiseBias("Noise Bias", Float) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface Surf NoLighting fullforwardshadows vertex:Vert

		#define NOISE_OCTAVES 8

		float3 _Color;
		float  _Brightness;
		float  _TimeScale;

		float3 _RimColor;
		float  _RimPower;

		sampler3D _NoiseTex;
		float4    _NoiseTex_TexelSize;
		float3    _NoiseStep;
		float3    _NoiseColor;
		float     _NoiseStrength;
		float     _NoiseTile;
		float     _NoiseBias;

		float Noise4D(float4 p)
		{
			float4 i     = floor(p);
			float4 f     = smoothstep(0.0, 1.0, frac(p));
			float3 pixel = i.xyz + f.xyz + i.w * _NoiseStep;
			float4 noise = tex3D(_NoiseTex, (pixel + 0.5) / _NoiseTex_TexelSize.w);

			return lerp(noise.x, noise.y, f.w);
		}

		struct Input
		{
			float3 worldNormal;
			float3 worldRefl;
			float3 localPos;
		};

		void Vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.localPos = v.vertex.xyz;
		}

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			return fixed4(s.Albedo, s.Alpha);
		}

		void Surf(Input i, inout SurfaceOutput o)
		{
			// Calc initial noise params
			float  str   = 1.0;
			float4 pos   = float4(i.localPos * _NoiseTile, _Time.x * _TimeScale);
			float  noise = 0.0;

			// Loop through each octave and contribute
			for (int j = 0; j < NOISE_OCTAVES; j++)
			{
				noise += (Noise4D(pos) - _NoiseBias) * str;
				str   /= 2;
				pos   *= 2;
			}

			// Normalize vectors before use
			i.worldNormal = normalize(i.worldNormal);
			i.worldRefl   = normalize(i.worldRefl);

			// Find dot between normal and reflection vectors
			float nfDot = abs(dot(i.worldNormal, i.worldRefl));

			// Make the color a rim gradient
			float rim = 1.0f - pow(1.0f - nfDot, _RimPower);

			o.Albedo = lerp(_RimColor, _Color, rim) * _Brightness - _NoiseColor * _NoiseStrength * noise;
		}
		ENDCG
	}
	FallBack "Diffuse"
}