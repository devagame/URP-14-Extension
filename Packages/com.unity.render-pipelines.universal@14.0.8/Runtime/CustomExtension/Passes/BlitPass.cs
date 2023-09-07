using System;

namespace UnityEngine.Rendering.Universal.Internal
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the destination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    public class BlitPass : ScriptableRenderPass
    {
        Material m_BlitMaterial;

        private int m_Width;
        private int m_Height;
        private BlitColorTransform m_BlitColorTransform;
        public enum BlitColorTransform
        {
            None,
            Gamma2Line,
            Line2Gamma
        }

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public BlitPass(RenderPassEvent evt, Material blitMaterial)
        {
            base.profilingSampler = new ProfilingSampler(nameof(BlitPass));

            m_BlitMaterial = blitMaterial;
            renderPassEvent = evt;
            base.useNativeRenderPass = false;
        }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
        public void Setup(int width,int height, BlitColorTransform blitColorTransform)
        {
            m_Width = width;
            m_Height = height;
            m_BlitColorTransform = blitColorTransform;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd,ref renderingData);
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_BlitMaterial == null)
            {
                Debug.LogErrorFormat("Missing {0}. {1} render pass will not execute. Check for missing reference in the renderer resources.", m_BlitMaterial, GetType().Name);
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();

            var renderer = renderingData.cameraData.renderer as UniversalRenderer;
            var colorBuffer = renderer.m_ColorBufferSystem;
            
            bool useDrawProceduleBlit = renderingData.cameraData.xr.enabled;
            if (m_BlitColorTransform == BlitColorTransform.Gamma2Line)
            {
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, false);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion, true);
            }
            else if (m_BlitColorTransform == BlitColorTransform.Line2Gamma)
            {
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion,true);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion,false);
            }
            
            RTHandle source,target;
            source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            
            bool needChangeSize = RenderTargetBufferSystem.GetDesc().width != m_Width|| RenderTargetBufferSystem.GetDesc().height!= m_Height;
            if (needChangeSize)
            {
                colorBuffer.ReSizeFrontBuffer(cmd,m_Width,m_Height);
            }
            target = colorBuffer.GetFrontBuffer();
            
            var pixelRect = new Rect(
                Vector2.zero, 
                new Vector2(m_Width, m_Height));
            
            RenderingUtils.Blit(cmd, 
                source,
                pixelRect, 
                target,
                RenderBufferLoadAction.DontCare,
                RenderBufferStoreAction.Store,
                ClearFlag.None,
                Color.black,
                m_BlitMaterial, 0);
            
            CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.LinearToSRGBConversion, false);
            CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SRGBToLinearConversion,false);
            if (needChangeSize)
            {
                //只用重置Depth ,multiBuff会自动重置Back
                //colorBuffer.ReSizeBackBuffer(cmd);
                renderer.ResizeDepth(cmd,m_Width, m_Height,ref renderingData);
            }
            renderer.SwapColorBuffer(cmd);
           
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }
    }
}
