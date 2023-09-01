#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using ShaderKeywordFilter = UnityEditor.ShaderKeywordFilter;
#endif
using System;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.Universal
{
    public partial class UniversalRendererData : ScriptableRendererData, ISerializationCallbackReceiver
    {
        [SerializeField]   bool m_sUISplitEnable = false;
        [SerializeField]   bool m_sIsGammaCorrectEnable = true;
        [SerializeField]   bool m_sEnableUICameraUseSwapBuffer = false;
        
        public bool UISplitEnable
        {
            get => m_sUISplitEnable;
            set
            {
                SetDirty();
                m_sUISplitEnable = value;
            }
        }
        
        public bool IsGammaCorrectEnable
        {
            get => m_sIsGammaCorrectEnable;
            set
            {
                SetDirty();
                m_sIsGammaCorrectEnable = value;
            }
        }
        
        public bool EnableUICameraUseSwapBuffer
        {
            get => m_sEnableUICameraUseSwapBuffer;
            set
            {
                SetDirty();
                m_sEnableUICameraUseSwapBuffer = value;
            }
        }
    }
}