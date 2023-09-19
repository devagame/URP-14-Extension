using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.VOLUMEROOT + "*深度检测描边 (edge outline)")]
    public class EdageOutline : VolumeSetting
    {
        public override bool IsActive() => isActive.value;
        public BoolParameter isActive = new BoolParameter(false);
        public ClampedFloatParameter sampleOff = new ClampedFloatParameter(0.5f, 0, 5);
        public ClampedFloatParameter checkValue = new ClampedFloatParameter(0.01f, 0, 10);
        
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/EdageOutline";
        }
    }

    public class EdageOutlineRenderer : VolumeRenderer<EdageOutline>
    {
        public const string PROFILER_TAG = "EdgeOutline";
        //public override string ShaderName => "Hidden/PostProcessing/EdageOutline";
        
        static class ShaderContants
        {
            public static readonly int sampleDisID = Shader.PropertyToID("_SampleDistance");
            public static readonly int checkValueID = Shader.PropertyToID("_CheckValue");
        }
        
        public override void Render(CommandBuffer cmd, 
            RenderTargetIdentifier source, 
            RenderTargetIdentifier target, 
            ref RenderingData renderingData)
        {
            m_BlitMaterial.SetFloat(ShaderContants.sampleDisID, settings.sampleOff.value);
            m_BlitMaterial.SetFloat(ShaderContants.checkValueID, settings.checkValue.value);

            cmd.Blit(source, target, m_BlitMaterial);
        }
    }


}