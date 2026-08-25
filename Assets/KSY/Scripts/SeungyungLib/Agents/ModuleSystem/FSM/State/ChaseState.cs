using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class ChaseState : AbstractState
    {
        public ChaseState(IStateModule stateModule, IRenderModule renderModule, int animationNameHash, Transition[] transitions) : base(stateModule, renderModule, animationNameHash, transitions)
        {
        }
    }
}
