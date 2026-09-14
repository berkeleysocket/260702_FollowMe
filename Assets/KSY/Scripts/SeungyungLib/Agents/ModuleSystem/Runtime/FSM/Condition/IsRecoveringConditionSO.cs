using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Core;
    
using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsRecovering" + nameof(ConditionSO), menuName = "SeungyungLib/FSM/" + nameof(ConditionSO) + "/Is Recovering", order = 0)]
    public class IsRecoveringConditionSO : ConditionSO
    {
        protected override ICondition OnCreate(IModuleOwner owner)
        {
            return new IsRecoveringCondition(owner, Type, IsNot);
        }
    }
}