using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Beautify.Universal.Beautify;

namespace Beautify.Universal {
    public class BeautifyRendererFeature : ScriptableRendererFeature {

        public const string SKW_SHARPEN = "BEAUTIFY_SHARPEN";
        public const string SKW_TONEMAP_ACES = "BEAUTIFY_TONEMAP_ACES";
        public const string SKW_LUT = "BEAUTIFY_LUT";
        public const string SKW_LUT3D = "BEAUTIFY_3DLUT";
        public const string SKW_BLOOM = "BEAUTIFY_BLOOM";
        public const string SKW_BLOOM_USE_DEPTH = "BEAUTIFY_BLOOM_USE_DEPTH";
        public const string SKW_BLOOM_USE_LAYER = "BEAUTIFY_BLOOM_USE_LAYER";
        public const string SKW_DIRT = "BEAUTIFY_DIRT";
        public const string SKW_ANAMORPHIC_FLARES_USE_DEPTH = "BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH";
        public const string SKW_ANAMORPHIC_FLARES_USE_LAYER = "BEAUTIFY_ANAMORPHIC_FLARES_USE_LAYER";
        public const string SKW_DEPTH_OF_FIELD = "BEAUTIFY_DEPTH_OF_FIELD";
        public const string SKW_DEPTH_OF_FIELD_TRANSPARENT = "BEAUTIFY_DOF_TRANSPARENT";
        public const string SKW_VIGNETTING = "BEAUTIFY_VIGNETTING";
        public const string SKW_VIGNETTING_MASK = "BEAUTIFY_VIGNETTING_MASK";
        public const string SKW_PURKINJE = "BEAUTIFY_PURKINJE";
        public const string SKW_EYE_ADAPTATION = "BEAUTIFY_EYE_ADAPTATION";
        public const string SKW_OUTLINE = "BEAUTIFY_OUTLINE";
        public const string SKW_OUTLINE_DEPTH_FADE = "BEAUTIFY_DEPTH_FADE";
        public const string SKW_TURBO = "BEAUTIFY_TURBO";
        public const string SKW_COLOR_TWEAKS = "BEAUTIFY_COLOR_TWEAKS";
        public const string SKW_NIGHT_VISION = "BEAUTIFY_NIGHT_VISION";
        public const string SKW_DITHER = "BEAUTIFY_DITHER";
        public const string SKW_CHROMATIC_ABERRATION = "BEAUTIFY_CHROMATIC_ABERRATION";
        public const string SKW_FRAME = "BEAUTIFY_FRAME";
        public const string SKW_FRAME_MASK = "BEAUTIFY_FRAME_MASK";
        public const string SKW_SUN_FLARES_USE_DEPTH = "BEAUTIFY_SF_USE_DEPTH";
        public const string SKW_SUN_FLARES_USE_GHOSTS = "BEAUTIFY_SF_USE_GHOSTS";
        public const string SKW_CUSTOM_DEPTH_ALPHA_TEST = "DEPTH_PREPASS_ALPHA_TEST";
        public const string SKW_EDGE_ANTIALIASING = "BEAUTIFY_EDGE_AA";

        static class ShaderParams {
            public static int mainTex = Shader.PropertyToID("_MainTex");
            public static int inputTex = Shader.PropertyToID("_BeautifyInputTex");
            public static int sharpen = Shader.PropertyToID("_Sharpen");
            public static int colorParams = Shader.PropertyToID("_Params");
            public static int colorBoost = Shader.PropertyToID("_ColorBoost");
            public static int tintColor = Shader.PropertyToID("_TintColor");
            public static int compareTex = Shader.PropertyToID("_CompareTex");
            public static int compareParams = Shader.PropertyToID("_CompareParams");
            public static int fxColor = Shader.PropertyToID("_FXColor");
            public static int lutTex = Shader.PropertyToID("_LUTTex");
            public static int lut3DTexture = Shader.PropertyToID("_LUT3DTex");
            public static int lut3DParams = Shader.PropertyToID("_LUT3DParams");
            public static int colorTemp = Shader.PropertyToID("_ColorTemp");
            public static int flipY = Shader.PropertyToID("_FlipY");

            public static int blurScale = Shader.PropertyToID("_BlurScale");
            public static int tempBlurRT = Shader.PropertyToID("_BeautifyTempBlurRT");
            public static int tempBloomCustomComposeRT = Shader.PropertyToID("_BeautifyTempBloomCustomComposeRT");
            public static int tempBloomCustomComposeRTOriginal = Shader.PropertyToID("_BeautifyTempBloomCustomComposeRT");
            public static int tempBlurOneDirRT = Shader.PropertyToID("_BeautifyTempBlurOneDir0");
            public static int tempBlurOneDirRTOriginal = Shader.PropertyToID("_BeautifyTempBlurOneDir0");
            public static int tempBlurDownscaling = Shader.PropertyToID("_BeautifyTempBlurDownscaling");

            public static int bloom = Shader.PropertyToID("_Bloom");
            public static int bloomWeights = Shader.PropertyToID("_BloomWeights");
            public static int bloomWeights2 = Shader.PropertyToID("_BloomWeights2");
            public static int bloomDepthThreshold = Shader.PropertyToID("_BloomDepthThreshold");
            public static int bloomNearThreshold = Shader.PropertyToID("_BloomNearThreshold");
            public static int bloomTex = Shader.PropertyToID("_BloomTex");
            public static int bloomTex1 = Shader.PropertyToID("_BloomTex1");
            public static int bloomTex2 = Shader.PropertyToID("_BloomTex2");
            public static int bloomTex3 = Shader.PropertyToID("_BloomTex3");
            public static int bloomTex4 = Shader.PropertyToID("_BloomTex4");
            public static int bloomTint = Shader.PropertyToID("_BloomTint");
            public static int bloomSpread = Shader.PropertyToID("_BloomSpread");
            public static int bloomExclusionZBias = Shader.PropertyToID("_BloomLayerZBias");

            public static int dirt = Shader.PropertyToID("_Dirt");
            public static int dirtTex = Shader.PropertyToID("_OverlayTex");
            public static int screenLum = Shader.PropertyToID("_ScreenLum");

            public static int afData = Shader.PropertyToID("_AFData");
            public static int afDepthThreshold = Shader.PropertyToID("_AFDepthThreshold");
            public static int afNearThreshold = Shader.PropertyToID("_AFNearThreshold");
            public static int afTintColor = Shader.PropertyToID("_AFTint");
            public static int afCombineTex = Shader.PropertyToID("_CombineTex");

            public static int sfSunData = Shader.PropertyToID("_SunData");
            public static int sfSunPos = Shader.PropertyToID("_SunPos");
            public static int sfSunDir = Shader.PropertyToID("_SunDir");
            public static int sfSunTintColor = Shader.PropertyToID("_SunTint");
            public static int sfCoronaRays1 = Shader.PropertyToID("_SunCoronaRays1");
            public static int sfCoronaRays2 = Shader.PropertyToID("_SunCoronaRays2");
            public static int sfGhosts1 = Shader.PropertyToID("_SunGhosts1");
            public static int sfGhosts2 = Shader.PropertyToID("_SunGhosts2");
            public static int sfGhosts3 = Shader.PropertyToID("_SunGhosts3");
            public static int sfGhosts4 = Shader.PropertyToID("_SunGhosts4");
            public static int sfHalo = Shader.PropertyToID("_SunHalo");
            public static int sfRT = Shader.PropertyToID("_BeautifyTempSF0");
            public static int sfFlareTex = Shader.PropertyToID("_FlareTex");
            public static int sfAspectRatio = Shader.PropertyToID("_SunFlaresAspectRatio");

            public static int dofRT = Shader.PropertyToID("_DoFTex");
            public static int dofTempBlurDoFAlphaRT = Shader.PropertyToID("_BeautifyTempBlurAlphaDoF");
            public static int dofTempBlurDoFTemp1RT = Shader.PropertyToID("_BeautifyTempBlurPass1DoF");
            public static int dofTempBlurDoFTemp2RT = Shader.PropertyToID("_BeautifyTempBlurPass2DoF");
            public static int dofBokehData = Shader.PropertyToID("_BokehData");
            public static int dofBokehData2 = Shader.PropertyToID("_BokehData2");
            public static int dofBokehData3 = Shader.PropertyToID("_BokehData3");
            public static int dofBokehRT = Shader.PropertyToID("_DofBokeh");

            public static int vignette = Shader.PropertyToID("_Vignetting");
            public static int vignetteData = Shader.PropertyToID("_VignettingData");
            public static int vignetteMask = Shader.PropertyToID("_VignettingMask");

            public static int purkinje = Shader.PropertyToID("_Purkinje");

            public static int eaLumSrc = Shader.PropertyToID("_EALumSrc");
            public static int eaHist = Shader.PropertyToID("_EAHist");
            public static int eaParams = Shader.PropertyToID("_EyeAdaptation");

            public static int outline = Shader.PropertyToID("_Outline");
            public static int outlineIntensityMultiplier = Shader.PropertyToID("_OutlineIntensityMultiplier");
            public static int outlineDistanceFade = Shader.PropertyToID("_OutlineDistanceFade");
            public static int outlineRT = Shader.PropertyToID("_OutlineRT");

            public static int blurRT = Shader.PropertyToID("_BlurTex");
            public static int blurMaskedRT = Shader.PropertyToID("_BlurMaskedTex");
            public static int blurMask = Shader.PropertyToID("_BlurMask");

            public static int nightVision = Shader.PropertyToID("_NightVision");

            public static int chromaticAberrationData = Shader.PropertyToID("_ChromaticAberrationData");
            public static int chromaticTempTex = Shader.PropertyToID("_ChromaticTex");

            public static int lutPreview = Shader.PropertyToID("_LUTPreview");

            public static int frameColor = Shader.PropertyToID("_Frame");
            public static int frameMask = Shader.PropertyToID("_FrameMask");
            public static int frameData = Shader.PropertyToID("_FrameData");

            public static int CustomDepthAlphaCutoff = Shader.PropertyToID("_Cutoff");
            public static int CustomDepthBaseMap = Shader.PropertyToID("_BaseMap");

            public static int edgeAntialiasing = Shader.PropertyToID("_AntialiasData");
        }

        class BeautifyRenderPass : ScriptableRenderPass {

            Beautify beautify;

            enum Pass {
                CopyExact = 0,
                Compare = 1,
                Beautify = 2,
                BloomLuminance = 3,
                BloomDebug = 4,
                BlurHoriz = 5,
                BlurVert = 6,
                BloomCompose = 7,
                BloomResample = 8,
                BloomResampleAndCombine = 9,
                BloomLuminanceAntiflicker = 10,
                AnamorphicFlaresResample = 11,
                AnamorphicFlaresResampleAndCombine = 12,
                ComputeScreenLum = 13,
                DownsampleScreenLum = 14,
                BlendScreenLum = 15,
                SimpleBlendLum = 16,
                AnamorphicFlaresLuminance = 17,
                AnamorphicFlaresLuminanceAntiflicker = 18,
                SunFlares = 19,
                SunFlaresAdditive = 20,
                DoFCoC = 21,
                DoFCoCDebug = 22,
                DoFBlur = 23,
                DoFBlurWithoutBokeh = 24,
                DoFBlurHorizontally = 25,
                DoFBlurVertically = 26,
                CopyBilinear = 27,
                BloomExclusionLayerDebug = 28,
                DoFDebugTransparent = 29,
                ChromaticAberration = 30,
                OutlineDetect = 31,
                OutlineBlurH = 32,
                OutlineBlurV = 33,
                OutlineBlend = 34,
                BlurMask = 35,
                DoFBokeh = 36,
                DoFAdditive = 37,
                DoFBlurBokeh = 38,
                AnamorphicFlaresExclusionLayerDebug = 39
            }

            struct BloomMipData {
                public int rtDown, rtUp, width, height;
                public int rtDownOriginal, rtUpOriginal;
            }

            const int PYRAMID_COUNT_BLOOM = 5;
            const int PYRAMID_COUNT_BLOOM_TURBO = 3;
            const int PYRAMID_COUNT_EA = 9;

            Material bMat;
            ScriptableRenderer renderer;
#if UNITY_2022_2_OR_NEWER
            RTHandle source;
#else
            RenderTargetIdentifier source;
#endif
            CameraData cameraData;
            RenderTextureDescriptor sourceDesc, sourceDescHP;
            bool supportsFPTextures;
            BloomMipData[] rt, rtAF;
            int[] rtEA;
            Texture2D dirtTexture, flareTex;
            float sunFlareCurrentIntensity;
            Vector4 sunLastScrPos;
            float sunLastRot;
            float sunFlareTime;
            float dofPrevDistance, dofLastAutofocusDistance;
            Vector4 dofLastBokehData;
            RenderTexture rtEAacum, rtEAHist;
            bool requiresLuminanceComputation;
            bool usesBloomAndFlares, usesDepthOfField, usesVignetting, usesSeparateOutline;
            readonly List<string> keywords = new List<string>();
            string[] keywordsArray;
            bool setup;
            static Matrix4x4 matrix4x4identity = Matrix4x4.identity;
            bool supportsR8Format, supportsRGHalfFormat;
            RenderTexture rtCapture;

