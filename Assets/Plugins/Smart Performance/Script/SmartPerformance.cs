using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace JiRoEnt.Utils.SmartDynamicResolution
{
    public enum SmartPerformanceScene
    {
        Preset,
        Volume
    }

    public enum SmartPerformanceQuality
    {
        Quality,
        Balanced,
        Performance
    }

    public enum PerformanceTarget
    {
        FPS_30 = 30,
        FPS_45 = 45,
        FPS_60 = 60,
        FPS_90 = 90,
        FPS_120 = 120
    }

    public enum Reconstruction
    {
        Bilinear,
        FidelidyFxSuperResolution
    }

    public class SmartPerformance : MonoBehaviour
    {
        #region SINGLETON
        public static SmartPerformance Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
        #endregion

        [Header("STATE")]
        [SerializeField] bool _enableSmartDynamicResolution;

        [Header("SETTINGS")]
        [Tooltip("This is the Camera meant to be affected by SDR Volumes in Volume Mode, set this Camera to the Camera that's doing the rendering in your game.")]
        [SerializeField] Camera _volumeCamera;
        [Tooltip("Switch between Preset mode and Volume mode.")]
        [SerializeField] SmartPerformanceScene _sceneMode;
        [Tooltip("Choose between 2 reconstruction techniques, Bilinear and FSR for reconstructing low resolution frames.")]
        [SerializeField] Reconstruction _reconstructionTechnique = Reconstruction.FidelidyFxSuperResolution;
        [Tooltip("Quality setting of the Smart Dynamic Resolution, this setting affects the range of internal resolution, FPS sensitivity and MSAA value.")]
        [SerializeField] SmartPerformanceQuality _quality;
        [Tooltip("Performance target of Smart Dynamic Resolution.")]
        [SerializeField] PerformanceTarget _performanceTarget = PerformanceTarget.FPS_45;
        [Tooltip("Update rate of Smart Dynamic Resolution adjustments (In seconds).")]
        [SerializeField] float _frameratePoolingTime = 1f;
        [Tooltip("A value added above and below Performance Target to create a stability zone.")]
        [SerializeField][Range(1, 4)] int _tolerance = 4;
        [Tooltip("The render scale that the game defaults to when Smart Dynamic Resolution is disabled.")]
        [SerializeField][Range(0.1f, 1.0f)] float _nativeRenderScale = 1.0f;
        [Tooltip("The size of the Step of rendering resolution adjusments.")]
        [SerializeField][Range(0.025f, 0.1f)] float _renderScaleStepJump = 0.1f;

        [Header("SETUP")]
        [SerializeField] SmartPerformanceModeSO _performanceMode;
        [SerializeField] SmartPerformanceModeSO _balancedMode;
        [SerializeField] SmartPerformanceModeSO _qualityMode;

        #region CACHED_DATA
        UniversalAdditionalCameraData _universalAdditionalCameraData;
        List<SmartPerformanceVolume> _volumes = new List<SmartPerformanceVolume>();
        SmartPerformanceModeSO _modeSO;
        SmartPerformanceModeSO _presetQuality;
        PerformanceTarget _presetPerformanceTarget;
        PerformanceTarget _performanceTargetMode;
        FrametimeTracker _frametimeTracker;
        UniversalRenderPipelineAsset _urpAsset;
        int? fps;
        bool _minimunInternalResolutionReached;
        float _timeAccumulator;
        int _msaa;
        bool _downTick;
        bool _canDecreaseRenderScale;
        bool _isPaused;
        float _maximumScaling;
        #endregion

        private void Start()
        {
            _frametimeTracker = GetComponent<FrametimeTracker>();
            _urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (_volumeCamera != null)
                _universalAdditionalCameraData = _volumeCamera.GetComponent<UniversalAdditionalCameraData>();

            if (_urpAsset == null)
                Debug.LogError("URP not detected, Smart Performance is meant to work on URP only!");

            OnValidate();

            if (_volumeCamera != null)
                UpdateCameraComponents();

            NotifyOfCameraChange();
        }

        private void OnValidate()
        {
            Instance = this;
            _urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            _urpAsset.renderScale = _nativeRenderScale;

            // DynamicResolution_LockFPS();

            switch (_quality)
            {
                case SmartPerformanceQuality.Quality:
                    _presetQuality = _modeSO = _qualityMode;
                    break;

                case SmartPerformanceQuality.Balanced:
                    _presetQuality = _modeSO = _balancedMode;
                    break;

                case SmartPerformanceQuality.Performance:
                    _presetQuality = _modeSO = _performanceMode;
                    break;
            }
            _presetPerformanceTarget = _performanceTargetMode = _performanceTarget;
            ComputeMaximumScale();

            SetupEdges(_modeSO.EdgeQuality);

            if (_volumes.Count != 0)
                _volumes.RemoveAll(go => go == null);

            OptimizeVolumesPerformance();

            if (_reconstructionTechnique == Reconstruction.FidelidyFxSuperResolution)
                EnableFSR();
            else
                _urpAsset.upscalingFilter = UpscalingFilterSelection.Auto;

            if (_enableSmartDynamicResolution)
                EnableSmartDynamicResolution();
            else
                DisableSmartDynamicResolution();
        }

        private void OptimizeVolumesPerformance()
        {
            if (_volumes.Count == 0)
            {
                SmartPerformanceVolume[] spv = GameObject.FindObjectsOfType<SmartPerformanceVolume>(true);
                _volumes.AddRange(spv);
            }

            if (_sceneMode == SmartPerformanceScene.Preset)
            {
                foreach (var item in _volumes)
                {
                    item.gameObject.SetActive(false);
                }

                ApplyPresetDynamicResolutionMode();
            }
            else
            {
                foreach (var item in _volumes)
                {
                    item.gameObject.SetActive(true);
                }
            }
        }

        /* TOOL_INTERNAL */
        public void AddVolume(SmartPerformanceVolume item)
        {
            _volumes.Add(item);
        }

        #region PUBLIC_API
        public void EnableSmartDynamicResolution()
        {
            if (Application.isPlaying)
            {
                ComputeMaximumScale();
                _urpAsset.renderScale = _maximumScaling;
            }
            _msaa = _urpAsset.msaaSampleCount;
            SetupEdges(_modeSO.EdgeQuality);

            if (_reconstructionTechnique == Reconstruction.FidelidyFxSuperResolution)
                EnableFSR();

            _enableSmartDynamicResolution = true;
        }

        public void PauseDynamicResolution(bool state)
        {
            _isPaused = state;
            _frametimeTracker.IsPaused(state);
            _frametimeTracker.ClearAllData();
        }

        public void SetSmartDynamicResolutionMode(SmartPerformanceQuality mode, PerformanceTarget target)
        {
            if (_sceneMode == SmartPerformanceScene.Volume)
            {
                Debug.LogError("This function is not meant to be used when the Volume mode is enabled, it won't take effect!");
                return;
            }

            switch (mode)
            {
                case SmartPerformanceQuality.Quality:
                    _modeSO = _qualityMode;
                    break;

                case SmartPerformanceQuality.Balanced:
                    _modeSO = _balancedMode;
                    break;

                case SmartPerformanceQuality.Performance:
                    _modeSO = _performanceMode;
                    break;
            }
            ComputeMaximumScale();
            SetupEdges(_modeSO.EdgeQuality);

            _performanceTargetMode = target;
        }

        public void SetPerformanceTarget(PerformanceTarget target)
        {
            _performanceTargetMode = target;
        }

        public void SetVolumeCamera(Camera cam)
        {
            _volumeCamera = cam;
            UpdateCameraComponents();
        }

        #region NOTIFY_OF_CAMERA_CHANGE OVERLOADS
        public void NotifyOfCameraChange()
        {
            UniversalAdditionalCameraData[] cameras = FindObjectsOfType<UniversalAdditionalCameraData>(true);
            foreach (var item in cameras)
            {
                if (item.GetComponent<Camera>().targetTexture == null)
                    item.renderPostProcessing = true;
            }
        }

        public void NotifyOfCameraChange(Camera camera)
        {
            UniversalAdditionalCameraData cameraData = camera.GetComponent<UniversalAdditionalCameraData>();
            cameraData.renderPostProcessing = true;
        }
        #endregion

        public void NotifyOfScreenResolutionChange()
        {
            ComputeMaximumScale();
        }

        public void SetReconstructionTechnique(Reconstruction reconstruction)
        {
            bool canEnableFSR = reconstruction == Reconstruction.FidelidyFxSuperResolution && _enableSmartDynamicResolution;
            if (canEnableFSR)
                EnableFSR();
            else
                _urpAsset.upscalingFilter = UpscalingFilterSelection.Auto;
        }

        #region DISABLE_SDR OVERLOADS
        public void DisableSmartDynamicResolution()
        {
            _urpAsset.upscalingFilter = UpscalingFilterSelection.Auto;
            _urpAsset.renderScale = _nativeRenderScale;
            SetupEdges(_msaa);

            _enableSmartDynamicResolution = false;
            SmartPerformance.Instance.SetReconstructionTechnique(Reconstruction.Bilinear);
        }

        public void DisableSmartDynamicResolution(int msaaSampleCount)
        {
            _urpAsset.upscalingFilter = UpscalingFilterSelection.Auto;
            _urpAsset.renderScale = _nativeRenderScale;
            SetupEdges(msaaSampleCount);
        }
        #endregion

        /* External */
        public void ApplyPresetDynamicResolutionMode()
        {
            _modeSO = _presetQuality;
            _performanceTargetMode = _presetPerformanceTarget;
        }

        public int GetFPS()
        {
            return (int) fps;
        }

        public PerformanceTarget GetPerformanceTarget()
        {
            return _performanceTargetMode;
        }

        public SmartPerformanceModeSO GetQualityPreset()
        {
            return _modeSO;
        }

        public float GetRenderScale()
        {
            return _urpAsset.renderScale;
        }

        public Vector2 GetInternalResolution()
        {
            int currentWidth = Screen.width;
            int currentHeight = Screen.height;
            int renderingWidth = (int)(currentWidth * _urpAsset.renderScale);
            int renderingHeight = (int)(currentHeight * _urpAsset.renderScale);

            return new Vector2(renderingWidth, renderingHeight);
        }

        public bool IsDynamicResolutionenabled()
        {
            return _enableSmartDynamicResolution;
        }
        #endregion

        void UpdateCameraComponents()
        {
            if (!_volumeCamera.gameObject.GetComponent<SmartPerformanceCamera>())
                _volumeCamera.gameObject.AddComponent<SmartPerformanceCamera>();

            SmartPerformanceCamera[] _smartDynamicResolutionCameras = Resources.FindObjectsOfTypeAll<SmartPerformanceCamera>();
            if (_smartDynamicResolutionCameras.Length > 1)
            {
                foreach (var item in _smartDynamicResolutionCameras)
                {
                    if (item.gameObject != _volumeCamera.gameObject)
                        Destroy(item);
                }
            }

            if (_universalAdditionalCameraData == null)
                _universalAdditionalCameraData = _volumeCamera.GetComponent<UniversalAdditionalCameraData>();
            
            _universalAdditionalCameraData.renderPostProcessing = true;
        }

        private void Update()
        {
            if (_enableSmartDynamicResolution && !_isPaused)
                ComputeSDR();
        }

        private void ComputeSDR()
        {
            _timeAccumulator += Time.deltaTime;
            if (_timeAccumulator >= _frameratePoolingTime)
            {
                fps = _frametimeTracker.GetAverage(_modeSO.FramerateSensitity);

                if (fps != null)
                {
                    if (_urpAsset.renderScale <= _modeSO.MinimumRenderScale)
                        _minimunInternalResolutionReached = true;
                    else
                        _minimunInternalResolutionReached = false;

                    _canDecreaseRenderScale = (fps <= (int)_performanceTargetMode - _tolerance) && !_minimunInternalResolutionReached;
                    if (_canDecreaseRenderScale)
                    {
                        if (!_downTick)
                        {
                            float estimatedRenderScale = EstimateRenderScaleDrop((int)fps, _performanceTargetMode);

                            if (estimatedRenderScale > _urpAsset.renderScale)
                                DecreaseRenderScale(_renderScaleStepJump);
                            else
                                _urpAsset.renderScale = estimatedRenderScale;

                            _downTick = true;
                        }
                        else
                            DecreaseRenderScale(0.1f);

                        //_up.SetActive(false);
                        //_down.SetActive(true);
                    }
                    else if (fps >= (int)_performanceTargetMode + _tolerance)
                    {
                        IncreaseRenderScale(_renderScaleStepJump);

                        _downTick = false;
                        //_up.SetActive(true);
                        //_down.SetActive(false);
                    }
                    else
                    {
                        _downTick = false;
                        //_up.SetActive(false);
                        //_down.SetActive(false);
                    }
                }
                if (_urpAsset.renderScale < _modeSO.MinimumRenderScale)
                    _urpAsset.renderScale = _modeSO.MinimumRenderScale;

                if (_urpAsset.renderScale > _maximumScaling)
                    _urpAsset.renderScale = _maximumScaling;

                string formatedRenderScale = string.Format("{0:0.00}", _urpAsset.renderScale);

                _timeAccumulator = 0f;
            }
        }

        float EstimateRenderScaleDrop(int framerate, PerformanceTarget performanceTarget)
        {
            float relativeScaling = (float)framerate / (float)performanceTarget;
            float estimatedScaling = relativeScaling - 0.1f;

            return estimatedScaling;
        }

        void DecreaseRenderScale(float amount)
        {
            if (_urpAsset.renderScale >= _modeSO.MinimumRenderScale)
                _urpAsset.renderScale -= amount;
        }

        void IncreaseRenderScale(float amount)
        {
            if (_urpAsset.renderScale <= _maximumScaling)
                _urpAsset.renderScale += amount;
        }

        private void ComputeMaximumScale()
        {
            if (_modeSO.MaximumScalingMode == MaximumScalingMode.Percentage)
                _maximumScaling = _modeSO.MaximumRenderScale;
            else
            {
                if (_modeSO.MaximumFixedResolutionHeight >= Screen.height)
                {
                    Debug.LogError("Maximum Fixed Resolution Height is bigger than the current resolution height! Set a smaller value in Dynamic Resolution Mode or use the Percentage option instead! For now Dynamic Resolution will default to the Percentage option automatically.");
                    _maximumScaling = _modeSO.MaximumRenderScale;
                } else
                    _maximumScaling = (float) _modeSO.MaximumFixedResolutionHeight / (float) Screen.height;
            }
            _urpAsset.renderScale = _maximumScaling;
        }

        void EnableFSR()
        {
            if (SystemInfo.supportsComputeShaders)
                _urpAsset.upscalingFilter = UpscalingFilterSelection.FSR;
            else
                _urpAsset.upscalingFilter = UpscalingFilterSelection.Auto;
        }

        void SetupEdges(EdgeQuality msaa)
        {
            _urpAsset.msaaSampleCount = (int)msaa;
        }

        void SetupEdges(int msaa)
        {
            _urpAsset.msaaSampleCount = msaa;
        }
    }
}