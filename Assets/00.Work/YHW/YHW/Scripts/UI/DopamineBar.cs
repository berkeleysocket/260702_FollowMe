using UnityEngine;
using UnityEngine.UI;
using YHW.Items;

namespace YHW.UI
{
    public class DopamineBar : MonoBehaviour
    {
        [SerializeField] private PlayerItemCollector collector;
        [SerializeField] private Image fillImage;
        [SerializeField] private Text valueText;
        [SerializeField] private float maxValue = 100f;
        [SerializeField] private float drainPerSecond = 5f;
        [SerializeField] private float chargePerItemValue = 10f;
        [SerializeField] private Color highColor = new Color(1f, 0.24f, 0.65f);
        [SerializeField] private Color lowColor = new Color(0.85f, 0.15f, 0.15f);

        private float currentValue;

        private void Awake()
        {
            currentValue = maxValue;
        }

        private void OnEnable()
        {
            if (collector != null)
                collector.ItemCollected += HandleItemCollected;
        }

        private void OnDisable()
        {
            if (collector != null)
                collector.ItemCollected -= HandleItemCollected;
        }

        private void Update()
        {
            currentValue = Mathf.Max(0f, currentValue - drainPerSecond * Time.deltaTime);
            UpdateFill();
        }

        private void HandleItemCollected(ItemData item)
        {
            if (item == null) return;

            currentValue = Mathf.Min(maxValue, currentValue + item.Value * chargePerItemValue);
            UpdateFill();
        }

        private void UpdateFill()
        {
            float ratio = currentValue / maxValue;

            if (fillImage != null)
            {
                fillImage.fillAmount = ratio;
                fillImage.color = Color.Lerp(lowColor, highColor, ratio);
            }

            if (valueText != null)
                valueText.text = Mathf.RoundToInt(ratio * 100f) + "%";
        }
    }
}
