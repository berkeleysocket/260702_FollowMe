using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsFall" + nameof(ConditionSO), menuName = "SeungyungLib/FSM/" + nameof(ConditionSO) + "/Is Fall", order = 0)]
    public class IsFallConditionSO : ConditionSO
    {
        protected override ICondition OnCreate(IModuleOwner owner)
        {
            return new IsFallCondition(owner, Type, IsNot);
        }
    }
}
