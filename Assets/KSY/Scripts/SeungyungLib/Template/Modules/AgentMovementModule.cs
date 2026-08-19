using SeungyungLib.ModuleSystem.Interface;

using System;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.Core.InputSystem;
using SeungyungLib.Template.EventChannels;
using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class AgentMovementModule : MonoBehaviour, IAgentMovementModule
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private EventChannelSO playerEventChannel;
        
        
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;

        private Vector2 _velocity;
        private float _axis;
        private float _currentSpeed;

        public event Action<float> OnChangeAxis;

        public void Initialize(IModuleOwner owner)
        {
            playerEventChannel.AddListener<MoveInputEvent>(HandleMoveInput);
        }

        private void FixedUpdate()
        {
            CalculateVelocity();

            rb.linearVelocity = _velocity;
        }

        private void HandleMoveInput(MoveInputEvent moveInputEvent)
        {
            SetMovementVelocity(moveInputEvent.Axis);
        }

        public void SetMovementVelocity(float axis) 
        {
            OnChangeAxis?.Invoke(axis);
            
            DebugLogger.Log($"axis: {axis}", Color.yellow);
            _axis = axis;
        }

        public void Jump()
        {
        }

        private void CalculateVelocity()
        {
            if (_axis != 0)
            {
                bool isReversing = _currentSpeed < _axis;
                _currentSpeed = Mathf.Clamp(_currentSpeed - deceleration * Time.fixedDeltaTime, 0f, maxSpeed);
            }
            else if (_currentSpeed < maxSpeed)
            {
                _currentSpeed = Mathf.Clamp(_currentSpeed + acceleration * Time.fixedDeltaTime, 0f, maxSpeed);
            }
            
            _velocity = new Vector2(_currentSpeed * _axis, rb.linearVelocity.y);
        }
    }
}