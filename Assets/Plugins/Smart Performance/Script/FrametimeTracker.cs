using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

namespace JiRoEnt.Utils.SmartDynamicResolution
{
    public class FrametimeTracker : MonoBehaviour
    {
        int _limit = 90;
        bool _canRecord;
        float _highQualitySmoothedFrameRate, _lowQualitySmoothedFrameRate;
        List<int> _frameRateData = new List<int>(90);
        int? avg;

        public void IsPaused(bool state)
        {
            _canRecord = !state;
        }

        private void Start()
        {
            _canRecord = true;
        }

        private void Update()
        {
            if (_frameRateData.Count >= _limit)
                _frameRateData.RemoveAt(0);

            if (_canRecord)
            {
                float frameTime = Time.deltaTime;
                int frameRate = Mathf.RoundToInt(FormatFrametime(frameTime));
                if (frameRate > 0)
                    _frameRateData.Add(frameRate);
                else
                    _frameRateData.Clear();
            }
            else
            {
                _frameRateData.Clear();
            }

            _highQualitySmoothedFrameRate = ExponentialSmooth(_frameRateData.LastOrDefault(), _highQualitySmoothedFrameRate, 0.1f);
            _lowQualitySmoothedFrameRate = ExponentialSmooth(_frameRateData.LastOrDefault(), _lowQualitySmoothedFrameRate, 0.9f);
        }

        public int? GetAverage(FramerateTrackerSensitity sensitivity)
        {
            switch (sensitivity)
            {
                case FramerateTrackerSensitity.Low:
                    avg = (int?)_highQualitySmoothedFrameRate;
                    break;

                case FramerateTrackerSensitity.Medium:
                    avg = _frameRateData.Any() ? Mathf.RoundToInt((float)_frameRateData.Average()) : null;
                    _frameRateData.Clear();
                    break;

                case FramerateTrackerSensitity.High:
                    avg = (int?)_lowQualitySmoothedFrameRate;
                    break;
            }

            if (avg >= 0)
                return avg;
            else
                return null;
        }

        public void ClearAllData()
        {
            _frameRateData.Clear();
        }

        float ExponentialSmooth(int currentValue, float smoothedValue, float alpha)
        {
            // If _frameRateData is empty, return the current smoothed value
            if (_frameRateData.Count == 0)
            {
                return smoothedValue;
            }

            // If currentValue is not provided, use the last value in _frameRateData
            if (currentValue == 0)
            {
                currentValue = _frameRateData.LastOrDefault();
            }

            // Apply exponential smoothing equation
            return alpha * currentValue + (1 - alpha) * smoothedValue;
        }

        float FormatFrametime(float frameTime)
        {
            return 1f / frameTime;
        }
    }
}