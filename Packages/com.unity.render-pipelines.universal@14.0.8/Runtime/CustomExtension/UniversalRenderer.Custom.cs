﻿using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
    public sealed partial class UniversalRenderer
    {
        public  bool sUISplitEnable = false;
        public  bool sIsGammaCorrectEnable = true;

        public  bool sEnableUICameraUseSwapBuffer = false;
        
        public CopyTransparentPass m_CopyTransparentPass;
        
        //Material m_BlitCustom = null;
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
        
        public void ResizeDepth(CommandBuffer cmd,int width,int height,ref RenderingData renderingData)
        {
            var depthDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            depthDescriptor.width = width;
            depthDescriptor.height = height;
            
            depthDescriptor.useMipMap = false;
            depthDescriptor.autoGenerateMips = false;
            depthDescriptor.bindMS = false;

            bool hasMSAA = depthDescriptor.msaaSamples > 1 && (SystemInfo.supportsMultisampledTextures != 0);

            // if MSAA is enabled and we are not resolving depth, which we only do if the CopyDepthPass is AfterTransparents,
            // then we want to bind the multisampled surface.
            if (hasMSAA)
            {
                // if depth priming is enabled the copy depth primed pass is meant to do the MSAA resolve, so we want to bind the MS surface
                if (IsDepthPrimingEnabled(ref renderingData.cameraData))
                    depthDescriptor.bindMS = true;
                else
                    depthDescriptor.bindMS = !(RenderingUtils.MultisampleDepthResolveSupported() && m_CopyDepthMode == CopyDepthMode.AfterTransparents);
            }

            // binding MS surfaces is not supported by the GLES backend, and it won't be fixed after investigating
            // the high performance impact of potential fixes, which would make it more expensive than depth prepass (fogbugz 1339401 for more info)
            if (IsGLESDevice())
                depthDescriptor.bindMS = false;

            depthDescriptor.graphicsFormat = GraphicsFormat.None;
            depthDescriptor.depthStencilFormat = k_DepthStencilFormat;
            
            RenderingUtils.ReAllocateIfNeeded(ref m_CameraDepthAttachment, depthDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_CameraDepthAttachment");
            cmd.SetGlobalTexture(m_CameraDepthAttachment.name, m_CameraDepthAttachment.nameID);
            
            m_ActiveCameraDepthAttachment = m_CameraDepthAttachment; //立即激活
        }
        
        public override void FinishRendering(CommandBuffer cmd,RenderingData renderingData)
        {
            m_ColorBufferSystem.Clear();
            //************** CUSTOM ADD START ***************//
            if(sUISplitEnable && renderingData.cameraData.isUICamera)
            {
                //m_ActiveCameraDepthAttachment?.Release();
            }
            //*************** CUSTOM ADD END ****************//
            m_ActiveCameraColorAttachment = null;
            m_ActiveCameraDepthAttachment = null;
        }
    }
}