using UnityEngine;

namespace SeungyungLib.Core.BaseCollider
{
    public readonly struct CollisionContext
    {
        public CollisionContext(GameObject other, LayerMask layer)
        {
            this.other = other;
            this.layer = layer;
        }
        
        public readonly GameObject other;
        public readonly LayerMask layer;
    }
}
