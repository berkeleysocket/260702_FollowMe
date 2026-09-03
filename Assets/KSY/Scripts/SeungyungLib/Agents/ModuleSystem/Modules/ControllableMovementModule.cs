using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Interface;

using System;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Modules
{
    public class ControllableMovementModule : MonoBehaviour, IControllableMovementModule, IAfterInitModule
    {
        [SerializeField] private float maxSpeed = 0f;
        [SerializeField] private float acceleration = 0f;
        [SerializeField] private float deceleration = 0f;
        [SerializeField] private float jumpForce = 0f;
        [SerializeField] private float fallMultiplier = 0f;
        [SerializeField] private float lowFallMultiplier = 0f;
        [SerializeField] private float airMultiplier = 0f;

        public bool IsJumpKeyPressed { get; set; }
        public bool IsActive { get; private set; }

        public event Action<int> OnMoved;
        
        public int Axis => _axis;
        public bool IsMoving => _axis != 0;
        public bool IsJumping => _rb.linearVelocityY > 0.5f;
        public bool IsFall => _rb.linearVelocityY < -0.5f;
        
        private Rigidbody2D _rb;
        private IGroundCheckModule _groundChecker;
        private Vector2 _velocity;
        private int _axis;
        private float _currentSpeed;

        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            _groundChecker = owner.GetModule<IGroundCheckModule>();
            
            DebugLogger.Assert(_groundChecker != null, "[AgentMovementModule]: _groundChecker is null.");
            DebugLogger.Assert(_rb != null, "[AgentMovementModule]: _rb is null.");
        }
        
        public void AfterInitialization(IModuleOwner owner)
        {
            _rb = owner.GetModule<IBodyModule>().Body;
            
            if (_groundChecker != null)
            {
                _groundChecker.NotifyIsGround.OnChanged += (bool isGround) =>
                {
                    if (isGround)
                        _rb.linearVelocity = new Vector2(_rb.linearVelocityX, 0f);
                };
            }

            IsActive = true;
        }
        #endregion

        #region Unity Events
        private void FixedUpdate()
        {
            ApplyGravity();
            
            if (!IsActive) return;
            CalculateVelocity();
            Run();
            Jump();
        }
        #endregion

        public void Activate()
        {
            IsActive = true;
            OnMoved?.Invoke(_axis);
        }

        public void Deactivate()
        {
            IsActive = false;
            OnMoved?.Invoke(0);
            _rb.linearVelocity = Vector2.zero;
        }
        
        public void MoveToDirection(int axis)
        {
            _axis = axis;
            
            if (IsActive)
                OnMoved?.Invoke(axis);
        }

        private void Run()
        {
            _rb.linearVelocity = _velocity;
        }
            
        private void Jump()
        {
            if (IsJumpKeyPressed && _groundChecker.NotifyIsGround.Value)
                _rb.linearVelocity = new Vector2(_rb.linearVelocityX, jumpForce);
        }

        private void ApplyGravity()
        { 
            if (_rb.linearVelocity.y < -0.5f)
                _rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
            else if (IsJumping && !IsJumpKeyPressed)
                _rb.linearVelocity += Vector2.up * (Physics.gravity.y * (lowFallMultiplier - 1) * Time.fixedDeltaTime);
        }
        
        private void CalculateVelocity()
        {
            float currentX = _rb.linearVelocityX;
            
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
            
            _velocity = new Vector2(currentX, _rb.linearVelocity.y);
        }
    }
}