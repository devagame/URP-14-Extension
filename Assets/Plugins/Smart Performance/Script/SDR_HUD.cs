using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace JiRoEnt.Utils.SmartDynamicResolution
{
    public class SDR_HUD : MonoBehaviour
    {
        [SerializeField] bool _enableHUD;

        [Header("UI")]
        [SerializeField] TMP_Text _fps;
        [SerializeField] TMP_Text _screenResolution;
        [SerializeField] TMP_Text _internalResolution;
        [SerializeField] TMP_Text _renderingScale;
        [SerializeField] TMP_Text _mode;
        [SerializeField] TMP_Text _performanceTarget;
        [SerializeField] GameObject _HUD;

        FrametimeTracker _frametimeTracker;
        SmartPerformance _sp;

        private void Start()
        {
            _frametimeTracker = GetComponent<FrametimeTracker>();
            _sp = GetComponent<SmartPerformance>();
        }

        private void OnValidate()
        {
            if (_enableHUD)
                _HUD.SetActive(true);
            else
                _HUD.SetActive(false);
        }

        private void Update()
        {
            if (_enableHUD)
            {
                _fps.SetText(_frametimeTracker.GetAverage(FramerateTrackerSensitity.Low).ToString());
                _screenResolution.SetText(Screen.width + "x" + Screen.height);
                Vector2 i = _sp.GetInternalResolution();
                string internalRes = i.x.ToString() + "x" + i.y.ToString();
                _internalResolution.SetText(_sp.IsDynamicResolutionenabled() ? internalRes : "--");
                string renderScale = String.Format("{0:0.00}", _sp.GetRenderScale());
                _renderingScale.SetText(renderScale);
                _mode.SetText(_sp.GetQualityPreset().Name);
                _performanceTarget.SetText(_sp.GetPerformanceTarget().ToString());
            }
        }
    }
}