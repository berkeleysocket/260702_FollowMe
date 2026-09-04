using SeungyungLib.Core.NotifyValue;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.ModuleSystem.Modules
{
    public class GroundCheckModule : MonoBehaviour, IGroundCheckModule, IAfterInitModule
    {
        [SerializeField] private Vector2 checkCenter;
        [SerializeField] private Vector2 checkRange;
        [SerializeField] private LayerMask whatIsGround;

        public bool IsActive { get; private set; }
        
        public NotifyValue<bool> NotifyIsGrounded { get; private set; } = new NotifyValue<bool>(false);

        private IMovementModule _movementModule;
        
        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            this.NotifyIsGrounded =  new NotifyValue<bool>(false);

            DebugLogger.Assert(NotifyIsGrounded != null, "[GroundCheckModule]: NotifyIsGround is null.");
        }
        
        public void AfterInitialization(IModuleOwner owner)
        {
            this._movementModule = owner.GetModule<IMovementModule>();
            
            DebugLogger.Assert(_movementModule != null, "[GroundCheckModule]: _movementModule is null.");
        }
        #endregion
        
        #region Unity Events
        private void Update()
        {
            if (_movementModule == null) return;
            
            if (!NotifyIsGrounded.Value)
                CheckGround();
            else if (_movementModule.IsJumping || _movementModule.IsFall)
                NotifyIsGrounded.Value = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube((Vector2)transform.position + checkCenter, checkRange);
        }
        #endregion
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        private void CheckGround()
        {
            Collider2D groundCollider = Physics2D.OverlapBox(
                (Vector2)transform.position + checkCenter,
                checkRange,
                angle: 0f,
                whatIsGround
                );
            
            if (groundCollider != null)
                NotifyIsGrounded.Value = true;
        }
    }
}