using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class IdleState : AbstractState
    {
        public IdleState(IStateModule stateModule, IAgentRenderModule renderModule, int animationNameHash, Transition[] transitions) : base(stateModule, renderModule, animationNameHash, transitions)
        {
        }
    }
}