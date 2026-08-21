using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.Template.EventChannels;

using System;
using SeungyungLib.Core.ParameterSO;
using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class MovementModule : MonoBehaviour, IAgentMovementModule, IAfterInitModule
    {
        [SerializeField] private AssetNameSO dustParticleName;
        [SerializeField] private AssetNameSO smokeParticleName;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private EventChannelSO playerEventChannel;
        [SerializeField] private float maxSpeed = 0f;
        [SerializeField] private float acceleration = 0f;
        [SerializeField] private float deceleration = 0f;
        [SerializeField] private float jumpForce = 0f;
        [SerializeField] private float fallMultiplier = 0f;
        [SerializeField] private float lowFallMultiplier = 0f;
        [SerializeField] private float airMultiplier = 0f;

        public float Axis => _axis;
        public bool IsMoving => _axis != 0;
        public bool IsJumping => rb.linearVelocityY > 0f;
        public bool IsFall => rb.linearVelocityY < 0f;
        public event Action<float> OnChangeAxis;

        private IAgentGroundCheckModule _groundChecker;
        private IAgentVfxModule _vfxModule; 
        private Vector2 _velocity;
        private float _axis;
        private float _currentSpeed;
        private bool _isJumpKeyPressed;
        
        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            _groundChecker = owner.GetModule<IAgentGroundCheckModule>();
            _vfxModule = owner.GetModule<IAgentVfxModule>();
            
            DebugLogger.Assert(rb != null, "[AgentMovementModule]: rb is null.");
            DebugLogger.Assert(playerEventChannel != null, "[AgentMovementModule]: playerEventChannel is null.");
            DebugLogger.Assert(_groundChecker != null, "[AgentMovementModule]: _groundChecker is null.");
            DebugLogger.Assert(_vfxModule != null, "[AgentMovementModule]: _vfxModule is null.");
            
            playerEventChannel.AddListener<MoveInputEvent>(HandleMoveInput);
            playerEventChannel.AddListener<JumpInputEvent>(HandleJumpInput);
        }
        
        public void AfterInitialization()
        {
            _groundChecker.NotifyIsGround.OnChanged += (isGround) =>
            {
                if (isGround)
                {
                    _vfxModule.PlayVfx(smokeParticleName.Hash, 
                        new Vector2(transform.position.x, transform.position.y - 0.5f), 
                        Quaternion.identity);
                    
                    if (_axis != 0)
                    {
                        bool isFlip = _axis < 0f;
                        _vfxModule.PlayVfx(dustParticleName.Hash, isFlip);
                    }
                    else
                        _vfxModule.StopVfx(dustParticleName.Hash);
                }
                else
                    _vfxModule.StopVfx(dustParticleName.Hash);
            };
        }
        #endregion

        #region Unity Events
        private void FixedUpdate()
        {
            ApplyGravity();
            CalculateVelocity();
            Run();
            Jump();
        }
        #endregion
       
        #region Event Handlers
        private void HandleMoveInput(MoveInputEvent evt)
            => MoveToDirection(evt.Axis);

        public void MoveToDirection(float axis) 
        {
            this._axis = axis;
            
            OnChangeAxis?.Invoke(axis);

            if (_axis != 0 && _groundChecker.NotifyIsGround.Value)
            {
                bool isFlip = _axis < 0f;
                _vfxModule.PlayVfx(dustParticleName.Hash, isFlip);
            }
            else
                _vfxModule.StopVfx(dustParticleName.Hash);
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
            if (_isJumpKeyPressed && _groundChecker.NotifyIsGround.Value)
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
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