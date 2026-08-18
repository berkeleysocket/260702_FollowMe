using SeungyungLib.Agents.FSM;
using SeungyungLib.Agents.ModuleSystem.Interface;

using System;

namespace SeungyungLib.Template.States
{
    [Serializable]
    public class JumpState : AbstractState
    {
        public JumpState(IModuleOwner owner, int animationHash) : base(owner, animationHash) { }
        
        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
