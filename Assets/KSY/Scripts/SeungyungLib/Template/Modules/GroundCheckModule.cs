using System;
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
            Gizmos.DrawCube(checkCenter, checkRange);
        }
        #endregion

        public bool IsGrounded()
        {
            Collider2D collider = Physics2D.OverlapBox(checkCenter, checkRange, whatIsGround);
            return collider != null;
        }
    }
}