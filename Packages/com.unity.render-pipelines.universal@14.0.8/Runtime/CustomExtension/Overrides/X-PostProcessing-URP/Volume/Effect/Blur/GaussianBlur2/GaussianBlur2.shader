Shader "PostProcess/ColorTint"//名字开放位置

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
    };

    sampler2D _MainTex;
    sampler2D _CameraDepthTexture;
    float4 _MainTex_ST;
    float4 _ColorTint;//设置颜色校正位置
    //【高斯模糊】
    float _BlurRange;

    v2f ColorTintvert(appdata v)
    {
        v2f o;
        o.vertex = TransformObjectToHClip(v.vertex);//裁剪空间转换
        o.uv = v.uv;
        return o;
    }

    float4 ColorTintfrag(v2f i) : SV_Target
    {
        // sample the texture
        float4 col = tex2D(_CameraDepthTexture, i.uv) * _ColorTint;
        // apply fog
        return col;
    }

    float4 GuassianBluracrossfrag(v2f i) : SV_Target
    {
        // sample the texture
        //【包围盒:横模糊】
        float blurrange = _BlurRange / 50;
        float4 Left = tex2D(_MainTex, i.uv + float2(-blurrange, 0.0)) * 0.2;
        float4 Mid = tex2D(_MainTex, i.uv + float2(0, 0.0)) * 0.6;
        float4 Right = tex2D(_MainTex, i.uv + float2(blurrange, 0.0)) * 0.2;
        float4 col = Left + Mid + Right;
        // apply fog
        return col;
    }
    float4 GuassianBlurcolumnfrag(v2f i) : SV_Target
    {
        // sample the texture
        //【包围盒:纵模糊】
        float blurrange = _BlurRange / 50;
        float4 Down = tex2D(_MainTex, i.uv + float2(0.0, -blurrange)) * 0.2;
        float4 Mid = tex2D(_MainTex, i.uv + float2(0, 0.0)) * 0.6;
        float4 Up = tex2D(_MainTex, i.uv + float2(0.0, +blurrange)) * 0.2;
        float4 col = Down + Mid + Up;
        // apply fog
        return col;
    }
    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

            #pragma vertex ColorTintvert
            #pragma fragment ColorTintfrag
            ENDHLSL

        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex ColorTintvert
            #pragma fragment GuassianBluracrossfrag
            ENDHLSL

        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex ColorTintvert
            #pragma fragment GuassianBlurcolumnfrag
            ENDHLSL

        }
    }
}
