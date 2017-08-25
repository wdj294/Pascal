Shader "World Political Map/Unlit Alpha Single Color" {
 
Properties {
    _Color ("Color", Color) = (1,1,1,0.5)
}
 
SubShader {
	Tags {
        "Queue"="Transparent+5"
        "RenderType"="Transparent"
    }
    Color [_Color]
   	Blend SrcAlpha OneMinusSrcAlpha
   	ZWrite Off
    Pass {
 
    }
}
 
}
