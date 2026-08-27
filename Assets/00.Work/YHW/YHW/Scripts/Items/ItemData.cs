using UnityEngine;

namespace YHW.Items
{
    [CreateAssetMenu(fileName = "Item_", menuName = "YHW/Item Data", order = 0)]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private ItemType itemType;
        [SerializeField] private Sprite icon;
        [SerializeField, TextArea] private string description;
        [SerializeField] private int value = 1;

        public string ItemId => itemId;
        public string DisplayName => displayName;
        public ItemType ItemType => itemType;
        public Sprite Icon => icon;
        public string Description => description;
        public int Value => value;
    }
}
