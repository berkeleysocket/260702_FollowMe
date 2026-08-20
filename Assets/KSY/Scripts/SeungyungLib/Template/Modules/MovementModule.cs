using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.Template.EventChannels;

using System;
using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class MovementModule : MonoBehaviour, IAgentMovementModule
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private EventChannelSO playerEventChannel;
        [SerializeField] private float maxSpeed = 0f;
        [SerializeField] private float acceleration = 0f;
        [SerializeField] private float deceleration = 0f;
        [SerializeField] private float jumpForce = 0f;
        [SerializeField] private float fallMultiplier = 0f;
        [SerializeField] private float lowFallMultiplier = 0f;

        public event Action<float> OnChangeAxis;
        
        private IAgentGroundCheckModule _groundChecker;
        private Vector2 _velocity;
        private float _axis;
        private float _currentSpeed;
        private bool _isJumpKeyPressed;
        
        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            _groundChecker = owner.GetModule<IAgentGroundCheckModule>();
            
            DebugLogger.Assert(rb != null, "[AgentMovementModule]: rb is null]");
            DebugLogger.Assert(playerEventChannel != null, "[AgentMovementModule]: playerEventChannel is null]");
            DebugLogger.Assert(rb != null, "[AgentMovementModule]: _groundChecker is null]");
            
            playerEventChannel.AddListener<MoveInputEvent>(HandleMoveInput);
            playerEventChannel.AddListener<JumpInputEvent>(HandleJumpInput);
        }
        #endregion

        #region Unity Events
        private void FixedUpdate()
        {
            ApplyGravity();
            CalculateVelocity();

            rb.linearVelocity = _velocity;
        }
        #endregion
       
        #region Event Handlers
        private void HandleMoveInput(MoveInputEvent evt)
            => MoveToDirection(evt.Axis);

        private void HandleJumpInput(JumpInputEvent evt)
        {
            this._isJumpKeyPressed = evt.JumpKeyPressed;
            Jump();
        }
        #endregion

        public void MoveToDirection(float axis) 
        {
            OnChangeAxis?.Invoke(axis);
            
            this._axis = axis;
        }
        public void Jump()
        {
            if (_groundChecker.IsGrounded())
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }

        private void ApplyGravity()
        {
            bool isFall =  rb.linearVelocityY < 0f;
            bool isJumping = rb.linearVelocityY > 0f;
            
            if (isFall)
                rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
            else if (isJumping && !_isJumpKeyPressed)
                rb.linearVelocity += Vector2.up * (Physics.gravity.y * (lowFallMultiplier - 1) * Time.fixedDeltaTime);
        }
        
        private void CalculateVelocity()
        {
            float currentX = rb.linearVelocityX;
            
            if (_axis != 0)
            {
                bool isReversing = (currentX * _axis < 0);
                float accelRate = isReversing ? (acceleration + deceleration) : acceleration;
                float targetX = _axis * maxSpeed;

                currentX = Mathf.MoveTowards(currentX, targetX, accelRate * Time.fixedDeltaTime);
            }
            else
                currentX = Mathf.MoveTowards(currentX, 0, deceleration * Time.fixedDeltaTime);
            
            _velocity = new Vector2(currentX, rb.linearVelocity.y);
        }
    }
}