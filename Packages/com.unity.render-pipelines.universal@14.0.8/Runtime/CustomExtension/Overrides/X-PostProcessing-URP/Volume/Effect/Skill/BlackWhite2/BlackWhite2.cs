using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Skill + "黑白闪 (BlackWhite2)")]
    public class BlackWhite2 : VolumeSetting
    {
        public override bool IsActive() => Enable.value;

        public BoolParameter Enable = new BoolParameter(false);
        //noise dissolve center
        [Space(10),Header("噪点图极坐标")]
        public Vector2Parameter Center = new Vector2Parameter(new Vector2(0.5f, 0.5f));
        public ClampedFloatParameter TillingX = new ClampedFloatParameter(0.1f, 0, 20);
        public ClampedFloatParameter TillingY = new ClampedFloatParameter(5, 0, 20);
        
        [Space(10),Header("黑白闪控制")]
        //整体颜色对比
        public ColorParameter TintColor = new ColorParameter(Color.white);
        
        //纯黑白强度
        public ClampedFloatParameter Threshold = new ClampedFloatParameter(0.51f, 0f, 1f);
        
        //黑白灰度过度
        public ClampedFloatParameter greyThreshold = new ClampedFloatParameter(0f, 0f, 0.51f);
        
        //更替闪烁
        public ClampedIntParameter Change = new ClampedIntParameter(0, 0, 1);
        
        //扰动图
        [Space(10),Header("噪点图")]
        public TextureParameter PolarNoiseTex = new TextureParameter(null);
        public Vector4Parameter PolarNoiseTexST = new Vector4Parameter(new Vector4(1, 1, 0, 0));
        // 贴图移动速度
        public ClampedFloatParameter NoiseSpeed = new ClampedFloatParameter(0.1f, -10, 10);
        
        [Space(10),Header("Mask图")]
        //溶解贴图
        public TextureParameter PolarDissolveTex = new TextureParameter(null);
        public Vector4Parameter PolarDissolveTexST = new Vector4Parameter(new Vector4(1, 1, 0, 0));
        public ClampedFloatParameter DissolveSpeed = new ClampedFloatParameter(0.1f, -10, 10);

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
            return "Hidden/PostProcessing/Skill/BlackWhite2";
        }
    }

    public class BlackWhite2Renderer : VolumeRenderer<BlackWhite2>
    {
        //public override string ShaderName => "Hidden/PostProcessing/Skill/BlackWhite";
        public const string PROFILER_TAG = "BlackWhite2";

        static class ShaderIDs
        {
            public static readonly int ParamsID = Shader.PropertyToID("_Params");
            public static readonly int Params2ID = Shader.PropertyToID("_Params2");
            public static readonly int Params3ID = Shader.PropertyToID("_Params3");
            public static readonly int ColorID = Shader.PropertyToID("_Color");
            public static readonly int NoiseTexID = Shader.PropertyToID("_NoiseTex");
            public static readonly int NoiseTexSTID = Shader.PropertyToID("_NoiseTex_ST");
            public static readonly int DissolveTexID = Shader.PropertyToID("_DissolveTex");
            public static readonly int DissolveTexSTID = Shader.PropertyToID("_DissolveTex_ST");
        }

        public override void Render(CommandBuffer cmd, 
            RenderTargetIdentifier source,
            RenderTargetIdentifier target,
            ref RenderingData renderingData)
        {
            m_BlitMaterial.SetTexture(ShaderIDs.NoiseTexID, settings.PolarNoiseTex.value);
            m_BlitMaterial.SetVector(ShaderIDs.NoiseTexSTID, settings.PolarNoiseTexST.value);
            m_BlitMaterial.SetTexture(ShaderIDs.DissolveTexID, settings.PolarDissolveTex.value);
            m_BlitMaterial.SetVector(ShaderIDs.DissolveTexSTID, settings.PolarDissolveTexST.value);
            m_BlitMaterial.SetColor(ShaderIDs.ColorID, settings.TintColor.value);
            m_BlitMaterial.SetVector(ShaderIDs.ParamsID, new Vector4(settings.Threshold.value , settings.Center.value.x, settings.Center.value.y, settings.DissolveSpeed.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params2ID, new Vector4(settings.TillingX.value, settings.TillingY.value, settings.NoiseSpeed.value, settings.Change.value));
            m_BlitMaterial.SetVector(ShaderIDs.Params3ID, new Vector4(settings.greyThreshold.value, 0, 0,0));
            
            cmd.Blit(source, target, m_BlitMaterial, 0);
        }

    }
}
