using SeungyungLib.Core.BaseCollider;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Core;

using System;
using UnityEngine;

namespace SeungyungLib.CollectSystem
{
    public class CollectModule : MonoBehaviour, ICollectModule, IAfterInitModule
    {
        [SerializeField] private BaseCollider collectBaseCollider;
        
        public event Action<CollectContext> OnCollected;
        
        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            DebugLogger.Assert(collectBaseCollider != null, "[ColliderModule]: collider is null");
        }
        
        public void AfterInitialization(IModuleOwner owner)
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Action<CollisionContext> onContacted = (context) =>
            {
                GameObject go = context.other;
                
                if (go != null && go.TryGetComponent(out ICollectable collectable))
                {
                    collectable.Collect(this);
                    OnCollected?.Invoke(new CollectContext(this, collectable));
                }
            };
            collectBaseCollider.RegisterAction(CollisionOption.Trigger | CollisionOption.Enter, onContacted);
        }
        #endregion

        #region Activable
        public bool IsActive { get; private set; }
        
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
        #endregion


    }
}

