using System;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using FidelityFX;
using UnityEngine.Profiling;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AEG.FSR
{
    public class FSR3_POST_PROCESS_PASS : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [HideInInspector]
        public BoolParameter enable = new BoolParameter(false);
        public bool IsActive() => enable.value;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.BeforeTAA;

        private readonly int depthTexturePropertyID = Shader.PropertyToID("_CameraDepthTexture");
        private readonly int motionTexturePropertyID = Shader.PropertyToID("_CameraMotionVectorsTexture");
        Texture depthTexture;

        private FSR3_HDRP m_hdrp;
        private FSR_Quality currentQuality;

        public override void Setup() {
        }


        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination) {
#if UNITY_EDITOR
            if(Application.isPlaying) {
#endif
                if(!IsActive()) {
                    return;
                }

                if(m_hdrp == null) {
                    m_hdrp = camera.camera.GetComponent<FSR3_HDRP>();
                    if(m_hdrp == null) {
                        return;
                    }
                }

                if(camera.camera.cameraType != CameraType.Game) {
                    cmd.Blit(source, destination, 0, 0);
                    return;
                }

                if(currentQuality != m_hdrp.FSRQuality) {
                    currentQuality = m_hdrp.FSRQuality;
                    return;
                }

                depthTexture = Shader.GetGlobalTexture(depthTexturePropertyID);
                m_hdrp.m_dispatchDescription.Color = source.rt;
                m_hdrp.m_dispatchDescription.Depth = depthTexture;
                m_hdrp.m_dispatchDescription.MotionVectors = Shader.GetGlobalTexture(motionTexturePropertyID);
                m_hdrp.m_dispatchDescription.Output = destination.rt;

                //This is needed because HDRP is potentially missing depth and motion vectors the first rendering of the camera.
                if(m_hdrp.m_skipFirstFrame) {
                    m_hdrp.m_dispatchDescription.DepthFormat = false;
                    m_hdrp.m_skipFirstFrame = false;
                    m_hdrp.m_dispatchDescription.Depth = source.rt;
                    m_hdrp.m_dispatchDescription.MotionVectors = source.rt;
                } else if(depthTexture.graphicsFormat != UnityEngine.Experimental.Rendering.GraphicsFormat.None) {
                    m_hdrp.m_dispatchDescription.DepthFormat = false;
                } else {
                    m_hdrp.m_dispatchDescription.DepthFormat = true;
                }

                if(m_hdrp.m_dispatchDescription.Color != null && m_hdrp.m_dispatchDescription.Depth != null && m_hdrp.m_dispatchDescription.MotionVectors != null && m_hdrp.m_dispatchDescription.MotionVectorScale.x != 0) {
                    if(m_hdrp.generateReactiveMask) {
                        m_hdrp.m_genReactiveDescription.OutReactive = m_hdrp.m_reactiveMaskOutput;
                        m_hdrp.m_dispatchDescription.Reactive = m_hdrp.m_reactiveMaskOutput;
                        m_hdrp.m_context.GenerateReactiveMask(m_hdrp.m_genReactiveDescription, cmd);
                    }
                    m_hdrp.m_context.Dispatch(m_hdrp.m_dispatchDescription, cmd);
                }
#if UNITY_EDITOR
            }
#endif
        }

        public override void Cleanup() {
        }
    }
}
