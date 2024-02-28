#if UNITY_URP
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;

namespace AEG.FSR
{
    //Not allowed to be in a namespace
    public class FSRScriptableRenderFeature : ScriptableRendererFeature
    {
        [HideInInspector]
        public bool IsEnabled = false;

        private FSR3_URP m_fsrURP;

        private FSRBufferPass fsrBufferPass;
        private FSRRenderPass fsrRenderPass;
        private FSROpaqueOnlyPass fsrReactiveMaskPass;
        private FSRTransparentPass fsrReactiveMaskTransparentPass;

        private CameraData cameraData;

        public void OnSetReference(FSR3_URP _fsrURP) {
            m_fsrURP = _fsrURP;
            fsrBufferPass.OnSetReference(m_fsrURP);
            fsrRenderPass.OnSetReference(m_fsrURP);
            fsrReactiveMaskPass.OnSetReference(m_fsrURP);
            fsrReactiveMaskTransparentPass.OnSetReference(m_fsrURP);
        }

        public override void Create() {
            name = "FSRScriptableRenderFeature";

            // Pass the settings as a parameter to the constructor of the pass.
            fsrBufferPass = new FSRBufferPass(m_fsrURP);
            fsrRenderPass = new FSRRenderPass(m_fsrURP);
            fsrReactiveMaskPass = new FSROpaqueOnlyPass(m_fsrURP);
            fsrReactiveMaskTransparentPass = new FSRTransparentPass(m_fsrURP);

            fsrBufferPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
        }

        public void OnDispose() {
        }

#if UNITY_2022_1_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) {
            fsrBufferPass.Setup(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }
#endif

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            if(!IsEnabled) {
                return;
            }
            if(!Application.isPlaying) {
                return;
            }

            if(m_fsrURP == null) {
                return;
            }
            if(m_fsrURP.m_context == null) {
                return;
            }

            cameraData = renderingData.cameraData;
            if(cameraData.cameraType != CameraType.Game) {
                return;
            }
            if(cameraData.camera.GetComponent<FSR3_URP>() == null) {
                return;
            }
            if(!cameraData.resolveFinalTarget) {
                return;
            }

            m_fsrURP.m_autoHDR = cameraData.isHdrEnabled;

            // Here you can queue up multiple passes after each other.
            renderer.EnqueuePass(fsrBufferPass);
            renderer.EnqueuePass(fsrRenderPass);
            if(m_fsrURP.generateReactiveMask) {
                renderer.EnqueuePass(fsrReactiveMaskPass);
                renderer.EnqueuePass(fsrReactiveMaskTransparentPass);
            }
        }
    }
}
#endif