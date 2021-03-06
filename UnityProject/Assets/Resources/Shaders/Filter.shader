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
	float _ColorSigma;
	float _SpatialSigma;
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

	float Gaussian(float x, float sigma)
	{
		float sigmaSqu = sigma * sigma;
		float TWO_PI = 6.28319;
		float E = 2.71828;
		return (1 / sqrt(TWO_PI * sigmaSqu)) * pow(E, -(x * x) / (2 * sigmaSqu));
		//return 1;

	}

	float CompareColorRGB(float2 uv1, float2 uv2, float sigma)
	{
		float4 color1 = tex2D(_MainTex, uv1);
		float4 color2 = tex2D(_MainTex, uv2);
		float output = Gaussian(sqrt(pow((color1.x - color2.x), 2) + pow((color1.y - color2.y), 2) + pow((color1.z - color2.z), 2)), sigma);
		
		return output;
	}

	float CompareColorYCrCb(float2 uv1, float2 uv2, float sigma)
	{
		float4 color1 = tex2D(_MainTex, uv1);
		float4 color2 = tex2D(_MainTex, uv2);
		float Y1 = 0.299*color1.x + 0.587*color1.y + 0.114*color1.z;
		float Cr1 = (color1.x - Y1)*0.713 + 0.5;
		float Cb1 = (color1.z - Y1)*0.564 + 0.5;
		float Y2 = 0.299*color2.x + 0.587*color2.y + 0.114*color2.z;
		float Cr2 = (color2.x - Y2)*0.713 + 0.5;
		float Cb2 = (color2.z - Y2)*0.564 + 0.5;
		float output = Gaussian(sqrt(pow((Cr1 - Cr2), 2) + pow((Cb1 - Cb2), 2)), sigma);
		return output;
	}
	
	float Filter(float2 uv, int size)

	{
		float factor = 0;
		float output = 0;
		for (int i = -(size-1)/2; i < (size+1)/2; i++)
		{
			for (int j = -(size - 1) / 2; j < (size + 1) / 2; j++)
			{

				float2 uv2 = uv + float2(i / _FrameSize.x, j / _FrameSize.y);

				float handDepth = tex2D(_HandsTex, uv2).r;
				float spatialDiff = Gaussian(sqrt(i * i + j * j), _SpatialSigma);
				float colorDiff = CompareColorYCrCb(uv, uv2, _ColorSigma);
				factor += spatialDiff * colorDiff;
				output += handDepth * spatialDiff * colorDiff;

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
		
		float filteredDepth = Filter(input.uv, _Size);
		float4 outputColor;

		if (filteredDepth < 1e-6 && virtualDepth < 1e-6) {
			outputColor = imageColor;
		}
		else {
			if (filteredDepth > virtualDepth) {
				outputColor = imageColor;
			}
			else {
				outputColor = virtualColor;
			}

		}


		//if (handDepth < 1e-6 && virtualDepth < 1e-6) {
		//	outputColor = imageColor;
		//}
		//else {

		//	if (virtualDepth > 1e-6) {
		//		outputColor = float4(1, 1, 0, 1);
		//	}
		//	//if (handDepth > 1e-6){
		//	//	outputColor = float4(0, 0, 0, 1);
		//	//}

		//
		//}
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
