using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
    public class BlurConfig : ScriptableObject
    {
        [SerializeField] public SourceRenderType sourceRenderType = SourceRenderType.UIStyle;
    }
}