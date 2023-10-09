using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    public enum UVNoiseType:int
    {
        None = 0,
        Vertical = 1,
        Horizal = 2,
        Polar = 3,
    }
    [Serializable, DebuggerDisplay(k_DebuggerDisplay)]
    public class UVNoiseTypeEnumParameter : VolumeParameter<UVNoiseType>
    {
        public UVNoiseTypeEnumParameter(UVNoiseType value, bool overrideState = false)
            : base(value, overrideState) { }
        public sealed override void Interp(UVNoiseType from, UVNoiseType to, float t)
        {
            m_Value = from;
        }
    }
    
    
    [VolumeComponentMenu(VolumeDefine.Skill + "黑白闪 (BlackWhite4)")]
    public class BlackWhite4 : VolumeSetting
    {
        public override bool IsActive() => Enable.value;

        public BoolParameter Enable = new(false);
        //noise dissolve center
        [Space(10),Header("噪点图极坐标")]
        public ClampedFloatParameter blackBlend = new(0f, 0, 1);
        public ClampedFloatParameter whiteBlend = new(1f, 0, 1);
        public Vector2Parameter Center = new(new Vector2(0f, 0f));

        [Space(10), Header("拉伸")]
        public ClampedIntParameter uvNoise = new(0,0,3);
        public ClampedFloatParameter JitterX = new(0, -1, 1);
        public ClampedFloatParameter JitterY = new(0, -1, 1);
        
        [Space(10),Header("黑白闪控制")]
        public ColorParameter TintColor1 = new(Color.black,true,true,true);
        public ColorParameter TintColor2 = new(Color.white,true,true,true);
        
        //纯黑白强度
        public ClampedFloatParameter blackWhiteThreshold = new(0.51f, 0f, 1f);
        //黑白灰度过度
        public ClampedFloatParameter greyThreshold = new(0f, 0f, 0.51f);
        //更替闪烁
        public ClampedIntParameter Change = new(0, 0, 1);
        
        //扰动图
        [Space(10), Header("Turbulence")]
        public TextureParameter TurbulenceTex = new(null);
        public Vector4Parameter TurbulenceTex_ST = new(new Vector4(1, 1, 0, 0));
        public ClampedFloatParameter TurbulencePolarBlend = new(0f, 0f, 1f);
        public Vector2Parameter TurbulenceTexPolarTill = new(new Vector2(1,1));
        public ClampedFloatParameter TurbulencePolarRaduis = new(0f, 0f, 360f);
        
        public FloatParameter TurbulenceStrength = new(0); 
        
        public Vector2Parameter TurbulenceTexMove = new(new Vector2(0,0));
        public ClampedFloatParameter TurbulenceTexRotate = new(0f, 0f, 360f);

        [Space(10), Header("Mask图")]
        //溶解贴图
        public BoolParameter UseMask = new(false);
        public TextureParameter MaskTex = new(null);
        public Vector4Parameter MaskTexTex_ST = new(new Vector4(1, 1, 0, 0));

        //TODO 
        /*
         * 1 黑白闪控制值最大和最小，分别对应纯黑和纯白，可以使用曲线控制
         * 2 黑白闪的中间值能调整，灰度调节可以保留
         * 3 扰动溶解图添加除极坐标外的上下和水平方向，分开控制tilling 和 speed, speed 使用曲线控制
         * 4 扰动贴图范围区域通过数值可以控制
         * 5 change  参数可以通过曲线控制
         * 6 黑白闪和径向模糊的混合 添加贴图实现混合效果
         * 
         */
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Skill/BlackWhite4";
        }
    }

    public class BlackWhite4Renderer : VolumeRenderer<BlackWhite4>
    {
        //public override string ShaderName => "Hidden/PostProcessing/Skill/BlackWhite";
        public const string PROFILER_TAG = "BlackWhite4";

        static class ShaderIDs
        {
            public static readonly int ParamsID = Shader.PropertyToID("_Params");
            public static readonly int Params2ID = Shader.PropertyToID("_Params2");
            public static readonly int Params3ID = Shader.PropertyToID("_Params3");
            public static readonly int Params4ID = Shader.PropertyToID("_Params4");
            public static readonly int Params5ID = Shader.PropertyToID("_Params5");
            public static readonly int Color1ID = Shader.PropertyToID("_Color1");
            public static readonly int Color2ID = Shader.PropertyToID("_Color2");
            public static readonly int TurbulenceID = Shader.PropertyToID("_TurbulenceTex");
            public static readonly int TurbulenceTexSTID = Shader.PropertyToID("_TurbulenceTex_ST");
            public static readonly int MaskTexID = Shader.PropertyToID("_MaskTex");
            public static readonly int MaskTexSTID = Shader.PropertyToID("_MaskTex_ST");
        }

        public override void Render(CommandBuffer cmd, 
            RenderTargetIdentifier source,
            RenderTargetIdentifier target,
            ref RenderingData renderingData)
        {
            m_BlitMaterial.SetTexture(ShaderIDs.TurbulenceID, settings.TurbulenceTex.value);
            m_BlitMaterial.SetVector(ShaderIDs.TurbulenceTexSTID, settings.TurbulenceTex_ST.value);
            
           m_BlitMaterial.SetTexture(ShaderIDs.MaskTexID, settings.MaskTex.value);
            m_BlitMaterial.SetVector(ShaderIDs.MaskTexSTID, settings.MaskTexTex_ST.value);
            
            m_BlitMaterial.SetColor(ShaderIDs.Color1ID, settings.TintColor1.value);
            m_BlitMaterial.SetColor(ShaderIDs.Color2ID, settings.TintColor2.value);
            
            m_BlitMaterial.SetVector(ShaderIDs.ParamsID, 
                new Vector4(
                    settings.TurbulencePolarBlend.value, 
                    settings.TurbulencePolarRaduis.value, 
                    settings.TurbulenceTexMove.value.x, 
                    settings.TurbulenceTexMove.value.y));
            m_BlitMaterial.SetVector(ShaderIDs.Params2ID, 
                new Vector4(
                    settings.TurbulenceTexRotate.value,
                    settings.TurbulenceStrength.value,
                    settings.UseMask.value?1:0,
                    settings.Change.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params3ID, 
                new Vector4(
                    settings.blackBlend.value, 
                    settings.whiteBlend.value, 
                    settings.JitterX.value,
                    settings.JitterY.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params4ID, 
                new Vector4(
                    settings.blackWhiteThreshold.value, 
                    settings.greyThreshold.value, 
                    settings.Center.value.x,
                    settings.Center.value.y));
            
            m_BlitMaterial.SetVector(ShaderIDs.Params5ID, 
                new Vector4(
                    settings.uvNoise.value, 
            settings.TurbulenceTexPolarTill.value.x, 
                    settings.TurbulenceTexPolarTill.value.y,
                    0));
            
            cmd.Blit(source, target, m_BlitMaterial, 0);
        }
    }
}
