// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Matter"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_Normal("Normal", 2D) = "bump" {}
		_Emission("Emission", Range( 0 , 1)) = 0
		_NormalScale("Normal Scale", Range( 0 , 10)) = 0
		_AlbedoColor("AlbedoColor", Color) = (0,0,0,0)
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NormalScale;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _AlbedoColor;
		uniform float4 _EmissionColor;
		uniform float _Emission;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,_NormalScale );
			o.Albedo = _AlbedoColor.rgb;
			o.Emission = ( _EmissionColor * _Emission ).rgb;
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
111;83;861;649;1109.078;834.2742;1.804688;True;False
Node;AmplifyShaderEditor.ColorNode;221;-446.5401,-615.6431;Float;False;Property;_EmissionColor;EmissionColor;10;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;213;-488.2373,-310.2353;Float;False;Property;_Emission;Emission;6;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;138;-2130.308,197.1151;Float;False;1223.975;464.9008;Comment;9;148;147;146;145;144;143;142;141;140;Shield Distortion;0;0
Node;AmplifyShaderEditor.RangedFloatNode;211;-584.2057,-962.7593;Float;False;Property;_NormalScale;Normal Scale;8;0;0;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;214;-2878.192,-525.3531;Float;False;830.728;358.1541;Comment;3;217;216;215;Animation Speed;0;0
Node;AmplifyShaderEditor.WireNode;146;-1430.132,539.4889;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NoiseGeneratorNode;145;-1541.998,292.32;Float;False;Simplex3D;1;0;FLOAT3;0,0;False;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;215;-2754.6,-475.3542;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;216;-2819.333,-277.299;Float;False;Property;_ShieldSpeed;ShieldSpeed;7;0;3;0;100;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;144;-1687.791,276.9823;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT4;0.0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;227;-50.07164,-203.7891;Float;False;Property;_AlbedoColor;AlbedoColor;9;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;143;-1845.529,547.0161;Float;True;Property;_ShieldDistortion;Shield Distortion;4;0;0.01;0;0.03;0;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;142;-1951.159,247.1151;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;140;-2041.18,503.9486;Float;False;Constant;_Float7;Float 7;7;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;187;-593.9132,112.3442;Float;False;Property;_IndexofRefraction;Index of Refraction;1;0;0.5;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;207;-200.5496,-947.2187;Float;True;Property;_Normal;Normal;5;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-1522.029,389.918;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;188;-590.0386,-2.942192;Float;False;Property;_Smoothness;Smoothness;3;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;222;35.32178,-339.474;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;189;-590.1489,-114.4488;Float;False;Property;_Metallic;Metallic;2;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;217;-2512.063,-351.9402;Float;False;2;2;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleDivideOpNode;141;-1856.392,434.3912;Float;False;2;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;186;-590.8519,210.6212;Float;False;Property;_Opacity;Opacity;0;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;148;-1352.628,337.5571;Float;True;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.01;False;4;FLOAT;0.01;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;337.409,185.7051;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;Matter;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;146;0;143;0
WireConnection;145;0;144;0
WireConnection;144;0;142;0
WireConnection;144;1;141;0
WireConnection;207;5;211;0
WireConnection;147;0;143;0
WireConnection;222;0;221;0
WireConnection;222;1;213;0
WireConnection;217;0;215;0
WireConnection;217;1;216;0
WireConnection;141;0;217;0
WireConnection;141;1;140;0
WireConnection;148;0;145;0
WireConnection;148;3;147;0
WireConnection;148;4;146;0
WireConnection;0;0;227;0
WireConnection;0;1;207;0
WireConnection;0;2;222;0
WireConnection;0;3;189;0
WireConnection;0;4;188;0
ASEEND*/
//CHKSM=AEDDF92B4F24239238F45B1F557FB9DDEC3250A9