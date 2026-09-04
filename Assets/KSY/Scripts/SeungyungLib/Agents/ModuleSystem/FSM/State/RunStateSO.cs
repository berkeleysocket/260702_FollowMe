using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "Run" + nameof(StateSO), menuName = "SeungyungLib/FSM/" + nameof(StateSO) + "/Run", order = 0)]
    public class RunStateSO : StateSO
    {
        protected override IState Create(IModuleOwner owner, int enterAnimHash, ITransition[] transitions)
        {
            return new RunState(owner, enterAnimHash, transitions);
        }
    }
}