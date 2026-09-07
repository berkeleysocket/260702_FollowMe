using UnityEngine;
using YHW.Items;

namespace YHW.Stats
{
    /// <summary>
    /// Bridges item pickups to the stress meter: every collected item
    /// reduces stress by itemValue * reducePerItemValue.
    /// </summary>
    public class StressItemDrain : MonoBehaviour
    {
        [SerializeField] private PlayerItemCollector collector;
        [SerializeField] private StressMeter meter;
        [SerializeField] private float reducePerItemValue = 8f;

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

        private void HandleItemCollected(ItemData item)
        {
            if (item == null || meter == null) return;

            meter.ReduceStress(item.Value * reducePerItemValue);
        }
    }
}
