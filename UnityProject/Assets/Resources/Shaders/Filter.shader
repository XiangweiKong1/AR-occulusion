Shader "Hidden/Filter"
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
	float2 _FrameSize;
	float _Sigma;
	int _Size;

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

	float Gaussian(int x)
	{
		float sigmaSqu = _Sigma * _Sigma;
		float TWO_PI = 6.28319;
		float E = 2.71828;
		return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(x * x) / (2 * sigmaSqu));

	}

	float CompareColor(float2 uv1, float2 uv2)
	{
		float4 color1 = tex2D(_MainTex, uv1);
		float4 color2 = tex2D(_MainTex, uv2);
		float output = Gaussian(sqrt(pow((color1.x - color2.x), 2) + pow((color1.y - color2.y), 2) + pow((color1.z - color2.z), 2)));
		
		return output;
	}
	
	float4 Filter(float2 uv, int size)
	{
		float factor = 0;
		float output = 0;
		for (int i = -(size-1)/2; i < (size+1)/2; i++)
		{
			for (int j = -(size - 1) / 2; j < (size + 1) / 2; j++)
			{

				float2 uv2 = uv + float2(i / _FrameSize.x, j / _FrameSize.y);

				float handDepth = tex2D(_HandsTex, uv2).r;
				float depthMap = 0;
				if (handDepth != 0) {
					depthMap = 1;
				}

				float spatialDiff = Gaussian(sqrt(i ^ 2 + j ^ 2));
				float colorDiff = CompareColor(uv, uv2);
				factor += spatialDiff * colorDiff;
				output += depthMap * spatialDiff * colorDiff;

			}
		}
		output /= factor;
		return output;
	}




	float4 fragment(in Varyings input) : SV_Target
	{
		float4 imageColor = tex2D(_MainTex, input.uv );
		float4 virtualColor = tex2D(_VirtualTex, input.uv);
		float4 handColor = tex2D(_HandsTex, input.uv);

		//float ImageDepth = tex2D(_MainTex, input.uv).r;
		float handDepth = tex2D(_HandsTex, input.uv).r;
		float virtualDepth = tex2D(_CameraDepthTexture, input.uv).r;
		float4 plotHnadDepth = float4(0, 0, 0, 1);
		float filteredDepth = Filter(input.uv, _Size);
		plotHnadDepth = float4(filteredDepth, filteredDepth, filteredDepth, 1);
		float4 outputColor = plotHnadDepth;
		if (filteredDepth == 0) {
			outputColor = imageColor;
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
