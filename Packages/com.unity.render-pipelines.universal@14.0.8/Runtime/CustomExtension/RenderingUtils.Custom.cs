using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
    public static partial class RenderingUtils
    {
        internal static void FinalBlitColorTexture(
            CommandBuffer cmd,
            ref CameraData cameraData,
            RTHandle source,
            RTHandle destination,
            RenderBufferLoadAction loadAction,
            RenderBufferStoreAction storeAction,
            Material material, int passIndex)
        {
            bool isRenderToBackBufferTarget = !cameraData.isSceneViewCamera;
#if ENABLE_VR && ENABLE_XR_MODULE
                if (cameraData.xr.enabled)
                    isRenderToBackBufferTarget = new RenderTargetIdentifier(destination.nameID, 0, CubemapFace.Unknown, -1) == new RenderTargetIdentifier(cameraData.xr.renderTarget, 0, CubemapFace.Unknown, -1);
#endif

            Vector2 viewportScale = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;

            // We y-flip if
            // 1) we are blitting from render texture to back buffer(UV starts at bottom) and
            // 2) renderTexture starts UV at top
           // bool yflip = isRenderToBackBufferTarget && cameraData.targetTexture == null && SystemInfo.graphicsUVStartsAtTop;
           // Vector4 scaleBias = yflip ? new Vector4(viewportScale.x, -viewportScale.y, 0, viewportScale.y) : new Vector4(viewportScale.x, viewportScale.y, 0, 0);
           Vector4 scaleBias = new Vector4(viewportScale.x, viewportScale.y, 0, 0);
           CoreUtils.SetRenderTarget(cmd, destination, loadAction, storeAction, ClearFlag.None, Color.clear);
            if (isRenderToBackBufferTarget)
                cmd.SetViewport(cameraData.pixelRect);

            // cmd.Blit must be used in Scene View for wireframe mode to make the full screen draw with fill mode
            // This branch of the if statement must be removed for render graph and the new command list with a novel way of using Blitter with fill mode
            if (GL.wireframe && cameraData.isSceneViewCamera)
            {
                // This set render target is necessary so we change the LOAD state to DontCare.
                cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget,
                    loadAction, storeAction, // color
                    RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare); // depth
                cmd.Blit(source.nameID, destination.nameID);
            }
            else if (source.rt == null)
                Blitter.BlitTexture(cmd, source.nameID, scaleBias, material, passIndex);  // Obsolete usage of RTHandle aliasing a RenderTargetIdentifier
            else
                Blitter.BlitTexture(cmd, source, scaleBias, material, passIndex);
        }
    }
}