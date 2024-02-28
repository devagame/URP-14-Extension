using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting.APIUpdating;
using ShaderIdCommon = LeTai.Asset.TranslucentImage.ShaderId;

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
    enum RendererType
    {
        Universal,
        Renderer2D
    }

    [MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
    struct TISPassData
    {
        public RendererType rendererType;
        public RenderTargetIdentifier cameraColorTarget;
        public TranslucentImageSource blurSource;
        public IBlurAlgorithm blurAlgorithm;
        public RenderPassEvent renderOrder;
        public bool canvasDisappearWorkaround;
        public bool isPreviewing;
    }

    [MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
    public class TranslucentImageBlurRenderPass : ScriptableRenderPass
    {
        private const string PROFILER_TAG = "Translucent Image Source";

        readonly URPRendererInternal urpRendererInternal;
        readonly RenderTargetIdentifier afterPostprocessTexture;

        TISPassData currentPassData;
        Material previewMaterial;

        public Material PreviewMaterial
        {
            get
            {
                if (!previewMaterial)
                    previewMaterial = CoreUtils.CreateEngineMaterial("Hidden/FillCrop_UniversalRP");

                return previewMaterial;
            }
        }

        internal TranslucentImageBlurRenderPass(URPRendererInternal urpRendererInternal)
        {
            this.urpRendererInternal = urpRendererInternal;
            afterPostprocessTexture = new RenderTargetIdentifier(Shader.PropertyToID("_AfterPostProcessTexture"), 0,
                CubemapFace.Unknown, -1);
        }

        RenderTargetIdentifier GetAfterPostColor()
        {
#if URP12_OR_NEWER
            return urpRendererInternal.GetAfterPostColor();
#else
        return afterPostprocessTexture;
#endif
        }

        ~TranslucentImageBlurRenderPass()
        {
            CoreUtils.Destroy(previewMaterial);
        }

        internal void Setup(TISPassData passData)
        {
            currentPassData = passData;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(PROFILER_TAG);
            RenderTargetIdentifier source;


#if UNITY_2022_3_OR_NEWER
            source = urpRendererInternal.GetBackBuffer();
#else
#if URP12_OR_NEWER
        if (currentPassData.rendererType == RendererType.Universal)
        {
            source = urpRendererInternal.GetBackBuffer();
        }
        else
        {
#endif
            bool useAfterPostTex = renderingData.cameraData.postProcessEnabled;
#if URP12_OR_NEWER
            useAfterPostTex &= currentPassData.renderOrder == RenderOrder.AfterPostProcessing;
#endif
            source = useAfterPostTex
                         ? GetAfterPostColor()
                         : currentPassData.cameraColorTarget;
#if URP12_OR_NEWER
        }
#endif
#endif

            var blurSource = currentPassData.blurSource;
            var blurredScreen = blurSource.BlurredScreen;
            var blurRegion = blurSource.BlurRegion;

            currentPassData.blurAlgorithm.Blur(cmd,
                source,
                blurRegion,
                currentPassData.blurSource.BackgroundFill,
                blurredScreen);

            bool shouldResetTarget =
                currentPassData.canvasDisappearWorkaround && renderingData.cameraData.resolveFinalTarget;
            if (currentPassData.isPreviewing)
            {
                PreviewMaterial.SetVector(ShaderIdCommon.CROP_REGION, blurRegion.ToMinMaxVector());
                RenderTargetIdentifier previewDst = shouldResetTarget
                    ? BuiltinRenderTextureType.CameraTarget
                    : source;
                cmd.BlitCustom(blurredScreen,
                    previewDst,
                    PreviewMaterial,
                    0);
            }

            if (shouldResetTarget)
                CoreUtils.SetRenderTarget(cmd, BuiltinRenderTextureType.CameraTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}