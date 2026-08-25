using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsFallConditionSo", menuName = "SeungyungLib/FSM/ConditionSo/IsFall", order = 0)]
    public class IsFallConditionSo : AbstractConditionSo
    {
        public override ICondition Create(IModuleOwner owner) => new IsFallCondition(owner, IsNot);
    }
}
