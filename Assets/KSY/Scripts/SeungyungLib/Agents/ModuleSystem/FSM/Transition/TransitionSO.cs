using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using SeungyungLib.Core.FlyweightService;

using System.Linq;
using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "TransitionSO",menuName = "SeungyungLib/FSM/TransitionSO", order = 2)]
    public class TransitionSO : ScriptableObject
    {
        [field: SerializeField] public StateType TransitionTarget { get; private set; }
        [field: SerializeField] public ConditionSO[] Conditions { get; private set; }

        public Transition Create(IModuleOwner owner, IFlyweightFactory<ConditionType, ICondition> conditionFactory)
        {
            ICondition[] conditions = Conditions.Select(condition => condition.Create(owner)).ToArray();
            return new Transition(TransitionTarget, conditions);
        }
    }
}
