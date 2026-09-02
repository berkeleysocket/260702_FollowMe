using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsJumpingConditionSo", menuName = "SeungyungLib/FSM/ConditionSo/Is Jumping", order = 0)]
    public class IsJumpingConditionSo : AbstractConditionSo
    {
        public override ICondition Create(IModuleOwner owner) => new IsJumpingCondition(owner, Type, IsNot);
    }
}
