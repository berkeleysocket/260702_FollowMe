using SeungyungLib.Core.NotifyValue;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class GroundCheckModule : MonoBehaviour, IGroundCheckModule
    {
        [SerializeField] private Vector2 checkCenter;
        [SerializeField] private Vector2 checkRange;
        [SerializeField] private LayerMask whatIsGround;

        public NotifyValue<bool> NotifyIsGround { get; private set; } = new NotifyValue<bool>(false);

        private IMovementModule _movementModule;
        
        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            this.NotifyIsGround =  new NotifyValue<bool>(false);
            this._movementModule = owner.GetModule<IMovementModule>();

            DebugLogger.Assert(NotifyIsGround != null, "[GroundCheckModule]: NotifyIsGround is null.");
            DebugLogger.Assert(_movementModule != null, "[GroundCheckModule]: _movementModule is null.");
        }
        #endregion
        
        #region Unity Events
        private void Update()
        {
            if (_movementModule == null || NotifyIsGround == null) return;
            
            if (!NotifyIsGround.Value)
                CheckGround();
            else if (_movementModule.IsJumping || _movementModule.IsFall)
                NotifyIsGround.Value = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube((Vector2)transform.position + checkCenter, checkRange);
        }
        #endregion

        private void CheckGround()
        {
            Collider2D groundCollider = Physics2D.OverlapBox(
                (Vector2)transform.position + checkCenter,
                checkRange,
                angle: 0f,
                whatIsGround
                );
            
            if (groundCollider != null)
                NotifyIsGround.Value = true;
        }
    }
}