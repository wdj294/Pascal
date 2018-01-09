// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/TranslucentImage"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[HideInInspector] _BlurTex("Blur Texture", 2D) = "gray" {}

		_Vibrancy("Vibrancy", Float) = 1
		_Brightness("Brightness", Float) = 0
		_Flatten("Flatten", Float) = 0

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
        {
            "Queue" = "Transparent"
//            "Queue" = "AlphaTest"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

		Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

		Cull Off	
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
    
                #include "UnityCG.cginc"
                #include "UnityUI.cginc"
    
                #pragma multi_compile __ UNITY_UI_ALPHACLIP
    
                struct appdata
                {
                    half4 vertex   : POSITION;
                    half4 color    : COLOR;
                    half2 texcoord : TEXCOORD0;
                    half2 extraTexcoord : TEXCOORD1;
                };
    
                struct v2f
                {
                    half4 vertex   : SV_POSITION;
                    half4 color : COLOR;
                    half2 texcoord  : TEXCOORD0;
                    half4 worldPosition : TEXCOORD1;
                    half4 screenPos : TEXCOORD2;
                    half2 extraTexcoord : TEXCOORD3;
                };
    
                fixed4 _TextureSampleAdd;
                half4 _ClipRect;
    
                v2f vert(appdata IN)
                {
                    v2f OUT;
    
                    OUT.worldPosition = IN.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
    
                    #ifdef UNITY_HALF_TEXEL_OFFSET
                        OUT.vertex.xy += (_ScreenParams.zw - 1.0)*half2(-1,1);
                    #endif
    
                    OUT.color = IN.color;
                    OUT.texcoord = IN.texcoord;			    
                    OUT.screenPos = ComputeScreenPos(OUT.vertex);
                    OUT.extraTexcoord = IN.extraTexcoord;
    
                    return OUT;
                }
    
                sampler2D _MainTex;
                sampler2D _BlurTex;
                uniform half _Vibrancy;
                uniform half _Flatten;
                uniform half _Brightness;
    
                half4 frag(v2f IN) : SV_Target
                {
                    half4 backgroundColor = tex2Dproj(_BlurTex, IN.screenPos);
                    half4 foregroundColor = tex2D(_MainTex, IN.texcoord.xy) + _TextureSampleAdd;
    
                    //saturate help keep color in range 				
                    
                    //Exclusion blend with white
                    //backgroundColor.rgb = saturate(backgroundColor.rgb * (1 - 2 * _Flatten) + _Flatten);
                    backgroundColor.rgb = saturate(lerp(backgroundColor.rgb, 0.5h, _Flatten));
                    
                    //Vibrancy
                    backgroundColor.rgb = saturate(lerp(Luminance(backgroundColor.rgb), backgroundColor.rgb, _Vibrancy));
    
                    //Brightness
                    backgroundColor.rgb = saturate(backgroundColor.rgb + _Brightness);
    
    
                    //Overlay
                    half4 color = foregroundColor * IN.color;
    
                    //Alpha blend with backgroundColor
                    //color.rgb = color.rgb * color.a + backgroundColor.rgb * (1 - color.a);
                    color.rgb = lerp(backgroundColor.rgb, color.rgb, IN.extraTexcoord[0]);
    
//                    color.a = foregroundColor.a;
//                    color.a *= IN.extraTexcoord[0];
    
                    //UI stuff
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                    #ifdef UNITY_UI_ALPHACLIP
                        clip(color.a - 0.001);
                    #endif
    
                    return color;
                }
			ENDCG
		}
	}
}
