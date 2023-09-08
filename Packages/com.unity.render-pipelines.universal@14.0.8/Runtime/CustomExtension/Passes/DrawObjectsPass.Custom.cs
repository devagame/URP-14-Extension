namespace UnityEngine.Rendering.Universal.Internal
{
    public partial class DrawObjectsPass: ScriptableRenderPass
    {
       static bool m_IsUICamera;
        
        public void Setup(bool isUICamera = false)
        {
            m_IsUICamera = isUICamera;
        }
        
    }
}