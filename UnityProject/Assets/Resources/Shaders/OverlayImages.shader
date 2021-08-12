Shader "Hidden/OverlayImages"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		CGINCLUDE
#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _VirtualTex;
	sampler2D _HandsTex;
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
		float4 imageColor = tex2D(_MainTex, input.uv);
		float4 virtualColor = tex2D(_VirtualTex, input.uv);
		float4 handColor = tex2D(_HandsTex, input.uv);

		//float ImageDepth = tex2D(_MainTex, input.uv).r;
		float handDepth = tex2D(_HandsTex, input.uv).r;
		float virtualDepth = tex2D(_CameraDepthTexture, input.uv).r;
		float4 outputColor;

		if (handDepth == 0 && virtualDepth == 0) {
			outputColor = imageColor;
		}
		else {
			if (handDepth > virtualDepth) {
				outputColor = imageColor;
			}
			else {
				outputColor = virtualColor;
			}
		}

			
		

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
