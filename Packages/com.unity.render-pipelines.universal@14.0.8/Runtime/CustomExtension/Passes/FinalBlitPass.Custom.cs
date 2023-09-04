using System;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
    /// <summary>
    /// Copy the given color target to the current camera target
    ///
    /// You can use this pass to copy the result of rendering to
    /// the camera target. The pass takes the screen viewport into
    /// consideration.
    /// </summary>
    public partial class FinalBlitPass : ScriptableRenderPass
    {
        bool m_IsLine = true;
        
        /// <summary>
        /// Configure the pass
        /// </summary>
        /// <param name="baseDescriptor"></param>
        /// <param name="colorHandle"></param>
        public void Setup(RenderTextureDescriptor baseDescriptor, RTHandle colorHandle,bool isLine = true)
        {
            this.m_IsLine = isLine;
            m_Source = colorHandle;
        }
    }
    
    
}