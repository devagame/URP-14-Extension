using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JiRoEnt.Utils.SmartDynamicResolution
{
    public enum MaximumScalingMode
    {
        Percentage,
        Fixed
    }

    public enum EdgeQuality
    {
        Low = 1,
        Medium = 2,
        High = 4
    }

    public enum FramerateTrackerSensitity
    {
        Low,
        Medium,
        High
    }

    [CreateAssetMenu(fileName = "New Mode", menuName = "Smart Performance/Mode")]
    public class SmartPerformanceModeSO : ScriptableObject
    {
        public string Name;
        [Range(0.33f, 0.95f)] public float MinimumRenderScale = 0.4f;
        public MaximumScalingMode MaximumScalingMode;
        [Range(0.33f, 0.95f)] public float MaximumRenderScale = 0.95f;
        public int MaximumFixedResolutionHeight = 720;
        public FramerateTrackerSensitity FramerateSensitity;
        public EdgeQuality EdgeQuality;

        public void OnValidate()
        {
            if (MinimumRenderScale >= MaximumRenderScale)
            {
                Debug.LogError("Minimum render scale is higher than maximum render scale, please adjust the values or revert to the default ones!");
            }
        }
    }
}