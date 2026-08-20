using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsMovingConditionSO", menuName = "SeungyungLib/FSM/Condition/IsMoving", order = 0)]
    public class IsMovingConditionSO : AbstractConditionSO
    {
        public override ICondition Create(IModuleOwner owner)=> new IsMovingCondition(owner, IsNot);
    }
}