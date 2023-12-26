using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using XPostProcessing;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Blur + "高斯模糊2 (Gaussian Blur2)")]
    public class GaussianBlur2 : VolumeSetting
    {
        //【设置颜色参数】
        public ColorParameter colorChange = new ColorParameter(Color.white, true); //如果有两个true,则为HDR设置

        //【高斯模糊：次数】
        public IntParameter BlurTimes = new ClampedIntParameter(1, 1, 5); //模糊次数限制在0-5

        //【高斯模糊：半径】
        public FloatParameter BlurRange = new ClampedFloatParameter(0f, 0.0f, 0.5f); //模糊半径

        
        public FloatParameter BlurRadius = new ClampedFloatParameter(0f, 0f, 5f);
        public IntParameter Iteration = new ClampedIntParameter(6, 1, 15);
        public FloatParameter RTDownScaling = new ClampedFloatParameter(1f, 1f, 8f);

        
        public override bool IsActive()
        {
            return BlurRange.value > 0;
        }

        public override string GetShaderName()
        {
            throw new System.NotImplementedException();
        }
    }
}