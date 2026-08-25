using SeungyungLib.Core.ObjectPool;

using UnityEngine;

namespace SeungyungLib.Core.Effects
{
    public class RecycleParticleVfx : PlayableParticleVfx, IPoolable
    {
        [field: SerializeField] public PoolItemSo Item { get; set; }
        public GameObject GameObject => gameObject;
        public void ResetItem()
        {
            StopVfx();
            gameObject.SetActive(false);
        }
    }
}
