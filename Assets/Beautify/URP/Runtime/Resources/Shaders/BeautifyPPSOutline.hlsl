#ifndef BEAUTIFY_PPSOUTLINE
#define BEAUTIFY_PPSOUTLINE
	// Copyright 2016-2021 Ramiro Oliva (Kronnect) - All Rights Reserved.
	
	#include "BeautifyCommon.hlsl"

	TEXTURE2D_X(_MainTex);

	float4 _MainTex_TexelSize;
	float4 _MainTex_ST;
	float4 _Outline;
	half   _BlurScale;
	half   _OutlineIntensityMultiplier;
	float  _OutlineDistanceFade;

   
	struct VaryingsOutline {
		float4 positionCS : SV_POSITION;
		float2 uv: TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
	};

	struct VaryingsCross {
	    float4 positionCS : SV_POSITION;
	    float2 uv: TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsOutline VertOutline(AttributesSimple input) {
	    VaryingsOutline output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = input.positionOS;
		output.positionCS.y *= _ProjectionParams.x * _FlipY;
        output.uv = input.uv;
        return output;
	}

	float OutlinePass(VaryingsOutline i) {

		float3 uvInc      = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);

		#if BEAUTIFY_DEPTH_FADE || !BEAUTIFY_OUTLINE_SOBEL
	        float  depth      = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
		#endif

		float outline = 0;
		#if !BEAUTIFY_OUTLINE_SOBEL
			float  depthS     = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv - uvInc.zy);
			float  depthW     = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv - uvInc.xz);
			float  depthE     = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv + uvInc.xz);		
			float  depthN     = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv + uvInc.zy);
   			float3 normalNW   = getNormal(depth, depthN, depthW, uvInc.zy, float2(-uvInc.x, -uvInc.z));
   			float3 normalSE   = getNormal(depth, depthS, depthE, -uvInc.zy,  uvInc.xz);
			float  dnorm      = dot(normalNW, normalSE);
   			outline = (float)(dnorm  < _Outline.a);
   		#else
			float3 rgbS   = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv - uvInc.zy).rgb;
	   		float3 rgbN   = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + uvInc.zy).rgb;
	    	float3 rgbW   = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv - uvInc.xz).rgb;
    		float3 rgbE   = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + uvInc.xz).rgb;
			float3 rgbSW  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv - uvInc.xy).rgb;	// was tex2Dlod
			float3 rgbNE  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + uvInc.xy).rgb;
			float3 rgbSE  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + float2( uvInc.x, -uvInc.y)).rgb;
			float3 rgbNW  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + float2(-uvInc.x,  uvInc.y)).rgb;
			float3 gx  = rgbSW * -1.0;
 			       gx += rgbSE *  1.0;
		       	   gx += rgbW  * -2.0;
			       gx += rgbE  *  2.0;
			       gx += rgbNW * -1.0;
			       gx += rgbNE *  1.0;
			float3 gy  = rgbSW * -1.0;
			       gy += rgbS  * -2.0;
			       gy += rgbSE * -1.0;
			       gy += rgbNW *  1.0;
			       gy += rgbN  *  2.0;
			       gy += rgbNE *  1.0;
			outline = (length(gx * gx + gy * gy) - _Outline.a) > 0.0;
   		#endif

		#if BEAUTIFY_DEPTH_FADE
			float factor = max(0, (_OutlineDistanceFade - depth) / _OutlineDistanceFade);
			outline *= factor;
		#endif

		return outline;
	}
	
	float4 fragOutline (VaryingsOutline i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
   		float outline = OutlinePass(i);
  		return outline;
	}


	VaryingsCross VertBlur(AttributesSimple v) {
    	VaryingsCross o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = v.positionOS;
		o.positionCS.y *= _ProjectionParams.x * _FlipY;
    	o.uv = v.uv;
        BEAUTIFY_VERTEX_OUTPUT_GAUSSIAN_UV(o)

    	return o;
	}
	
	half4 FragBlur (VaryingsCross i): SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_GAUSSIAN_UV(i)

		half4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv) * 0.2270270270
					+ (SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1) + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2)) * 0.3162162162
					+ (SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3) + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4)) * 0.0702702703;
   		return pixel;
	}	

	half4 FragCopy (VaryingsSimple i): SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
		half outline = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv).r;
		half4 color = half4(_Outline.rgb, _Outline.a * outline);
		color *= _OutlineIntensityMultiplier;
		color.a = saturate(color.a);
		return color;
	}

#endif