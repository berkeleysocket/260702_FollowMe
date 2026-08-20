using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.Core.ParameterSO;
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
        [SerializeField] private AssetNameSO dustParticleName;
        [SerializeField] private float maxSpeed = 0f;
        [SerializeField] private float acceleration = 0f;
        [SerializeField] private float deceleration = 0f;
        [SerializeField] private float jumpForce = 0f;
        [SerializeField] private float fallMultiplier = 0f;
        [SerializeField] private float lowFallMultiplier = 0f;
        [SerializeField] private float airMultiplier = 0f;

        private bool IsJumping => rb.linearVelocityY > 0f;
        private bool IsFall => rb.linearVelocityY < 0f;
        public event Action<float> OnChangeAxis;

        private IAgentGroundCheckModule _groundChecker;
        private IAgentVfxModule _agentVfxModule;
        private Vector2 _velocity;
        private float _axis;
        private float _currentSpeed;
        private bool _isJumpKeyPressed;
        private bool _isPlayingDustParticle;
        
        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            _groundChecker = owner.GetModule<IAgentGroundCheckModule>();
            _agentVfxModule = owner.GetModule<IAgentVfxModule>();
            
            DebugLogger.Assert(rb != null, "[AgentMovementModule]: rb is null]");
            DebugLogger.Assert(playerEventChannel != null, "[AgentMovementModule]: playerEventChannel is null]");
            DebugLogger.Assert(dustParticleName != null, "[AgentMovementModule]: dustParticleName is null]");
            DebugLogger.Assert(_groundChecker != null, "[AgentMovementModule]: _groundChecker is null]");
            DebugLogger.Assert(_agentVfxModule != null, "[AgentMovementModule]: _vfxModule is null]");

            playerEventChannel.AddListener<MoveInputEvent>(HandleMoveInput);
            playerEventChannel.AddListener<JumpInputEvent>(HandleJumpInput);
        }
        #endregion

        #region Unity Events
        private void FixedUpdate()
        {
            ApplyGravity();
            CalculateVelocity();
            Run();
            Jump();
            PlayDustParticle();
        }
        #endregion
       
        #region Event Handlers
        private void HandleMoveInput(MoveInputEvent evt)
            => MoveToDirection(evt.Axis);

        public void MoveToDirection(float axis) 
        {
            OnChangeAxis?.Invoke(axis);

            this._axis = axis;
        }

        private void HandleJumpInput(JumpInputEvent evt)
            => this._isJumpKeyPressed = evt.JumpKeyPressed;

        #endregion

        private void Run()
        {
            rb.linearVelocity = _velocity;
        }
            
        private void Jump()
        {
            if (_isJumpKeyPressed && _groundChecker.IsGrounded())
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }

        private void PlayDustParticle()
        {
            if (!IsJumping && !IsFall && _axis != 0f && !_isPlayingDustParticle)
            {
                bool isFlip = _axis < 0f;
                _isPlayingDustParticle = true;
                _agentVfxModule.PlayVfx(dustParticleName.Hash, isFlip);
            }   
            else if ((IsJumping || IsFall || _axis == 0f) && _isPlayingDustParticle)
            {
                _isPlayingDustParticle = false;
                _agentVfxModule.StopVfx(dustParticleName.Hash); 
            }
        }

        private void ApplyGravity()
        {
            if (IsFall)
                rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
            else if (IsJumping && !_isJumpKeyPressed)
                rb.linearVelocity += Vector2.up * (Physics.gravity.y * (lowFallMultiplier - 1) * Time.fixedDeltaTime);
        }
        
        private void CalculateVelocity()
        {
            float currentX = rb.linearVelocityX;
            
            if (_axis != 0)
            {
                float airMultiplier = IsJumping || IsFall ? this.airMultiplier : 1f;
                bool isReversing = (currentX * _axis < 0);
                float accelRate = isReversing ? (acceleration + deceleration) : acceleration;
                float targetX = _axis * maxSpeed * airMultiplier;

                currentX = Mathf.MoveTowards(currentX, targetX, accelRate * Time.fixedDeltaTime);
            }
            else
                currentX = Mathf.MoveTowards(currentX, 0, deceleration * Time.fixedDeltaTime);
            
            _velocity = new Vector2(currentX, rb.linearVelocity.y);
        }
    }
}