#if UNITY_BIRP
using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using FidelityFX;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace AEG.FSR
{
    /// <summary>
    /// FSR implementation for the Built-in RenderRipeline
    /// </summary>
    public class FSR3_BIRP : FSR3_BASE
    {
        // Commandbuffers
        private CommandBuffer m_colorGrabPass;
        private CommandBuffer m_fsrComputePass;
        private CommandBuffer m_opaqueOnlyGrabPass;
        private CommandBuffer m_afterOpaqueOnlyGrabPass;
        private CommandBuffer m_blitToCamPass;

        // Rendertextures
        private RenderTexture m_opaqueOnlyColorBuffer;
        private RenderTexture m_afterOpaqueOnlyColorBuffer;
        private RenderTexture m_reactiveMaskOutput;
        private RenderTexture m_colorBuffer;
        private RenderTexture m_fsrOutput;

        // Commandbuffer events
        private const CameraEvent m_OPAQUE_ONLY_EVENT = CameraEvent.BeforeForwardAlpha;
        private const CameraEvent m_AFTER_OPAQUE_ONLY_EVENT = CameraEvent.AfterForwardAlpha;
        private const CameraEvent m_COLOR_EVENT = CameraEvent.BeforeImageEffects;
        private const CameraEvent m_FSR_EVENT = CameraEvent.BeforeImageEffects;
        private const CameraEvent m_BlITFSR_EVENT = CameraEvent.AfterImageEffects;

        private Matrix4x4 m_jitterMatrix;
        private Matrix4x4 m_projectionMatrix;

        private readonly Fsr3.DispatchDescription m_dispatchDescription = new Fsr3.DispatchDescription();
        private readonly Fsr3.GenerateReactiveDescription m_genReactiveDescription = new Fsr3.GenerateReactiveDescription();
        private IFsr3Callbacks Callbacks { get; set; } = new Fsr3CallbacksBase();
        private Fsr3Context m_context;

        private bool initFirstFrame = false;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void InitializeFSR() {
            base.InitializeFSR();
            m_mainCamera.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.MotionVectors;

            m_colorGrabPass = new CommandBuffer { name = "AMD FSR: Color Grab Pass" };
            m_opaqueOnlyGrabPass = new CommandBuffer { name = "AMD FSR: Opaque Only Grab Pass" };
            m_afterOpaqueOnlyGrabPass = new CommandBuffer { name = "AMD FSR: After Opaque Only Grab Pass" };
            m_fsrComputePass = new CommandBuffer { name = "AMD FSR: Compute Pass" };
            m_blitToCamPass = new CommandBuffer { name = "AMD FSR: Blit to Camera" };

            SendMessage("RemovePPV2CommandBuffers", SendMessageOptions.DontRequireReceiver);
            SetupResolution();

            if(!m_fsrInitialized) {
                Camera.onPreRender += OnPreRenderCamera;
                Camera.onPostRender += OnPostRenderCamera;
            }

        }

        /// <summary>
        /// Sets up the buffers, initializes the fsr context, and sets up the command buffer
        /// Must be recalled whenever the display resolution changes
        /// </summary>
        private void SetupCommandBuffer() {
            ClearCommandBufferCoroutine();

            if(m_colorBuffer) {
                if(m_opaqueOnlyColorBuffer) {
                    m_opaqueOnlyColorBuffer.Release();
                    m_afterOpaqueOnlyColorBuffer.Release();
                    m_reactiveMaskOutput.Release();
                }
                m_colorBuffer.Release();
                m_fsrOutput.Release();
            }

            m_renderWidth = (int)(m_displayWidth / m_scaleFactor);
            m_renderHeight = (int)(m_displayHeight / m_scaleFactor);

            m_colorBuffer = new RenderTexture(m_renderWidth, m_renderHeight, 0, RenderTextureFormat.Default);
            m_colorBuffer.Create();
            m_fsrOutput = new RenderTexture(m_displayWidth, m_displayHeight, 0, m_mainCamera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);
            m_fsrOutput.enableRandomWrite = true;
            m_fsrOutput.Create();


            m_dispatchDescription.InputResourceSize = new Vector2Int(m_renderWidth, m_renderHeight);
            m_dispatchDescription.Color = m_colorBuffer;
            if(m_mainCamera.actualRenderingPath == RenderingPath.Forward) {
                m_dispatchDescription.Depth = BuiltinRenderTextureType.Depth;
            } else {
                m_dispatchDescription.Depth = BuiltinRenderTextureType.ResolvedDepth;
            }
            m_dispatchDescription.MotionVectors = BuiltinRenderTextureType.MotionVectors;
            m_dispatchDescription.Output = m_fsrOutput;

            if(generateReactiveMask) {
                m_opaqueOnlyColorBuffer = new RenderTexture(m_colorBuffer);
                m_opaqueOnlyColorBuffer.Create();
                m_afterOpaqueOnlyColorBuffer = new RenderTexture(m_colorBuffer);
                m_afterOpaqueOnlyColorBuffer.Create();
                m_reactiveMaskOutput = new RenderTexture(m_colorBuffer);
                m_reactiveMaskOutput.enableRandomWrite = true;
                m_reactiveMaskOutput.Create();

                m_genReactiveDescription.ColorOpaqueOnly = m_opaqueOnlyColorBuffer;
                m_genReactiveDescription.ColorPreUpscale = m_afterOpaqueOnlyColorBuffer;
                m_genReactiveDescription.OutReactive = m_reactiveMaskOutput;
                m_dispatchDescription.Reactive = m_reactiveMaskOutput;
            } else {
                m_genReactiveDescription.ColorOpaqueOnly = null;
                m_genReactiveDescription.ColorPreUpscale = null;
                m_genReactiveDescription.OutReactive = null;
                m_dispatchDescription.Reactive = null;
            }
            //Experimental! (disabled)
            if(generateTCMask) {
                if(generateReactiveMask) {
                    m_dispatchDescription.ColorOpaqueOnly = m_reactiveMaskOutput;
                } else {
                    m_dispatchDescription.ColorOpaqueOnly = m_opaqueOnlyColorBuffer;
                }
            } else {
                m_dispatchDescription.ColorOpaqueOnly = null;
            }

            if(m_fsrComputePass != null) {
                m_mainCamera.RemoveCommandBuffer(m_COLOR_EVENT, m_colorGrabPass);
                m_mainCamera.RemoveCommandBuffer(m_FSR_EVENT, m_fsrComputePass);
                m_mainCamera.RemoveCommandBuffer(m_BlITFSR_EVENT, m_blitToCamPass);

                if(m_opaqueOnlyGrabPass != null) {
                    m_mainCamera.RemoveCommandBuffer(m_OPAQUE_ONLY_EVENT, m_opaqueOnlyGrabPass);
                    m_mainCamera.RemoveCommandBuffer(m_AFTER_OPAQUE_ONLY_EVENT, m_afterOpaqueOnlyGrabPass);
                }
            }

            m_colorGrabPass.Clear();
            m_fsrComputePass.Clear();
            m_blitToCamPass.Clear();

            m_colorGrabPass.Blit(BuiltinRenderTextureType.CameraTarget, m_colorBuffer);

            if(generateReactiveMask) {
                m_opaqueOnlyGrabPass.Clear();
                m_opaqueOnlyGrabPass.Blit(BuiltinRenderTextureType.CameraTarget, m_opaqueOnlyColorBuffer);

                m_afterOpaqueOnlyGrabPass.Clear();
                m_afterOpaqueOnlyGrabPass.Blit(BuiltinRenderTextureType.CameraTarget, m_afterOpaqueOnlyColorBuffer);
            }

            m_blitToCamPass.Blit(m_fsrOutput, BuiltinRenderTextureType.None);

            SendMessage("OverridePPV2TargetTexture", m_colorBuffer, SendMessageOptions.DontRequireReceiver);
            buildCommandBuffers = StartCoroutine(BuildCommandBuffer());
        }

        /// <summary>
        /// Built-in has no way to properly order commandbuffers, so we have to add them in the order we want ourselves.
        /// </summary>
        private Coroutine buildCommandBuffers;
        private IEnumerator BuildCommandBuffer() {
            SendMessage("RemovePPV2CommandBuffers", SendMessageOptions.DontRequireReceiver);
            yield return null;
            if(generateReactiveMask) {
                if(m_opaqueOnlyGrabPass != null) {
                    m_mainCamera.AddCommandBuffer(m_OPAQUE_ONLY_EVENT, m_opaqueOnlyGrabPass);
                    m_mainCamera.AddCommandBuffer(m_AFTER_OPAQUE_ONLY_EVENT, m_afterOpaqueOnlyGrabPass);
                }
            }
            yield return null;
            SendMessage("AddPPV2CommandBuffer", SendMessageOptions.DontRequireReceiver);
            yield return null;
            if(m_fsrComputePass != null) {
                m_mainCamera.AddCommandBuffer(m_COLOR_EVENT, m_colorGrabPass);
                m_mainCamera.AddCommandBuffer(m_FSR_EVENT, m_fsrComputePass);
                m_mainCamera.AddCommandBuffer(m_BlITFSR_EVENT, m_blitToCamPass);
            }

            buildCommandBuffers = null;
        }

        private void ClearCommandBufferCoroutine() {
            if(buildCommandBuffers != null) {
                StopCoroutine(buildCommandBuffers);
            }
        }

        private void OnPreRenderCamera(Camera camera) {
            if(camera != m_mainCamera) {
                return;
            }

            // Set up the parameters to auto-generate a reactive mask
            if(generateReactiveMask) {
                m_genReactiveDescription.RenderSize = new Vector2Int(m_renderWidth, m_renderHeight);
                m_genReactiveDescription.Scale = autoReactiveScale;
                m_genReactiveDescription.CutoffThreshold = autoReactiveThreshold;
                m_genReactiveDescription.BinaryValue = autoReactiveBinaryValue;
                m_genReactiveDescription.Flags = reactiveFlags;
            }

            m_dispatchDescription.Exposure = null;
            m_dispatchDescription.PreExposure = 1;
            m_dispatchDescription.EnableSharpening = sharpening;
            m_dispatchDescription.Sharpness = sharpness;
            m_dispatchDescription.MotionVectorScale.x = -m_renderWidth;
            m_dispatchDescription.MotionVectorScale.y = -m_renderHeight;
            m_dispatchDescription.RenderSize = new Vector2Int(m_renderWidth, m_renderHeight);
            m_dispatchDescription.FrameTimeDelta = Time.deltaTime;
            m_dispatchDescription.CameraNear = m_mainCamera.nearClipPlane;
            m_dispatchDescription.CameraFar = m_mainCamera.farClipPlane;
            m_dispatchDescription.CameraFovAngleVertical = m_mainCamera.fieldOfView * Mathf.Deg2Rad;
            m_dispatchDescription.ViewSpaceToMetersFactor = 1.0f;
            m_dispatchDescription.Reset = m_resetCamera;

            //Experimental!  (disabled)
            m_dispatchDescription.EnableAutoReactive = generateTCMask;
            m_dispatchDescription.AutoTcThreshold = autoTcThreshold;
            m_dispatchDescription.AutoTcScale = autoTcScale;
            m_dispatchDescription.AutoReactiveScale = autoReactiveScale;
            m_dispatchDescription.AutoReactiveMax = autoTcReactiveMax;

            m_resetCamera = false;

            if(SystemInfo.usesReversedZBuffer) {
                // Swap the near and far clip plane distances as FSR3 expects this when using inverted depth
                (m_dispatchDescription.CameraNear, m_dispatchDescription.CameraFar) = (m_dispatchDescription.CameraFar, m_dispatchDescription.CameraNear);
            }

            JitterTAA();

            m_mainCamera.targetTexture = m_colorBuffer;

            //Check if display resolution has changed
            if(m_displayWidth != Display.main.renderingWidth || m_displayHeight != Display.main.renderingHeight || m_previousHDR != m_mainCamera.allowHDR) {
                SetupResolution();
            }

            if(m_previousScaleFactor != m_scaleFactor || m_previousReactiveMask != generateReactiveMask || m_previousTCMask != generateTCMask || m_previousRenderingPath != m_mainCamera.actualRenderingPath || !initFirstFrame) {
                initFirstFrame = true;
                SetupFrameBuffers();
            }
            UpdateDispatch();
        }


        private void OnPostRenderCamera(Camera camera) {
            if(camera != m_mainCamera) {
                return;
            }

            m_mainCamera.targetTexture = null;

            m_mainCamera.ResetProjectionMatrix();
        }

        /// <summary>
        ///  TAA Jitter
        /// </summary>
        private void JitterTAA() {

            int jitterPhaseCount = Fsr3.GetJitterPhaseCount(m_renderWidth, (int)(m_renderWidth * m_scaleFactor));

            Fsr3.GetJitterOffset(out float jitterX, out float jitterY, Time.frameCount, jitterPhaseCount);
            m_dispatchDescription.JitterOffset = new Vector2(jitterX, jitterY);

            jitterX = 2.0f * jitterX / (float)m_renderWidth;
            jitterY = 2.0f * jitterY / (float)m_renderHeight;

            jitterX += UnityEngine.Random.Range(-0.001f * antiGhosting, 0.001f * antiGhosting);
            jitterY += UnityEngine.Random.Range(-0.001f * antiGhosting, 0.001f * antiGhosting);

            m_jitterMatrix = Matrix4x4.Translate(new Vector2(jitterX, jitterY));
            m_projectionMatrix = m_mainCamera.projectionMatrix;
            m_mainCamera.nonJitteredProjectionMatrix = m_projectionMatrix;
            m_mainCamera.projectionMatrix = m_jitterMatrix * m_projectionMatrix;
            m_mainCamera.useJitteredProjectionMatrixForTransparentRendering = true;
        }

        /// <summary>
        /// Creates new buffers and sends them to the plugin
        /// </summary>
        private void SetupFrameBuffers() {
            m_previousScaleFactor = m_scaleFactor;
            m_previousReactiveMask = generateReactiveMask;
            m_previousTCMask = generateTCMask;

            SetupCommandBuffer();

            m_previousRenderingPath = m_mainCamera.actualRenderingPath;
        }

        /// <summary>
        /// Creates new buffers, sends them to the plugin, and reintilized FSR to adjust the display size
        /// </summary>
        private void SetupResolution() {
            m_displayWidth = Display.main.renderingWidth;
            m_displayHeight = Display.main.renderingHeight;

            m_previousHDR = m_mainCamera.allowHDR;

            Fsr3.InitializationFlags flags = Fsr3.InitializationFlags.EnableAutoExposure;

            if(m_mainCamera.allowHDR)
                flags |= Fsr3.InitializationFlags.EnableHighDynamicRange;
            if(enableF16)
                flags |= Fsr3.InitializationFlags.EnableFP16Usage;

            if(m_context != null) {
                m_context.Destroy();
                m_context = null;
            }

            m_context = Fsr3.CreateContext(new Vector2Int(m_displayWidth, m_displayHeight), new Vector2Int((int)(m_displayWidth), (int)(m_displayHeight)), Callbacks, flags);

            SetupFrameBuffers();
        }

        private void UpdateDispatch() {
            if(m_fsrComputePass != null) {
                m_fsrComputePass.Clear();
                if(generateReactiveMask) {
                    m_context.GenerateReactiveMask(m_genReactiveDescription, m_fsrComputePass);
                }
                m_context.Dispatch(m_dispatchDescription, m_fsrComputePass);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void DisableFSR() {
            base.DisableFSR();
            Camera.onPreRender -= OnPreRenderCamera;
            Camera.onPostRender -= OnPostRenderCamera;

            initFirstFrame = false;

            ClearCommandBufferCoroutine();
            SendMessage("ResetPPV2CommandBuffer", SendMessageOptions.DontRequireReceiver);
            SendMessage("ResetPPV2TargetTexture", SendMessageOptions.DontRequireReceiver);

            OnResetAllMipMaps();

            if(m_mainCamera != null) {
                m_mainCamera.targetTexture = null;
                m_mainCamera.ResetProjectionMatrix();

                if(m_opaqueOnlyGrabPass != null) {
                    m_mainCamera.RemoveCommandBuffer(m_OPAQUE_ONLY_EVENT, m_opaqueOnlyGrabPass);
                    m_mainCamera.RemoveCommandBuffer(m_AFTER_OPAQUE_ONLY_EVENT, m_afterOpaqueOnlyGrabPass);
                }
                if(m_fsrComputePass != null) {
                    m_mainCamera.RemoveCommandBuffer(m_COLOR_EVENT, m_colorGrabPass);
                    m_mainCamera.RemoveCommandBuffer(m_FSR_EVENT, m_fsrComputePass);
                    m_mainCamera.RemoveCommandBuffer(m_BlITFSR_EVENT, m_blitToCamPass);
                }
            }

            m_fsrComputePass = m_colorGrabPass = m_opaqueOnlyGrabPass = m_afterOpaqueOnlyGrabPass = m_blitToCamPass = null;

            if(m_colorBuffer) {
                if(m_opaqueOnlyColorBuffer) {
                    m_opaqueOnlyColorBuffer.Release();
                    m_afterOpaqueOnlyColorBuffer.Release();
                    m_reactiveMaskOutput.Release();
                }
                m_colorBuffer.Release();
                m_fsrOutput.Release();
            }

            if(m_context != null) {
                m_context.Destroy();
                m_context = null;
            }
        }
    }
}
#endif