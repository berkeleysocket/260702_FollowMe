using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.Template.EventChannels;

using System;
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
            float currentX = rb.linearVelocityX;
            
            if (_axis != 0)
            {
                bool isReversing = (currentX * _axis < 0);
                float accelRate = isReversing ? (acceleration + deceleration) : acceleration;
                float targetX = _axis * maxSpeed;

                currentX = Mathf.MoveTowards(currentX, targetX, accelRate * Time.fixedDeltaTime);
            }
            else
            {
                currentX = Mathf.MoveTowards(currentX, 0, deceleration * Time.fixedDeltaTime);
            }
            
            _velocity = new Vector2(currentX, rb.linearVelocity.y);
        }
    }
}