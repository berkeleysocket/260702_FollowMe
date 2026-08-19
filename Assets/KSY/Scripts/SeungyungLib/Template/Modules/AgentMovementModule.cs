using SeungyungLib.ModuleSystem.Interface;

using System;
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
        [SerializeReference] private float speed;

        public event Action<float> OnChangeAxis;

        public void Initialize(IModuleOwner owner)
        {
            playerEventChannel.AddListener<MoveInputEvent>(HandleMoveInput);
        }

        private void HandleMoveInput(MoveInputEvent moveInputEvent)
        {
            SetMovementVelocity(moveInputEvent.Axis);
        }

        public void SetMovementVelocity(float axis) 
        {
            OnChangeAxis?.Invoke(axis);
            rb.linearVelocity = new Vector2(axis * speed, rb.linearVelocity.y);
        }

        public void Jump()
        {
        }
    }
}