using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class RunState : AbstractState
    {
        public RunState(IStateModule stateModule, IAgentRenderModule renderModule, int animationNameHash, Transition[] transitions) : base(stateModule, renderModule, animationNameHash, transitions)
        {
        }
    }
}