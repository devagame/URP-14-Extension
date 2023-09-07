namespace UnityEngine.Rendering.Universal.Internal
{
    internal sealed partial class RenderTargetBufferSystem
    {
        public static RenderTextureDescriptor GetDesc()
        {
            return m_Desc;
        }

        public RTHandle GetFrontBuffer()
        {
            if (!m_AllowMSAA && frontBuffer.msaa > 1)
                frontBuffer.msaa = 1;
            return (m_AllowMSAA && frontBuffer.msaa > 1) ? frontBuffer.rtMSAA : frontBuffer.rtResolve;
        }
        
        public void ReSizeFrontBuffer(CommandBuffer cmd, int width,int height)
        {
            m_Desc.width = width;
            m_Desc.height = height;
            
            var desc = m_Desc;
            desc.msaaSamples = frontBuffer.msaa;
            if (desc.msaaSamples > 1)
                RenderingUtils.ReAllocateIfNeeded(ref frontBuffer.rtMSAA, desc, m_FilterMode, TextureWrapMode.Clamp, name: frontBuffer.name);
            
            desc.msaaSamples = 1;
            RenderingUtils.ReAllocateIfNeeded(ref frontBuffer.rtResolve, desc, m_FilterMode, TextureWrapMode.Clamp, name: frontBuffer.name);
          
            cmd.SetGlobalTexture(frontBuffer.name, frontBuffer.rtResolve);
        }
   
        public void ReSizeBackBuffer(CommandBuffer cmd)
        {
            var desc = m_Desc;
            desc.msaaSamples = backBuffer.msaa;
            if (desc.msaaSamples > 1)
                RenderingUtils.ReAllocateIfNeeded(ref backBuffer.rtMSAA, desc, m_FilterMode, TextureWrapMode.Clamp, name: backBuffer.name);
            
            desc.msaaSamples = 1;
            RenderingUtils.ReAllocateIfNeeded(ref backBuffer.rtResolve, desc, m_FilterMode, TextureWrapMode.Clamp, name: backBuffer.name);
          
            cmd.SetGlobalTexture(backBuffer.name, backBuffer.rtResolve);
        }
    }
}