namespace UnityEngine.Rendering.Universal.Internal
{
    internal sealed partial class RenderTargetBufferSystem
    {
        public static RenderTextureDescriptor GetDesc()
        {
            return m_Desc;
        }
        
        public void ReSizeFrontBuffer(CommandBuffer cmd, int width,int height)
        {
            RTHandles.Release( frontBuffer.rtMSAA);
            //cmd.ReleaseTemporaryRT(Shader.PropertyToID(frontBuffer.name));
            var tempDes = m_Desc;
            tempDes.width = width;
            tempDes.height = height;
            RenderingUtils.ReAllocateIfNeeded(ref frontBuffer.rtMSAA, tempDes, m_FilterMode, TextureWrapMode.Clamp, name: frontBuffer.name);
            //cmd.GetTemporaryRT(frontBuffer.name, tempDes, m_FilterMode);
            
            RTHandles.Release( frontBuffer.rtResolve);
            //cmd.ReleaseTemporaryRT(Shader.PropertyToID(frontBuffer.name));
            var tempDes0 = m_Desc;
            tempDes0.width = width;
            tempDes0.height = height;
            RenderingUtils.ReAllocateIfNeeded(ref frontBuffer.rtResolve, tempDes0, m_FilterMode, TextureWrapMode.Clamp, name: frontBuffer.name);
        }
        
        public void ReSizeBackBufferAndSave(CommandBuffer cmd, int width, int height)
        {
            RTHandles.Release(backBuffer.rtMSAA);
            //cmd.ReleaseTemporaryRT(Shader.PropertyToID(backBuffer.name));
            m_Desc.width = width;
            m_Desc.height = height;
            RenderingUtils.ReAllocateIfNeeded(ref backBuffer.rtMSAA, m_Desc, m_FilterMode, TextureWrapMode.Clamp, name: backBuffer.name);
            //cmd.GetTemporaryRT(backBuffer.name, m_Desc, m_FilterMode);
            
            RTHandles.Release(backBuffer.rtResolve);
            //cmd.ReleaseTemporaryRT(Shader.PropertyToID(backBuffer.name));
            m_Desc.width = width;
            m_Desc.height = height;
            RenderingUtils.ReAllocateIfNeeded(ref backBuffer.rtResolve, m_Desc, m_FilterMode, TextureWrapMode.Clamp, name: backBuffer.name);
        }
    }
}