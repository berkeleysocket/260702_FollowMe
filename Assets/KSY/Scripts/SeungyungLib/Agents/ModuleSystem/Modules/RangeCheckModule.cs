using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Interface;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Modules
{
    [RequireComponent(typeof(Collider2D))]
    public class RangeCheckModule : MonoBehaviour, IRangeCheckModule
    {
        [SerializeField] private LayerMask whatIsCheck;

        public void Initialize(IModuleOwner owner)
        {
            
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (((1 << other.gameObject.layer) & whatIsCheck) >= 1)
                DebugLogger.Log("Is Check !");
        }
    }
}
