using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    public abstract class AbstractVolumeRenderer
    {
        public abstract bool IsActive();
        public abstract bool Init();
        public abstract bool CheckActive(ref RenderingData renderingData);
        public abstract void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, ref RenderingData renderingData);
        public virtual void Cleanup() { }
    }

    public abstract class VolumeRenderer<T> : AbstractVolumeRenderer where T : VolumeSetting
    {
        public T settings => VolumeManager.instance.stack.GetComponent<T>();
        public override bool IsActive()
        {
            bool active=  settings.IsActive();
            if (!active) return false;
            return m_BlitMaterial != null;
        } 

        private Shader m_Shader = null;
        public  Material m_BlitMaterial=null;
        public override bool Init()
        {
            string shaderName = settings.GetShaderName();
            m_Shader = null ;
            if (!string.IsNullOrEmpty(shaderName))
            {
                m_Shader = Shader.Find(shaderName);
            }
            if (m_Shader != null)
            {
                m_BlitMaterial = CoreUtils.CreateEngineMaterial(m_Shader);
                return true;
            }
            return false;
        }

        public override void Cleanup()
        {
            if (m_BlitMaterial != null)
            {
                CoreUtils.Destroy(m_BlitMaterial);
                m_BlitMaterial = null;
            }
        }

        public override bool CheckActive(ref RenderingData renderingData)
        {
            return IsActive();
        }

        // public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        // {

        //     if (settings.IsActive())
        //         return;

        //     Render(cmd, source, target);
        // }

        // public abstract void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target);

    }

}