// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Matter"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_ShieldDistortion("ShieldDistortion", Range( 0 , 0.03)) = 0.01
		_ShieldSpeed("ShieldSpeed", Range( 0 , 100)) = 3
		_Normal("Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 5)) = 0
		_RimPower("RimPower", Range( -10 , 10)) = 0
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_EmissionIntensity("EmissionIntensity", Range( 0 , 1)) = 0
		_FlowSpeed("FlowSpeed", Range( 0 , 10000)) = 0
		_Refraction("Refraction", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma surface surf Standard alpha:fade keepalpha finalcolor:RefractionF noshadow exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float3 viewDir;
			INTERNAL_DATA
			float4 screenPos;
			float3 worldPos;
		};

		uniform fixed _NormalScale;
		uniform sampler2D _Normal;
		uniform fixed _FlowSpeed;
		uniform fixed _EmissionIntensity;
		uniform fixed _RimPower;
		uniform fixed4 _EmissionColor;
		uniform fixed _Metallic;
		uniform fixed _Smoothness;
		uniform fixed _Opacity;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;
		uniform fixed _Refraction;
		uniform fixed _ShieldSpeed;
		uniform fixed _ShieldDistortion;


		float3 mod289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float3 ase_vertexNormal = v.normal.xyz;
			float simplePerlin3D11 = snoise( ( fixed4( ase_vertexNormal , 0.0 ) + ( ( _Time * _ShieldSpeed ) / 5.0 ) ).xyz );
			fixed3 temp_cast_2 = ((( _ShieldDistortion * 0.0 ) + (simplePerlin3D11 - 0.0) * (_ShieldDistortion - ( _ShieldDistortion * 0.0 )) / (1.0 - 0.0))).xxx;
			v.vertex.xyz += temp_cast_2;
		}

		inline float4 Refraction( Input i, SurfaceOutputStandard o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );
			float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout fixed4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
				color.rgb = color.rgb + Refraction( i, o, _Refraction, _ChromaticAberration ) * ( 1 - color.a );
				color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			fixed3 tex2DNode24 = UnpackScaleNormal( tex2D( _Normal, (abs( i.texcoord_0+( _Time.x * _FlowSpeed ) * float2(-1,0 ))) ) ,_NormalScale );
			o.Normal = tex2DNode24;
			float3 normalizeResult36 = normalize( i.viewDir );
			float dotResult35 = dot( tex2DNode24 , normalizeResult36 );
			o.Emission = ( _EmissionIntensity * ( pow( ( 1.0 - saturate( dotResult35 ) ) , _RimPower ) * _EmissionColor ) ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = _Opacity;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
0;94;841;260;2556.144;1783.871;2.187581;True;False
Node;AmplifyShaderEditor.RangedFloatNode;63;-2146.905,-832.2755;Float;False;Property;_FlowSpeed;FlowSpeed;13;0;0;0;10000;0;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;60;-2235.866,-1068.108;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;61;-2145.867,-1271.108;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-1954.906,-989.2755;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;25;-1354.45,-1043.281;Float;False;Property;_NormalScale;Normal Scale;8;0;0;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;37;-1859.063,-312.4326;Float;True;Tangent;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;62;-1863.867,-1140.108;Float;False;-1;0;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;9;-2976.888,712.9103;Float;False;830.728;358.1541;Comment;3;19;13;12;Animation Speed;1,1,1,1;0;0
Node;AmplifyShaderEditor.NormalizeNode;36;-1594.798,-278.0634;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;24;-961.5266,-1170.208;Float;True;Property;_Normal;Normal;7;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;35;-1295.971,-693.8234;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-2918.029,960.9643;Float;False;Property;_ShieldSpeed;ShieldSpeed;6;0;3;0;100;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;8;-1802.767,697.2465;Float;False;1223.975;464.9008;Comment;8;21;20;18;17;16;14;11;10;Shield Distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.TimeNode;12;-2853.296,762.9093;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-2610.759,886.3223;Float;False;2;2;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SaturateNode;42;-1065.121,-375.9555;Float;True;1;0;FLOAT;1.23;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;17;-1779.061,987.1189;Float;False;Constant;_Float5;Float 5;7;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;41;-1015.71,-562.8107;Float;False;Property;_RimPower;RimPower;9;0;0;-10;10;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;43;-896.1175,-333.6561;Float;True;1;0;FLOAT;0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;20;-1528.851,934.5226;Float;False;2;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.NormalVertexDataNode;16;-1623.618,747.2465;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;15;-1517.988,1047.147;Float;True;Property;_ShieldDistortion;ShieldDistortion;5;0;0.01;0;0.03;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-1360.25,777.1136;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT4;0.0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.PowerNode;39;-701.262,-407.0087;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;40;-837.9827,196.8831;Float;False;Property;_EmissionColor;EmissionColor;10;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1194.488,890.0493;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-460.9207,-295.0558;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;53;-358.7074,-411.9773;Float;False;Property;_EmissionIntensity;EmissionIntensity;12;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.WireNode;10;-1102.591,1039.62;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NoiseGeneratorNode;11;-1214.457,792.4513;Float;False;Simplex3D;1;0;FLOAT3;0,0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;21;-1025.087,837.6885;Float;True;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.01;False;4;FLOAT;0.01;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;52;-265.0587,149.9159;Float;False;Property;_Refraction;Refraction;14;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;1;-316.334,-60.80289;Float;False;Property;_Metallic;Metallic;2;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;51;-348.989,261.6373;Float;False;Property;_Opacity;Opacity;11;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;3;-56.12281,-602.6631;Float;False;Property;_Albedo;Albedo;4;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;-325.7518,32.45255;Float;False;Property;_Smoothness;Smoothness;3;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-145.1223,-288.7551;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;157,-159;Fixed;False;True;2;Fixed;ASEMaterialInspector;0;0;Standard;Matter;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;3;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;0;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;64;0;60;1
WireConnection;64;1;63;0
WireConnection;62;0;61;0
WireConnection;62;1;64;0
WireConnection;36;0;37;0
WireConnection;24;1;62;0
WireConnection;24;5;25;0
WireConnection;35;0;24;0
WireConnection;35;1;36;0
WireConnection;19;0;12;0
WireConnection;19;1;13;0
WireConnection;42;0;35;0
WireConnection;43;0;42;0
WireConnection;20;0;19;0
WireConnection;20;1;17;0
WireConnection;14;0;16;0
WireConnection;14;1;20;0
WireConnection;39;0;43;0
WireConnection;39;1;41;0
WireConnection;18;0;15;0
WireConnection;38;0;39;0
WireConnection;38;1;40;0
WireConnection;10;0;15;0
WireConnection;11;0;14;0
WireConnection;21;0;11;0
WireConnection;21;3;18;0
WireConnection;21;4;10;0
WireConnection;54;0;53;0
WireConnection;54;1;38;0
WireConnection;0;1;24;0
WireConnection;0;2;54;0
WireConnection;0;3;1;0
WireConnection;0;4;2;0
WireConnection;0;8;52;0
WireConnection;0;9;51;0
WireConnection;0;11;21;0
ASEEND*/
//CHKSM=36C99B440E3665674061ADED233A3A51D9AE12DC