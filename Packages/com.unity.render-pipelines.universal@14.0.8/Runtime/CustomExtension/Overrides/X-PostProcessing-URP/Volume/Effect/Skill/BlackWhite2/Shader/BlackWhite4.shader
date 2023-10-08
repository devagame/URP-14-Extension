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
    float4 _Params5;
    float3 _Color1;
    float3 _Color2;

    TEXTURE2D(_TurbulenceTex);SAMPLER(sampler_TurbulenceTex);
    float4 _TurbulenceTex_ST;
    
    TEXTURE2D(_MaskTex);SAMPLER(sampler_MaskTex);
    float4 _MaskTex_ST;

    //turbulence
    #define TurbulencePolarBlend _Params.x
    #define TurbulencePolarRaduis _Params.y
    #define TexMove_X _Params.z
    #define TexMove_Y _Params.w

    #define TurbulenceRotate _Params2.x
    #define TurbulenceStrength _Params2.y
    #define useMask _Params2.z
    #define ChangeRate _Params2.w

    //黑白控制
    #define ColorBlend0 _Params3.x
    #define ColorBlend1 _Params3.y
    
    #define jitterX _Params3.z
    #define jitterY _Params3.w
    
    #define blackWhiteThreshold _Params4.x
    #define GreyThreshold _Params4.y
    
    #define Center _Params4.zw
    
    half luminance(half3 color)
    {
        return dot(color, half3(0.222, 0.707, 0.071));
    }
    
    float randomNoise(float x, float y)
    {
        return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
    }

    float2 Polaruv(float2 uv,float radius)
    {
        float2 puv = uv;
        puv -= float2(0.5,0.5);
        puv = float2(atan2(puv.y,puv.x)/3.14159*0.5 + 0.5,length(puv)+radius);
        return puv;
    }

    float2 rotation(float2 uv , float angle)
    {
        float2 ruv;
        float rgl = radians(angle);
        ruv = uv;
        ruv -= float2(0.5,0.5);
        ruv = float2(ruv.x * cos(rgl) - ruv.y * sin(rgl) , ruv.x * sin(rgl) + ruv.y * cos(rgl));
        ruv += float2(0.5,0.5);
        return ruv;
    }
    
    void Unity_Remap_float4(float4 In, float4 minOld,float4 maxOld,float minNew, float maxNew, out float4 Out)
    {
       // Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        Out = ( (minNew).xxxx + (In - minOld) * ( (maxNew).xxxx - (minNew).xxxx) / maxOld - minOld);
    }

    half4 Frag(VaryingsDefault input): SV_Target
    {
        //极坐标纹理
        float2 uv = input.uv - Center;
        float2 turbulenceUv = lerp(uv , Polaruv(uv, TurbulencePolarRaduis) , TurbulencePolarBlend);

        float2 turbulenceMove = float2(TexMove_X,TexMove_Y) * _Time.y;
        turbulenceUv = turbulenceUv + turbulenceMove;
        float2 _turbulenceUv = rotation(turbulenceUv, TurbulenceRotate);

        float4 turbulence = SAMPLE_TEXTURE2D(_TurbulenceTex,sampler_TurbulenceTex, _turbulenceUv.xy *_TurbulenceTex_ST.xy +_TurbulenceTex_ST.zw  );

        //计算扭曲强度
        float suv = turbulence.r * turbulence.a * TurbulenceStrength;
        float2 baseUV = suv + input.uv;

        //计算mask

        float mask = SAMPLE_TEXTURE2D(_MaskTex,sampler_MaskTex, input.uv.xy *_MaskTex_ST.xy +_MaskTex_ST.zw  );
        baseUV = lerp( baseUV , input.uv , (1- mask) * useMask);
        
        //给uv 添加 noise 
        float jitter = randomNoise(input.uv.y, _Time.x) * 2 - 1;
        jitter *= step(jitterY, abs(jitter)) * jitterX;
        
        //float2 noisePolarUV = float2(length(noiseUV) * NoiseTillingX * 2, atan2(noiseUV.x, noiseUV.y) * (1.0 / TWO_PI) * NoiseTillingY);

        half grey = luminance(GetScreenColor(baseUV).rgb);    
        //屏幕颜色
        grey = saturate(grey);
        float greyValue = smoothstep(blackWhiteThreshold , blackWhiteThreshold + GreyThreshold , grey.r);
        greyValue = lerp(greyValue,1-greyValue,ChangeRate);
        
        float3 finalColor = lerp(_Color1,_Color2,greyValue);

        //黑白渐变
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