using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "Hit" + nameof(StateSO), menuName = "SeungyungLib/FSM/" + nameof(StateSO) + "/Hit", order = 0)]
    public class HitStateSO : StateSO
    {
        protected override IState OnCreate(IModuleOwner owner, int enterAnimHash, ITransition[] transitions)
        {
            return new HitState(owner, enterAnimHash, transitions);
        }
    }
}