Shader "World Political Map/Unlit Surface Single Color Alpha" {
 
Properties {
    _Color ("Color", Color) = (1,1,1)
}
 
SubShader {
    Tags {
        "Queue"="Geometry+1"
        "RenderType"="Transparent"
    	}
    Color [_Color]
    Blend SrcAlpha OneMinusSrcAlpha
    Pass {
    ZTest Always
    }
}
 
}
