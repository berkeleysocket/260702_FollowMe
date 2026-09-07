using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Core;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsHit" + nameof(ConditionSO), menuName = "SeungyungLib/FSM/" + nameof(ConditionSO) + "/Is Hit", order = 0)]
    public class IsHitConditionSO : ConditionSO
    {
        protected override ICondition OnCreate(IModuleOwner owner)
        {
            return new IsHitCondition(owner, Type, IsNot);
        }
    }
}