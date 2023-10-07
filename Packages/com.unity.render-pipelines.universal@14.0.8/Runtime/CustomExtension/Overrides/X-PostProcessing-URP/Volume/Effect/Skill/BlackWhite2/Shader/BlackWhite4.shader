Shader "Hidden/PostProcessing/Skill/BlackWhite4"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
        _NoiseTex ("NoiseTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../../Shader/PostProcessing.hlsl"

    float4 _Params;
    float4 _Params2;
    float4 _Params3;
    float4 _Params4;
    float3 _Color1;
    float3 _Color2;

    TEXTURE2D(_NoiseTex);SAMPLER(sampler_NoiseTex);
    float4 _NoiseTex_ST;
    
    TEXTURE2D(_DissolveTex);SAMPLER(sampler_DissolveTex);
    float4 _DissolveTex_ST;

    #define blendType _Params4.x
    #define ColorBlend0 _Params4.y
     #define ColorBlend1 _Params4.z
    
    #define blackWhiteThreshold _Params3.y
    #define GreyThreshold _Params3.x
    #define NoiseAngle  _Params3.z
    #define DissolvAngle  _Params3.w   
    
    #define Threshold _Params.x
    #define Center _Params.yz

    #define NoiseTillingX _Params2.x
    #define NoiseTillingY _Params2.y
    #define NoiseSpeed _Params2.z
    #define ChangeRate _Params2.w

    #define DissolveSpeed _Params.w
    
    half luminance(half3 color)
    {
        return dot(color, half3(0.222, 0.707, 0.071));
    }

    void Unity_Remap_float4(float4 In, float4 minOld,float4 maxOld,float minNew, float maxNew, out float4 Out)
    {
       // Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        Out = ( (minNew).xxxx + (In - minOld) * ( (maxNew).xxxx - (minNew).xxxx) / maxOld - minOld);
    }

    half4 Frag(VaryingsDefault input): SV_Target
    {
        half grey = luminance(GetScreenColor(input.uv).rgb);
        //return grey;

        half2x2 NoiseRot = half2x2(cos(NoiseAngle),-sin(NoiseAngle) ,sin(NoiseAngle),cos(NoiseAngle));
        half2x2 DissolveRot = half2x2(cos(DissolvAngle),-sin(DissolvAngle) ,sin(DissolvAngle),cos(DissolvAngle));

        //uv 旋转
        float angle = NoiseAngle*0.017453292519943295;
		float2	newNoiseUV = input.uv - float2(0.5,0.5);
		newNoiseUV = float2(
		    newNoiseUV.x * cos(angle) - newNoiseUV.y * sin(angle)
		    ,newNoiseUV.y * cos(angle) + newNoiseUV.x * sin(angle));
		newNoiseUV += float2(0.5,0.5);
        
        float2 noiseUV = newNoiseUV * _NoiseTex_ST.xy + _NoiseTex_ST.zw;

        //溶解图uv旋转
        angle = DissolvAngle*0.017453292519943295;
		float2	newDissolveUV = input.uv - float2(0.5,0.5);
		newDissolveUV = float2(
		    newDissolveUV.x * cos(angle) - newDissolveUV.y * sin(angle)
		    ,newDissolveUV.y * cos(angle) + newDissolveUV.x * sin(angle));
		newDissolveUV += float2(0.5,0.5);
        
        float2 _Dissolve = newDissolveUV * _DissolveTex_ST.xy + _DissolveTex_ST.zw;
        
        //极坐标纹理
        float2 centerdUV = input.uv - Center;

        noiseUV -= Center;
        _Dissolve -= Center;

        float2 noisePolarUV = float2(length(noiseUV) * NoiseTillingX * 2, atan2(noiseUV.x, noiseUV.y) * (1.0 / TWO_PI) * NoiseTillingY);
        float2 _DissolvePolarUV = float2(length(_Dissolve) * NoiseTillingX * 2, atan2(_Dissolve.x, _Dissolve.y) * (1.0 / TWO_PI) * NoiseTillingY);
        
        noisePolarUV += _Time.y * NoiseSpeed.xx;
        _DissolvePolarUV += _Time.y * DissolveSpeed.xx;
        
        half polarColor = luminance(SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noisePolarUV ).rgb);
        //half dissloveColor = SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex, _DissolvePolarUV * 0.5).r;

        //polarColor *= dissloveColor;
        grey = lerp(grey + grey * polarColor,grey + polarColor ,blendType) ;
            
        //屏幕颜色
        grey = saturate(grey);

        float greyValue = smoothstep(blackWhiteThreshold , blackWhiteThreshold + GreyThreshold , grey.r);
        
        greyValue = lerp(greyValue,1-greyValue,ChangeRate);
        
        float3 finalColor = lerp(_Color1,_Color2,greyValue);

        float4 colorRemap ;
        Unity_Remap_float4(float4(finalColor,1), float4(0,0,0,0),float4(1,1,1,1),ColorBlend0,ColorBlend1, colorRemap);
        
        return float4(colorRemap.xyz,1);
    }


    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        
        Cull Off
        ZWrite Off
        ZTest Always
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment Frag

            ENDHLSL

        }
    }
}