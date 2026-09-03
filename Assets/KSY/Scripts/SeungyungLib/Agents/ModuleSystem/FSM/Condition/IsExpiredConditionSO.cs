using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsExpired" + nameof(ConditionSO), menuName = "SeungyungLib/FSM/" + nameof(ConditionSO) + "/Is Expired", order = 0)]
    public class IsExpiredConditionSO : ConditionSO, IOptionalConditionSO
    {
        [SerializeField] private float seconds;

        protected override ICondition OnCreate(IModuleOwner owner)
        {
            return new IsExpiredCondition(owner, Type, IsNot, seconds);
        }
    }
}