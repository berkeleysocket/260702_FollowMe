using SeungyungLib.Agents.FSM;
using SeungyungLib.Agents.ModuleSystem.Interface;
using SeungyungLib.Core.EventChannelSystem;

using System;
using SeungyungLib.Template.EventChannels;
using UnityEngine;

namespace SeungyungLib.Template.States
{
    [Serializable]
    public class RunState : AbstractState
    {
        [SerializeField] private EventChannelSO InputChannel;
        
        public RunState(IModuleOwner owner, int animationHash) : base(owner, animationHash) { }

        public override void Enter()
        {
            base.Enter();
            InputChannel.AddListener<MoveInputEvent>(HandleMoveKeyInput);
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

