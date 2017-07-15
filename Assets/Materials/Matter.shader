// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Matter"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Color("Color", Color) = (0,0,0,0)
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_Albedo("Albedo", 2D) = "white" {}
		_ShieldPatternColor("Shield Pattern Color", Color) = (0.2470588,0.7764706,0.9098039,1)
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		_IndexofRefraction("Index of Refraction", Range( -1 , 1)) = 0.5
		_Metallic("Metallic", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_ShieldPattern("Shield Pattern", 2D) = "white" {}
		[IntRange]_ShieldPatternSize("Shield Pattern Size", Range( 1 , 20)) = 5
		_ShieldPatternPower("Shield Pattern Power", Range( 0 , 100)) = 5
		_ShieldRimPower("Shield Rim Power", Range( 0 , 10)) = 7
		_ShieldAnimSpeed("Shield Anim Speed", Range( -10 , 10)) = 3
		_ShieldPatternWaves("Shield Pattern Waves", 2D) = "white" {}
		_ShieldDistortion("Shield Distortion", Range( 0 , 0.03)) = 0.01
		_IntersectIntensity("Intersect Intensity", Range( 0 , 1)) = 0.2
		_IntersectColor("Intersect Color", Color) = (0.03137255,0.2588235,0.3176471,1)
		_HitPosition("Hit Position", Vector) = (0,0,0,0)
		_HitTime("Hit Time", Float) = 0
		_HitColor("Hit Color", Color) = (1,1,1,1)
		_HitSize("Hit Size", Float) = 0.2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ "RefractionGrab0" }
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
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
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 texcoord_0;
			float2 texcoord_1;
			float4 screenPos;
		};

		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _IntersectColor;
		uniform float _ShieldRimPower;
		uniform sampler2D _ShieldPattern;
		uniform float _ShieldPatternSize;
		uniform float _ShieldAnimSpeed;
		uniform sampler2D _ShieldPatternWaves;
		uniform float _HitTime;
		uniform float3 _HitPosition;
		uniform float _HitSize;
		uniform float4 _ShieldPatternColor;
		uniform float4 _HitColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _IntersectIntensity;
		uniform float _ShieldPatternPower;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Opacity;
		uniform sampler2D RefractionGrab0;
		uniform float _ChromaticAberration;
		uniform float _IndexofRefraction;
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
			float2 appendResult106 = float2( _ShieldPatternSize , _ShieldPatternSize );
			float4 ShieldSpeed = ( _Time * _ShieldAnimSpeed );
			float2 appendResult104 = float2( 1 , ShieldSpeed.x );
			o.texcoord_0.xy = v.texcoord.xy * appendResult106 + appendResult104;
			float2 appendResult77 = float2( 1 , ( 1.0 - ( ShieldSpeed / 5.0 ) ).x );
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + appendResult77;
			float simplePerlin3D145 = snoise( ( float4( v.normal , 0.0 ) + ( ShieldSpeed / 5.0 ) ).xyz );
			float VertexOffset = (( _ShieldDistortion * 0.0 ) + (simplePerlin3D145 - 0.0) * (_ShieldDistortion - ( _ShieldDistortion * 0.0 )) / (1.0 - 0.0));
			float3 temp_cast_4 = (VertexOffset).xxx;
			v.vertex.xyz += temp_cast_4;
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
			float4 redAlpha = tex2D( RefractionGrab0, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( RefractionGrab0, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( RefractionGrab0, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout fixed4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
				color.rgb = color.rgb + Refraction( i, o, _IndexofRefraction, _ChromaticAberration ) * ( 1 - color.a );
				color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 Albedo = ( _Color * tex2D( _Albedo, uv_Albedo ) );
			o.Albedo = Albedo.rgb;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float ShieldRimPower = _ShieldRimPower;
			float fresnelFinalVal135 = (0.0 + 1.0*pow( 1.0 - dot( ase_worldNormal, worldViewDir ) , (10.0 + (ShieldRimPower - 0.0) * (0.0 - 10.0) / (10.0 - 0.0))));
			float ShieldRim = fresnelFinalVal135;
			float4 temp_cast_1 = (ShieldRim).xxxx;
			float4 ShieldPattern = tex2D( _ShieldPattern, i.texcoord_0 );
			float4 waves = tex2D( _ShieldPatternWaves, i.texcoord_1 );
			float4 ase_vertexPos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_155_0 = distance( ase_vertexPos.xyz , _HitPosition );
			float HitSize = _HitSize;
			float4 ShieldPatternColor = _ShieldPatternColor;
			float4 HitColor = _HitColor;
			float4 hit = (( _HitTime > 0.0 ) ? (( temp_output_155_0 < HitSize ) ? lerp( ShieldPatternColor , ( HitColor * ( HitSize / temp_output_155_0 ) ) , (0.0 + (_HitTime - 0.0) * (1.0 - 0.0) / (100.0 - 0.0)) ) :  ShieldPatternColor ) :  ShieldPatternColor );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float screenDepth121 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth121 = abs( ( screenDepth121 - LinearEyeDepth( ase_screenPos.z/ ase_screenPos.w ) ) / _IntersectIntensity );
			float ShieldPower = _ShieldPatternPower;
			float4 Emission = ( lerp( _IntersectColor , ( ( ( temp_cast_1 + ShieldPattern ) * waves ) * ( hit * ShieldPatternColor ) ) , clamp( distanceDepth121 , 0.0 , 1.0 ) ) * ShieldPower );
			o.Emission = Emission.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = _Opacity;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
		CGPROGRAM
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma surface surf Standard alpha:fade keepalpha finalcolor:RefractionF fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

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
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
0;94;1080;930;2697.367;2048.985;3.099999;True;False
Node;AmplifyShaderEditor.CommentaryNode;81;-3923.027,-1047.735;Float;False;830.728;358.1541;Comment;4;84;83;82;85;Animation Speed;0;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-3873.027,-804.5805;Float;False;Property;_ShieldAnimSpeed;Shield Anim Speed;13;0;3;-10;10;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;151;-4138.382,254.1391;Float;False;1858.993;1001.87;Comment;22;167;152;170;173;172;171;169;168;166;165;164;163;162;161;160;159;158;157;156;155;154;153;Impact Effect;0;0
Node;AmplifyShaderEditor.TimeNode;83;-3799.435,-997.7358;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;70;-3815.919,-435.9767;Float;False;1608.543;477.595;Comment;10;80;79;74;77;78;76;72;73;71;75;Shield Wave Effect;0;0
Node;AmplifyShaderEditor.Vector3Node;153;-4079.775,850.0393;Float;False;Property;_HitPosition;Hit Position;18;0;0,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;152;-4088.382,665.7394;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-3556.898,-874.3218;Float;False;2;0;FLOAT4;0.0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;154;-4008.477,304.1391;Float;False;Property;_HitSize;Hit Size;21;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;-3344.323,-884.8804;Float;False;ShieldSpeed;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.DistanceOpNode;155;-3848.177,749.3394;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;72;-3727.905,-91.82573;Float;False;Constant;_Float1;Float 1;7;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;71;-3754.77,-178.4229;Float;False;85;0;1;FLOAT4
Node;AmplifyShaderEditor.CommentaryNode;99;-2906.848,-1306.561;Float;False;1504.24;684.7161;Comment;12;111;109;108;107;106;105;104;101;100;110;103;102;Shield Main Pattern;0;0
Node;AmplifyShaderEditor.WireNode;159;-3671.28,989.1074;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;156;-3628.189,918.6063;Float;False;157;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;157;-3795.277,308.0392;Float;False;HitSize;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;131;-2161.801,-436.2346;Float;False;1030.896;385.0003;Comment;6;135;134;133;132;137;136;Shield RIM;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;73;-3546.938,-164.5674;Float;False;2;0;FLOAT4;0.0;False;1;FLOAT;0,5,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;158;-4050.165,411.0398;Float;False;Property;_HitColor;Hit Color;20;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;163;-3416.392,1033.708;Float;False;162;0;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;102;-2856.848,-1057.004;Float;False;Property;_ShieldPatternSize;Shield Pattern Size;10;1;[IntRange];5;1;20;0;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;100;-2769.707,-938.861;Float;False;Constant;_Vector7;Vector 7;6;0;1,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;160;-3433.486,1123.008;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;162;-3768.994,413.5926;Float;False;HitColor;-1;True;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;103;-2734.775,-736.845;Float;False;85;0;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;101;-2843.709,-1255.661;Float;False;Property;_ShieldPatternColor;Shield Pattern Color;4;0;0.2470588,0.7764706,0.9098039,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;132;-2108.05,-384.4896;Float;False;Property;_ShieldRimPower;Shield Rim Power;12;0;7;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;74;-3417.799,-314.1213;Float;False;Constant;_Vector1;Vector 1;7;0;1,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;161;-3613.093,779.8535;Float;False;Property;_HitTime;Hit Time;19;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;75;-3413.578,-174.9589;Float;True;1;0;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;164;-3230.089,582.0073;Float;False;105;0;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;105;-2538.608,-1256.561;Float;False;ShieldPatternColor;-1;True;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;137;-2111.801,-253.1336;Float;False;133;0;1;FLOAT
Node;AmplifyShaderEditor.AppendNode;104;-2491.107,-824.262;Float;False;FLOAT2;0;0;0;0;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;-3190.292,1040.407;Float;False;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.Vector2Node;76;-3225.399,-385.9766;Float;False;Constant;_Vector3;Vector 3;7;0;1,1;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;133;-1754.142,-386.2343;Float;False;ShieldRimPower;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.AppendNode;77;-3222.09,-262.1631;Float;False;FLOAT2;0;0;0;0;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TFHCRemap;166;-3379.99,858.4073;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;100.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.AppendNode;106;-2516.848,-1033.004;Float;False;FLOAT2;0;0;0;0;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.WireNode;167;-3319.265,647.9454;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;134;-1816.342,-253.2337;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;10.0;False;3;FLOAT;10.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;168;-3248.959,708.1005;Float;False;157;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;80;-3026.379,-270.8228;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;107;-2307.205,-891.962;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;169;-3050.19,847.3074;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;170;-3358.642,545.4128;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;78;-2784.479,-286.1952;Float;True;Property;_ShieldPatternWaves;Shield Pattern Waves;14;0;Assets/AmplifyShaderEditor/Examples/Community/ForceShield/ForceShieldWaves.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;108;-2047.808,-909.061;Float;True;Property;_ShieldPattern;Shield Pattern;9;0;Assets/AmplifyShaderEditor/Examples/Community/ForceShield/ForceShieldPattern.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCCompareLower;171;-2860.076,726.3723;Float;False;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.FresnelNode;135;-1592.743,-251.534;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;112;-4196.827,1644.606;Float;False;1652.997;650.1895;Mix of Pattern, Wave, Rim , Impact and adding intersection highlight;17;125;122;113;124;127;126;120;114;130;129;123;121;119;116;115;118;117;Shield Mix for Emission;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-4127.951,2002.497;Float;False;79;0;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;113;-4146.827,1924.21;Float;False;109;0;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;115;-4136.892,1833.353;Float;False;136;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-2450.374,-295.8404;Float;False;waves;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RegisterLocalVarNode;136;-1373.905,-255.6029;Float;False;ShieldRim;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;138;-2130.308,197.1151;Float;False;1223.975;464.9008;Comment;11;149;148;147;146;145;144;143;142;141;140;139;Shield Distortion;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;109;-1658.607,-907.261;Float;False;ShieldPattern;-1;True;1;0;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.TFHCCompareGreater;172;-2714.688,478.5728;Float;False;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;COLOR;0.0;False;3;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;119;-3976.212,2093.099;Float;False;105;0;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;117;-3909.501,1873.347;Float;False;2;0;FLOAT;0.0;False;1;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;139;-2080.308,405.7322;Float;False;85;0;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;120;-3896.463,2010.174;Float;False;173;0;1;COLOR
Node;AmplifyShaderEditor.WireNode;116;-3929.531,1990.677;Float;False;1;0;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;140;-2041.18,503.9486;Float;False;Constant;_Float7;Float 7;7;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;-2522.388,482.3801;Float;False;hit;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;118;-3983.615,2173.476;Float;False;Property;_IntersectIntensity;Intersect Intensity;16;0;0.2;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-3652.336,2077.479;Float;False;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.NormalVertexDataNode;142;-1951.159,247.1151;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;141;-1856.392,434.3912;Float;False;2;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;-3733.367,1896.982;Float;False;2;0;FLOAT4;0.0;False;1;FLOAT4;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.DepthFade;121;-3617.475,2176.369;Float;False;1;0;FLOAT;0.5;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;126;-3401.322,2138.797;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-3491.02,1891.801;Float;True;2;0;FLOAT4;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;110;-2150.006,-1210.261;Float;False;Property;_ShieldPatternPower;Shield Pattern Power;11;0;5;0;100;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;143;-1845.529,547.0161;Float;False;Property;_ShieldDistortion;Shield Distortion;15;0;0.01;0;0.03;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;177;-495.0719,864.2045;Float;False;837.0001;689.9695;Comment;4;183;181;179;178;Textures;0;0
Node;AmplifyShaderEditor.ColorNode;124;-3513.928,1694.606;Float;False;Property;_IntersectColor;Intersect Color;17;0;0.03137255,0.2588235,0.3176471,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;144;-1687.791,276.9823;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT4;0.0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.WireNode;146;-1430.132,539.4889;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;128;-3483.001,2356.841;Float;False;111;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;179;-417.5681,914.2045;Float;False;Property;_Color;Color;0;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;127;-3212.729,1902.706;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.NoiseGeneratorNode;145;-1541.998,292.32;Float;False;Simplex3D;1;0;FLOAT3;0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;111;-1830.006,-1200.361;Float;False;ShieldPower;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;-1522.029,389.918;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;178;-438.6721,1104.973;Float;True;Property;_Albedo;Albedo;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-2970.131,1932.71;Float;False;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;183;-64.27198,1072.974;Float;False;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TFHCRemap;148;-1352.628,337.5571;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.01;False;4;FLOAT;0.01;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;187;-596.6633,203.0974;Float;False;Property;_IndexofRefraction;Index of Refraction;6;0;0.5;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;188;-592.7887,87.81106;Float;False;Property;_Smoothness;Smoothness;8;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;150;-112.5555,408.3332;Float;False;149;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;189;-592.899,-23.69551;Float;False;Property;_Metallic;Metallic;7;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;186;-596.3522,301.3744;Float;False;Property;_Opacity;Opacity;5;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;50;-172.7075,173.1073;Float;False;130;0;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-2786.831,1907.707;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;195;-347.3604,-598.896;Float;False;181;0;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;149;-1156.332,345.5887;Float;False;VertexOffset;-1;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;181;98.92811,1066.574;Float;False;Albedo;-1;True;1;0;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;337.409,185.7051;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;Matter;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;1;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;84;0;83;0
WireConnection;84;1;82;0
WireConnection;85;0;84;0
WireConnection;155;0;152;0
WireConnection;155;1;153;0
WireConnection;159;0;155;0
WireConnection;157;0;154;0
WireConnection;73;0;71;0
WireConnection;73;1;72;0
WireConnection;160;0;156;0
WireConnection;160;1;159;0
WireConnection;162;0;158;0
WireConnection;75;0;73;0
WireConnection;105;0;101;0
WireConnection;104;0;100;1
WireConnection;104;1;103;0
WireConnection;165;0;163;0
WireConnection;165;1;160;0
WireConnection;133;0;132;0
WireConnection;77;0;74;1
WireConnection;77;1;75;0
WireConnection;166;0;161;0
WireConnection;106;0;102;0
WireConnection;106;1;102;0
WireConnection;167;0;155;0
WireConnection;134;0;137;0
WireConnection;80;0;76;0
WireConnection;80;1;77;0
WireConnection;107;0;106;0
WireConnection;107;1;104;0
WireConnection;169;0;164;0
WireConnection;169;1;165;0
WireConnection;169;2;166;0
WireConnection;170;0;161;0
WireConnection;78;1;80;0
WireConnection;108;1;107;0
WireConnection;171;0;167;0
WireConnection;171;1;168;0
WireConnection;171;2;169;0
WireConnection;171;3;164;0
WireConnection;135;3;134;0
WireConnection;79;0;78;0
WireConnection;136;0;135;0
WireConnection;109;0;108;0
WireConnection;172;0;170;0
WireConnection;172;2;171;0
WireConnection;172;3;164;0
WireConnection;117;0;115;0
WireConnection;117;1;113;0
WireConnection;116;0;114;0
WireConnection;173;0;172;0
WireConnection;122;0;120;0
WireConnection;122;1;119;0
WireConnection;141;0;139;0
WireConnection;141;1;140;0
WireConnection;123;0;117;0
WireConnection;123;1;116;0
WireConnection;121;0;118;0
WireConnection;126;0;121;0
WireConnection;125;0;123;0
WireConnection;125;1;122;0
WireConnection;144;0;142;0
WireConnection;144;1;141;0
WireConnection;146;0;143;0
WireConnection;127;0;124;0
WireConnection;127;1;125;0
WireConnection;127;2;126;0
WireConnection;145;0;144;0
WireConnection;111;0;110;0
WireConnection;147;0;143;0
WireConnection;129;0;127;0
WireConnection;129;1;128;0
WireConnection;183;0;179;0
WireConnection;183;1;178;0
WireConnection;148;0;145;0
WireConnection;148;3;147;0
WireConnection;148;4;146;0
WireConnection;130;0;129;0
WireConnection;149;0;148;0
WireConnection;181;0;183;0
WireConnection;0;0;195;0
WireConnection;0;2;50;0
WireConnection;0;3;189;0
WireConnection;0;4;188;0
WireConnection;0;8;187;0
WireConnection;0;9;186;0
WireConnection;0;11;150;0
ASEEND*/
//CHKSM=6B387FB8AD608F41076C78368F077C6D0636D62E