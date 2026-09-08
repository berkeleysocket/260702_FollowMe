using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YHW.Stats;

namespace YHW.UI
{
    /// <summary>
    /// Never drains on its own - it only reflects whatever StressMeter
    /// reports, with fill width/color animating smoothly toward it.
    /// </summary>
    public class StressBar : MonoBehaviour
    {
        [SerializeField] private StressMeter meter;
        [SerializeField] private Image fillImage;
        [SerializeField] private RectTransform fillMaskRect;
        [SerializeField] private Text valueText;
        [SerializeField] private RectTransform punchTarget;
        [SerializeField] private Color lowColor = new Color(0.35f, 0.85f, 0.55f);
        [SerializeField] private Color highColor = new Color(0.9f, 0.15f, 0.15f);
        [SerializeField] private float punchScale = 0.15f;
        [SerializeField] private float punchDuration = 0.25f;

        private Tween _punchTween;
        private float _fillMaxWidth;

        private void Awake()
        {
            if (punchTarget == null)
                punchTarget = transform as RectTransform;

            if (fillImage != null)
                _fillMaxWidth = fillImage.rectTransform.sizeDelta.x;
        }

        private void OnEnable()
        {
            if (meter == null) return;

            meter.DisplayValueChanged += HandleDisplayValueChanged;
            meter.ValueChanged += HandleValueChanged;
            HandleDisplayValueChanged(meter.DisplayValue, meter.MaxValue);
        }

        private void OnDisable()
        {
            if (meter == null) return;

            meter.DisplayValueChanged -= HandleDisplayValueChanged;
            meter.ValueChanged -= HandleValueChanged;
            _punchTween?.Kill();
        }

        private void HandleDisplayValueChanged(float value, float maxValue)
        {
            float ratio = maxValue > 0f ? value / maxValue : 0f;

            if (fillMaskRect != null)
            {
                Vector2 size = fillMaskRect.sizeDelta;
                size.x = _fillMaxWidth * ratio;
                fillMaskRect.sizeDelta = size;
            }

            if (fillImage != null)
                fillImage.color = Color.Lerp(lowColor, highColor, ratio);

            if (valueText != null)
                valueText.text = Mathf.RoundToInt(ratio * 100f) + "%";
        }

        private void HandleValueChanged(float value, float maxValue)
        {
            if (punchTarget == null) return;

            _punchTween?.Kill();
            punchTarget.localScale = Vector3.one;
            _punchTween = punchTarget.DOPunchScale(Vector3.one * punchScale, punchDuration, 6, 0.6f);
        }
    }
}
