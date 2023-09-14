using System;
using System.Diagnostics;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Profiling;

namespace UnityEngine.Rendering.Universal
{
    public abstract partial class ScriptableRenderer : IDisposable
    {
        public virtual void FinishRendering(CommandBuffer cmd,RenderingData renderingData)
        {
          
        }
    }
}