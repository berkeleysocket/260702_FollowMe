using System.Reflection;
using SeungyungLib.Core.FlyweightService;
using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "IsFall" + nameof(ConditionSO), menuName = $"SeungyungLib/FSM/nameof(ConditionSO)/Is Fall", order = 0)]
    public class IsFallConditionSO : ConditionSO
    {
        public static string asdasd = "as";
        public override ICondition Create(IModuleOwner owner, IFlyweightFactory<ConditionType, ICondition> factory)
        {
            throw new System.NotImplementedException();
        }
    }
}
