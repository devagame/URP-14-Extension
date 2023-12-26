using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    public class GaussianBlur2RenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing; //在后处理前执行我们的颜色校正
            public Shader shader; //汇入shader
        }

        public Settings settings = new Settings(); //开放设置
        GaussianBlur2Pass colorTintPass; //设置渲染pass

        public override void Create() //新建pass
        {
            this.name = "ColorTintPass"; //名字
            colorTintPass = new GaussianBlur2Pass(RenderPassEvent.BeforeRenderingPostProcessing, settings.shader); //初始化
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) //Pass逻辑
        {
            
            //【渲染设置】
            /*var stack = VolumeManager.instance.stack; //传入volume数据
          var  colorTint = stack.GetComponent<GaussianBlur2>(); //拿到我们的Volume
            if (colorTint == null)
            {
                return;
            }
            
            if(!colorTint.IsActive())
                return;*/
            
            colorTintPass.Setup(renderer /*,renderer.cameraColorTarget*/); //初始化
            renderer.EnqueuePass(colorTintPass); //汇入队列
        }
    }

//【执行pass】
    public class GaussianBlur2Pass : ScriptableRenderPass
    {
        static readonly string k_RenderTag = "ColorTint Effects"; //设置tags
        static readonly int MainTexId = Shader.PropertyToID("_MainTex"); //设置主贴图
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetColorTint"); //设置暂存贴图

        GaussianBlur2 colorTint; //提供一个Volume传递位置

        Material colorTintMaterial; //后处理使用材质
        //RenderTargetIdentifier currentTarget;//设置当前渲染目标

        #region 设置渲染事件

        public GaussianBlur2Pass(RenderPassEvent evt, Shader ColorTintShader)
        {
            renderPassEvent = evt; //设置渲染事件位置
            var shader = ColorTintShader; //汇入shader
            //不存在则返回
            if (shader == null)
            {
                Debug.LogError("不存在ColorTint shader");
                return;
            }

            colorTintMaterial = CoreUtils.CreateEngineMaterial(ColorTintShader); //新建材质
        }

        #endregion

        #region 初始化

        private ScriptableRenderer renderer;

        public void Setup(ScriptableRenderer renderer /*,in RenderTargetIdentifier currentTarget*/)
        {
            //this.currentTarget = currentTarget;
            this.renderer = renderer;
        }

        #endregion

        #region 执行

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
            //【渲染设置】
            var stack = VolumeManager.instance.stack; //传入volume数据
            colorTint = stack.GetComponent<GaussianBlur2>(); //拿到我们的Volume
            if (colorTint == null)
            {
                return;
            }
            
            if(!colorTint.IsActive())
                return;
            
            if (colorTintMaterial == null) //材质是否存在
            {
                Debug.LogError("材质初始化失败");
                return;
            }

            //摄像机关闭后处理
            if (!renderingData.cameraData.postProcessEnabled)
            {
                return;
            }

            var cmd = CommandBufferPool.Get(k_RenderTag); //设置抬头
            Render(cmd, ref renderingData); //设置渲染函数
            context.ExecuteCommandBuffer(cmd); //执行函数
            CommandBufferPool.Release(cmd); //释放
        }

        #endregion

        #region 渲染
        private const string PROFILER_TAG = "GaussianBlur";

        static class ShaderIDs
        {
            internal static readonly int BlurRadius = Shader.PropertyToID("_BlurOffset");
            internal static readonly int BufferRT1 = Shader.PropertyToID("_BufferRT1");
            internal static readonly int BufferRT2 = Shader.PropertyToID("_BufferRT2");
        }
        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData; //汇入摄像机数据
            var camera = cameraData.camera; //传入摄像机数据
            var source = renderer.cameraColorTarget; // currentTarget;//当前渲染图片汇入
            int destination = TempTargetId; //渲染目的地

            /*
            colorTintMaterial.SetColor("_ColorTint", colorTint.colorChange.value); //汇入颜色校正
            colorTintMaterial.SetFloat("_BlurRange", colorTint.BlurRange.value); //汇入颜色校正

            cmd.SetGlobalTexture(MainTexId, source); //汇入当前渲染图片
            cmd.GetTemporaryRT(destination, cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight, 0,
                FilterMode.Trilinear, RenderTextureFormat.Default); //设置目标贴图
            for (int i = 0; i < colorTint.BlurTimes.value; i++)
            {
                cmd.Blit(source, destination, colorTintMaterial, 1); //设置后处理
                cmd.Blit(destination, source, colorTintMaterial, 2); //传入颜色校正
            }
            */
            
            
            
            cmd.BeginSample(PROFILER_TAG);

            int RTWidth = (int)(Screen.width / colorTint.RTDownScaling.value);
            int RTHeight = (int)(Screen.height / colorTint.RTDownScaling.value);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT1, RTWidth, RTHeight, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(ShaderIDs.BufferRT2, RTWidth, RTHeight, 0, FilterMode.Bilinear);

            // downsample screen copy into smaller RT
            cmd.Blit(source, ShaderIDs.BufferRT1);


            for (int i = 0; i < colorTint.Iteration.value; i++)
            {
                // horizontal blur
                colorTintMaterial.SetVector(ShaderIDs.BlurRadius, new Vector4(colorTint.BlurRadius.value / Screen.width, 0, 0, 0));
                cmd.Blit(ShaderIDs.BufferRT1, ShaderIDs.BufferRT2, colorTintMaterial);

                // vertical blur
                colorTintMaterial.SetVector(ShaderIDs.BlurRadius, new Vector4(0, colorTint.BlurRadius.value / Screen.height, 0, 0));
                cmd.Blit(ShaderIDs.BufferRT2, ShaderIDs.BufferRT1, colorTintMaterial);
            }

            // Render blurred texture in blend pass
            cmd.Blit(ShaderIDs.BufferRT1, source, colorTintMaterial);

            // release
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT1);
            cmd.ReleaseTemporaryRT(ShaderIDs.BufferRT2);

            cmd.EndSample(PROFILER_TAG);
            
        }

        #endregion
    }
}