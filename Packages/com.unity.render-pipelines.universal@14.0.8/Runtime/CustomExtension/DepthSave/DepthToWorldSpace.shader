Shader "Custom/DepthToWorldSpace"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        //_CameraDepthTexture ("Depth", 2D) = "white" {}
        //_MinZ ("Min Z", Float) = 0.0
        //_MaxZ ("Max Z", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            //sampler2D _MainTex;
            sampler2D _CameraDepthAttachment;
            float4 _MinMaxHeightZ;
           // #define  _MinZ = _MinMaxHeightZ.x;
            //#define  _MaxZ = _MinMaxHeightZ.y;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewRayWorld : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 posWorld = mul(unity_ObjectToWorld, float4( o.vertex.xyz, 1.0)).xyz;
                float3 dir = posWorld - _WorldSpaceCameraPos;
                o.viewRayWorld = normalize(dir);
                return o;
            }

            /*float LinearizeDepth(float z)
            {
                float n = _ProjectionParams.y; // Camera near
                float f = _ProjectionParams.z; // Camera far
                return (2.0 * n) / (f + n - z * (f - n));
            }*/

            float4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_CameraDepthAttachment, i.uv).r;
                float linearEyeDepth = LinearEyeDepth(depth);
               // float worldPos = _WorldSpaceCameraPos + linear01Depth * i.viewRayWorld.xyz;
                //float3 cameraSpacePos = UnityWorldToViewPos(worldPos);
                float view_depth = linearEyeDepth * (_ProjectionParams.z -  _ProjectionParams.y);
               // return view_depth;
                float viewZ = (view_depth - _MinMaxHeightZ.x) / (_MinMaxHeightZ.y - _MinMaxHeightZ.x); // Normalize depth
                /*if(linearEyeDepth<=1 &&linearEyeDepth>=0)
                {
                    return float4(0,1,0,1);
                }
                if(linearEyeDepth<=0)
                {
                    return float4(0,0,0,1);
                }
                if(linearEyeDepth>0.5)
                {
                    return float4(1,0,0,1);
                }*/
                
                return float4(viewZ,viewZ,viewZ, 1.0);
            }
            ENDCG
        }
    }
}
