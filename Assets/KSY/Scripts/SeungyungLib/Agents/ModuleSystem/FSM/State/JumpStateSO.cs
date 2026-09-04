using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "Jump" + nameof(StateSO), menuName = "SeungyungLib/FSM/" + nameof(StateSO) + "/Jump", order = 0)]
    public class JumpStateSO : StateSO
    {
        protected override IState Create(IModuleOwner owner, int enterAnimHash, ITransition[] transitions)
        {
            return new JumpState(owner, enterAnimHash, transitions);
        }
    }
}