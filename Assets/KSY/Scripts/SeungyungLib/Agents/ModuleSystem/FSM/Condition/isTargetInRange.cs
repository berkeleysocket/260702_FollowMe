using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "isTargetInRangeConditionSo", menuName = "SeungyungLib/FSM/ConditionSo/isTargetInRange", order = 0)]
    public class isTargetInRangeConditionSo : AbstractConditionSo
    {
        public override ICondition Create(IModuleOwner owner)=> new isTargetInRangeCondition(owner, IsNot);
    }
}