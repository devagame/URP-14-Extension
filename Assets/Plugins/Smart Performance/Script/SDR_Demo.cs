using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace JiRoEnt.Utils.SmartDynamicResolution
{
    public class SDR_Demo : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown _sdrDropdown;
        [SerializeField] GameObject _instruction;
        [SerializeField] GameObject _setting;

        bool _isSettingsPanelOpen;

        private void Start()
        {
            _sdrDropdown.onValueChanged.AddListener(OnSDRValueChanged);
            _instruction.SetActive(true);
            _setting.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isSettingsPanelOpen)
                {
                    _instruction.SetActive(true);
                    _setting.SetActive(false);
                    _isSettingsPanelOpen = false;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    SmartPerformance.Instance.PauseDynamicResolution(false);
                    Time.timeScale = 1f;
                }
                else
                {
                    _instruction.SetActive(false);
                    _setting.SetActive(true);
                    _isSettingsPanelOpen = true;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    SmartPerformance.Instance.PauseDynamicResolution(true);
                    Time.timeScale = 0f;
                }
            }
        }

        void OnSDRValueChanged(int value)
        {
            switch (value)
            {
                case 0:
                    SmartPerformance.Instance.DisableSmartDynamicResolution();
                    break;

                case 1:
                    SmartPerformance.Instance.SetSmartDynamicResolutionMode(SmartPerformanceQuality.Balanced, PerformanceTarget.FPS_30);
                    SmartPerformance.Instance.EnableSmartDynamicResolution();
                    break;

                case 2:
                    SmartPerformance.Instance.SetSmartDynamicResolutionMode(SmartPerformanceQuality.Balanced, PerformanceTarget.FPS_45);
                    SmartPerformance.Instance.EnableSmartDynamicResolution();
                    break;

                case 3:
                    SmartPerformance.Instance.SetSmartDynamicResolutionMode(SmartPerformanceQuality.Performance, PerformanceTarget.FPS_60);
                    SmartPerformance.Instance.EnableSmartDynamicResolution();
                    break;
            }
        }
    }
}