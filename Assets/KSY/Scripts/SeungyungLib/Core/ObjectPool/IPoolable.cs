using UnityEngine;

namespace SeungyungLib.Core.ObjectPool
{
    public interface IPoolable
    {
        PoolItemSo Item { get; set; }
        GameObject GameObject { get; }
        void ResetItem();
    }
}