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
        
        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
        }
        #endregion
        
        #region Unity Events
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube((Vector2)transform.position + checkCenter, checkRange);
        }
        #endregion

        public bool IsGrounded()
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