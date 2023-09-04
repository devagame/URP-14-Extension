namespace UnityEngine.Rendering.Universal
{
    internal static partial class ShaderPropertyId
    {
        public static readonly int isInUICamera = Shader.PropertyToID("_IsInUICamera");
    }

    public partial struct CameraData
    {
        public bool nextIsUICamera;
        public bool isUICamera;
    }

    public static partial class ShaderKeywordStrings
    {
        public const string SRGBToLinearConversion = "_SRGB_TO_LINEAR_CONVERSION";
    }
}