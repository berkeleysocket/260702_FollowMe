using System.Collections.Generic;
using UnityEngine;

namespace YHW.Items
{
    public class ItemStackUI : MonoBehaviour
    {
        [SerializeField] private PlayerItemCollector collector;
        [SerializeField] private ItemStackSlotUI slotPrefab;
        [SerializeField] private Transform slotContainer;

        private readonly Dictionary<ItemData, ItemStackSlotUI> slots = new Dictionary<ItemData, ItemStackSlotUI>();
        private readonly Dictionary<ItemData, int> counts = new Dictionary<ItemData, int>();

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
            if (item == null) return;

            if (!counts.TryGetValue(item, out int count))
                count = 0;

            count++;
            counts[item] = count;

            if (!slots.TryGetValue(item, out ItemStackSlotUI slot))
            {
                slot = Instantiate(slotPrefab, slotContainer);
                slot.Setup(item.Icon);
                slots[item] = slot;
            }

            slot.SetCount(count);
        }
    }
}
