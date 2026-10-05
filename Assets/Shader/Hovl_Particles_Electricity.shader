Shader "Hovl/Particles/Electricity" {
	Properties {
		_MainTexture ("Main Texture", 2D) = "white" {}
		_Dissolveamount ("Dissolve amount", Range(0, 1)) = 0.332
		_Mask ("Mask", 2D) = "white" {}
		_Color ("Color", Vector) = (0.5,0.5,0.5,1)
		_Emission ("Emission", Float) = 6
		_RemapXYFresnelZW ("Remap XY/Fresnel ZW", Vector) = (-10,10,2,2)
		_Speed ("Speed", Vector) = (0.189,0.225,-0.2,-0.05)
		_Opacity ("Opacity", Range(0, 1)) = 1
		[MaterialToggle] _Usedepth ("Use depth?", Float) = 0
		_Depth ("Depth", Float) = 0.15
		[Enum(Cull Off,0, Cull Front,1, Cull Back,2)] _CullMode ("Culling", Float) = 2
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
}