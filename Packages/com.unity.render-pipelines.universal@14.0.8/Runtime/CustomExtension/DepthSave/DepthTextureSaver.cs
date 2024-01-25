using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;
using Sirenix.OdinInspector; // 引入Odin Inspector命名空间

public enum Mode
{
    CopyDepth,
    RenderObj,
}
public class DepthTextureSaver : ScriptableRendererFeature
{
    public Mode renderType = Mode.RenderObj;
    public RenderTexture renderObjTex;
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
        
        public Mode renderType = Mode.RenderObj;
        private List<ShaderTagId> m_ShaderTagIdList;
        private FilteringSettings filterSettings;
        
        private DepthTextureSaver feature;
        private RenderTexture renderObjTex;
        public Material mat;
        public DepthSavePass(string savePath,
            DepthTextureSaver feature,
            RenderPassEvent type,
            Material mat,
            RenderTexture renderObjTex)
        {
            this.mat = mat;
            this.type = type;
            this.feature = feature;
            this.savePath = savePath;
            this.renderPassEvent = type;
            this.renderObjTex = renderObjTex;
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
            filterSettings = new FilteringSettings(RenderQueueRange.opaque);
            
            ConfigureClear(ClearFlag.All, Color.black);
            ConfigureTarget(renderObjTex);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("SaveDepthTexture");
            if (renderType == Mode.CopyDepth)
            {
                
                depthTexture = renderingData.cameraData.renderer.cameraDepthTarget;
                Blit(cmd, depthTexture, tempTexture, mat);

                cmd.SetGlobalTexture("_SceneHeightMap", tempTexture);
                if (saveEnabled)
                {
                    SaveRenderTextureToPNG(tempTexture, savePath);
                    saveEnabled = false;
                }
            }
            else
            {


                m_ShaderTagIdList = new List<ShaderTagId>()
                {
                    new ShaderTagId("DepthRender")
                };
                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);
                
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings,  ref filterSettings);
                cmd.SetGlobalTexture("_SceneHeightMap", tempTexture);
                if (saveEnabled)
                {
                    SaveRenderTextureToPNG(renderObjTex, savePath);
                    saveEnabled = false;
                }
            }

            
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
        

        void SaveRenderTextureToPNG(RenderTexture renderTexture, string filePath)
        {
            if (renderType == Mode.CopyDepth)
            {
                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = renderTexture;
                Texture2D temp = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RFloat, false);
                temp.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                temp.Apply();
                byte[] bytes = temp.EncodeToPNG();
                File.WriteAllBytes(filePath, bytes);
                DestroyImmediate(temp);
                RenderTexture.active = previous;
            }
            else
            {
                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = renderTexture;
                
                Texture2D temp = new Texture2D(renderObjTex.width, renderObjTex.height, TextureFormat.RFloat, false);
                temp.ReadPixels(new Rect(0, 0, renderObjTex.width, renderObjTex.height), 0, 0);
                temp.Apply();
                byte[] bytes = temp.EncodeToPNG();
                File.WriteAllBytes(filePath, bytes);
                
                RenderTexture.active = previous;
            }
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
        depthSavePass = new DepthSavePass("Assets/depth.png",this,type,mat,renderObjTex);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(depthSavePass);
    }
   
}


