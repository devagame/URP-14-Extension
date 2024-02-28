using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JiRoEnt.Utils.SmartDynamicResolution
{
    public class SmartPerformanceVolume : MonoBehaviour
    {
        [SerializeField] SmartPerformanceQuality _mode;
        [SerializeField] PerformanceTarget _target;

        private void Start()
        {
            // Check if RB and Collider are corretly set up
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<SmartPerformanceCamera>())
            {
                SmartPerformance.Instance.SetSmartDynamicResolutionMode(_mode, _target);
                Debug.Log("Switching to Volume");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<SmartPerformanceCamera>())
            {
                SmartPerformance.Instance.ApplyPresetDynamicResolutionMode();
                Debug.Log("Defaulting to Preset");
            }
        }
    }
}