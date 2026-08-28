using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "TimerConditionSo", menuName = "SeungyungLib/FSM/ConditionSo/Timer", order = 0)]
    public class TimerConditionSo : AbstractConditionSo
    {
        [SerializeField] private float seconds;
        
        public override ICondition Create(IModuleOwner owner)=> new TimerCondition(owner, IsNot, seconds);
    }
}