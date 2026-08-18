using System;
using SeungyungLib.Agents.ModuleSystem.Interface;
using SeungyungLib.Core;
using SeungyungLib.Core.Template.Modules;

using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class AgentMovementModule : MonoBehaviour, IAgentMovementModule
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private AgentMovementData movementData;
        
        public event Action<float> OnChangeAxis;

        private Vector2 _currentVelocity;
        private float _axis;
        private float _speed;
        private float _acceleration;
        private float _deceleration;
        private float _jumpForce;
        private float _jumpDeceleration;

        private void Update()
        {
            CalculateVelocity();
        }

        private void FixedUpdate()
        {
            Move();
        }
        
        public void Initialize(IModuleOwner owner)
        {
            this._speed = movementData.Speed;
            this._acceleration = movementData.Acceleration;
            this._deceleration = movementData.Deceleration;
            this._jumpForce = movementData.JumpForce;
            this._jumpDeceleration = movementData.JumpDeceleration;
            
            CustomDebug.Assert(body != null, "body is null");
        }
        
        public void SetMovementVelocity(float axis)
        {
            this._axis = axis;
            OnChangeAxis?.Invoke(axis);
        }
        public void Jump() => body.linearVelocity = new Vector2(body.linearVelocity.x, _jumpForce);
        
        private void CalculateVelocity()
        {
            float targetSpeed = _axis * _speed;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _deceleration;
            float newXVelocity = Mathf.MoveTowards(body.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
            
            _currentVelocity = new Vector2(newXVelocity, body.linearVelocity.y);
        }
        private void Move() => body.linearVelocity = _currentVelocity;
    }
}