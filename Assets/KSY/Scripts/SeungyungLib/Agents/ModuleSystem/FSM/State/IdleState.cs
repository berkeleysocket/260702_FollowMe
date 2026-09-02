using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class IdleState : AbstractState
    {
        public IdleState(IStateMachineModule stateMachineModule, IModuleOwner owner, int animationNameHash, Transition[] transitions) : base(stateMachineModule, owner, animationNameHash, transitions)
        {
        }
    }
}