using System;
using System.Collections.Generic;
using UnityEngine;

namespace YHW.Items
{
    public class PlayerItemCollector : MonoBehaviour
    {
        public event Action<ItemData> ItemCollected;

        private readonly List<ItemData> collectedItems = new List<ItemData>();
        public IReadOnlyList<ItemData> CollectedItems => collectedItems;

        public void Collect(ItemData item)
        {
            if (item == null) return;

            collectedItems.Add(item);
            ItemCollected?.Invoke(item);
        }
    }
}
