using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class IdleStateSo : AbstractStateSo
    {
        protected override IState OnCreate(IModuleOwner owner, int enterAnimHash, ITransition[] transitions)
        {
            return new IdleState(owner, enterAnimHash, transitions);

        }
    }
}