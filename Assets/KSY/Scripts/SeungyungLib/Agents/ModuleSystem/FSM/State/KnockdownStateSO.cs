using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "Knockdown" + nameof(StateSO), menuName = "SeungyungLib/FSM/" + nameof(StateSO) + "/Knockdown", order = 0)]
    public class KnockdownStateSO : StateSO
    {
        protected override IState OnCreate(IModuleOwner owner, int enterAnimHash, ITransition[] transitions)
        {
            return new KnockdownState(owner, enterAnimHash, transitions);
        }
    }
}