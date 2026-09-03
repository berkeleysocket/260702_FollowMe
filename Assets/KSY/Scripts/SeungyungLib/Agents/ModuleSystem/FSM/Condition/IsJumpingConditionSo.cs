using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsJumping" + nameof(ConditionSO), menuName = "SeungyungLib/FSM/" + nameof(ConditionSO) + "/Is Jumping", order = 0)]
    public class IsJumpingConditionSo : ConditionSO
    {
        protected override ICondition OnCreate(IModuleOwner owner)
        {
            return new IsJumpingCondition(owner, Type, IsNot);
        }
    }
}
