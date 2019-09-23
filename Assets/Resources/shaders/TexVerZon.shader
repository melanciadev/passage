// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unphook/Simple/Texture Vertex, ZWrite On" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite On
	Blend SrcAlpha OneMinusSrcAlpha
	
	Pass {
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex:POSITION;
				float2 texcoord:TEXCOORD0;
				float4 color:COLOR;
			};

			struct v2f {
				float4 vertex:SV_POSITION;
				half2 texcoord:TEXCOORD0;
				float4 color:COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert(appdata_t v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag(v2f i):SV_Target {
				return tex2D(_MainTex,i.texcoord)*i.color;
			}
		ENDCG
	}
}
}