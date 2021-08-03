Shader "Hidden/RenderDepth"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		CGINCLUDE
#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;

	struct Input
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct Varyings
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	Varyings vertex(in Input input)
	{
		Varyings output;
		output.vertex = UnityObjectToClipPos(input.vertex.xyz);
		output.uv = input.uv;

		return output;
	}

	float4 fragment(in Varyings input) : SV_Target
	{
		float4 depth = tex2D(_CameraDepthTexture, input.uv);

		float4 outputColor = float4(depth.r, 0, 0, 1);

		return outputColor;
	}
		ENDCG

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			CGPROGRAM
			#pragma vertex vertex
			#pragma fragment fragment
			ENDCG
		}
	}
}
