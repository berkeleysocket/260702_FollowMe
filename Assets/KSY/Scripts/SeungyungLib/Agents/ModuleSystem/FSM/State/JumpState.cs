using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class JumpState : AbstractState
    {
        public JumpState(IStateModule stateModule, IAgentRenderModule renderModule, int animationNameHash, Transition[] transitions) : base(stateModule, renderModule, animationNameHash, transitions)
        {
        }
    }
}