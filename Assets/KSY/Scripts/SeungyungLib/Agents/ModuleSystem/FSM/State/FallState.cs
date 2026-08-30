using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class FallState : AbstractState
    {
        public FallState(IStateMachineModule stateMachineModule, IRenderModule renderModule, int animationNameHash, Transition[] transitions) : base(stateMachineModule, renderModule, animationNameHash, transitions)
        {
        }
    }
}