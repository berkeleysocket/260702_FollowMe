using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsGroundedConditionSo", menuName = "SeungyungLib/FSM/ConditionSo/Is Grounded", order = 0)]
    public class IsGroundedConditionSo : AbstractConditionSo
    {
        public override ICondition Create(IModuleOwner owner) => new IsGroundedCondition(owner, IsNot);
    }
}
