using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using UnityEngine;
using FidelityFX;

[assembly: InternalsVisibleTo("com.alteregogames.aeg-fsr.Runtime.BIRP")]
[assembly: InternalsVisibleTo("com.alteregogames.aeg-fsr.Runtime.URP")]
[assembly: InternalsVisibleTo("com.alteregogames.aeg-fsr.Runtime.HDRP")]

namespace AEG.FSR
{
    /// <summary>
    /// Base script for FSR
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public abstract class FSR3_BASE : MonoBehaviour
    {
        //Public Variables
        public FSR_Quality FSRQuality = FSR_Quality.Balanced;
        [Range(0, 1)]
        public float antiGhosting = 0.0f;
        public static float FSRScaleFactor;

        public bool sharpening = true;
        public float sharpness = 0.5f;

        public bool enableF16;
        public bool enableAutoExposure = true;
        public bool generateReactiveMask = true;
        public bool generateTCMask = false;

        public float autoReactiveScale = 0.9f;
        public float autoReactiveThreshold = 0.05f;
        public float autoReactiveBinaryValue = 0.5f;

        public float autoTcThreshold = 0.05f;
        public float autoTcScale = 1f;
        public float autoTcReactiveScale = 5f;
        public float autoTcReactiveMax = 0.9f;

        public Fsr3.GenerateReactiveFlags reactiveFlags = Fsr3.GenerateReactiveFlags.ApplyTonemap | Fsr3.GenerateReactiveFlags.ApplyThreshold | Fsr3.GenerateReactiveFlags.UseComponentsMax;

        public float mipmapBiasOverride = 1.0f;
        public bool autoTextureUpdate = true;
        public float mipMapUpdateFrequency = 2f;

        //Protected Variables
        protected bool m_fsrInitialized = false;
        protected Camera m_mainCamera;

        protected float m_scaleFactor = 1.5f;
        protected int m_renderWidth, m_renderHeight;
        protected int m_displayWidth, m_displayHeight;

        protected float m_nearClipPlane, m_farClipPlane, m_fieldOfView;

        protected FSR_Quality m_previousFsrQuality;
        protected bool m_previousHDR;

        protected bool m_previousReactiveMask;
        protected bool m_previousTCMask;
        protected float m_previousScaleFactor;
        protected RenderingPath m_previousRenderingPath;

        //Mipmap variables
        protected Texture[] m_allTextures;
        protected ulong m_previousLength;
        protected float m_mipMapBias;
        protected float m_prevMipMapBias;
        protected float m_mipMapTimer = float.MaxValue;

        public bool m_resetCamera = false;

        #region Public API
        /// <summary>
        /// Set FSR Quality settings.
        /// Quality = 1.5, Balanced = 1.7, Performance = 2, Ultra Performance = 3
        /// </summary>
        public void OnSetQuality(FSR_Quality value) {
            m_previousFsrQuality = value;
            FSRQuality = value;

            if(value == FSR_Quality.Off) {
                Initialize();
                DisableFSR();
                m_scaleFactor = 1;
            } else {
                switch(value) {
                    case FSR_Quality.TemporalAntiAliasingOnly:
                        m_scaleFactor = 1.0f;
                        break;
                    case FSR_Quality.Quality:
                        m_scaleFactor = 1.5f;
                        break;
                    case FSR_Quality.Balanced:
                        m_scaleFactor = 1.7f;
                        break;
                    case FSR_Quality.Performance:
                        m_scaleFactor = 2.0f;
                        break;
                    case FSR_Quality.UltraPerformance:
                        m_scaleFactor = 3.0f;
                        break;
                }

                Initialize();
            }
            FSRScaleFactor = m_scaleFactor;
        }

        public void OnSetAdaptiveQuality(float _value) {
            m_scaleFactor = _value;
        }

        /// <summary>
        /// Checks wether FSR is compatible using the current build settings 
        /// </summary>
        /// <returns></returns>
        public bool OnIsSupported() {
            bool fsr2Compatible = SystemInfo.supportsComputeShaders;
            enableF16 = SystemInfo.IsFormatSupported(UnityEngine.Experimental.Rendering.GraphicsFormat.R16_SFloat, UnityEngine.Experimental.Rendering.FormatUsage.Render);

            return fsr2Compatible;
        }

        /// <summary>
        /// Resets the camera for the next frame, clearing all the buffers saved from previous frames in order to prevent artifacts.
        /// Should be called in or before PreRender oh the frame where the camera makes a jumpcut.
        /// Is automatically disabled the frame after.
        /// </summary>
        public void OnResetCamera() {
            m_resetCamera = true;
        }

        /// <summary>
        /// Updates a single texture to the set MipMap Bias.
        /// Should be called when an object is instantiated, or when the ScaleFactor is changed.
        /// </summary>
        public void OnMipmapSingleTexture(Texture texture) {
            texture.mipMapBias = m_mipMapBias;
        }

        /// <summary>
        /// Updates all textures currently loaded to the set MipMap Bias.
        /// Should be called when a lot of new textures are loaded, or when the ScaleFactor is changed.
        /// </summary>
        public void OnMipMapAllTextures() {
            m_allTextures = Resources.FindObjectsOfTypeAll(typeof(Texture)) as Texture[];
            for(int i = 0; i < m_allTextures.Length; i++) {
                m_allTextures[i].mipMapBias = m_mipMapBias;
            }
        }
        /// <summary>
        /// Resets all currently loaded textures to the default mipmap bias. 
        /// </summary>
        public void OnResetAllMipMaps() {
            m_prevMipMapBias = -1;

            m_allTextures = Resources.FindObjectsOfTypeAll(typeof(Texture)) as Texture[];
            for(int i = 0; i < m_allTextures.Length; i++) {
                m_allTextures[i].mipMapBias = 0;
            }
            m_allTextures = null;
        }
        #endregion

        protected virtual void Initialize() {
            bool fsr2Compatible = OnIsSupported();

            //Reset mipmap timer so mipmap are instantly updated if automatic mip map is turned on
            m_mipMapTimer = float.MaxValue;

            if(m_fsrInitialized || !Application.isPlaying) {
                return;
            }
            if(fsr2Compatible) {
                InitializeFSR();
                m_fsrInitialized = true;
            } else {
                Debug.LogWarning($"FSR 2 is not supported");
                enabled = false;
            }

        }

        /// <summary>
        /// Initializes everything in order to run FSR
        /// </summary>
        protected virtual void InitializeFSR() {
            m_mainCamera = GetComponent<Camera>();
        }

        protected virtual void OnEnable() {
#if AMD_FIDELITY_FSR3_DEBUG
            RegisterDebugCallback(OnDebugCallback);
#endif

            OnSetQuality(FSRQuality);
        }

        protected virtual void Update() {
            if(m_previousFsrQuality != FSRQuality) {
                OnSetQuality(FSRQuality);
            }

            if(!m_fsrInitialized) {
                return;
            }
#if UNITY_BIRP
            if(autoTextureUpdate) {
                UpdateMipMaps();
            }
#endif
        }

        protected virtual void OnDisable() {
            DisableFSR();
        }

        protected virtual void OnDestroy() {
            DisableFSR();
        }

        /// <summary>
        /// Disables FSR and cleans up
        /// </summary>
        protected virtual void DisableFSR() {
            m_fsrInitialized = false;
        }


        #region Automatic Mip Map
#if UNITY_BIRP
        /// <summary>
        /// Automatically updates the mipmap of all loaded textures
        /// </summary>
        protected void UpdateMipMaps() {
            m_mipMapTimer += Time.deltaTime;

            if(m_mipMapTimer > mipMapUpdateFrequency) {
                m_mipMapTimer = 0;

                m_mipMapBias = (Mathf.Log((float)(m_renderWidth) / (float)(m_displayWidth), 2f) - 1) * mipmapBiasOverride;

                if(m_previousLength != Texture.currentTextureMemory || m_prevMipMapBias != m_mipMapBias) {
                    m_prevMipMapBias = m_mipMapBias;
                    m_previousLength = Texture.currentTextureMemory;

                    OnMipMapAllTextures();
                }
            }
        }
#endif
        #endregion


        #region Debug

#if AMD_FIDELITY_FSR3_DEBUG
        /// <summary>
        /// Register a callback to send debugging information
        /// </summary>
        /// <param name="cb"></param>
        [DllImport(m_DLL, CallingConvention = CallingConvention.Cdecl)]
        static extern void RegisterDebugCallback(debugCallback cb);

        /// <summary>
        /// Delegate for a debug callback
        /// </summary>
        delegate void debugCallback(IntPtr request, int messageType, int color, int size);
        enum Color { red, green, blue, black, white, yellow, orange };
        enum MessageType { Error, Warning, Info };

        /// <summary>
        /// Callback for debug messages send from the plugin
        /// </summary>
        /// <param name="request">Debug message</param>
        /// <param name="messageType">Message type</param>
        /// <param name="color">Color to use in the console</param>
        /// <param name="size">Size of the string</param>
        static void OnDebugCallback(IntPtr request, int messageType, int color, int size)
        {
            //Ptr to string
            string debug_string = Marshal.PtrToStringAnsi(request, size);

            //Add Specified Color
            debug_string =
                String.Format("{0}{1}{2}",
                $"<color={(Color)color}>",
                debug_string,
                "</color>"
                );

            switch ((MessageType)messageType)
            {
                case MessageType.Error:
                    Debug.LogError(debug_string);
                    break;
                case MessageType.Warning:
                    Debug.LogWarning(debug_string);
                    break;
                default:
                    Debug.Log(debug_string);
                    break;
            }
        }
#endif
        #endregion
    }
}
