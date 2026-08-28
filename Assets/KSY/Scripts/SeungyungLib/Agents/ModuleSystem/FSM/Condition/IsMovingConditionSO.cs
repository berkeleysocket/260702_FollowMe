using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsMovingConditionSo", menuName = "SeungyungLib/FSM/ConditionSo/Is Moving", order = 0)]
    public class IsMovingConditionSo : AbstractConditionSo
    {
        public override ICondition Create(IModuleOwner owner)=> new IsMovingCondition(owner, IsNot);
    }
}