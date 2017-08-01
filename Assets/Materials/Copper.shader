// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Copper"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Albedo("Albedo", Color) = (0,0,0,0)
		_EmissionIntensity("EmissionIntensity", Float) = 0
		_ShieldDistortion("ShieldDistortion", Range( 0 , 0.03)) = 0.01
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_ShieldSpeed("ShieldSpeed", Range( 0 , 100)) = 3
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			fixed filler;
		};

		uniform float4 _Albedo;
		uniform float4 _EmissionColor;
		uniform float _EmissionIntensity;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _ShieldSpeed;
		uniform float _ShieldDistortion;


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
			float simplePerlin3D11 = snoise( ( float4( v.normal , 0.0 ) + ( ( _Time * _ShieldSpeed ) / 5.0 ) ).xyz );
			float3 temp_cast_2 = ((( _ShieldDistortion * 0.0 ) + (simplePerlin3D11 - 0.0) * (_ShieldDistortion - ( _ShieldDistortion * 0.0 )) / (1.0 - 0.0))).xxx;
			v.vertex.xyz += temp_cast_2;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _Albedo.rgb;
			o.Emission = ( _EmissionColor * _EmissionIntensity ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=11001
111;83;861;649;2443.271;815.1746;2.423047;True;False
Node;AmplifyShaderEditor.CommentaryNode;9;-2503.45,-1295.344;Float;False;830.728;358.1541;Comment;3;19;13;12;Animation Speed;0;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-2444.591,-1047.29;Float;False;Property;_ShieldSpeed;ShieldSpeed;6;0;3;0;100;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;8;-1755.566,-572.8765;Float;False;1223.975;464.9008;Comment;8;21;20;18;17;16;14;11;10;Shield Distortion;0;0
Node;AmplifyShaderEditor.TimeNode;12;-2379.858,-1245.345;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;17;-1731.86,-283.0043;Float;False;Constant;_Float5;Float 5;7;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-2137.321,-1121.932;Float;False;2;2;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.NormalVertexDataNode;16;-1576.417,-522.8765;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;20;-1481.65,-335.6004;Float;False;2;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-1313.049,-493.0093;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT4;0.0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;15;-1470.787,-222.9755;Float;True;Property;_ShieldDistortion;ShieldDistortion;4;0;0.01;0;0.03;0;1;FLOAT
Node;AmplifyShaderEditor.WireNode;10;-1055.39,-230.5027;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;5;-289.5181,-392.7993;Float;False;Property;_EmissionColor;EmissionColor;5;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;6;-283.7973,-167.7811;Float;False;Property;_EmissionIntensity;EmissionIntensity;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NoiseGeneratorNode;11;-1167.256,-477.6716;Float;False;Simplex3D;1;0;FLOAT3;0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1147.287,-380.0736;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;1;-316.334,-60.80289;Float;False;Property;_Metallic;Metallic;0;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;21;-977.8862,-432.4345;Float;True;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.01;False;4;FLOAT;0.01;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;-325.7518,32.45255;Float;False;Property;_Smoothness;Smoothness;1;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;3;-144.591,-604.4686;Float;False;Property;_Albedo;Albedo;2;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-43.52362,-343.219;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;127,-153;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;Copper;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;0;12;0
WireConnection;19;1;13;0
WireConnection;20;0;19;0
WireConnection;20;1;17;0
WireConnection;14;0;16;0
WireConnection;14;1;20;0
WireConnection;10;0;15;0
WireConnection;11;0;14;0
WireConnection;18;0;15;0
WireConnection;21;0;11;0
WireConnection;21;3;18;0
WireConnection;21;4;10;0
WireConnection;7;0;5;0
WireConnection;7;1;6;0
WireConnection;0;0;3;0
WireConnection;0;2;7;0
WireConnection;0;3;1;0
WireConnection;0;4;2;0
WireConnection;0;11;21;0
ASEEND*/
//CHKSM=3584DBCE13E89E9A97E2013FC4B355A232C18013