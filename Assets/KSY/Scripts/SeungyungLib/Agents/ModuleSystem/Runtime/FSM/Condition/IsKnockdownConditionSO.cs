using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Core;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsKnockdown" + nameof(ConditionSO), menuName = "SeungyungLib/FSM/" + nameof(ConditionSO) + "/Is Knockdown", order = 0)]
    public class IsKnockdownConditionSO : ConditionSO
    {
        protected override ICondition OnCreate(IModuleOwner owner)
        {
            return new IsKnockdownCondition(owner, Type, IsNot);
        }
    }
}
