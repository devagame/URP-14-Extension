using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting.APIUpdating;
using Debug = UnityEngine.Debug;

#if URP12_OR_NEWER
#if UNITY_2022_1_OR_NEWER
using RTH = UnityEngine.Rendering.RTHandle;

#else
using RTH = UnityEngine.Rendering.Universal.RenderTargetHandle;
#endif
#endif

[assembly: InternalsVisibleTo("TranslucentImage.UniversalRP.Editor")]

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
    class URPRendererInternal
    {
#if URP12_OR_NEWER
        ScriptableRenderer renderer;
        Func<RTH> getBackBufferDelegate;
        Func<RTH> getAfterPostColorDelegate;
#endif

        public void CacheRenderer(ScriptableRenderer renderer)
        {
#if URP12_OR_NEWER
            if (this.renderer == renderer) return;

            this.renderer = renderer;
#if UNITY_2022_1_OR_NEWER
            const string backBufferMethodName = "PeekBackBuffer";
#else
        const string backBufferMethodName = "GetBackBuffer";
#endif

#if UNITY_2022_3_OR_NEWER
            var ur = renderer;
#else
        if (renderer is UniversalRenderer ur)
#endif
            {
                var cbs = ur.GetType()
                    .GetField("m_ColorBufferSystem",
                        BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(ur);
                var gbb = cbs.GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .First(m => m.Name == backBufferMethodName && m.GetParameters().Length == 0);

                getBackBufferDelegate = (Func<RTH>)gbb.CreateDelegate(typeof(Func<RTH>), cbs);
            }
#if !UNITY_2022_3_OR_NEWER
        else
        {
            getAfterPostColorDelegate = (Func<RTH>)renderer.GetType()
                                                           .GetProperty("afterPostProcessColorHandle",
                                                                        BindingFlags.NonPublic | BindingFlags.Instance)
                                                           .GetGetMethod(true)
                                                           .CreateDelegate(typeof(Func<RTH>), renderer);
        }
#endif
#endif
        }

#if URP12_OR_NEWER
        public RenderTargetIdentifier GetBackBuffer()
        {
            Debug.Assert(getBackBufferDelegate != null);

            var r = getBackBufferDelegate.Invoke();
#if UNITY_2022_1_OR_NEWER
            return r.nameID;
#else
        return r.Identifier();
#endif
        }

        public RenderTargetIdentifier GetAfterPostColor()
        {
            Debug.Assert(getAfterPostColorDelegate != null);

            var r = getAfterPostColorDelegate.Invoke();
#if UNITY_2022_1_OR_NEWER
            return r.nameID;
#else
        return r.Identifier();
#endif
        }
#endif
    }

    public enum RenderOrder
    {
        AfterPostProcessing,
        BeforePostProcessing,
    }

    [MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
    public class TranslucentImageBlurSource : ScriptableRendererFeature
    {
#if URP12_OR_NEWER
        public RenderPassEvent renderOrder = RenderPassEvent.AfterRenderingPostProcessing;
#endif
        public bool canvasDisappearWorkaround = false;

        readonly Dictionary<Camera, TranslucentImageSource> tisCache = new Dictionary<Camera, TranslucentImageSource>();
        readonly Dictionary<Camera, Camera> baseCameraCache = new Dictionary<Camera, Camera>();

        URPRendererInternal urpRendererInternal;
        TranslucentImageBlurRenderPass pass;
        IBlurAlgorithm blurAlgorithm;

        internal RendererType rendererType;

        /// <summary>
        /// When adding new Translucent Image Source to existing Camera at run time, the new Source must be registered here
        /// </summary>
        /// <param name="source"></param>
        public void RegisterSource(TranslucentImageSource source)
        {
            tisCache[source.GetComponent<Camera>()] = source;
        }

        public override void Create()
        {
            ShaderId.Init(14);

            blurAlgorithm = new ScalableBlur();

            urpRendererInternal = new URPRendererInternal();

            // ReSharper disable once JoinDeclarationAndInitializer
            RenderPassEvent renderPassEvent;
#if URP12_OR_NEWER
            renderPassEvent = renderOrder /*== RenderOrder.BeforePostProcessing
                              ? RenderPassEvent.BeforeRenderingPostProcessing
                              : RenderPassEvent.AfterRenderingPostProcessing*/;
#else
        renderPassEvent = RenderPassEvent.AfterRendering;
#endif
            pass = new TranslucentImageBlurRenderPass(urpRendererInternal)
            {
                renderPassEvent = renderPassEvent
            };

            tisCache.Clear();
        }

        void Setup(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            var cameraData = renderingData.cameraData;
            var tis = GetTIS(cameraData.camera);

            if (tis == null)
                return;

            urpRendererInternal.CacheRenderer(renderer);

#if UNITY_2021_3_OR_NEWER
            if (renderer is UniversalRenderer)
#else
        if (renderer is ForwardRenderer)
#endif
            {
                rendererType = RendererType.Universal;
            }
            else
            {
                rendererType = RendererType.Renderer2D;
            }

#if UNITY_BUGGED_HAS_PASSES_AFTER_POSTPROCESS
        bool applyFinalPostProcessing = renderingData.postProcessingEnabled
                                     && cameraData.resolveFinalTarget
                                     && (cameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing
                                        );
        pass.renderPassEvent =
 applyFinalPostProcessing ? RenderPassEvent.AfterRenderingPostProcessing : RenderPassEvent.AfterRendering;
#endif

            var passData = new TISPassData
            {
                rendererType = rendererType,
#if UNITY_2022_1_OR_NEWER
                cameraColorTarget = renderer.cameraColorTargetHandle,
#else
            cameraColorTarget = renderer.cameraColorTarget,
#endif
                blurAlgorithm = blurAlgorithm,
#if URP12_OR_NEWER
                renderOrder = renderOrder,
#endif
                blurSource = tis,
                isPreviewing = tis.Preview,
                canvasDisappearWorkaround = canvasDisappearWorkaround,
            };
            if (tis.CheckSourceType())
            {
                tis.SwtichSourceRendererType(tis.BlurConfig.sourceRenderType);
                pass.renderPassEvent = renderOrder;
            }

            pass.Setup(passData);
        }

#if UNITY_2022_1_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            Setup(renderer, renderingData);
        }
#endif

        readonly FieldInfo cameraDataPixelRectField =
            typeof(CameraData).GetField("pixelRect", BindingFlags.Instance | BindingFlags.NonPublic);

        public Rect GetPixelSize(CameraData cameraData)
        {
            if (cameraData.renderType == CameraRenderType.Base)
                return cameraData.camera.pixelRect;

            if (cameraDataPixelRectField == null)
                Debug.LogError("CameraData.pixelRect does not exists in this version of URP. Please report a bug.");

            return (Rect)cameraDataPixelRectField.GetValue(cameraData);
        }

        public override void AddRenderPasses(
            ScriptableRenderer renderer,
            ref RenderingData renderingData
        )
        {
#if !UNITY_2022_1_OR_NEWER
        Setup(renderer, renderingData);
#endif
            

            var cameraData = renderingData.cameraData;
            var camera = renderingData.cameraData.camera;
            var tis = GetTIS(camera);

            if (tis == null || !tis.shouldUpdateBlur())
                return;

            tis.CamRectOverride = Rect.zero;
            if (cameraData.renderType == CameraRenderType.Overlay)
            {
                var baseCam = GetBaseCamera(camera);
                if (baseCam)
                    tis.CamRectOverride = baseCam.rect;
            }


            var camPixelSize = GetPixelSize(cameraData).size;
            tis.OnBeforeBlur(Vector2Int.RoundToInt(camPixelSize));
            blurAlgorithm.Init(tis.BlurConfig, false);

            renderer.EnqueuePass(pass);
        }

        TranslucentImageSource GetTIS(Camera camera)
        {
            if (!tisCache.ContainsKey(camera))
            {
                tisCache.Add(camera, camera.GetComponent<TranslucentImageSource>());
            }

            return tisCache[camera];
        }

        Camera GetBaseCamera(Camera camera)
        {
            if (!baseCameraCache.ContainsKey(camera))
            {
                Camera baseCamera = null;

                foreach (var uacd in Shims.FindObjectsOfType<UniversalAdditionalCameraData>())
                {
                    if (uacd.renderType != CameraRenderType.Base) continue;
                    if (uacd.cameraStack == null) continue;
                    if (!uacd.cameraStack.Contains(camera)) continue;

                    baseCamera = uacd.GetComponent<Camera>();
                }

                baseCameraCache.Add(camera, baseCamera);
            }

            return baseCameraCache[camera];
        }
    }
}