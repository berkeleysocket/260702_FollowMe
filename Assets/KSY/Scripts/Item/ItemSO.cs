using SeungyungLib.CollectSystem;
using UnityEngine;

namespace KSY.Item
{
    [CreateAssetMenu(fileName = nameof(ItemSO), menuName = "KSY/Item/" + nameof(ItemSO))]
    public class ItemSO : CollectableSO
    {
        [field: SerializeField] public Sprite DefaultSprite { get; private set; }
        [field: SerializeField] public GameObject CollectParticle { get; private set; }
        [field: SerializeField] public int Score { get; private set; }
        [field: SerializeField] public int RecoveryValue { get; private set; }
    }
}