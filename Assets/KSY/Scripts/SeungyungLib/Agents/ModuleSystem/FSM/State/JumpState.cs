using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class JumpState : AbstractState
    {
        public JumpState(IStateMachineModule stateMachineModule,  IModuleOwner owner, int animationNameHash, Transition[] transitions) : base(stateMachineModule, owner, animationNameHash, transitions)
        {
        }
    }
}