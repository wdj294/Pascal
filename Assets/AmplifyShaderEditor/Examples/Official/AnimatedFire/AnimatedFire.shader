// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/AnimatedFire"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Albedo("Albedo", 2D) = "white" {}
		_Normals("Normals", 2D) = "bump" {}
		_Mask("Mask", 2D) = "white" {}
		_Specular("Specular", 2D) = "white" {}
		_TileableFire("TileableFire", 2D) = "white" {}
		_FireIntensity("FireIntensity", Range( 0 , 2)) = 0
		_Smoothness("Smoothness", Float) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
		};

		uniform sampler2D _Normals;
		uniform sampler2D _Albedo;
		uniform sampler2D _Mask;
		uniform sampler2D _TileableFire;
		uniform float _FireIntensity;
		uniform sampler2D _Specular;
		uniform float _Smoothness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			o.Normal = UnpackNormal( tex2D( _Normals, i.texcoord_0 ) );
			o.Albedo = tex2D( _Albedo, i.texcoord_0 ).xyz;
			o.Emission = ( ( tex2D( _Mask, i.texcoord_0 ) * tex2D( _TileableFire, (abs( i.texcoord_0+_Time.x * float2(-1,0 ))) ) ) * ( _FireIntensity * ( _SinTime.w + 1.5 ) ) ).xyz;
			o.Specular = tex2D( _Specular, i.texcoord_0 ).xyz;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10001
0;94;844;877;1382.736;1057.307;2.325581;False;False
Node;AmplifyShaderEditor.TimeNode;5;-990.7925,120.6489;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1221.05,-198.2688;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;4;-719.0791,60.86958;Float;False;-1;0;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SinTimeNode;9;-798.4472,-270.8411;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;-743.4472,-400.8412;Float;False;Property;_FireIntensity;FireIntensity;5;0;0;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-534.8409,-127.5401;Float;True;Property;_Mask;Mask;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1.0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-596.4472,-254.8411;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.5;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-537.9299,230.4654;Float;True;Property;_TileableFire;TileableFire;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1.0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-224.8717,65.39268;Float;False;2;0;FLOAT4;0.0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-428.447,-361.8412;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;12;-545.3217,-849.4019;Float;True;Property;_Albedo;Albedo;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1.0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;14;-545.7766,-645.4978;Float;True;Property;_Normals;Normals;1;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1.0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-101.1933,-139.5095;Float;False;2;0;FLOAT4;0.0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;15;-165.7008,595.32;Float;False;Property;_Smoothness;Smoothness;6;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;13;-712.4949,612.913;Float;True;Property;_Specular;Specular;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1.0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;171,-60;Float;False;True;2;Float;ASEMaterialInspector;0;StandardSpecular;ASESampleShaders/AnimatedFire;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;6;0
WireConnection;4;1;5;1
WireConnection;2;1;6;0
WireConnection;11;0;9;4
WireConnection;1;1;4;0
WireConnection;3;0;2;0
WireConnection;3;1;1;0
WireConnection;10;0;7;0
WireConnection;10;1;11;0
WireConnection;12;1;6;0
WireConnection;14;1;6;0
WireConnection;8;0;3;0
WireConnection;8;1;10;0
WireConnection;13;1;6;0
WireConnection;0;0;12;0
WireConnection;0;1;14;0
WireConnection;0;2;8;0
WireConnection;0;3;13;0
WireConnection;0;4;15;0
ASEEND*/
//CHKSM=57DACBAC9BA0EEF5C8CDB24C09ECDBADDD1DAD3A