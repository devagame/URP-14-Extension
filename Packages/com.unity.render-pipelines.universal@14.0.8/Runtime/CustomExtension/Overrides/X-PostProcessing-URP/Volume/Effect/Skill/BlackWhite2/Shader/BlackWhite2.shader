Shader "Hidden/PostProcessing/Skill/BlackWhite2"
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
    float3 _Color;

    TEXTURE2D(_NoiseTex);SAMPLER(sampler_NoiseTex);
    float4 _NoiseTex_ST;
    
    TEXTURE2D(_DissolveTex);SAMPLER(sampler_DissolveTex);
    float4 _DissolveTex_ST;

    #define GreyThreshold _Params3.x
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

    half4 Frag(VaryingsDefault input): SV_Target
    {
        half grey = luminance(GetScreenColor(input.uv).rgb);
        //return grey;

        float2 noiseUV = input.uv * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
        float2 _Dissolve = input.uv * _DissolveTex_ST.xy + _DissolveTex_ST.zw;
        
        //极坐标纹理
        float2 centerdUV = input.uv - Center;

        noiseUV -= Center;
        _Dissolve -= Center;

        float2 noisePolarUV = float2(length(noiseUV) * NoiseTillingX * 2, atan2(noiseUV.x, noiseUV.y) * (1.0 / TWO_PI) * NoiseTillingY);
        float2 _DissolvePolarUV = float2(length(_Dissolve) * NoiseTillingX * 2, atan2(_Dissolve.x, _Dissolve.y) * (1.0 / TWO_PI) * NoiseTillingY);
        //return float4(centerdUV,0,1);
        
        //float2 polarUV = float2(length(centerdUV) * NoiseTillingX * 2, atan2(centerdUV.x, centerdUV.y) * (1.0 / TWO_PI) * NoiseTillingY);
        //return float4(polarUV,0,1);

        noisePolarUV += _Time.y * NoiseSpeed.xx;
        _DissolvePolarUV += _Time.y * DissolveSpeed.xx;
        
        half polarColor = luminance(SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noisePolarUV ).rgb);
        //return polarColor;
        
        half dissloveColor = SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex, _DissolvePolarUV * 0.5).r;

        polarColor *= dissloveColor;

        // return polarColor * grey;
        grey = grey + grey * polarColor;

        //切换
        //grey = lerp(grey, 1 - grey, ChangeRate);

        // grey = polarColor;
        grey = saturate(grey);

        //纯黑白颜色控制
        float r =  step(GreyThreshold,grey.r);
        
        r = smoothstep(Threshold , Threshold + GreyThreshold  ,grey.r);
        
        r = lerp(r, 1-r, ChangeRate);
        
        half3 finalColor = saturate(r * _Color);

        return float4(finalColor,1);
        
        //提取黑白
        //return smoothstep(1 - GreyThreshold, GreyThreshold, half4(finalColor, 1));
        
        // return sceneColor;

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