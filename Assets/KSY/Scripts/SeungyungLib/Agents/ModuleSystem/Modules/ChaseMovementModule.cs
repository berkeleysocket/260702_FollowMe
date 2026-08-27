using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Interface;

using System;
using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class ChaseMovementModule : MonoBehaviour, IChaseMovementModule
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float maxSpeed = 0f;
        [SerializeField] private float acceleration = 0f;
        [SerializeField] private float deceleration = 0f;
        [SerializeField] private float jumpForce = 0f;
        [SerializeField] private float fallMultiplier = 0f;
        [SerializeField] private float lowFallMultiplier = 0f;
        [SerializeField] private float airMultiplier = 0f;

        public bool IsJumpKeyPressed { get; set; }

        public event Action<float> OnChangeAxis;

        public float Axis => _axis;
        public bool IsMoving => _axis != 0;
        public bool IsJumping => rb.linearVelocityY > 0f;
        public bool IsFall => rb.linearVelocityY < 0f;

        private Transform _targetTrm;
        private IGroundCheckModule _groundChecker;
        private Vector2 _velocity;
        private float _axis;
        private float _currentSpeed;

        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            _groundChecker = owner.GetModule<IGroundCheckModule>();
            
            DebugLogger.Assert(rb != null, "[AgentMovementModule]: rb is null.");
            DebugLogger.Assert(_groundChecker != null, "[AgentMovementModule]: _groundChecker is null.");
        }
        #endregion

        #region Unity Events
        private void FixedUpdate()
        {
            ApplyGravity();
            CalculateVelocity();
            Run();
            // Jump();
        }
        #endregion
        
        public void MoveToDirection(float axis) 
        {
            this._axis = axis;
            OnChangeAxis?.Invoke(axis);
        }

        private void Run()
        {
            rb.linearVelocity = _velocity;
        }
            
        // private void Jump()
        // {
        //     if (IsJumpKeyPressed && _groundChecker.NotifyIsGround.Value)
        //         rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        // }

        private void ApplyGravity()
        {
            if (IsFall)
                rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
            else if (IsJumping && !IsJumpKeyPressed)
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
