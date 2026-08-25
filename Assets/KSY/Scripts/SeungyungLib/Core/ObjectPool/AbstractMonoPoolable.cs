using UnityEngine;

namespace SeungyungLib.Core.ObjectPool
{
    public abstract class AbstractMonoPoolable : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolItemSo Item { get; set; }
        public GameObject GameObject => this != null ? gameObject : null;
        
        public virtual void ResetItem() { }
    }
}