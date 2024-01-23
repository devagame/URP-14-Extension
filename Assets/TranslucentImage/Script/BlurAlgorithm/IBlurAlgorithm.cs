using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace LeTai.Asset.TranslucentImage
{
    public enum BlurAlgorithmType
    {
        ScalableBlur
    }

    public enum SourceRenderType
    {
        UIStyle,
        Background,
    }

    public enum BackgroundFillMode
    {
        None,
        Color
    }

    [Serializable]
    public class BackgroundFill
    {
        public BackgroundFillMode mode = BackgroundFillMode.None;
        public Color color = Color.white;
    }

    public interface IBlurAlgorithm
    {
        void Init(BlurConfig config, bool isBirp);

        void Blur(
            CommandBuffer cmd,
            RenderTargetIdentifier src,
            Rect srcCropRegion,
            BackgroundFill backgroundFill,
            RenderTexture target
        );
    }
}