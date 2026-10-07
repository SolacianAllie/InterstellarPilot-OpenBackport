Shader "OpenFrontier/GasCloud Billboard" {
	Properties {
		_BaseMap ("Texture", 2D) = "white" {}
		_BaseColor ("Color", Color) = (0.32, 0.32, 0.32, 1)
		// The source puff texture is very soft (avg alpha ~0.29);
		// boost it so the distant billboard reads as a solid cloud.
		_AlphaBoost ("Alpha Boost", Range(1, 8)) = 3
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		ZWrite Off
		Cull Off
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _BaseMap;
			float4 _BaseMap_ST;
			fixed4 _BaseColor;
			float _AlphaBoost;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(2)
			};

			v2f vert(appdata input)
			{
				v2f output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
				UNITY_TRANSFER_FOG(output, output.pos);
				return output;
			}

			fixed4 frag(v2f input) : SV_Target
			{
				fixed4 col = tex2D(_BaseMap, input.uv) * _BaseColor;
				col.a = saturate(col.a * _AlphaBoost);
				// Open Frontier: fog now applies - from inside a cloud (dense
				// fog), other clouds' billboards are swallowed by the murk
				// instead of hanging visibly in it.
				UNITY_APPLY_FOG(input.fogCoord, col);
				return col;
			}
			ENDHLSL
		}
	}
}
