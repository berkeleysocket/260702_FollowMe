using SeungyungLib.CollectSystem;

using UnityEngine;

namespace KSY.Item
{
    [CreateAssetMenu(fileName = "ItemSO", menuName = "KSY/ItemSO", order = 0)]
    public class ItemSO : CollectableSO
    {
        [field: SerializeField] public int Score { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }  
}