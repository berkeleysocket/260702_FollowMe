using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Interface;

using System;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Modules
{
    [RequireComponent(typeof(Collider2D))]
    public class RangeCheckModule : MonoBehaviour, IRangeCheckModule
    {
        [SerializeField] private LayerMask whatIsCheck;
        
        public event Action OnEntered;
        
        private Collider2D _collider;
        
        public void Initialize(IModuleOwner owner)
        {
            _collider = GetComponent<Collider2D>();
            
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & whatIsCheck) >= 1)
            {
                OnEntered?.Invoke();
                DebugLogger.Log("Is Check !");
            }
        }
    }
}
