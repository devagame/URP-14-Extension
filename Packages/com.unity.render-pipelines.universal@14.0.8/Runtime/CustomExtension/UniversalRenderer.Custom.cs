namespace UnityEngine.Rendering.Universal
{
    public sealed partial class UniversalRenderer
    {
        public static bool sUISplitEnable = false;
        public static bool sIsGammaCorrectEnable = true;

        public static bool sEnableUICameraUseSwapBuffer = false;
        
        Material m_BlitCustom = null;
        public bool IsGammaCorrectEnable(ref CameraData cameraData)
        {
            return sUISplitEnable&&sIsGammaCorrectEnable && cameraData.isUICamera;
        }

        public void SetUIGammaController(UniversalRendererData data)
        {
            sUISplitEnable = data.UISplitEnable;
            sIsGammaCorrectEnable = data.IsGammaCorrectEnable;
            sEnableUICameraUseSwapBuffer = data.EnableUICameraUseSwapBuffer;
        }
        public void ResizeDepth(CommandBuffer cmd, RenderTextureDescriptor descriptor,int width,int height)
        {
            if (m_ActiveCameraDepthAttachment.nameID != BuiltinRenderTextureType.CameraTarget)
            {
                //cmd.ReleaseTemporaryRT(m_ActiveCameraDepthAttachment.nameID.id);
                var depthDescriptor = descriptor;
                depthDescriptor.useMipMap = false;
                depthDescriptor.autoGenerateMips = false;
                depthDescriptor.width = width;
                depthDescriptor.height = height;
                depthDescriptor.bindMS = depthDescriptor.msaaSamples > 1 && (SystemInfo.supportsMultisampledTextures != 0);

                // binding MS surfaces is not supported by the GLES backend, and it won't be fixed after investigating
                // the high performance impact of potential fixes, which would make it more expensive than depth prepass (fogbugz 1339401 for more info)
                if (IsGLESDevice())
                    depthDescriptor.bindMS = false;

                depthDescriptor.colorFormat = RenderTextureFormat.Depth;
                depthDescriptor.depthBufferBits = k_DepthBufferBits;

                RenderingUtils.ReAllocateIfNeeded(
                    ref m_ActiveCameraDepthAttachment,
                    depthDescriptor,
                    wrapMode: TextureWrapMode.Clamp,
                    name: m_ActiveCameraDepthAttachment.name);
                //cmd.GetTemporaryRT(m_ActiveCameraDepthAttachment.id, depthDescriptor, FilterMode.Point);
            }
        }
    }
}