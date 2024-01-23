using UnityEngine;
using UnityEngine.Rendering;

namespace LeTai.Asset.TranslucentImage
{
public class ScalableBlur : IBlurAlgorithm
{
    bool               isBirp;
    Material           material;
    ScalableBlurConfig config;

    const int BLUR_PASS      = 0;
    const int CROP_BLUR_PASS = 1;

    Material Material
    {
        get
        {
            if (material == null)
                Material = new Material(Shader.Find(isBirp
                                                        ? "Hidden/EfficientBlur"
                                                        : "Hidden/EfficientBlur_UniversalRP"));

            return material;
        }
        set => material = value;
    }

    public void Init(BlurConfig config, bool isBirp)
    {
        this.isBirp = isBirp;
        this.config = (ScalableBlurConfig)config;
    }

    public void Blur(
        CommandBuffer          cmd,
        RenderTargetIdentifier src,
        Rect                   srcCropRegion,
        BackgroundFill         backgroundFill,
        RenderTexture          target
    )
    {
        //target = config.SourceRenderType == SourceRenderType.UIStyle ? target : src.r;
        
        float radius = ScaleWithResolution(config.Radius,
                                           target.width * srcCropRegion.width,
                                           target.height * Mathf.Abs(srcCropRegion.height));
        ConfigMaterial(radius, srcCropRegion.ToMinMaxVector(), backgroundFill);

        int firstDownsampleFactor = config.Iteration > 0 ? 1 : 0;
        int stepCount             = Mathf.Max(config.Iteration * 2 - 1, 1);

        int firstIRT = ShaderId.intermediateRT[0];
        CreateTempRenderTextureFrom(cmd, firstIRT, target, firstDownsampleFactor);
        cmd.BlitCustom(src, firstIRT, Material, CROP_BLUR_PASS, isBirp);


        for (var i = 1; i < stepCount; i++)
        {
            BlurAtDepth(cmd, i, target);
        }

        if (config.SourceRenderType == SourceRenderType.UIStyle)
        {
            cmd.BlitCustom(ShaderId.intermediateRT[stepCount - 1],
                target,
                Material,
                BLUR_PASS, isBirp);
        }
        else
        {
            cmd.BlitCustom(ShaderId.intermediateRT[stepCount - 1],
                src,
                Material,
                BLUR_PASS, isBirp);
        }
        
        CleanupIntermediateRT(cmd, stepCount);
    }

    void CreateTempRenderTextureFrom(
        CommandBuffer cmd,
        int           nameId,
        RenderTexture src,
        int           downsampleFactor
    )
    {
        var desc = src.descriptor;
        desc.width  = src.width >> downsampleFactor; //= width / 2^downsample
        desc.height = src.height >> downsampleFactor;

        cmd.GetTemporaryRT(nameId, desc, FilterMode.Bilinear);
    }

    protected virtual void BlurAtDepth(CommandBuffer cmd, int depth, RenderTexture baseTexture)
    {
        int sizeLevel = SimplePingPong(depth, config.Iteration - 1) + 1;
        sizeLevel = Mathf.Min(sizeLevel, config.MaxDepth);
        CreateTempRenderTextureFrom(cmd, ShaderId.intermediateRT[depth], baseTexture, sizeLevel);

        cmd.BlitCustom(ShaderId.intermediateRT[depth - 1],
                       ShaderId.intermediateRT[depth],
                       Material, 0, isBirp);
    }

    private void CleanupIntermediateRT(CommandBuffer cmd, int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            cmd.ReleaseTemporaryRT(ShaderId.intermediateRT[i]);
        }
    }

    protected void ConfigMaterial(float radius, Vector4 cropRegion, BackgroundFill backgroundFill)
    {
        switch (backgroundFill.mode)
        {
        case BackgroundFillMode.None:
            Material.EnableKeyword("BACKGROUND_FILL_NONE");
            Material.DisableKeyword("BACKGROUND_FILL_COLOR");
            break;
        case BackgroundFillMode.Color:
            Material.EnableKeyword("BACKGROUND_FILL_COLOR");
            Material.DisableKeyword("BACKGROUND_FILL_NONE");
            Material.SetColor(ShaderId.BACKGROUND_COLOR, backgroundFill.color);
            break;
        }
        Material.SetFloat(ShaderId.RADIUS, radius);
        Material.SetVector(ShaderId.CROP_REGION, cropRegion);
    }

    ///<summary>
    /// Relative blur size to maintain same look across multiple resolution
    /// </summary>
    float ScaleWithResolution(float baseRadius, float width, float height)
    {
        float scaleFactor = Mathf.Min(width, height) / 1080f;
        scaleFactor = Mathf.Clamp(scaleFactor, .5f, 2f); //too much variation cause artifact
        return baseRadius * scaleFactor;
    }

    public static int SimplePingPong(int t, int max)
    {
        if (t > max)
            return 2 * max - t;
        return t;
    }
}
}
