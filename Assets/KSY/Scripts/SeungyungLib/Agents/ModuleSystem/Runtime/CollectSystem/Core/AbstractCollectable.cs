using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Core;

using UnityEngine;

namespace SeungyungLib.CollectSystem.Core
{
    public abstract class AbstractCollectable : MonoBehaviour, ICollectable
    {
        private bool _isCollected;
        
        public void Collect(IModuleOwner collector)
        {
            if (collector == null || _isCollected) return;
            
            OnCollect(collector);
        }
        protected abstract void OnCollect(IModuleOwner collector);
        
        protected void Delete()
        {
            _isCollected = true;

            OnDestroy();
        }
        protected virtual void OnDestroy()
        {
            //일단은 임시로 삭제하고, 먹었을 때 어떻게 할 지는 OnDestroy에서 재정의하도록 한다.
            Destroy(gameObject);
        }
    }
}
