// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unphook/Simple/Texture Shade, ZWrite On" {
Properties {
	_Color ("Main Color", Color) = (0,0,0,1)
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
			};

			struct v2f {
				float4 vertex:SV_POSITION;
				half2 texcoord:TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			
			v2f vert(appdata_t v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}
			
			fixed4 frag(v2f i):SV_Target {
				fixed4 col = tex2D(_MainTex,i.texcoord);
				fixed a = col[3]*_Color[3];
				col = 1-((1-col)*(1-_Color));
				col[3] = a;
				return col;
			}
		ENDCG
	}
}
}