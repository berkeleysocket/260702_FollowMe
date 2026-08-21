using System;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class GroundCheckModule : MonoBehaviour, IAgentGroundCheckModule
    {
        [SerializeField] private Vector2 checkCenter;
        [SerializeField] private Vector2 checkRange;
        [SerializeField] private LayerMask whatIsGround;

        private IAgentMovementModule _movementModule;
        
        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            _movementModule = owner.GetModule<IAgentMovementModule>();
            
            DebugLogger.Assert(_movementModule != null, "[GroundCheckModule]: _movementModule is null.");
        }
        #endregion
        
        #region Unity Events

        private void Update()
        {
            if (_movementModule.IsJumping || _movementModule.IsFall)
                IsGrounded();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube((Vector2)transform.position + checkCenter, checkRange);
        }
        #endregion

        private bool IsGrounded()
        {
            Collider2D groundCollider = Physics2D.OverlapBox(
                (Vector2)transform.position + checkCenter,
                checkRange,
                angle: 0f,
                whatIsGround
                );
            
            return groundCollider != null;
        }
    }
}