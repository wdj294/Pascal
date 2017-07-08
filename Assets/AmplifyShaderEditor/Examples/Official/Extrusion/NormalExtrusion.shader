// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/NormalExtrusion"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_ExtrusionPoint("ExtrusionPoint", Float) = 0
		_ExtrusionAmount("Extrusion Amount", Range( -1 , 20)) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _ExtrusionPoint;
		uniform float _ExtrusionAmount;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * max( ( sin( ( ( v.vertex.y + _Time.x ) / _ExtrusionPoint ) ) / _ExtrusionAmount ) , 0.0 ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = tex2D( _Albedo, uv_Albedo ).xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10001
0;94;844;877;1373.211;1073.206;2.763442;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;18;-980.0377,98.29941;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TimeNode;25;-981.2358,327.3;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;21;-767.0372,392.2995;Float;False;Property;_ExtrusionPoint;ExtrusionPoint;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-776.0372,190.0994;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;19;-624.0369,223.2994;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;20;-479.9368,132.2993;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-550.2001,349.7331;Float;False;Property;_ExtrusionAmount;Extrusion Amount;1;0;0.5;-1;20;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;24;-299.237,166.3993;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;10.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMaxOp;26;-140.5377,221.3996;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;2;-237.2496,-72.3555;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-349.5,-300;Float;True;Property;_Albedo;Albedo;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;3.5,1;Float;False;2;0;FLOAT3;0.0,0,0;False;1;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;240.5624,-334.5286;Float;False;True;6;Float;ASEMaterialInspector;0;Standard;ASESampleShaders/NormalExtrusion;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0;False;2;FLOAT3;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0.0;False;7;FLOAT3;0.0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT3;0.0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;18;2
WireConnection;22;1;25;1
WireConnection;19;0;22;0
WireConnection;19;1;21;0
WireConnection;20;0;19;0
WireConnection;24;0;20;0
WireConnection;24;1;3;0
WireConnection;26;0;24;0
WireConnection;4;0;2;0
WireConnection;4;1;26;0
WireConnection;0;0;1;0
WireConnection;0;11;4;0
ASEEND*/
//CHKSM=D1DE41985674D0EC26EFA738EAF789DCB884AF46