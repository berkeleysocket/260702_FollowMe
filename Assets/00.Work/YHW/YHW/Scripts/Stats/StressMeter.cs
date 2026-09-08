using System;
using DG.Tweening;
using UnityEngine;

namespace YHW.Stats
{
    /// <summary>
    /// Stress rises on its own at autoRisePercentPerSecond, and otherwise only
    /// changes when AddStress/ReduceStress/SetStress is called by gameplay code.
    /// Raw changes fire threshold events immediately; the displayed value
    /// animates toward the new raw value for smooth UI/VFX.
    /// </summary>
    public class StressMeter : MonoBehaviour
    {
        [SerializeField] private float maxValue = 100f;
        [SerializeField] private float animateDuration = 0.4f;
        [SerializeField] private Ease animateEase = Ease.OutQuad;
        [SerializeField] private float autoRisePercentPerSecond = 1f;

        public StressEventChannel Thresholds { get; } = new StressEventChannel();

        /// <summary>Raw value changed (fires immediately, before the display animates).</summary>
        public event Action<float, float> ValueChanged;

        /// <summary>Animated display value changed (drives UI/VFX smoothly).</summary>
        public event Action<float, float> DisplayValueChanged;

        private float _currentValue;
        private float _displayValue;
        private Tween _displayTween;
        private int _lastPercent;

        public float CurrentValue => _currentValue;
        public float DisplayValue => _displayValue;
        public float MaxValue => maxValue;
        public int CurrentPercent => PercentOf(_currentValue);

        private void Awake()
        {
            _currentValue = 0f;
            _displayValue = 0f;
            _lastPercent = PercentOf(_currentValue);
        }

        private void Update()
        {
            if (autoRisePercentPerSecond <= 0f) return;
            AddStress(maxValue * autoRisePercentPerSecond * 0.01f * Time.deltaTime);
        }

        public void AddStress(float amount) => SetStress(_currentValue + amount);

        public void ReduceStress(float amount) => SetStress(_currentValue - amount);

        public void SetStress(float value)
        {
            float clamped = Mathf.Clamp(value, 0f, maxValue);
            if (Mathf.Approximately(clamped, _currentValue)) return;

            _currentValue = clamped;
            ValueChanged?.Invoke(_currentValue, maxValue);

            int newPercent = PercentOf(_currentValue);
            if (newPercent != _lastPercent)
            {
                Thresholds.RaiseCrossed(_lastPercent, newPercent);
                _lastPercent = newPercent;
            }

            _displayTween?.Kill();
            _displayTween = DOTween.To(
                    () => _displayValue,
                    x =>
                    {
                        _displayValue = x;
                        DisplayValueChanged?.Invoke(_displayValue, maxValue);
                    },
                    _currentValue,
                    animateDuration)
                .SetEase(animateEase);
        }

        private int PercentOf(float value)
        {
            return Mathf.RoundToInt(Mathf.Clamp01(value / maxValue) * 100f);
        }
    }
}
