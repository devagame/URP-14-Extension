namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Renders the post-processing effect stack.
    /// </summary>
    internal partial class PostProcessPass : ScriptableRenderPass
    {
        bool m_FinalPassUseRenderColorBuff = false;
        // if camera target to sRGB
        bool m_hasExternalPostPasses = false;
        
        public static bool sUseCustomPostProcess = true;
        XPostProcessing.CustomPostProcess m_CustomPostProcess = null;
        public void SetupFinalPass(
            in RTHandle source,
            bool useSwapBuffer = false,
            bool enableSRGBConversion = true,
            bool hasExternalPasses = false, //add new
            bool useRenderColorBuff = false //add new
        )
        {
            m_Source = source;
            m_Destination = k_CameraTarget;
            m_IsFinalPass = true;
            m_HasFinalPass = false;
            m_EnableColorEncodingIfNeeded = enableSRGBConversion;
            m_UseSwapBuffer = useSwapBuffer;

            m_hasExternalPostPasses = hasExternalPasses;
            m_FinalPassUseRenderColorBuff = useRenderColorBuff;
        }
    }
}