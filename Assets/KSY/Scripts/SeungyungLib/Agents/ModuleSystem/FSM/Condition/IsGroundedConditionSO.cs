using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsGrounded" + nameof(ConditionSO), menuName = "SeungyungLib/FSM/" + nameof(ConditionSO) + "/Is Grounded", order = 0)]
    public class IsGroundedConditionSO : ConditionSO
    {
        protected override ICondition OnCreate(IModuleOwner owner)
        {
            return new IsGroundedCondition(owner, Type, IsNot);
        }
    }
}
