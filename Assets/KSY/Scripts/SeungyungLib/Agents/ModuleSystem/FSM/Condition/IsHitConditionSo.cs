using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsHitConditionSo", menuName = "SeungyungLib/FSM/ConditionSo/Is Hit", order = 0)]
    public class IsHitConditionSo : AbstractConditionSo
    {
        [SerializeField] private EventChannelSO playerEvtChannel;
        public override ICondition Create(IModuleOwner owner)=> new IsHitCondition(owner, IsNot, playerEvtChannel);
    }
}