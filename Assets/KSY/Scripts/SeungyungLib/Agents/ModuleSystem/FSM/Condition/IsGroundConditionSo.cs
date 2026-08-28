using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsGroundConditionSo", menuName = "SeungyungLib/FSM/ConditionSo/Is Ground", order = 0)]
    public class IsGroundConditionSo : AbstractConditionSo
    {
        public override ICondition Create(IModuleOwner owner) => new IsGroundCondition(owner, IsNot);
    }
}