            public bool Setup(Shader shader, ScriptableRenderer renderer, RenderingData renderingData, RenderPassEvent renderingPassEvent, bool ignorePostProcessingOption) {

                // Configures where the render pass should be injected.
                beautify = VolumeManager.instance.stack.GetComponent<Beautify>();
                bool isActive = true;
                if (beautify != null) {
                    isActive = beautify.IsActive();
                    if (beautify.directWrite.value) {
                        renderingPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
#if UNITY_2022_1_OR_NEWER
                        renderingPassEvent++;
#endif
#if UNITY_2020_3_OR_NEWER
                        if (ignorePostProcessingOption) {
                            renderingPassEvent = RenderPassEvent.AfterRendering + 3; // queue after FinalBlit is present
                        }
#endif
                    }
                }

                renderPassEvent = renderingPassEvent;

                cameraData = renderingData.cameraData;
                if (setup && cameraData.camera != null) return isActive;
                setup = true;

                CheckSceneSettings();
                BeautifySettings.UnloadBeautify(); // reset any cached profile

                this.renderer = renderer;
                supportsFPTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
                supportsR8Format = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8);
                supportsRGHalfFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf);

                if (bMat == null) {
                    if (shader == null) {
                        Debug.LogWarning("Could not load Beautify shader. Please make sure BeautifyCore.shader is present.");
                    } else {
                        bMat = CoreUtils.CreateEngineMaterial(shader);
                    }
                }

                // Initialize bloom buffers descriptors
                if (rt == null || rt.Length != PYRAMID_COUNT_BLOOM + 1) {
                    rt = new BloomMipData[PYRAMID_COUNT_BLOOM + 1];
                }
                for (int k = 0; k < rt.Length; k++) {
                    rt[k].rtDown = rt[k].rtDownOriginal = Shader.PropertyToID("_BeautifyBloomDownMip" + k);
                    rt[k].rtUp = rt[k].rtUpOriginal = Shader.PropertyToID("_BeautifyBloomUpMip" + k);
                }

                // Initialize anamorphic flare buffers descriptors
                if (rtAF == null || rtAF.Length != PYRAMID_COUNT_BLOOM + 1) {
                    rtAF = new BloomMipData[PYRAMID_COUNT_BLOOM + 1];
                }
                for (int k = 0; k < rtAF.Length; k++) {
                    rtAF[k].rtDown = rtAF[k].rtDownOriginal = Shader.PropertyToID("_BeautifyAFDownMip" + k);
                    rtAF[k].rtUp = rtAF[k].rtUpOriginal = Shader.PropertyToID("_BeautifyAFUpMip" + k);
                }

                // Initialize eye adaptation buffers descriptors
                if (rtEA == null || rtEA.Length != PYRAMID_COUNT_EA) {
                    rtEA = new int[PYRAMID_COUNT_EA];
                }
                for (int k = 0; k < rtEA.Length; k++) {
                    rtEA[k] = Shader.PropertyToID("_BeautifyEAMip" + k);
                }
                return isActive;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {

                if (bMat == null) return;

                if (beautify == null) {
                    beautify = VolumeManager.instance.stack.GetComponent<Beautify>();
                }
                if (beautify == null || !beautify.IsActive()) return;

                sourceDesc = cameraTextureDescriptor;
                sourceDesc.msaaSamples = 1;
                sourceDesc.depthBufferBits = 0;

                if (beautify.downsampling.value) {
                    UniversalRenderPipelineAsset pipe = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
                    float downsamplingMultiplier = 1f / beautify.downsamplingMultiplier.value;
                    if (downsamplingMultiplier < 1f) {
                        DownsamplingMode mode = beautify.downsamplingMode.value;
                        if (mode == DownsamplingMode.BeautifyEffectsOnly) {
                            sourceDesc.width = (int)(sourceDesc.width * downsamplingMultiplier);
                            sourceDesc.height = (int)(sourceDesc.height * downsamplingMultiplier);
                            if (pipe.renderScale != 1f) {
                                pipe.renderScale = 1f;
                            }
                        } else {
                            if (pipe.renderScale != downsamplingMultiplier) {
                                pipe.renderScale = downsamplingMultiplier;
                                beautify.downsamplingMultiplier.value = 1f / pipe.renderScale;
                            }
                        }
                    } else {
                        if (pipe.renderScale != 1f) {
                            pipe.renderScale = 1f;
                        }
                    }
                }

                sourceDescHP = sourceDesc;
                if (supportsFPTextures) {
                    sourceDescHP.colorFormat = RenderTextureFormat.ARGBHalf;
                }
                UpdateMaterialProperties();

#if UNITY_2021_3_OR_NEWER
                ConfigureInput(ScriptableRenderPassInput.Depth);
#endif

            }

#if UNITY_2021_3_OR_NEWER
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
                base.OnCameraSetup(cmd, ref renderingData);
                #if UNITY_2022_2_OR_NEWER
                    source = renderer.cameraColorTargetHandle;
                #else
                    source = renderer.cameraColorTarget;
                #endif
            }
#endif

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
                if (bMat == null) {
                    Debug.LogError("Beautify material not initialized.");
                    return;
                }

                Camera cam = cameraData.camera;
                if (beautify == null || cam == null || !beautify.IsActive()) return;

#if UNITY_2021_3_OR_NEWER
                if (!beautify.directWrite.value) {
                    #if UNITY_2022_2_OR_NEWER
                        source = renderer.cameraColorTargetHandle;
                    #else
                        source = renderer.cameraColorTarget;
                    #endif
                }
#else
                source = renderer.cameraColorTarget;
#endif

                var cmd = CommandBufferPool.Get("Beautify");

#if UNITY_EDITOR
                if (requestScreenCapture && cam != null && cam.cameraType == captureCameraType) {
                    requestScreenCapture = false;
                    if (rtCapture != null) {
                        rtCapture.Release();
                    }
                    rtCapture = new RenderTexture(sourceDesc);
                    FullScreenBlit(cmd, source, rtCapture, bMat, (int)Pass.CopyExact);
                    cmd.SetGlobalTexture(ShaderParams.lutPreview, rtCapture);
                } else {
                    if (cam.cameraType == CameraType.SceneView && beautify.hideInSceneView.value && !requestScreenCapture) return;
                }
#else
                if (cam.cameraType == CameraType.SceneView && beautify.hideInSceneView.value) return;
#endif


                RestoreRTBufferIds();

                bMat.SetFloat(ShaderParams.flipY, beautify.flipY.value ? -1 : 1); // workaround for 2D renderer bug with camera stacking

                if (usesSeparateOutline && beautify.outlineStageParameter.value == Beautify.OutlineStage.BeforeBloom) {
                    DoSeparateOutline(cmd);
                }

                if (usesBloomAndFlares) {
                    DoBloomAndFlares(cmd);
                }

                if (usesSeparateOutline && beautify.outlineStageParameter.value == Beautify.OutlineStage.AfterBloom) {
                    DoSeparateOutline(cmd);
                }

                if (requiresLuminanceComputation) {
                    DoEyeAdaptation(cmd);
                }

                if (usesDepthOfField) {
                    DoDoF(cmd);
                }

                if (usesVignetting) {
                    DoVignette();
                }

                bool usesChromaticAberrationAsPost = beautify.chromaticAberrationIntensity.value > 0 && (beautify.depthOfField.value || beautify.chromaticAberrationSeparatePass.value);
                bool usesFinalBlur = beautify.blurIntensity.value > 0;

