using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsExpiredSo", menuName = "SeungyungLib/FSM/ConditionSo/Is Expired", order = 0)]
    public class IsExpiredConditionSo : ConditionSO
    {
        [SerializeField] private float seconds;
        
        public override ICondition Create(IModuleOwner owner) => new IsExpiredCondition(owner, Type, IsNot, seconds);
    }
}