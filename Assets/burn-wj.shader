// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "burn-wj"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_TextureSample9("Texture Sample 9", 2D) = "white" {}
		[Header(Refraction)]
		[Header(AnimatedFire_02)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_Float1("Float 1", Range( 0 , 2)) = 0
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_Normal("Normal", 2D) = "white" {}
		_Specular("Specular", Range( 0 , 1)) = 0
		_SoapAmount("Soap Amount", Range( 0 , 1)) = 0
		_IndexofRefraction("Index of Refraction", Range( -3 , 4)) = 1
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.8
		_Foam("Foam", 2D) = "white" {}
		_TextureSample4("Texture Sample 4", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ "RefractionGrab0" }
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float2 texcoord_0;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float2 texcoord_1;
			float4 screenPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _TextureSample9;
		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform float _Float1;
		uniform float _Specular;
		uniform sampler2D _TextureSample4;
		uniform sampler2D _Foam;
		uniform float _SoapAmount;
		uniform float _Smoothness;
		uniform float _Opacity;
		uniform sampler2D RefractionGrab0;
		uniform float _ChromaticAberration;
		uniform float _IndexofRefraction;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float3 temp_cast_0 = (( ( cos( ( ( 5.0 * v.vertex.y ) + _Time.y ) ) * 0.01 ) + ( sin( ( ( 5.0 * v.vertex.y ) + _Time.y ) ) * 0.01 ) )).xxx;
			v.vertex.xyz += temp_cast_0;
		}

		inline float4 Refraction( Input i, SurfaceOutputStandardSpecular o, float indexOfRefraction, float chomaticAberration ) {
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
			float4 redAlpha = tex2D( RefractionGrab0, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( RefractionGrab0, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( RefractionGrab0, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandardSpecular o, inout fixed4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
				color.rgb = color.rgb + Refraction( i, o, _IndexofRefraction, _ChromaticAberration ) * ( 1 - color.a );
				color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal );
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			o.Emission = ( ( tex2D( _TextureSample9, tex2D( _TextureSample1, uv_TextureSample1 ).rg ) * tex2D( _TextureSample1, (abs( i.texcoord_0+_Time.x * float2(-1,0 ))) ) ) * _Float1 ).xyz;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float4 temp_cast_2 = (_Specular).xxxx;
			float2 temp_cast_3 = (( ( tex2D( _Foam, (abs( i.texcoord_1+(_SinTime.x*0.5 + 0.5) * float2(1,1 ))) ).r + ( abs( (i.texcoord_1.x*2.0 + -1.0) ) * 0.5 ) ) + _Time.x )).xx;
			o.Specular = ( ( 1.0 - saturate( ( pow( dot( ase_worldNormal , worldViewDir ) , 2.0 ) - 0.1 ) ) ) * lerp( temp_cast_2 , saturate( tex2D( _TextureSample4, temp_cast_3 ) ) , _SoapAmount ) ).xyz;
			o.Smoothness = _Smoothness;
			o.Alpha = _Opacity;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
		CGPROGRAM
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma surface surf StandardSpecular keepalpha finalcolor:RefractionF fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				float4 texcoords01 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.texcoords01.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10001
0;94;1122;877;612.1845;291.2159;1.200098;True;False
Node;AmplifyShaderEditor.CommentaryNode;11;-2473.509,-743.2245;Float;False;2577.155;665.7997;;24;49;48;47;46;45;44;43;42;41;40;39;38;37;36;35;34;33;31;30;29;28;27;26;25;Chromatic Specular Reflection;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-2425.509,-679.2245;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SinTimeNode;26;-2169.509,-551.2246;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ScaleAndOffsetNode;27;-2009.509,-551.2246;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.ScaleAndOffsetNode;30;-2185.509,-343.2245;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;2.0;False;2;FLOAT;-1.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;33;-1785.509,-679.2245;Float;False;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.AbsOpNode;29;-1897.509,-343.2245;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;37;-1081.509,-535.2247;Float;False;World;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1673.509,-343.2245;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;38;-1129.509,-679.2245;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;12;-904.1702,514.8278;Float;False;1036;492;;10;32;21;20;19;18;17;16;15;14;13;Wobble;0;0
Node;AmplifyShaderEditor.SamplerNode;25;-1593.509,-695.2245;Float;True;Property;_Foam;Foam;14;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;39;-921.5093,-615.2245;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;32;-854.1702,564.8278;Float;False;Constant;_DeformFrequency;Deform Frequency;8;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;35;-1321.509,-311.2245;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;13;-838.1702,660.8287;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-1273.509,-567.2246;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;1.32,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-630.1702,628.8287;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;42;-793.5095,-615.2245;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT
Node;AmplifyShaderEditor.TimeNode;14;-854.1702,804.8289;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-1049.509,-327.2245;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;43;-633.5095,-615.2245;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.1;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-502.1716,756.8289;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;49;-841.5093,-487.2245;Float;True;Property;_TextureSample4;Texture Sample 4;15;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;48;-877.5093,-263.2244;Float;False;Property;_SoapAmount;Soap Amount;10;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;47;-841.5093,-167.2243;Float;False;Property;_Specular;Specular;9;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;46;-505.5094,-471.2245;Float;False;1;0;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SinOpNode;18;-342.1713,756.8289;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CosOpNode;17;-342.1713,660.8287;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;40;-489.5094,-615.2245;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-198.1716,756.8289;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.01;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-198.1716,660.8287;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.01;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-539.4244,209.2859;Float;True;Property;_TextureSample1;Texture Sample 1;12;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;45;-313.5095,-471.2245;Float;False;3;0;FLOAT;0,0,0,0;False;1;FLOAT4;0.0;False;2;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.OneMinusNode;41;-345.5094,-615.2245;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-22.17187,708.8289;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;23;-138.701,75.50996;Float;False;Property;_IndexofRefraction;Index of Refraction;11;0;1;-3;4;0;1;FLOAT
Node;AmplifyShaderEditor.FunctionNode;51;-95,334;Float;False;AnimatedFire_02;0;;7;1;0;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;22;-138.701,-20.49002;Float;False;Property;_Smoothness;Smoothness;13;0;0.8;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;24;-138.701,171.5098;Float;False;Property;_Opacity;Opacity;7;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-121.5097,-551.2246;Float;True;2;0;FLOAT;0.0;False;1;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;50;-4.850952,-293.7786;Float;True;Property;_Normal;Normal;8;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;262,15;Float;False;True;2;Float;ASEMaterialInspector;0;StandardSpecular;burn-wj;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Translucent;0.5;True;True;0;False;Opaque;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;0;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;27;0;26;1
WireConnection;30;0;36;1
WireConnection;33;0;36;0
WireConnection;33;1;27;0
WireConnection;29;0;30;0
WireConnection;28;0;29;0
WireConnection;25;1;33;0
WireConnection;39;0;38;0
WireConnection;39;1;37;0
WireConnection;31;0;25;1
WireConnection;31;1;28;0
WireConnection;15;0;32;0
WireConnection;15;1;13;2
WireConnection;42;0;39;0
WireConnection;34;0;31;0
WireConnection;34;1;35;1
WireConnection;43;0;42;0
WireConnection;16;0;15;0
WireConnection;16;1;14;2
WireConnection;49;1;34;0
WireConnection;46;0;49;0
WireConnection;18;0;16;0
WireConnection;17;0;16;0
WireConnection;40;0;43;0
WireConnection;20;0;18;0
WireConnection;19;0;17;0
WireConnection;45;0;47;0
WireConnection;45;1;46;0
WireConnection;45;2;48;0
WireConnection;41;0;40;0
WireConnection;21;0;19;0
WireConnection;21;1;20;0
WireConnection;51;0;6;0
WireConnection;44;0;41;0
WireConnection;44;1;45;0
WireConnection;0;1;50;0
WireConnection;0;2;51;0
WireConnection;0;3;44;0
WireConnection;0;4;22;0
WireConnection;0;8;23;0
WireConnection;0;9;24;0
WireConnection;0;11;21;0
ASEEND*/
//CHKSM=FB2221D24E6DA5445B30169BC852A9A94868A5BE