                if (beautify.directWrite.value) {
                    // direct output to camera
                    if (beautify.debugOutput.value == Beautify.DebugOutput.DepthOfFieldCoC) {
                        if (beautify.depthOfField.value) {
                            // we ignore input contents
                            FullScreenBlit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.DoFCoCDebug);
                        }
                    } else if (beautify.debugOutput.value == Beautify.DebugOutput.BloomAndFlares) {
                        if (beautify.bloomIntensity.value > 0 || beautify.anamorphicFlaresIntensity.value > 0 || beautify.sunFlaresIntensity.value > 0 || beautify.lensDirtIntensity.value > 0) {
                            // we ignore input contents
                            FullScreenBlit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.BloomDebug);
                        }
                    } else if (beautify.debugOutput.value == Beautify.DebugOutput.BloomExclusionPass) {
                        if (beautify.bloomIntensity.value > 0 && beautify.bloomExcludeLayers.value) {
                            // we ignore input contents
                            FullScreenBlit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.BloomExclusionLayerDebug);
                        }
                    } else if (beautify.debugOutput.value == Beautify.DebugOutput.AnamorphicFlaresExclusionPass) {
                        if (beautify.anamorphicFlaresIntensity.value > 0 && beautify.anamorphicFlaresExcludeLayers.value) {
                            // we ignore input contents
                            FullScreenBlit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.AnamorphicFlaresExclusionLayerDebug);
                        }
                    } else if (beautify.debugOutput.value == Beautify.DebugOutput.DepthOfFieldTransparentPass) {
                        if (beautify.depthOfField.value && (beautify.depthOfFieldTransparentSupport.value || beautify.depthOfFieldAlphaTestSupport.value)) {
                            // we ignore input contents
                            FullScreenBlit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.DoFDebugTransparent);
                        }
                    } else if (beautify.compareMode.value) {
                        cmd.GetTemporaryRT(ShaderParams.compareTex, sourceDesc, FilterMode.Point);
                        RenderTargetIdentifier dest = ShaderParams.compareTex;
                        if (usesChromaticAberrationAsPost) {
                            // chromatic aberration added as a post-pass due to depth of field
                            cmd.GetTemporaryRT(ShaderParams.chromaticTempTex, sourceDesc, FilterMode.Point);
                            FullScreenBlit(cmd, source, ShaderParams.chromaticTempTex, bMat, (int)Pass.Beautify);
                            FullScreenBlit(cmd, ShaderParams.chromaticTempTex, ShaderParams.compareTex, bMat, (int)Pass.ChromaticAberration);
                            cmd.ReleaseTemporaryRT(ShaderParams.chromaticTempTex);
                        } else {
                            FullScreenBlit(cmd, source, ShaderParams.compareTex, bMat, (int)Pass.Beautify);
                        }
                        if (usesFinalBlur) {
                            // final blur
                            int blurSource = ApplyFinalBlur(cmd, ShaderParams.compareTex);
                            FullScreenBlit(cmd, blurSource, ShaderParams.compareTex, bMat, (int)Pass.CopyBilinear);
                        }
                        FullScreenBlit(cmd, source, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.Compare);
                        cmd.ReleaseTemporaryRT(ShaderParams.compareTex);
                    } else {
                        RenderTargetIdentifier preBlurDest = BuiltinRenderTextureType.CameraTarget;
                        if (usesFinalBlur) {
                            cmd.GetTemporaryRT(ShaderParams.inputTex, sourceDesc, FilterMode.Point);
                            preBlurDest = ShaderParams.inputTex;
                        }
                        if (usesChromaticAberrationAsPost) {
                            // chromatic aberration added as a post-pass due to depth of field
                            cmd.GetTemporaryRT(ShaderParams.chromaticTempTex, sourceDesc, FilterMode.Point);
                            FullScreenBlit(cmd, source, ShaderParams.chromaticTempTex, bMat, (int)Pass.Beautify);
                            FullScreenBlit(cmd, ShaderParams.chromaticTempTex, preBlurDest, bMat, (int)Pass.ChromaticAberration);
                            cmd.ReleaseTemporaryRT(ShaderParams.chromaticTempTex);
                        } else {
                            FullScreenBlit(cmd, source, preBlurDest, bMat, (int)Pass.Beautify);
                        }
                        if (usesFinalBlur) {
                            // final blur
                            int blurSource = ApplyFinalBlur(cmd, preBlurDest);
                            FullScreenBlit(cmd, blurSource, BuiltinRenderTextureType.CameraTarget, bMat, (int)Pass.CopyBilinear);
                            cmd.ReleaseTemporaryRT(ShaderParams.inputTex);
                        }
                    }

                } else {

                    // non direct to camera

                    bool useBilinearFiltering = beautify.downsampling.value && beautify.downsamplingMultiplier.value > 1f && beautify.downsamplingBilinear.value;
                    int copyPass = useBilinearFiltering ? (int)Pass.CopyBilinear : (int)Pass.CopyExact;

                    cmd.GetTemporaryRT(ShaderParams.inputTex, sourceDesc, (!beautify.downsampling.value || (beautify.downsamplingMultiplier.value > 1f && !beautify.downsamplingBilinear.value)) ? FilterMode.Point : FilterMode.Bilinear);

                    if (beautify.debugOutput.value == Beautify.DebugOutput.DepthOfFieldCoC) {
                        if (beautify.depthOfField.value) {
                            // we ignore input contents
                            FullScreenBlit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.DoFCoCDebug);
                        }
                    } else if (beautify.debugOutput.value == Beautify.DebugOutput.BloomAndFlares) {
                        if (beautify.bloomIntensity.value > 0 || beautify.anamorphicFlaresIntensity.value > 0 || beautify.sunFlaresIntensity.value > 0 || beautify.lensDirtIntensity.value > 0) {
                            // we ignore input contents
                            FullScreenBlit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.BloomDebug);
                        }
                    } else if (beautify.debugOutput.value == Beautify.DebugOutput.BloomExclusionPass) {
                        if (beautify.bloomIntensity.value > 0 && beautify.bloomExcludeLayers.value) {
                            // we ignore input contents
                            FullScreenBlit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.BloomExclusionLayerDebug);
                        }
                    } else if (beautify.debugOutput.value == Beautify.DebugOutput.AnamorphicFlaresExclusionPass) {
                        if (beautify.anamorphicFlaresIntensity.value > 0 && beautify.anamorphicFlaresExcludeLayers.value) {
                            // we ignore input contents
                            FullScreenBlit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.AnamorphicFlaresExclusionLayerDebug);
                        }
                    } else if (beautify.debugOutput.value == Beautify.DebugOutput.DepthOfFieldTransparentPass) {
                        if (beautify.depthOfField.value && (beautify.depthOfFieldTransparentSupport.value || beautify.depthOfFieldAlphaTestSupport.value)) {
                            // we ignore input contents
                            FullScreenBlit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.DoFDebugTransparent);
                        }
                    } else if (beautify.compareMode.value) {
                        cmd.GetTemporaryRT(ShaderParams.compareTex, sourceDesc, FilterMode.Point);
                        RenderTargetIdentifier dest = ShaderParams.compareTex;
                        if (usesChromaticAberrationAsPost) {
                            // chromatic aberration added as a post-pass due to depth of field
                            FullScreenBlit(cmd, source, ShaderParams.inputTex, bMat, (int)Pass.Beautify);
                            FullScreenBlit(cmd, ShaderParams.inputTex, ShaderParams.compareTex, bMat, (int)Pass.ChromaticAberration);
                        } else {
                            FullScreenBlit(cmd, source, ShaderParams.compareTex, bMat, (int)Pass.Beautify);
                        }
                        if (usesFinalBlur) {
                            // final blur
                            int blurSource = ApplyFinalBlur(cmd, ShaderParams.compareTex);
                            FullScreenBlit(cmd, blurSource, ShaderParams.compareTex, bMat, (int)Pass.CopyBilinear);
                        }
                        FullScreenBlit(cmd, source, ShaderParams.inputTex, bMat, copyPass);
                        FullScreenBlit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.Compare);
                        cmd.ReleaseTemporaryRT(ShaderParams.compareTex);
                    } else {
                        if (usesChromaticAberrationAsPost) {
                            // chromatic aberration added as a post-pass due to depth of field
                            FullScreenBlit(cmd, source, ShaderParams.inputTex, bMat, (int)Pass.Beautify);
                            FullScreenBlit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.ChromaticAberration);
                        } else {
                            FullScreenBlit(cmd, source, ShaderParams.inputTex, bMat, copyPass);
                            FullScreenBlit(cmd, ShaderParams.inputTex, source, bMat, (int)Pass.Beautify);
                        }
                        if (usesFinalBlur) {
                            // final blur
                            int blurSource = ApplyFinalBlur(cmd, source);
                            FullScreenBlit(cmd, blurSource, source, bMat, (int)Pass.CopyBilinear);
                        }
                    }

                    cmd.ReleaseTemporaryRT(ShaderParams.inputTex);
                }

                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);

            }

            static Mesh _fullScreenMesh;

            Mesh fullscreenMesh {
                get {
                    if (_fullScreenMesh != null) {
                        return _fullScreenMesh;
                    }
                    float num = 1f;
                    float num2 = 0f;
                    Mesh val = new Mesh();
                    _fullScreenMesh = val;
                    _fullScreenMesh.SetVertices(new List<Vector3> {
            new Vector3 (-1f, -1f, 0f),
            new Vector3 (-1f, 1f, 0f),
            new Vector3 (1f, -1f, 0f),
            new Vector3 (1f, 1f, 0f)
        });
                    _fullScreenMesh.SetUVs(0, new List<Vector2> {
            new Vector2 (0f, num2),
            new Vector2 (0f, num),
            new Vector2 (1f, num2),
            new Vector2 (1f, num)
        });
                    _fullScreenMesh.SetIndices(new int[6] { 0, 1, 2, 2, 1, 3 }, (MeshTopology)0, 0, false);
                    _fullScreenMesh.UploadMeshData(true);
                    return _fullScreenMesh;
                }
            }


            void FullScreenBlit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int passIndex) {
                destination = new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1);
                cmd.SetRenderTarget(destination);
                cmd.SetGlobalTexture(ShaderParams.mainTex, source);
                cmd.DrawMesh(fullscreenMesh, matrix4x4identity, material, 0, passIndex);
            }

            /// Cleanup any allocated resources that were created during the execution of this render pass.
            public override void FrameCleanup(CommandBuffer cmd) {
            }

            public void Cleanup() {

                UniversalRenderPipelineAsset pipe = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
                if (beautify != null && pipe != null && beautify.downsampling.value) { 
                    pipe.renderScale = 1f;
                }

                CoreUtils.Destroy(bMat);
#if UNITY_EDITOR
                if (rtCapture != null) {
                    rtCapture.Release();
                }
#endif
            }


            void RestoreRTBufferIds() {
                // Restore temorary rt ids
                for (int k = 0; k < rt.Length; k++) {
                    rt[k].rtDown = rt[k].rtDownOriginal;
                    rt[k].rtUp = rt[k].rtUpOriginal;
                }
                for (int k = 0; k < rtAF.Length; k++) {
                    rtAF[k].rtDown = rtAF[k].rtDownOriginal;
                    rtAF[k].rtUp = rtAF[k].rtUpOriginal;
                }
                ShaderParams.tempBlurOneDirRT = ShaderParams.tempBlurOneDirRTOriginal;
                ShaderParams.tempBloomCustomComposeRT = ShaderParams.tempBloomCustomComposeRTOriginal;
            }

            int ApplyFinalBlur(CommandBuffer cmd, RenderTargetIdentifier source) {

                int size;
                RenderTextureDescriptor rtBlurDesc = sourceDescHP;

                float blurIntensity = beautify.blurIntensity.value;
                if (blurIntensity < 1f) {
                    size = (int)Mathf.Lerp(rtBlurDesc.width, 512, blurIntensity);
                } else {
                    size = (int)(512 / blurIntensity);
                }
                float aspectRatio = (float)sourceDesc.height / sourceDesc.width;
                rtBlurDesc.width = size;
                rtBlurDesc.height = Mathf.Max(1, (int)(size * aspectRatio));
                cmd.GetTemporaryRT(ShaderParams.blurRT, rtBlurDesc, FilterMode.Bilinear);

                float ratio = (float)sourceDesc.width / size;
                float blurScale = blurIntensity > 1f ? 1f : blurIntensity;

                cmd.GetTemporaryRT(ShaderParams.tempBlurDownscaling, rtBlurDesc, FilterMode.Bilinear);
                cmd.SetGlobalFloat(ShaderParams.blurScale, blurScale * ratio);
                FullScreenBlit(cmd, source, ShaderParams.tempBlurDownscaling, bMat, (int)Pass.BlurHoriz);
                cmd.SetGlobalFloat(ShaderParams.blurScale, blurScale);
                FullScreenBlit(cmd, ShaderParams.tempBlurDownscaling, ShaderParams.blurRT, bMat, (int)Pass.BlurVert);
                cmd.ReleaseTemporaryRT(ShaderParams.tempBlurDownscaling);

                BlurThis(cmd, rtBlurDesc, ShaderParams.blurRT, rtBlurDesc.width, rtBlurDesc.height, bMat, blurScale);
                if (!beautify.turboMode.value) {
                    BlurThis(cmd, rtBlurDesc, ShaderParams.blurRT, rtBlurDesc.width, rtBlurDesc.height, bMat, blurScale);
                    BlurThis(cmd, rtBlurDesc, ShaderParams.blurRT, rtBlurDesc.width, rtBlurDesc.height, bMat, blurScale);
                }

                if (beautify.blurMask.value != null) {
                    cmd.GetTemporaryRT(ShaderParams.blurMaskedRT, sourceDesc);
                    FullScreenBlit(cmd, source, ShaderParams.blurMaskedRT, bMat, (int)Pass.BlurMask);
                    return ShaderParams.blurMaskedRT;
                } else {
                    return ShaderParams.blurRT;
                }
            }



            void DoBloomAndFlares(CommandBuffer cmd) {

                Camera cam = cameraData.camera;
                bool sunFlareEnabled = false;
                if (beautify.sunFlaresIntensity.value > 0) {
                    CheckSun();
                    sunFlareEnabled = sceneSettings != null && sceneSettings.sun != null;
                }

                if (beautify.lensDirtIntensity.value > 0 || beautify.bloomIntensity.value > 0 || beautify.anamorphicFlaresIntensity.value > 0 || sunFlareEnabled) {

                    int mipCount = beautify.turboMode.value ? PYRAMID_COUNT_BLOOM_TURBO : PYRAMID_COUNT_BLOOM;
                    float aspectRatio = (float)sourceDesc.height / sourceDesc.width;
                    int rtBloom = -1;
                    int downsamping = beautify.turboMode.value ? 4 : 2;
                    int lensDirtSpread = beautify.turboMode.value ? 2 : beautify.lensDirtSpread.value;

                    if (beautify.bloomIntensity.value > 0 || (beautify.lensDirtIntensity.value > 0 && beautify.anamorphicFlaresIntensity.value <= 0)) {

                        int size = (int)(Mathf.Lerp(512, sourceDesc.width, beautify.bloomResolution.value / 10f) / 4f) * 4;
                        RenderTextureDescriptor bloomDesc = sourceDescHP;
                        for (int k = 0; k <= mipCount; k++) {
                            rt[k].width = size;
                            rt[k].height = Mathf.Max(1, (int)(size * aspectRatio));
                            bloomDesc.width = rt[k].width;
                            bloomDesc.height = rt[k].height;
                            cmd.ReleaseTemporaryRT(rt[k].rtDown);
                            cmd.GetTemporaryRT(rt[k].rtDown, bloomDesc, FilterMode.Bilinear);
                            cmd.ReleaseTemporaryRT(rt[k].rtUp);
                            cmd.GetTemporaryRT(rt[k].rtUp, bloomDesc, FilterMode.Bilinear);
                            size /= downsamping;
                        }

                        rtBloom = rt[0].rtDown;
                        if (beautify.bloomAntiflicker.value) {
                            FullScreenBlit(cmd, source, rtBloom, bMat, (int)Pass.BloomLuminanceAntiflicker);
                        } else {
                            FullScreenBlit(cmd, source, rtBloom, bMat, (int)Pass.BloomLuminance);
                        }

                        // Blitting down...
                        if (beautify.bloomQuickerBlur.value) {
                            for (int k = 0; k < mipCount; k++) {
                                BlurThisDownsampling(cmd, bloomDesc, rt[k].rtDown, rt[k + 1].rtDown, rt[k + 1].width, rt[k + 1].height, bMat);
                            }
                        } else {
                            for (int k = 0; k < mipCount; k++) {
                                FullScreenBlit(cmd, rt[k].rtDown, rt[k + 1].rtDown, bMat, (int)Pass.BloomResample);
                                BlurThis(cmd, bloomDesc, rt[k + 1].rtDown, rt[k + 1].width, rt[k + 1].height, bMat);
                            }
                        }   
                        if (beautify.bloomIntensity.value > 0 || beautify.lensDirtIntensity.value > 0) {
                            // Blitting up...
                            rtBloom = rt[mipCount].rtDown;
                            for (int k = mipCount; k > 0; k--) {
                                cmd.SetGlobalTexture(ShaderParams.bloomTex, rt[k - 1].rtDown);
                                FullScreenBlit(cmd, rtBloom, rt[k - 1].rtUp, bMat, (int)Pass.BloomResampleAndCombine);
                                rtBloom = rt[k - 1].rtUp;
                            }
                            if (beautify.bloomCustomize.value) {
                                cmd.SetGlobalTexture(ShaderParams.bloomTex4, mipCount < 4 ? rt[3].rtUp: rt[4].rtUp);
                                cmd.SetGlobalTexture(ShaderParams.bloomTex3, rt[3].rtUp);
                                cmd.SetGlobalTexture(ShaderParams.bloomTex2, rt[2].rtUp);
                                cmd.SetGlobalTexture(ShaderParams.bloomTex1, rt[1].rtUp);
                                cmd.SetGlobalTexture(ShaderParams.bloomTex, rt[0].rtUp);
                                bloomDesc.width = rt[0].width;
                                bloomDesc.height = rt[0].height;
                                cmd.ReleaseTemporaryRT(ShaderParams.tempBloomCustomComposeRT);
                                cmd.GetTemporaryRT(ShaderParams.tempBloomCustomComposeRT, bloomDesc, FilterMode.Bilinear);
                                rtBloom = ShaderParams.tempBloomCustomComposeRT;
                                FullScreenBlit(cmd, rt[mipCount].rtUp, rtBloom, bMat, (int)Pass.BloomCompose);
                            }
                        }
                    }

                    // anamorphic flares
                    if (beautify.anamorphicFlaresIntensity.value > 0) {

                        int sizeAF = (int)(Mathf.Lerp(512, sourceDescHP.width, beautify.anamorphicFlaresResolution.value / 10f) / 4f) * 4;

                        RenderTextureDescriptor afDesc = sourceDescHP;
                        float spread = beautify.anamorphicFlaresSpread.value * sizeAF / 512f;
                        for (int origSize = sizeAF, k = 0; k <= mipCount; k++) {
                            int w = Mathf.Max(1, (int)(sizeAF * aspectRatio / spread));
                            if (beautify.anamorphicFlaresVertical.value) {
                                rtAF[k].width = origSize;
                                rtAF[k].height = w;
                            } else {
                                rtAF[k].width = w;
                                rtAF[k].height = origSize;
                            }
                            afDesc.width = rtAF[k].width;
                            afDesc.height = rtAF[k].height;
                            cmd.ReleaseTemporaryRT(rtAF[k].rtDown);
                            cmd.GetTemporaryRT(rtAF[k].rtDown, afDesc, FilterMode.Bilinear);
                            cmd.ReleaseTemporaryRT(rtAF[k].rtUp);
                            cmd.GetTemporaryRT(rtAF[k].rtUp, afDesc, FilterMode.Bilinear);
                            sizeAF /= downsamping;
                        }

                        if (beautify.anamorphicFlaresAntiflicker.value) {
                            FullScreenBlit(cmd, source, rtAF[0].rtDown, bMat, (int)Pass.AnamorphicFlaresLuminanceAntiflicker);
                        } else {
                            FullScreenBlit(cmd, source, rtAF[0].rtDown, bMat, (int)Pass.AnamorphicFlaresLuminance);
                        }

                        BlurThisOneDirection(cmd, afDesc, ref rtAF[0].rtDown, rtAF[0].width, rtAF[0].height, beautify.anamorphicFlaresVertical.value);

                        if (beautify.anamorphicFlaresQuickerBlur.value) {
                            for (int k = 0; k < mipCount; k++) {
                                BlurThisOneDirectionDownscaling(cmd, afDesc, rtAF[k].rtDown, rtAF[k + 1].rtDown, rtAF[k + 1].width, rtAF[k + 1].height, beautify.anamorphicFlaresVertical.value);
                            }
                        } else {
                            for (int k = 0; k < mipCount; k++) {
                                FullScreenBlit(cmd, rtAF[k].rtDown, rtAF[k + 1].rtDown, bMat, (int)Pass.BloomResample);
                                BlurThisOneDirection(cmd, afDesc, ref rtAF[k + 1].rtDown, rtAF[k + 1].width, rtAF[k + 1].height, beautify.anamorphicFlaresVertical.value);
                            }
                        }

                        int last = rtAF[mipCount].rtDown;
                        for (int k = mipCount; k > 0; k--) {
                            cmd.SetGlobalTexture(ShaderParams.bloomTex, rtAF[k].rtDown);
                            if (k == 1) {
                                FullScreenBlit(cmd, last, rtAF[k - 1].rtUp, bMat, (int)Pass.AnamorphicFlaresResample); // applies intensity in last stage
                            } else {
                                FullScreenBlit(cmd, last, rtAF[k - 1].rtUp, bMat, (int)Pass.BloomResampleAndCombine);
                            }
                            last = rtAF[k - 1].rtUp;
                        }
                        if (beautify.bloomIntensity.value > 0) {
                            if (beautify.lensDirtIntensity.value > 0) {
                                BlendOneOne(cmd, rtAF[lensDirtSpread].rtUp, ref rt[lensDirtSpread].rtUp, ref rt[lensDirtSpread].rtDown);
                            }
                            BlendOneOne(cmd, last, ref rtBloom, ref rt[0].rtDown);
                        } else {
                            rtBloom = last;
                        }
                    }

                    if (sunFlareEnabled) {
                        // check if Sun is visible
                        Vector3 sunDirection = sceneSettings.sun.transform.forward;
                        Vector3 sunWorldPosition = cam.transform.position - sunDirection * 1000f;
                        float flareIntensity = 0;
                        Vector3 sunScrPos = cam.WorldToViewportPoint(sunWorldPosition);
                        bool sunVisible = sunScrPos.z > 0 && sunScrPos.x >= -0.1f && sunScrPos.x < 1.1f && sunScrPos.y >= -0.1f && sunScrPos.y < 1.1f;
                        if (sunVisible) {
                            if (beautify.sunFlaresUseLayerMask.value) {
                                Ray ray = new Ray(cam.transform.position, -sunDirection);
                                if (Physics.Raycast(ray, cam.farClipPlane, beautify.sunFlaresLayerMask.value)) {
                                    sunVisible = false;
                                }
                            }
                            if (sunVisible) {
                                Vector2 dd = sunScrPos - new Vector3(0.5f, 0.5f, 0.5f);
                                flareIntensity = beautify.sunFlaresIntensity.value * Mathf.Clamp01((0.7f - Mathf.Max(Mathf.Abs(dd.x), Mathf.Abs(dd.y))) / 0.7f);
                                if (beautify.bloomIntensity.value <= 0 && beautify.anamorphicFlaresIntensity.value <= 0) { // ensure _Bloom.x is 1 into the shader for sun flares to be visible if no bloom nor anamorphic flares are enabled
                                    bMat.SetVector(ShaderParams.bloom, Vector4.one);
                                } else {
                                    flareIntensity /= (beautify.bloomIntensity.value + 0.0001f);
                                }
                            }
                        }

                        sunFlareCurrentIntensity = Mathf.Lerp(sunFlareCurrentIntensity, flareIntensity, Application.isPlaying ? beautify.sunFlaresAttenSpeed.value * Time.deltaTime : 1f);
                        if (sunFlareCurrentIntensity > 0) {
                            if (flareIntensity > 0) {
                                sunLastScrPos = sunScrPos;
                            }
                            bMat.SetColor(ShaderParams.sfSunTintColor, beautify.sunFlaresTint.value * sunFlareCurrentIntensity);
                            sunLastScrPos.z = 0.5f + sunFlareTime * beautify.sunFlaresSolarWindSpeed.value;
                            Vector2 sfDist = new Vector2(0.5f - sunLastScrPos.y, sunLastScrPos.x - 0.5f);
                            if (!beautify.sunFlaresRotationDeadZone.value || sfDist.sqrMagnitude > 0.00025f) {
                                sunLastRot = Mathf.Atan2(sfDist.x, sfDist.y);
                            }
                            sunLastScrPos.w = sunLastRot;
                            sunFlareTime += Time.unscaledDeltaTime;
                            bMat.SetVector(ShaderParams.sfSunPos, sunLastScrPos);
                            bMat.SetVector(ShaderParams.sfSunDir, sunDirection);
                            RenderTextureDescriptor sfDesc = sourceDesc;
                            sfDesc.width /= beautify.sunFlaresDownsampling.value;
                            sfDesc.height /= beautify.sunFlaresDownsampling.value;
                            bMat.SetFloat(ShaderParams.sfAspectRatio, (float)sourceDesc.height / sourceDesc.width);
                            cmd.GetTemporaryRT(ShaderParams.sfRT, sfDesc, FilterMode.Bilinear);
                            if (rtBloom >= 0) {
                                FullScreenBlit(cmd, rtBloom, ShaderParams.sfRT, bMat, (int)Pass.SunFlaresAdditive);
                            } else {
                                FullScreenBlit(cmd, source, ShaderParams.sfRT, bMat, (int)Pass.SunFlares);
                            }
                            if (beautify.lensDirtIntensity.value > 0 && beautify.bloomIntensity.value > 0) {
                                BlendOneOne(cmd, ShaderParams.sfRT, ref rt[lensDirtSpread].rtUp, ref rt[lensDirtSpread].rtDown);
                            }
                            rtBloom = ShaderParams.sfRT;
                        }
                    }

                    if (rtBloom >= 0) {
                        cmd.SetGlobalTexture(ShaderParams.bloomTex, rtBloom);
                    } else {
                        bMat.DisableKeyword(SKW_BLOOM);
                    }

                    if (beautify.lensDirtIntensity.value > 0) {
                        int rtID = (beautify.anamorphicFlaresIntensity.value > 0 && beautify.bloomIntensity.value <= 0) ? rtAF[lensDirtSpread].rtUp : rt[lensDirtSpread].rtUp;
                        cmd.SetGlobalTexture(ShaderParams.screenLum, rtID);
                    }

                }
            }

            void BlendOneOne(CommandBuffer cmd, int source, ref int destination, ref int tempBuffer) {
                cmd.SetGlobalTexture(ShaderParams.afCombineTex, destination); // _BloomTex used as temporary rt for combining
                FullScreenBlit(cmd, source, tempBuffer, bMat, (int)Pass.AnamorphicFlaresResampleAndCombine);
                // swap buffers
                int tmp = destination;
                destination = tempBuffer;
                tempBuffer = tmp;
            }

            void BlurThis(CommandBuffer cmd, RenderTextureDescriptor desc, int rt, int width, int height, Material blurMat, float blurScale = 1f) {
                desc.width = width;
                desc.height = height;
                cmd.GetTemporaryRT(ShaderParams.tempBlurRT, desc, FilterMode.Bilinear);
                cmd.SetGlobalFloat(ShaderParams.blurScale, blurScale);
                FullScreenBlit(cmd, rt, ShaderParams.tempBlurRT, blurMat, (int)Pass.BlurHoriz);
                FullScreenBlit(cmd, ShaderParams.tempBlurRT, rt, blurMat, (int)Pass.BlurVert);
                cmd.ReleaseTemporaryRT(ShaderParams.tempBlurRT);
            }

            void BlurThisDownsampling(CommandBuffer cmd, RenderTextureDescriptor desc, int rtSource, int rt, int width, int height, Material blurMat, float blurScale = 1f) {
                desc.width = width;
                desc.height = height;
                cmd.GetTemporaryRT(ShaderParams.tempBlurRT, desc, FilterMode.Bilinear);
                cmd.SetGlobalFloat(ShaderParams.blurScale, blurScale * 4f);
                FullScreenBlit(cmd, rtSource, ShaderParams.tempBlurRT, blurMat, (int)Pass.BlurHoriz);
                cmd.SetGlobalFloat(ShaderParams.blurScale, blurScale);
                FullScreenBlit(cmd, ShaderParams.tempBlurRT, rt, blurMat, (int)Pass.BlurVert);
                cmd.ReleaseTemporaryRT(ShaderParams.tempBlurRT);
            }

            void BlurThisOneDirection(CommandBuffer cmd, RenderTextureDescriptor desc, ref int rt, int width, int height, bool vertical, float blurScale = 1f) {
                desc.width = width;
                desc.height = height;
                cmd.ReleaseTemporaryRT(ShaderParams.tempBlurOneDirRT);
                cmd.GetTemporaryRT(ShaderParams.tempBlurOneDirRT, desc, FilterMode.Bilinear);
                cmd.SetGlobalFloat(ShaderParams.blurScale, blurScale);
                FullScreenBlit(cmd, rt, ShaderParams.tempBlurOneDirRT, bMat, vertical ? (int)Pass.BlurVert : (int)Pass.BlurHoriz);
                int aux = rt;
                rt = ShaderParams.tempBlurOneDirRT;
                ShaderParams.tempBlurOneDirRT = aux;
            }

            void BlurThisOneDirectionDownscaling(CommandBuffer cmd, RenderTextureDescriptor desc, int rtSource, int rt, int width, int height, bool vertical, float blurScale = 1f) {
                cmd.SetGlobalFloat(ShaderParams.blurScale, blurScale * 2f);
                FullScreenBlit(cmd, rtSource, rt, bMat, vertical ? (int)Pass.BlurVert : (int)Pass.BlurHoriz);
            }

            void DoDoF(CommandBuffer cmd) {

                Camera cam = cameraData.camera;
                if (cam.cameraType != CameraType.Game) {
                    bMat.DisableKeyword(SKW_DEPTH_OF_FIELD);
                    return;
                }

                UpdateDepthOfFieldData(cmd);

                BeautifySettings.dofTransparentLayerMask = beautify.depthOfFieldTransparentLayerMask.value;
                BeautifySettings.dofTransparentDoubleSided = beautify.depthOfFieldTransparentDoubleSided.value;

                int width = cam.pixelWidth / beautify.depthOfFieldDownsampling.value;
                int height = cam.pixelHeight / beautify.depthOfFieldDownsampling.value;
                RenderTextureDescriptor dofDesc = sourceDescHP;
                dofDesc.width = width;
                dofDesc.height = height;
                dofDesc.colorFormat = RenderTextureFormat.ARGBHalf;
                cmd.GetTemporaryRT(ShaderParams.dofRT, dofDesc, FilterMode.Bilinear);
                FullScreenBlit(cmd, source, ShaderParams.dofRT, bMat, (int)Pass.DoFCoC);

                if (beautify.depthOfFieldForegroundBlur.value && beautify.depthOfFieldForegroundBlurHQ.value) {
                    BlurThisAlpha(cmd, dofDesc, ShaderParams.dofRT, beautify.depthOfFieldForegroundBlurHQSpread.value);
                }

                if (beautify.depthOfFieldBokehComposition.value == Beautify.DoFBokehComposition.Integrated || !beautify.depthOfFieldBokeh.value) {
                    Pass pass = beautify.depthOfFieldBokeh.value ? Pass.DoFBlur : Pass.DoFBlurWithoutBokeh;
                    BlurThisDoF(cmd, dofDesc, ShaderParams.dofRT, (int)pass);
                } else {
                    BlurThisDoF(cmd, dofDesc, ShaderParams.dofRT, (int)Pass.DoFBlurWithoutBokeh);

                    // separate & blend bokeh
                    cmd.GetTemporaryRT(ShaderParams.dofBokehRT, dofDesc, FilterMode.Bilinear);
                    FullScreenBlit(cmd, source, ShaderParams.dofBokehRT, bMat, (int)Pass.DoFBokeh);
                    BlurThisDoF(cmd, dofDesc, ShaderParams.dofBokehRT, (int)Pass.DoFBlurBokeh);
                    FullScreenBlit(cmd, ShaderParams.dofBokehRT, ShaderParams.dofRT, bMat, (int)Pass.DoFAdditive);
                    cmd.ReleaseTemporaryRT(ShaderParams.dofBokehRT);
                }


                cmd.SetGlobalTexture(ShaderParams.dofRT, ShaderParams.dofRT);
            }

            void BlurThisDoF(CommandBuffer cmd, RenderTextureDescriptor dofDesc, int rt, int renderPass) {
                cmd.GetTemporaryRT(ShaderParams.dofTempBlurDoFTemp1RT, dofDesc, beautify.depthOfFieldFilterMode.value);
                cmd.GetTemporaryRT(ShaderParams.dofTempBlurDoFTemp2RT, dofDesc, beautify.depthOfFieldFilterMode.value);

                UpdateDepthOfFieldBlurData(cmd, new Vector2(0.44721f, -0.89443f));
                FullScreenBlit(cmd, rt, ShaderParams.dofTempBlurDoFTemp1RT, bMat, renderPass);

                UpdateDepthOfFieldBlurData(cmd, new Vector2(-1f, 0f));
                FullScreenBlit(cmd, ShaderParams.dofTempBlurDoFTemp1RT, ShaderParams.dofTempBlurDoFTemp2RT, bMat, renderPass);

                UpdateDepthOfFieldBlurData(cmd, new Vector2(0.44721f, 0.89443f));
                FullScreenBlit(cmd, ShaderParams.dofTempBlurDoFTemp2RT, rt, bMat, renderPass);

                cmd.ReleaseTemporaryRT(ShaderParams.dofTempBlurDoFTemp2RT);
                cmd.ReleaseTemporaryRT(ShaderParams.dofTempBlurDoFTemp1RT);
            }


            void BlurThisAlpha(CommandBuffer cmd, RenderTextureDescriptor dofDesc, int rt, float blurScale = 1f) {
                cmd.GetTemporaryRT(ShaderParams.dofTempBlurDoFAlphaRT, dofDesc, FilterMode.Bilinear);
                cmd.SetGlobalFloat(ShaderParams.blurScale, blurScale);
                FullScreenBlit(cmd, rt, ShaderParams.dofTempBlurDoFAlphaRT, bMat, (int)Pass.DoFBlurHorizontally);
                FullScreenBlit(cmd, ShaderParams.dofTempBlurDoFAlphaRT, rt, bMat, (int)Pass.DoFBlurVertically);
                cmd.ReleaseTemporaryRT(ShaderParams.dofTempBlurDoFAlphaRT);
            }

            void UpdateDepthOfFieldBlurData(CommandBuffer cmd, Vector2 blurDir) {
                float downsamplingRatio = 1f / (float)beautify.depthOfFieldDownsampling.value;
                blurDir *= downsamplingRatio;
                dofLastBokehData.z = blurDir.x;
                dofLastBokehData.w = blurDir.y;
                cmd.SetGlobalVector(ShaderParams.dofBokehData, dofLastBokehData);
            }

            void DoVignette() {
                float outerRing = 1f - beautify.vignettingOuterRing.value;
                float innerRing = 1f - beautify.vignettingInnerRing.value;
                bool vignettingEnabled = outerRing < 1 || innerRing < 1f || beautify.vignettingFade.value > 0 || beautify.vignettingBlink.value > 0;
                if (vignettingEnabled) {
                    Color vignettingColorAdjusted = beautify.vignettingColor.value;
                    float vb = 1f - beautify.vignettingBlink.value * 2f;
                    if (vb < 0) vb = 0;
                    vignettingColorAdjusted.r *= vb;
                    vignettingColorAdjusted.g *= vb;
                    vignettingColorAdjusted.b *= vb;
                    bMat.SetColor(ShaderParams.vignette, vignettingColorAdjusted);
                    Camera cam = cameraData.camera;
                    Vector4 vignetteData = new Vector4(beautify.vignettingCenter.value.x, beautify.vignettingCenter.value.y, (beautify.vignettingCircularShape.value && beautify.vignettingBlink.value <= 0) ? 1.0f / cam.aspect : beautify.vignettingAspectRatio.value + 1.001f / (1.001f - beautify.vignettingBlink.value) - 1f, outerRing);
                    if (beautify.vignettingBlinkStyle.value == Beautify.BlinkStyle.Human) {
                        vignetteData.y -= beautify.vignettingBlink.value * 0.5f;
                    }
                    bMat.SetVector(ShaderParams.vignetteData, vignetteData);
                }
            }

            void DoEyeAdaptation(CommandBuffer cmd) {

                int sizeEA = (int)Mathf.Pow(2, rtEA.Length);

                RenderTextureDescriptor eaDesc = sourceDescHP;
                if (supportsRGHalfFormat) {
                    eaDesc.colorFormat = RenderTextureFormat.RGHalf;
                }
                for (int k = 0; k < rtEA.Length; k++) {
                    eaDesc.width = eaDesc.height = sizeEA;
                    cmd.GetTemporaryRT(rtEA[k], eaDesc, FilterMode.Bilinear);
                    sizeEA /= 2;
                }

                FullScreenBlit(cmd, source, rtEA[0], bMat, (int)Pass.CopyBilinear);

                int lumRT = rtEA.Length - 1;
                for (int k = 0; k < lumRT; k++) {
                    FullScreenBlit(cmd, rtEA[k], rtEA[k + 1], bMat, k == 0 ? (int)Pass.ComputeScreenLum : (int)Pass.DownsampleScreenLum);
                }
                cmd.SetGlobalTexture(ShaderParams.eaLumSrc, rtEA[lumRT]);
                if (rtEAacum == null) {
                    RenderTextureDescriptor rtEASmallDesc = sourceDescHP;
                    rtEASmallDesc.width = rtEASmallDesc.height = 2;
                    rtEASmallDesc.colorFormat = RenderTextureFormat.ARGBFloat;
                    rtEAacum = new RenderTexture(rtEASmallDesc);
                    rtEAacum.Create();
                    FullScreenBlit(cmd, rtEA[lumRT], rtEAacum, bMat, (int)Pass.CopyExact);
                    rtEAHist = new RenderTexture(rtEASmallDesc);
                    rtEAHist.Create();
                    FullScreenBlit(cmd, rtEAacum, rtEAHist, bMat, (int)Pass.CopyExact);
                } else {
                    //rtEAacum.MarkRestoreExpected();
                    FullScreenBlit(cmd, rtEA[lumRT], rtEAacum, bMat, (int)Pass.BlendScreenLum);
                    FullScreenBlit(cmd, rtEAacum, rtEAHist, bMat, (int)Pass.SimpleBlendLum);
                }
                cmd.SetGlobalTexture(ShaderParams.eaHist, rtEAHist);
            }

            void DoSeparateOutline(CommandBuffer cmd) {
                RenderTextureDescriptor rtOutlineDescriptor = sourceDesc;
                rtOutlineDescriptor.colorFormat = supportsR8Format ? RenderTextureFormat.R8 : sourceDesc.colorFormat;
                cmd.GetTemporaryRT(ShaderParams.outlineRT, rtOutlineDescriptor);
                FullScreenBlit(cmd, source, ShaderParams.outlineRT, bMat, (int)Pass.OutlineDetect);
                int passCount = beautify.outlineBlurPassCount.value;
                float spread = beautify.outlineSpread.value;
                bool downscale = beautify.outlineBlurDownscale.value;
                for (int k = 1; k <= passCount; k++) {
                    BlurThisOutline(cmd, rtOutlineDescriptor, spread, downscale ? k : 1);
                }
                FullScreenBlit(cmd, ShaderParams.outlineRT, source, bMat, (int)Pass.OutlineBlend);
                cmd.ReleaseTemporaryRT(ShaderParams.outlineRT);
            }

            void BlurThisOutline(CommandBuffer cmd, RenderTextureDescriptor desc, float blurScale, int downscale) {
                desc.width = desc.width / downscale;
                desc.height = desc.height / downscale;
                cmd.GetTemporaryRT(ShaderParams.tempBlurRT, desc, FilterMode.Bilinear);
                cmd.SetGlobalFloat(ShaderParams.blurScale, blurScale);
                FullScreenBlit(cmd, ShaderParams.outlineRT, ShaderParams.tempBlurRT, bMat, (int)Pass.OutlineBlurH);
                float ratio = (float)desc.height / desc.width;
                cmd.SetGlobalFloat(ShaderParams.blurScale, blurScale * ratio);
                FullScreenBlit(cmd, ShaderParams.tempBlurRT, ShaderParams.outlineRT, bMat, (int)Pass.OutlineBlurV);
                cmd.ReleaseTemporaryRT(ShaderParams.tempBlurRT);
            }

            Vector3 camPrevPos;
            Quaternion camPrevRotation;
            float currSens;

            void UpdateMaterialProperties() {

                Camera cam = cameraData.camera;
                if (cam == null) return;

                CheckCameraDepthTextureMode(cam);

                keywords.Clear();

                // Compute motion sensibility
                float sharpenIntensity = beautify.sharpenIntensity.value;
                bool usesSharpen = sharpenIntensity > 0 && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifySharpen.value);
                if (usesSharpen) {
                    keywords.Add(SKW_SHARPEN);
                    float tempSharpen = sharpenIntensity;
                    float sensibility = beautify.sharpenMotionSensibility.value;
                    if (sensibility > 0) {

                        // Motion sensibility v2
                        Vector3 pos = cam.transform.position;
                        Quaternion q = cam.transform.rotation;

                        float dt = Time.deltaTime;
                        if (pos != camPrevPos || q.x != camPrevRotation.x || q.y != camPrevRotation.y || q.z != camPrevRotation.z || q.w != camPrevRotation.w) {
                            currSens = Mathf.Lerp(currSens, sharpenIntensity * sensibility, 30f * sensibility * dt);
                            camPrevPos = pos;
                            camPrevRotation = q;
                        } else {
                            currSens -= 30f * beautify.sharpenMotionRestoreSpeed.value * dt;
                        }

                        currSens = Mathf.Clamp(currSens, 0, sharpenIntensity);
                        tempSharpen = sharpenIntensity - currSens;
                    }

                    bMat.SetVector(ShaderParams.sharpen, new Vector4(tempSharpen, beautify.sharpenDepthThreshold.value + 0.000001f, beautify.sharpenClamp.value, beautify.sharpenRelaxation.value));
                }

                bool isOrtho = cam.orthographic;
                bool linearColorSpace = QualitySettings.activeColorSpace == ColorSpace.Linear;

                bMat.SetVector(ShaderParams.colorParams, new Vector4(beautify.sepia.value, beautify.daltonize.value, (beautify.sharpenMinMaxDepth.value.x + beautify.sharpenMinMaxDepth.value.y) * 0.5f, Mathf.Abs(beautify.sharpenMinMaxDepth.value.y - beautify.sharpenMinMaxDepth.value.x) * 0.5f + (isOrtho ? 1000.0f : 0f)));

                float contrast = linearColorSpace ? 1.0f + (beautify.contrast.value - 1.0f) / 2.2f : beautify.contrast.value;
                bMat.SetVector(ShaderParams.colorBoost, new Vector4(beautify.brightness.value, contrast, beautify.saturate.value, beautify.downsamplingMultiplier.value > 1f ? 0 : beautify.ditherIntensity.value));
                bMat.SetColor(ShaderParams.tintColor, beautify.tintColor.value);

                bMat.SetVector(ShaderParams.colorTemp, new Vector4(beautify.colorTemp.value, beautify.colorTempBlend.value, 0));

                float angle = beautify.compareSameSide.value ? Mathf.PI * 0.5f : beautify.compareLineAngle.value;
                bMat.SetVector(ShaderParams.compareParams, new Vector4(Mathf.Cos(angle), Mathf.Sin(angle), beautify.compareSameSide.value ? beautify.comparePanning.value : -10, beautify.compareLineWidth.value));
                bMat.SetVector(ShaderParams.fxColor, new Color(beautify.tonemapExposurePre.value, beautify.tonemapBrightnessPost.value, beautify.tonemapMaxInputBrightness.value, beautify.lutIntensity.value));

                // edge AA related
                if (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyEdgeAA.value) {
                    float aaStrength = beautify.antialiasStrength.value;
                    if (aaStrength > 0) {
                        bMat.SetVector(ShaderParams.edgeAntialiasing, new Vector4(aaStrength, beautify.antialiasDepthThreshold.value, beautify.antialiasDepthAttenuation.value * 10f, beautify.antialiasSpread.value));
                        keywords.Add(SKW_EDGE_ANTIALIASING);
                    }
                }

                // bloom related
                usesBloomAndFlares = false;
                BeautifySettings.bloomExcludeMask = 0;
                BeautifySettings.anamorphicFlaresExcludeMask = 0;
                if (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyBloom.value) {
                    bool sunFlareEnabled = false;
                    if (beautify.sunFlaresIntensity.value > 0) {
                        CheckSun();
                        sunFlareEnabled = sceneSettings != null && sceneSettings.sun != null;
                    }

                    if (beautify.lensDirtIntensity.value > 0 || beautify.bloomIntensity.value > 0 || beautify.anamorphicFlaresIntensity.value > 0 || sunFlareEnabled) {

                        BeautifySettings.bloomExcludeMask = beautify.bloomExcludeLayers.value ? (int)beautify.bloomExclusionLayerMask.value : 0;

                        float bloomWeightsSum = 0.00001f + beautify.bloomWeight0.value + beautify.bloomWeight1.value + beautify.bloomWeight2.value + beautify.bloomWeight3.value + beautify.bloomWeight4.value + beautify.bloomWeight5.value;
                        bMat.SetVector(ShaderParams.bloomWeights2, new Vector4(beautify.bloomWeight4.value / bloomWeightsSum + beautify.bloomBoost4.value, beautify.bloomWeight5.value / bloomWeightsSum + beautify.bloomBoost5.value, beautify.bloomMaxBrightness.value, bloomWeightsSum));
                        bMat.SetColor(ShaderParams.bloomTint, beautify.bloomTint.value);

                        float spread = Mathf.Lerp(0.05f, 0.95f, beautify.bloomSpread.value);
                        bMat.SetFloat(ShaderParams.bloomSpread, spread);

                        UpdateMaterialBloomIntensityAndThreshold();
                        if (beautify.bloomIntensity.value > 0 || (beautify.lensDirtIntensity.value > 0 && beautify.anamorphicFlaresIntensity.value <= 0)) {
                            bMat.SetVector(ShaderParams.bloomWeights, new Vector4(beautify.bloomWeight0.value / bloomWeightsSum + beautify.bloomBoost0.value, beautify.bloomWeight1.value / bloomWeightsSum + beautify.bloomBoost1.value, beautify.bloomWeight2.value / bloomWeightsSum + beautify.bloomBoost2.value, beautify.bloomWeight3.value / bloomWeightsSum + beautify.bloomBoost3.value));
                            if (beautify.bloomDepthAtten.value > 0 || beautify.bloomNearAtten.value > 0) {
                                keywords.Add(SKW_BLOOM_USE_DEPTH);
                                bMat.SetFloat(ShaderParams.bloomDepthThreshold, beautify.bloomDepthAtten.value);
                                bMat.SetFloat(ShaderParams.bloomNearThreshold, (beautify.bloomNearAtten.value / cam.farClipPlane) + 0.00001f);
                            }
                            if (beautify.bloomExcludeLayers.value && beautify.bloomExclusionLayerMask.value > 0) {
                                keywords.Add(SKW_BLOOM_USE_LAYER);
                            }
                        }
                        keywords.Add(SKW_BLOOM);
                        usesBloomAndFlares = true;

                        if (beautify.lensDirtIntensity.value > 0 && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyLensDirt.value)) {
                            Vector4 dirtData = new Vector4(1.0f, beautify.lensDirtIntensity.value * beautify.lensDirtIntensity.value, beautify.lensDirtThreshold.value, Mathf.Max(beautify.bloomIntensity.value, 1f));
                            bMat.SetVector(ShaderParams.dirt, dirtData);
                            Texture tex = beautify.lensDirtTexture.value;
                            if (tex == null) {
                                if (dirtTexture == null) {
                                    dirtTexture = Resources.Load<Texture2D>("Textures/lensDirt") as Texture2D;
                                }
                                tex = dirtTexture;
                            }
                            if (tex != null) {
                                bMat.SetTexture(ShaderParams.dirtTex, tex);
                                keywords.Add(SKW_DIRT);
                            }
                        }
                    }

                    // anamorphic flares related
                    if (beautify.anamorphicFlaresIntensity.value > 0) {
                        BeautifySettings.anamorphicFlaresExcludeMask = beautify.anamorphicFlaresExcludeLayers.value ? (int)beautify.anamorphicFlaresExclusionLayerMask.value : 0;
                        usesBloomAndFlares = true;
                        if (beautify.anamorphicFlaresDepthAtten.value > 0 || beautify.anamorphicFlaresNearAtten.value > 0) {
                            keywords.Add(SKW_ANAMORPHIC_FLARES_USE_DEPTH);
                            bMat.SetFloat(ShaderParams.afDepthThreshold, beautify.anamorphicFlaresDepthAtten.value);
                            bMat.SetFloat(ShaderParams.afNearThreshold, (beautify.anamorphicFlaresNearAtten.value / cam.farClipPlane) + 0.00001f);
                        }
                        if (beautify.anamorphicFlaresExcludeLayers.value && beautify.anamorphicFlaresExclusionLayerMask.value > 0) {
                            keywords.Add(SKW_ANAMORPHIC_FLARES_USE_LAYER);
                        }

                        bMat.SetColor(ShaderParams.afTintColor, beautify.anamorphicFlaresTint.value);
                    }

                    // sun flares related
                    if (sunFlareEnabled) {
                        usesBloomAndFlares = true;
                        bMat.SetVector(ShaderParams.sfSunData, new Vector4(beautify.sunFlaresSunIntensity.value, beautify.sunFlaresSunDiskSize.value, beautify.sunFlaresSunRayDiffractionIntensity.value, beautify.sunFlaresSunRayDiffractionThreshold.value));
                        bMat.SetVector(ShaderParams.sfCoronaRays1, new Vector4(beautify.sunFlaresCoronaRays1Length.value, Mathf.Max(beautify.sunFlaresCoronaRays1Streaks.value / 2f, 1), Mathf.Max(beautify.sunFlaresCoronaRays1Spread.value, 0.0001f), beautify.sunFlaresCoronaRays1AngleOffset.value));
                        bMat.SetVector(ShaderParams.sfCoronaRays2, new Vector4(beautify.sunFlaresCoronaRays2Length.value, Mathf.Max(beautify.sunFlaresCoronaRays2Streaks.value / 2f, 1), Mathf.Max(beautify.sunFlaresCoronaRays2Spread.value, 0.0001f), beautify.sunFlaresCoronaRays2AngleOffset.value));

#if UNITY_2020_3_OR_NEWER

#if ENABLE_VR && ENABLE_XR_MODULE
                        if (!cameraData.xrRendering) 
#endif
                        {
                            keywords.Add(SKW_SUN_FLARES_USE_GHOSTS);
                            bMat.SetVector(ShaderParams.sfGhosts1, new Vector4(0, beautify.sunFlaresGhosts1Size.value, beautify.sunFlaresGhosts1Offset.value, beautify.sunFlaresGhosts1Brightness.value));
                            bMat.SetVector(ShaderParams.sfGhosts2, new Vector4(0, beautify.sunFlaresGhosts2Size.value, beautify.sunFlaresGhosts2Offset.value, beautify.sunFlaresGhosts2Brightness.value));
                            bMat.SetVector(ShaderParams.sfGhosts3, new Vector4(0, beautify.sunFlaresGhosts3Size.value, beautify.sunFlaresGhosts3Offset.value, beautify.sunFlaresGhosts3Brightness.value));
                            bMat.SetVector(ShaderParams.sfGhosts4, new Vector4(0, beautify.sunFlaresGhosts4Size.value, beautify.sunFlaresGhosts4Offset.value, beautify.sunFlaresGhosts4Brightness.value));
                            bMat.SetVector(ShaderParams.sfHalo, new Vector4(beautify.sunFlaresHaloOffset.value, beautify.sunFlaresHaloAmplitude.value, beautify.sunFlaresHaloIntensity.value * 100f, 0));
                        }
#else
                        if (sourceDesc.vrUsage == VRTextureUsage.None) {
                            keywords.Add(SKW_SUN_FLARES_USE_GHOSTS);
                            bMat.SetVector(ShaderParams.sfGhosts1, new Vector4(0, beautify.sunFlaresGhosts1Size.value, beautify.sunFlaresGhosts1Offset.value, beautify.sunFlaresGhosts1Brightness.value));
                            bMat.SetVector(ShaderParams.sfGhosts2, new Vector4(0, beautify.sunFlaresGhosts2Size.value, beautify.sunFlaresGhosts2Offset.value, beautify.sunFlaresGhosts2Brightness.value));
                            bMat.SetVector(ShaderParams.sfGhosts3, new Vector4(0, beautify.sunFlaresGhosts3Size.value, beautify.sunFlaresGhosts3Offset.value, beautify.sunFlaresGhosts3Brightness.value));
                            bMat.SetVector(ShaderParams.sfGhosts4, new Vector4(0, beautify.sunFlaresGhosts4Size.value, beautify.sunFlaresGhosts4Offset.value, beautify.sunFlaresGhosts4Brightness.value));
                            bMat.SetVector(ShaderParams.sfHalo, new Vector4(beautify.sunFlaresHaloOffset.value, beautify.sunFlaresHaloAmplitude.value, beautify.sunFlaresHaloIntensity.value * 100f, 0));
                        }
#endif
                        if (!beautify.sunFlaresUseLayerMask.value) {
                            keywords.Add(SKW_SUN_FLARES_USE_DEPTH);
                        }
                        if (flareTex == null) {
                            flareTex = Resources.Load<Texture2D>("Textures/flareNoise") as Texture2D;
                        }
                        bMat.SetTexture(ShaderParams.sfFlareTex, flareTex);
                    }
                }

                // DoF
                usesDepthOfField = false;
                BeautifySettings.dofTransparentSupport = false;
                BeautifySettings.dofAlphaTestSupport = false;
                if (beautify.depthOfField.value && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyDoF.value)) {
                    usesDepthOfField = true;
                    bool transparentSupport = beautify.depthOfFieldTransparentSupport.value && beautify.depthOfFieldTransparentLayerMask.value > 0;
                    bool alphaTestSupport = beautify.depthOfFieldAlphaTestSupport.value && beautify.depthOfFieldAlphaTestLayerMask.value > 0;
                    if ((transparentSupport || alphaTestSupport) && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyDoFTransparentSupport.value)) {
                        keywords.Add(SKW_DEPTH_OF_FIELD_TRANSPARENT);
                        BeautifySettings.dofTransparentSupport = transparentSupport;
                        if (alphaTestSupport) {
                            BeautifySettings.dofAlphaTestSupport = true;
                            BeautifySettings.dofAlphaTestLayerMask = beautify.depthOfFieldAlphaTestLayerMask.value;
                            BeautifySettings.dofAlphaTestDoubleSided = beautify.depthOfFieldAlphaTestDoubleSided.value;
                        }
                    } else {
                        keywords.Add(SKW_DEPTH_OF_FIELD);
                    }
                }

                // Vignette
                usesVignetting = false;
                float innerRing = 1f - beautify.vignettingInnerRing.value;
                if (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyVignetting.value) {
                    float outerRing = 1f - beautify.vignettingOuterRing.value;
                    usesVignetting = outerRing < 1 || innerRing < 1f || beautify.vignettingFade.value > 0 || beautify.vignettingBlink.value > 0;
                    if (innerRing >= outerRing) {
                        innerRing = outerRing - 0.0001f;
                    }
                    if (usesVignetting) {
                        if (beautify.vignettingMask.value != null) {
                            bMat.SetTexture(ShaderParams.vignetteMask, beautify.vignettingMask.value);
                            keywords.Add(SKW_VIGNETTING_MASK);
                        } else {
                            keywords.Add(SKW_VIGNETTING);
                        }
                    }
                }

                // Frame
                if (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyFrame.value) {
                    if (beautify.frame.value) {
                        Color frameColorAdjusted = beautify.frameColor.value;
                        float fparam;
                        if (beautify.frameMask.value != null) {
                            bMat.SetTexture(ShaderParams.frameMask, beautify.frameMask.value);
                            keywords.Add(SKW_FRAME_MASK);
                            fparam = frameColorAdjusted.a;
                        } else {
                            fparam = (1.00001f - frameColorAdjusted.a) * 0.5f;
                            keywords.Add(SKW_FRAME);
                        }
                        if (beautify.frameStyle.value == Beautify.FrameStyle.Border) {
                            bMat.SetColor(ShaderParams.frameColor, frameColorAdjusted);
                            bMat.SetVector(ShaderParams.frameData, new Vector4(fparam, 50, fparam, 50));
                        } else {
                            bMat.SetColor(ShaderParams.frameColor, Color.black);
                            bMat.SetVector(ShaderParams.frameData, new Vector4(0.5f - beautify.frameBandHorizontalSize.value, 1f / (0.0001f + beautify.frameBandHorizontalSmoothness.value), 0.5f - beautify.frameBandVerticalSize.value, 1f / (0.0001f + beautify.frameBandVerticalSmoothness.value)));
                        }
                    }
                }

                // Purkinje and vignetting data
                bool usesPurkinje = beautify.purkinje.value && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyPurkinje.value);
                if (usesPurkinje || usesVignetting) {
                    float vd = beautify.vignettingFade.value + beautify.vignettingBlink.value * 0.5f;
                    if (beautify.vignettingBlink.value > 0.99f) vd = 1f;
                    Vector4 purkinjeData = new Vector4(beautify.purkinjeAmount.value, beautify.purkinjeLuminanceThreshold.value, vd, innerRing);
                    bMat.SetVector(ShaderParams.purkinje, purkinjeData);
                    if (beautify.purkinje.value) {
                        keywords.Add(SKW_PURKINJE);
                    }
                }

                // Eye adaptation
                bool usesEyeAdaptation = beautify.eyeAdaptation.value && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyEyeAdaptation.value);
                requiresLuminanceComputation = Application.isPlaying && (usesEyeAdaptation || usesPurkinje);
                if (requiresLuminanceComputation) {
                    Vector4 eaData = new Vector4(beautify.eyeAdaptationMinExposure.value, beautify.eyeAdaptationMaxExposure.value, beautify.eyeAdaptationSpeedToDark.value, beautify.eyeAdaptationSpeedToLight.value);
                    bMat.SetVector(ShaderParams.eaParams, eaData);
                    if (usesEyeAdaptation) {
                        keywords.Add(SKW_EYE_ADAPTATION);
                    }
                }

                // Outline
                usesSeparateOutline = false;
                if (beautify.outline.value && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyOutline.value)) {
                    usesSeparateOutline = beautify.outlineCustomize.value;
                    if (usesSeparateOutline) {
                        bMat.SetFloat(ShaderParams.outlineIntensityMultiplier, beautify.outlineIntensityMultiplier.value);
                        float outlineDistanceFade = beautify.outlineDistanceFade.value / cam.farClipPlane;
                        bMat.SetFloat(ShaderParams.outlineDistanceFade, outlineDistanceFade);
                        if (outlineDistanceFade > 0) {
                            keywords.Add(SKW_OUTLINE_DEPTH_FADE);
                        }
                    } else {
                        keywords.Add(SKW_OUTLINE);
                    }
                    Color color = beautify.outlineColor.value;
                    color.a = 1f - beautify.outlineThreshold.value;
                    bMat.SetColor(ShaderParams.outline, color);
                }

                // Color tweaks
                if (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyColorTweaks.value) {
                    if (beautify.sepia.value > 0 || beautify.daltonize.value > 0 || beautify.colorTempBlend.value > 0) {
                        keywords.Add(SKW_COLOR_TWEAKS);
                    }
                }

                // ACES Tonemapping
                if (beautify.tonemap.value == Beautify.TonemapOperator.ACES && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyTonemapping.value)) {
                    keywords.Add(SKW_TONEMAP_ACES);
                }

                // LUT or Nightvision
                Texture lutTex = beautify.lutTexture.value;
                bool hasLut = beautify.lut.value && beautify.lutIntensity.value > 0 && lutTex != null;
                bool hasLut3D = hasLut && lutTex is Texture3D;

                if (hasLut3D) {
                    hasLut3D = beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyLUT3D.value;
                } else if (hasLut) {
                    hasLut = beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyLUT.value;
                }
                if (hasLut || hasLut3D) {
                    if (hasLut3D) {
                        bMat.SetTexture(ShaderParams.lut3DTexture, lutTex);
                        bMat.SetVector(ShaderParams.lut3DParams, new Vector4(1f / lutTex.width, lutTex.width - 1f, 0, 0));
                        keywords.Add(SKW_LUT3D);
                    } else {
                        bMat.SetTexture(ShaderParams.lutTex, beautify.lutTexture.value);
                        keywords.Add(SKW_LUT);
                    }
                } else if (beautify.nightVision.value && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyNightVision.value)) {
                    keywords.Add(SKW_NIGHT_VISION);
                    Color nightVisionAdjusted = beautify.nightVisionColor.value;
                    if (linearColorSpace) {
                        nightVisionAdjusted.a *= 5.0f * nightVisionAdjusted.a;
                    } else {
                        nightVisionAdjusted.a *= 3.0f * nightVisionAdjusted.a;
                    }
                    nightVisionAdjusted.r *= nightVisionAdjusted.a;
                    nightVisionAdjusted.g *= nightVisionAdjusted.a;
                    nightVisionAdjusted.b *= nightVisionAdjusted.a;
                    bMat.SetColor(ShaderParams.nightVision, nightVisionAdjusted);
                }

                // Dither
                if (beautify.ditherIntensity.value > 0f && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyDithering.value)) {
                    keywords.Add(SKW_DITHER);
                }

                // Best performance mode
                if (beautify.turboMode.value) {
                    keywords.Add(SKW_TURBO);
                }

                // Chromatic Aberration
                if (beautify.chromaticAberrationIntensity.value > 0f && (beautify.optimizeBuildBeautifyAuto.value || !beautify.stripBeautifyChromaticAberration.value)) {
                    bMat.SetVector(ShaderParams.chromaticAberrationData, new Vector4(beautify.chromaticAberrationIntensity.value, beautify.chromaticAberrationSmoothing.value, beautify.chromaticAberrationShift.value, 0));
                    if (!beautify.depthOfField.value) {
                        keywords.Add(SKW_CHROMATIC_ABERRATION);
                    }
                }

                // Final blur mask
                if (beautify.blurIntensity.value > 0 && beautify.blurMask.value != null) {
                    bMat.SetTexture(ShaderParams.blurMask, beautify.blurMask.value);
                }

                int keywordsCount = keywords.Count;
                if (keywordsArray == null || keywordsArray.Length < keywordsCount) {
                    keywordsArray = new string[keywordsCount];
                }
                for (int k = 0; k < keywordsArray.Length; k++) {
                    if (k < keywordsCount) {
                        keywordsArray[k] = keywords[k];
                    } else {
                        keywordsArray[k] = "";
                    }
                }
                bMat.shaderKeywords = keywordsArray;
            }



            void UpdateMaterialBloomIntensityAndThreshold() {
                float bloomThreshold = beautify.bloomThreshold.value;
                float anamorphicThreshold = beautify.anamorphicFlaresThreshold.value;
                if (QualitySettings.activeColorSpace == ColorSpace.Linear) {
                    bloomThreshold *= bloomThreshold;
                    anamorphicThreshold *= anamorphicThreshold;
                }
                float anamorphicFlaresIntensity = beautify.turboMode.value ? beautify.anamorphicFlaresIntensity.value * 2f : beautify.anamorphicFlaresIntensity.value;
                float bloomIntensity = beautify.turboMode.value ? beautify.bloomIntensity.value * 2f : beautify.bloomIntensity.value;
                if (anamorphicFlaresIntensity > 0) {
                    float intensity = anamorphicFlaresIntensity / (bloomIntensity + 0.0001f);
                    bMat.SetVector(ShaderParams.afData, new Vector4(intensity, anamorphicThreshold, 0, 0));
                }
                Vector4 b4 = new Vector4(bloomIntensity + (anamorphicFlaresIntensity > 0 ? 0.0001f : 0f), 0, 0, bloomThreshold);
                bMat.SetVector(ShaderParams.bloom, b4);
            }

            void UpdateDepthOfFieldData(CommandBuffer cmd) {
                // TODO: get focal length from camera FOV: FOV = 2 arctan (x/2f) x = diagonal of film (0.024mm)
                if (!CheckSceneSettings()) return;
                Camera cam = cameraData.camera;
                float d = beautify.depthOfFieldDistance.value;
                switch ((int)beautify.depthOfFieldFocusMode.value) {
                    case (int)Beautify.DoFFocusMode.AutoFocus:
                        UpdateDoFAutofocusDistance(cam);
                        d = dofLastAutofocusDistance > 0 ? dofLastAutofocusDistance : cam.farClipPlane;
                        BeautifySettings.depthOfFieldCurrentFocalPointDistance = dofLastAutofocusDistance;
                        break;
                    case (int)Beautify.DoFFocusMode.FollowTarget:
                        if (sceneSettings.depthOfFieldTarget != null) {
                            Vector3 spos = cam.WorldToScreenPoint(sceneSettings.depthOfFieldTarget.position);
                            if (spos.z < 0) {
                                d = cam.farClipPlane;
                            } else {
                                d = Vector3.Distance(cam.transform.position, sceneSettings.depthOfFieldTarget.position);
                            }
                        }
                        break;
                }

                if (sceneSettings.OnBeforeFocus != null) {
                    d = sceneSettings.OnBeforeFocus(d);
                }
                dofPrevDistance = Mathf.Lerp(dofPrevDistance, d, Application.isPlaying ? beautify.depthOfFieldFocusSpeed.value * Time.unscaledDeltaTime * 30f : 1f);
                float dofCoc;
                if (beautify.depthOfFieldCameraSettings.value == Beautify.DoFCameraSettings.Real) {
                    float focalLength = beautify.depthOfFieldFocalLengthReal.value;
                    float aperture = (focalLength / beautify.depthOfFieldFStop.value);
                    dofCoc = aperture * (focalLength / Mathf.Max(dofPrevDistance * 1000f - focalLength, 0.001f)) * (1f / beautify.depthOfFieldImageSensorHeight.value) * cam.pixelHeight;
                } else {
                    // focal length in meters; aperture in mm
                    dofCoc = beautify.depthOfFieldAperture.value * (beautify.depthOfFieldFocalLength.value / Mathf.Max(dofPrevDistance - beautify.depthOfFieldFocalLength.value, 0.001f)) * (1f / 0.024f);
                }
                dofLastBokehData = new Vector4(dofPrevDistance, dofCoc, 0, 0);
                cmd.SetGlobalVector(ShaderParams.dofBokehData, dofLastBokehData);
                bMat.SetVector(ShaderParams.dofBokehData2, new Vector4(beautify.depthOfFieldForegroundBlur.value ? beautify.depthOfFieldForegroundDistance.value : cam.farClipPlane, beautify.depthOfFieldMaxSamples.value, beautify.depthOfFieldBokehThreshold.value, beautify.depthOfFieldBokehIntensity.value * beautify.depthOfFieldBokehIntensity.value));
                bMat.SetVector(ShaderParams.dofBokehData3, new Vector4(beautify.depthOfFieldMaxBrightness.value, beautify.depthOfFieldMaxDistance.value * (cam.farClipPlane + 1f), 0, 0));
            }


            void UpdateDoFAutofocusDistance(Camera cam) {
                Vector3 p = beautify.depthOfFieldAutofocusViewportPoint.value;
                p.z = 10f;
                Ray r = cam.ViewportPointToRay(p);
                RaycastHit hit;
                if (Physics.Raycast(r, out hit, cam.farClipPlane, beautify.depthOfFieldAutofocusLayerMask.value)) {
                    // we don't use hit.distance as ray origin has a small shift from camera
                    float distance = Vector3.Distance(cam.transform.position, hit.point);
                    distance += beautify.depthOfFieldAutofocusDistanceShift.value;
                    dofLastAutofocusDistance = Mathf.Clamp(distance, beautify.depthOfFieldAutofocusMinDistance.value, beautify.depthOfFieldAutofocusMaxDistance.value);
                } else {
                    dofLastAutofocusDistance = cam.farClipPlane;
                }
            }



            // Scene dependant settings
            BeautifySettings sceneSettings;

            void CheckSun() {

                if (!CheckSceneSettings()) return;

                // Fetch a valid Sun reference
                if (sceneSettings.sun == null) {
                    Light[] lights = FindObjectsOfType<Light>();
                    for (int k = 0; k < lights.Length; k++) {
                        Light light = lights[k];
                        if (light.type == LightType.Directional && light.isActiveAndEnabled) {
                            sceneSettings.sun = light.transform;
                            break;
                        }
                    }
                }
            }

            bool CheckSceneSettings() {
                sceneSettings = BeautifySettings.instance;
                return sceneSettings != null;
            }

            void CheckCameraDepthTextureMode(Camera cam) {
                if (beautify.RequiresDepthTexture()) {
                    cam.depthTextureMode |= DepthTextureMode.Depth;
                }
            }

        }

        class BeautifyBloomLumMaskPass : ScriptableRenderPass {

            readonly List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

            const string bloomSourceDepthRT = "_BloomSourceDepth";
            static int bloomSourceDepthId = Shader.PropertyToID(bloomSourceDepthRT);
            RTHandle maskRT;

            public BeautifyBloomLumMaskPass() {
                RenderTargetIdentifier rti = new RenderTargetIdentifier(bloomSourceDepthRT, 0, CubemapFace.Unknown, -1);
                maskRT = RTHandles.Alloc(rti, name: bloomSourceDepthRT);
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
                m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                RenderTextureDescriptor depthDesc = cameraTextureDescriptor;
                depthDesc.colorFormat = RenderTextureFormat.ARGB32;
                depthDesc.depthBufferBits = 0;
                depthDesc.msaaSamples = 1;
                cmd.GetTemporaryRT(bloomSourceDepthId, depthDesc, FilterMode.Point);
                cmd.SetGlobalTexture(bloomSourceDepthRT, bloomSourceDepthId);
                if (BeautifySettings.anamorphicFlaresExcludeMask == BeautifySettings.bloomExcludeMask) {
                    cmd.SetGlobalTexture(BeautifyAnamorphicFlaresLumMaskPass.afSourceDepthRT, bloomSourceDepthId);
                }
                ConfigureTarget(maskRT);
                ConfigureClear(ClearFlag.Color, Color.black);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

                SortingCriteria sortingCriteria = SortingCriteria.None;
                var drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
                var filter = new FilteringSettings(RenderQueueRange.all) { layerMask = BeautifySettings.bloomExcludeMask };
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filter);
            }

            public override void FrameCleanup(CommandBuffer cmd) {
                if (cmd == null) return;
                cmd.ReleaseTemporaryRT(bloomSourceDepthId);
            }

        }


        class BeautifyAnamorphicFlaresLumMaskPass : ScriptableRenderPass {

            readonly List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

            public const string afSourceDepthRT = "_AFSourceDepth";
            static int afSourceDepthId = Shader.PropertyToID(afSourceDepthRT);
            RTHandle maskRT;

            public BeautifyAnamorphicFlaresLumMaskPass() {
                RenderTargetIdentifier rti = new RenderTargetIdentifier(afSourceDepthRT, 0, CubemapFace.Unknown, -1);
                maskRT = RTHandles.Alloc(rti, name: afSourceDepthRT);
                renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
                m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                RenderTextureDescriptor depthDesc = cameraTextureDescriptor;
                depthDesc.colorFormat = RenderTextureFormat.ARGB32;
                depthDesc.depthBufferBits = 0;
                depthDesc.msaaSamples = 1;
                cmd.GetTemporaryRT(afSourceDepthId, depthDesc, FilterMode.Point);
                cmd.SetGlobalTexture(afSourceDepthRT, afSourceDepthId);
                ConfigureTarget(maskRT);
                ConfigureClear(ClearFlag.Color, Color.black);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

                SortingCriteria sortingCriteria = SortingCriteria.None;
                var drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
                var filter = new FilteringSettings(RenderQueueRange.all) { layerMask = BeautifySettings.anamorphicFlaresExcludeMask };
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filter);
            }

            public override void FrameCleanup(CommandBuffer cmd) {
                if (cmd == null) return;
                cmd.ReleaseTemporaryRT(afSourceDepthId);
            }

        }

        class BeautifyDoFTransparentMaskPass : ScriptableRenderPass {

            readonly List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
            static readonly List<Renderer> cutOutRenderers = new List<Renderer>();

            const string dofTransparentDepthRT = "_DoFTransparentDepth";
            static int dofTransparentDepthId = Shader.PropertyToID(dofTransparentDepthRT);
            static int m_CullPropertyId = Shader.PropertyToID("_Cull");
            const string m_ProfilerTag = "CustomDepthPrePass";
            const string m_DepthOnlyShader = "Hidden/Beautify2/DepthOnly";

            RTHandle m_Depth;

            Material depthOnlyMaterial, depthOnlyMaterialCutOff;
            int currentAlphaCutoutLayerMask = -999;
            Material[] depthOverrideMaterials;

            public BeautifyDoFTransparentMaskPass() {
                RenderTargetIdentifier rti = new RenderTargetIdentifier(dofTransparentDepthRT, 0, CubemapFace.Unknown, -1);
                m_Depth = RTHandles.Alloc(rti, name: dofTransparentDepthRT);
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
                m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            }


            public void FindAlphaClippingRenderers() {
                cutOutRenderers.Clear();
                currentAlphaCutoutLayerMask = BeautifySettings.dofAlphaTestLayerMask;
                Renderer[] rr = FindObjectsOfType<Renderer>();
                for (int r = 0; r < rr.Length; r++) {
                    if (((1 << rr[r].gameObject.layer) & currentAlphaCutoutLayerMask) != 0) {
                        cutOutRenderers.Add(rr[r]);
                    }
                }
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                RenderTextureDescriptor depthDesc = cameraTextureDescriptor;
                depthDesc.colorFormat = RenderTextureFormat.Depth;
                depthDesc.depthBufferBits = 24;
                depthDesc.msaaSamples = 1;
                cmd.GetTemporaryRT(dofTransparentDepthId, depthDesc, FilterMode.Point);
                cmd.SetGlobalTexture(dofTransparentDepthRT, dofTransparentDepthId);
                ConfigureTarget(m_Depth);
                ConfigureClear(ClearFlag.All, Color.black);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                cmd.Clear();

                if (BeautifySettings.dofAlphaTestSupport) {
                    if (BeautifySettings.dofAlphaTestLayerMask != 0) {
                        if (BeautifySettings.dofAlphaTestLayerMask != currentAlphaCutoutLayerMask) {
                            FindAlphaClippingRenderers();
                        }
                        if (depthOnlyMaterialCutOff == null) {
                            Shader depthOnlyCutOff = Shader.Find(m_DepthOnlyShader);
                            depthOnlyMaterialCutOff = new Material(depthOnlyCutOff);
                        }
                        int renderersCount = cutOutRenderers.Count;
                        if (depthOverrideMaterials == null || depthOverrideMaterials.Length < renderersCount) {
                            depthOverrideMaterials = new Material[renderersCount];
                        }
                        for (int k = 0; k < renderersCount; k++) {
                            Renderer renderer = cutOutRenderers[k];
                            if (renderer != null && renderer.isVisible) {
                                Material mat = renderer.sharedMaterial;
                                if (mat != null) {
                                    if (depthOverrideMaterials[k] == null) {
                                        depthOverrideMaterials[k] = Instantiate(depthOnlyMaterialCutOff);
                                        depthOverrideMaterials[k].EnableKeyword(SKW_CUSTOM_DEPTH_ALPHA_TEST);
                                    }
                                    Material overrideMaterial = depthOverrideMaterials[k];

                                    if (mat.HasProperty(ShaderParams.CustomDepthAlphaCutoff)) {
                                        overrideMaterial.SetFloat(ShaderParams.CustomDepthAlphaCutoff, mat.GetFloat(ShaderParams.CustomDepthAlphaCutoff));
                                    } else {
                                        overrideMaterial.SetFloat(ShaderParams.CustomDepthAlphaCutoff, 0.5f);
                                    }
                                    if (mat.HasProperty(ShaderParams.CustomDepthBaseMap)) {
                                        overrideMaterial.SetTexture(ShaderParams.mainTex, mat.GetTexture(ShaderParams.CustomDepthBaseMap));
                                    } else if (mat.HasProperty(ShaderParams.mainTex)) {
                                        overrideMaterial.SetTexture(ShaderParams.mainTex, mat.GetTexture(ShaderParams.mainTex));
                                    }
                                    overrideMaterial.SetInt(m_CullPropertyId, BeautifySettings.dofAlphaTestDoubleSided ? (int)CullMode.Off : (int)CullMode.Back);

                                    cmd.DrawRenderer(renderer, overrideMaterial);
                                }
                            }
                        }

                    }
                }

                // Render transparent objects
                if (BeautifySettings.dofTransparentSupport) {
                    if (depthOnlyMaterial == null) {
                        depthOnlyMaterial = new Material(Shader.Find(m_DepthOnlyShader));
                    }
                    depthOnlyMaterial.SetInt(m_CullPropertyId, BeautifySettings.dofTransparentDoubleSided ? (int)CullMode.Off : (int)CullMode.Back);

                    SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
                    var drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
                    drawingSettings.perObjectData = PerObjectData.None;
                    drawingSettings.overrideMaterial = depthOnlyMaterial;
                    var filter = new FilteringSettings(RenderQueueRange.transparent) { layerMask = BeautifySettings.dofTransparentLayerMask };
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filter);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd) {
                if (cmd == null) return;
                cmd.ReleaseTemporaryRT(dofTransparentDepthId);
            }

        }

        [SerializeField, HideInInspector]
        Shader shader;
        BeautifyRenderPass m_BeautifyRenderPass;
        BeautifyDoFTransparentMaskPass m_BeautifyDoFTransparentMaskPass;

        BeautifyBloomLumMaskPass m_BeautifyBloomLumMaskPass;
        BeautifyAnamorphicFlaresLumMaskPass m_BeautifyAnamorphicFlaresLumMaskPass;

        [Tooltip("Note: this option is ignored if Direct Write To Camera option in Beautify volume inspector is enabled.")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        [Tooltip("Allows Beautify to be executed even if camera has Post Processing option disabled.")]
        public bool ignorePostProcessingOption;

        [Tooltip("Specify which cameras can render Beautify effects")]
        public LayerMask cameraLayerMask = -1;

        public static bool installed;
        public static bool ignoringPostProcessingOption;

#if UNITY_EDITOR
        public static CameraType captureCameraType = CameraType.SceneView;
        public static bool requestScreenCapture;
#endif


        void OnDisable() {
            if (m_BeautifyRenderPass != null) {
                m_BeautifyRenderPass.Cleanup();
            }
            installed = false;
        }


        public override void Create() {
            name = "Beautify";
            m_BeautifyRenderPass = new BeautifyRenderPass();
            m_BeautifyBloomLumMaskPass = new BeautifyBloomLumMaskPass();
            m_BeautifyAnamorphicFlaresLumMaskPass = new BeautifyAnamorphicFlaresLumMaskPass();
            m_BeautifyDoFTransparentMaskPass = new BeautifyDoFTransparentMaskPass();
            shader = Shader.Find("Hidden/Kronnect/Beautify");
        }

        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            installed = true;
            ignoringPostProcessingOption = ignorePostProcessingOption;

            if (ignorePostProcessingOption || renderingData.cameraData.postProcessEnabled) {
                if ((cameraLayerMask & (1 << renderingData.cameraData.camera.gameObject.layer)) == 0) return;
                if (m_BeautifyRenderPass.Setup(shader, renderer, renderingData, renderPassEvent, ignorePostProcessingOption)) {
                    if (BeautifySettings.bloomExcludeMask > 0) {
                        renderer.EnqueuePass(m_BeautifyBloomLumMaskPass);
                    }
                    if (BeautifySettings.anamorphicFlaresExcludeMask > 0 && BeautifySettings.anamorphicFlaresExcludeMask != BeautifySettings.bloomExcludeMask) {
                        renderer.EnqueuePass(m_BeautifyAnamorphicFlaresLumMaskPass);
                    }
                    if (BeautifySettings.dofTransparentSupport || BeautifySettings.dofAlphaTestSupport) {
                        renderer.EnqueuePass(m_BeautifyDoFTransparentMaskPass);
                    }
                    renderer.EnqueuePass(m_BeautifyRenderPass);
                }
            }
        }
    }
}
