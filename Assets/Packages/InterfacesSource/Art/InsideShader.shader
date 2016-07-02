Shader "Show Insides" { //http://answers.unity3d.com/questions/176487/materialtexture-on-the-inside-of-a-sphere.html
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
	}
     SubShader {
       
       Tags { "Queue"="Transparent" "RenderType" = "Transparent" "LightMode" = "Vertex" }
       
       Cull Front
	   Blend SrcAlpha OneMinusSrcAlpha
	   Color [_Color] Pass {
	   
	   }
     }
     
     Fallback "Diffuse"
     
   }