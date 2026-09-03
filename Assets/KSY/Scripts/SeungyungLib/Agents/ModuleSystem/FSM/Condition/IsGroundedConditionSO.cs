using SeungyungLib.Core.FlyweightService;
using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsGrounded" + nameof(ConditionSO), menuName = "SeungyungLib/FSM/" + nameof(ConditionSO) + "/Is Grounded", order = 0)]
    public class IsGroundedConditionSO : ConditionSO
    {
        public override ICondition Create(IModuleOwner owner, IFlyweightFactory<ConditionType, ICondition> factory)
        {
            throw new System.NotImplementedException();
        }
    }
}
