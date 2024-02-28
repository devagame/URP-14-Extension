Shader "Hidden/Beautify2/DepthOnly"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaCutOff("Alpha CutOff", Float) = 0
    }
    SubShader
    {
        ColorMask 0
        ZWrite On
        Cull [_Cull]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ DEPTH_PREPASS_ALPHA_TEST

            #include "UnityCG.cginc"

            half _Cutoff;
            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                #if DEPTH_PREPASS_ALPHA_TEST
                    float2 uv : TEXCOORD0;
                #endif
		        UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                #if DEPTH_PREPASS_ALPHA_TEST
                    float2 uv : TEXCOORD0;
                #endif
		        UNITY_VERTEX_INPUT_INSTANCE_ID
		        UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
		        UNITY_SETUP_INSTANCE_ID(v);
		        UNITY_TRANSFER_INSTANCE_ID(v, o);
		        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                #if DEPTH_PREPASS_ALPHA_TEST
                    o.uv = v.uv;
                #endif
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
		        UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                #if DEPTH_PREPASS_ALPHA_TEST
                    half4 color = tex2D(_MainTex, i.uv);
                    clip(color.a - _Cutoff);
                #endif
                return 0;
            }
            ENDCG
        }
    }
}
