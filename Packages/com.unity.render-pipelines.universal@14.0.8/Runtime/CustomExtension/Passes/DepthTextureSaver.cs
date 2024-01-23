using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;
using Sirenix.OdinInspector; // 引入Odin Inspector命名空间

public class DepthTextureSaver : ScriptableRendererFeature
{
    public bool save = false;
    public RenderPassEvent type = RenderPassEvent.BeforeRenderingTransparents;
    public Material mat;
    class DepthSavePass : ScriptableRenderPass
    {
        private RenderTargetIdentifier depthTexture;
        private RenderTextureDescriptor descriptor;
        private RenderTexture tempTexture;
        private string savePath;
        public bool saveEnabled = false;
        public RenderPassEvent type = RenderPassEvent.BeforeRenderingTransparents;
        private DepthTextureSaver feature;
        public Material mat;
        public DepthSavePass(string savePath,DepthTextureSaver feature,RenderPassEvent type,Material mat)
        {
            this.mat = mat;
            this.type = type;
            this.feature = feature;
            this.savePath = savePath;
            this.renderPassEvent = type;
        }

        public void EnableSave()
        {
            saveEnabled = true;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            this.descriptor = cameraTextureDescriptor;
            this.descriptor.colorFormat = RenderTextureFormat.ARGB32;
            this.descriptor.depthBufferBits = 0;
            tempTexture = RenderTexture.GetTemporary(this.descriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //if (saveEnabled)
            {
                CommandBuffer cmd = CommandBufferPool.Get("SaveDepthTexture");
                depthTexture = renderingData.cameraData.renderer.cameraDepthTarget;
                Blit(cmd, depthTexture, tempTexture,mat);
                
                cmd.SetGlobalTexture("_SceneHeightMap",tempTexture);
                
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

               // SaveRenderTextureToPNG(tempTexture, savePath);

                CommandBufferPool.Release(cmd);
                saveEnabled = false;
                feature.save = false;
            }
        }
        

        void SaveRenderTextureToPNG(RenderTexture renderTexture, string filePath)
        {
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;
            Texture2D tempTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            tempTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tempTexture.Apply();
            byte[] bytes = tempTexture.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            DestroyImmediate(tempTexture);
            RenderTexture.active = previous;
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (tempTexture != null)
            {
                RenderTexture.ReleaseTemporary(tempTexture);
                tempTexture = null;
            }
        }
    }

    private DepthSavePass depthSavePass;

    [Button("Save Depth Texture")]
    public void SaveDepthTexture()
    {
        if (depthSavePass != null)
        {
            depthSavePass.EnableSave();
        }
    }

    public override void Create()
    {
        depthSavePass = new DepthSavePass("Assets/depth.png",this,type,mat);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(depthSavePass);
    }
   
}


