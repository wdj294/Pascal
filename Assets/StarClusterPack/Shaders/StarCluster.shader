// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "StarClusters/StarCluster" {
    Properties {
        _Shape ("Shape", 2D) = "white" {}
        _Color ("Color", 2D) = "white" {}
        _Size ("Size", 2D) = "white" {}
        _Color_Tint ("Color_Tint", Color) = (1,1,1,1)
        _Color_Multiplier ("Color_Multiplier", Float ) = 1
        _Size_Multiplier ("Size_Multiplier", Float ) = 1
        _Variation_Shift ("Variation_Shift", Float ) = 0
        [MaterialToggle] _Use_Camera_Position ("Use_Camera_Position", Float ) = 0
        _Camera_Position ("Camera_Position", Vector) = (0,0,0,0)
        [MaterialToggle] _Use_Attenuation ("Use_Attenuation", Float ) = 0
        _Attenuation_Strength ("Attenuation_Strength", Float ) = 1
        _Attenuation_Exponent ("Attenuation_Exponent", Float ) = 0.8
        [MaterialToggle] _Use_LensEffect ("Use_LensEffect", Float ) = 0
        _LensEffect ("LensEffect", 2D) = "white" {}
        _LensEffect_Distance ("LensEffect_Distance", Float ) = 1
        _LensEffect_DistanceExponent ("LensEffect_DistanceExponent", Float ) = 3
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform sampler2D _Shape; uniform float4 _Shape_ST;
            uniform sampler2D _Color; uniform float4 _Color_ST;
            uniform sampler2D _Size; uniform float4 _Size_ST;
            uniform float _Variation_Shift;
            uniform float4 _Color_Tint;
            uniform float _Size_Multiplier;
            uniform float _Color_Multiplier;
            uniform float4 _Camera_Position;
            uniform fixed _Use_Camera_Position;
            uniform float _Attenuation_Strength;
            uniform float _Attenuation_Exponent;
            uniform fixed _Use_Attenuation;
            uniform sampler2D _LensEffect; uniform float4 _LensEffect_ST;
            uniform float _LensEffect_Distance;
            uniform float _LensEffect_DistanceExponent;
            uniform fixed _Use_LensEffect;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                float3 node_4094 = mul( unity_WorldToObject, float4(lerp( viewDirection, normalize((mul(unity_ObjectToWorld, v.vertex).rgb-_Camera_Position.rgb)), _Use_Camera_Position ),0) ).xyz.rgb;
                float3 node_9535 = cross(float3(0,1,0),node_4094);
                float2 node_176 = (o.uv0-0.5).rg;
                float3 node_3810 = normalize(((normalize(cross(node_9535,node_4094))*node_176.g)+(normalize(node_9535)*node_176.r)));
                float node_1067 = length((mul(unity_ObjectToWorld, v.vertex).rgb-_WorldSpaceCameraPos));
                float3 _Use_Attenuation_var = lerp( node_3810, (node_3810*(1.0/pow((node_1067*50.0*clamp(_Attenuation_Strength,0,100)),clamp(_Attenuation_Exponent,0,100)))), _Use_Attenuation );
                float node_7370 = clamp(_LensEffect_Distance,0,100);
                float node_6610 = pow(clamp((node_7370-node_1067),0.0,node_7370),clamp(_LensEffect_DistanceExponent,0,100));
                float node_4455 = (_Variation_Shift/100.0);
                float2 node_6482 = float2((o.vertexColor.g+node_4455),0.0);
                float4 _Size_var = tex2Dlod(_Size,float4(TRANSFORM_TEX(node_6482, _Size),0.0,0));
                float4 _Shape_var = tex2Dlod(_Shape,float4(TRANSFORM_TEX(o.uv0, _Shape),0.0,0));
                v.vertex.xyz += (lerp(_Use_Attenuation_var,(_Use_Attenuation_var*((node_6610*1.0)+1.0)),_Use_LensEffect)*_Size_var.r*0.1*(_Shape_var.b*2.5)*_Size_Multiplier);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
////// Lighting:
////// Emissive:
                float node_4455 = (_Variation_Shift/100.0);
                float2 node_8182 = float2((i.vertexColor.g+node_4455),0.0);
                float4 _Color_var = tex2D(_Color,TRANSFORM_TEX(node_8182, _Color));
                float4 _Shape_var = tex2D(_Shape,TRANSFORM_TEX(i.uv0, _Shape));
                float3 node_2646 = (_Color_var.rgb*_Color_Tint.rgb*(_Shape_var.a*5.0)*_Color_Multiplier);
                float3 node_4795 = (node_2646*(_Shape_var.r+(_Shape_var.g*10.0)));
                float4 _LensEffect_var = tex2D(_LensEffect,TRANSFORM_TEX(i.uv0, _LensEffect));
                float node_7370 = clamp(_LensEffect_Distance,0,100);
                float node_1067 = length((i.posWorld.rgb-_WorldSpaceCameraPos));
                float node_6610 = pow(clamp((node_7370-node_1067),0.0,node_7370),clamp(_LensEffect_DistanceExponent,0,100));
                float3 emissive = lerp(node_4795,lerp(node_4795,(node_2646*(_LensEffect_var.r+_LensEffect_var.g+(_LensEffect_var.b*10.0))),saturate(node_6610)),_Use_LensEffect);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform sampler2D _Shape; uniform float4 _Shape_ST;
            uniform sampler2D _Size; uniform float4 _Size_ST;
            uniform float _Variation_Shift;
            uniform float _Size_Multiplier;
            uniform float4 _Camera_Position;
            uniform fixed _Use_Camera_Position;
            uniform float _Attenuation_Strength;
            uniform float _Attenuation_Exponent;
            uniform fixed _Use_Attenuation;
            uniform float _LensEffect_Distance;
            uniform float _LensEffect_DistanceExponent;
            uniform fixed _Use_LensEffect;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                float3 node_4094 = mul( unity_WorldToObject, float4(lerp( viewDirection, normalize((mul(unity_ObjectToWorld, v.vertex).rgb-_Camera_Position.rgb)), _Use_Camera_Position ),0) ).xyz.rgb;
                float3 node_9535 = cross(float3(0,1,0),node_4094);
                float2 node_176 = (o.uv0-0.5).rg;
                float3 node_3810 = normalize(((normalize(cross(node_9535,node_4094))*node_176.g)+(normalize(node_9535)*node_176.r)));
                float node_1067 = length((mul(unity_ObjectToWorld, v.vertex).rgb-_WorldSpaceCameraPos));
                float3 _Use_Attenuation_var = lerp( node_3810, (node_3810*(1.0/pow((node_1067*50.0*clamp(_Attenuation_Strength,0,100)),clamp(_Attenuation_Exponent,0,100)))), _Use_Attenuation );
                float node_7370 = clamp(_LensEffect_Distance,0,100);
                float node_6610 = pow(clamp((node_7370-node_1067),0.0,node_7370),clamp(_LensEffect_DistanceExponent,0,100));
                float node_4455 = (_Variation_Shift/100.0);
                float2 node_6482 = float2((o.vertexColor.g+node_4455),0.0);
                float4 _Size_var = tex2Dlod(_Size,float4(TRANSFORM_TEX(node_6482, _Size),0.0,0));
                float4 _Shape_var = tex2Dlod(_Shape,float4(TRANSFORM_TEX(o.uv0, _Shape),0.0,0));
                v.vertex.xyz += (lerp(_Use_Attenuation_var,(_Use_Attenuation_var*((node_6610*1.0)+1.0)),_Use_LensEffect)*_Size_var.r*0.1*(_Shape_var.b*2.5)*_Size_Multiplier);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
