namespace UnityEngine.Rendering.Universal.Internal
{
    public partial class DrawObjectsPass: ScriptableRenderPass
    {
        bool m_IsUICamera;
        
        public void Setup(bool isUICamera = false)
        {
            this.m_IsUICamera = isUICamera;
        }
        
    }
}