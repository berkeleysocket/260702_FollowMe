using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "Idle" + nameof(StateSO), menuName = "SeungyungLib/FSM/" + nameof(StateSO) + "/Idle", order = 0)]
    public class IdleStateSO : StateSO
    {
        protected override IState OnCreate(IModuleOwner owner, int enterAnimHash, ITransition[] transitions)
        {
            return new IdleState(owner, enterAnimHash, transitions);
        }
    }
}