using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsMoving" + nameof(ConditionSO), menuName = "SeungyungLib/FSM/" + nameof(ConditionSO) + "/Is Moving", order = 0)]
    public class IsMovingConditionSO : ConditionSO
    {
        protected override ICondition OnCreate(IModuleOwner owner)
        {
            return new IsMovingCondition(owner, Type, IsNot);
        }
    }
}