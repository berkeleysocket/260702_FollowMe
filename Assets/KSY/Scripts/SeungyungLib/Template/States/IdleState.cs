using SeungyungLib.Agents.FSM;
using SeungyungLib.Agents.ModuleSystem.Interface;

using System;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.Template.EventChannels;
using UnityEngine;

namespace SeungyungLib.Template.States
{
    [Serializable]
    public class IdleState : AbstractState
    {
        [SerializeField] private EventChannelSO InputChannel;
        
        public IdleState(IModuleOwner owner, int animationHash) : base(owner, animationHash) { }
           
        public override void Enter()
        {
            base.Enter();
            _movementModule.SetMovementVelocity(0f);
        }

        public override void Update()
        {
            base.Update();
        }
        
        public override void Exit()
        {
            base.Exit();
            InputChannel.RemoveListener<MoveInputEvent>(HandleMoveKeyInput);
        }

        private void HandleMoveKeyInput(MoveInputEvent evt)
        {
            _movementModule.SetMovementVelocity(evt.Axis);
        }
    }
}

