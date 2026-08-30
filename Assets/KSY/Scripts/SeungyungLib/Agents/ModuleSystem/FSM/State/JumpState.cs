using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class JumpState : AbstractState
    {
        public JumpState(IStateMachineModule stateMachineModule, IRenderModule renderModule, int animationNameHash, Transition[] transitions) : base(stateMachineModule, renderModule, animationNameHash, transitions)
        {
        }
    }
}