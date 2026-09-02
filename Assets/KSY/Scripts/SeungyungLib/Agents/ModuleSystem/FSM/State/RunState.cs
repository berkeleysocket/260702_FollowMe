using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class RunState : AbstractState
    {
        public RunState(IStateMachineModule stateMachineModule, IModuleOwner owner, int animationNameHash, Transition[] transitions) : base(stateMachineModule, owner, animationNameHash, transitions)
        {
        }
    }
}