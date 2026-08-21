using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using System.Linq;
using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "TransitionSo",menuName = "SeungyungLib/FSM/TransitionSo", order = 2)]
    public class TransitionSo : ScriptableObject
    {
        [field: SerializeField] public StateType TransitionTarget { get; private set; }
        [field: SerializeField] public AbstractConditionSo[] Conditions { get; private set; }

        public Transition Create(IModuleOwner owner)
        {
            ICondition[] conditions = Conditions.Select(condition=>condition.Create(owner)).ToArray();
            return new Transition(TransitionTarget, conditions);
        }
    }
}
