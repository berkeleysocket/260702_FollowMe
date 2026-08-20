using UnityEngine;

namespace SeungyungLib.Core.ObjectPool
{
    [CreateAssetMenu(fileName = "PoolItemSo", menuName = "SeungyungLib/Core/ObjectPool/PoolItemSo", order = 0)]
    public class PoolItemSo : ScriptableObject
    {
        [HideInInspector] public string itemName;
        public GameObject prefab;
        public int initCount;

        private void OnValidate()
        {
            if (prefab != null && !prefab.TryGetComponent(out IPoolable _))
            {
                Debug.LogError($"Poolable component not found on {prefab.name}");
                prefab = null;
            }
        }
    }
}