using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "Fall" + nameof(StateSO), menuName = "SeungyungLib/FSM/" + nameof(StateSO) + "/Fall", order = 0)]
    public class FallStateSO : StateSO
    {
        protected override IState Create(IModuleOwner owner, int enterAnimHash, ITransition[] transitions)
        {
            return new FallState(owner, enterAnimHash, transitions);
        }
    }
}