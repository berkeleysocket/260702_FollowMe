using SeungyungLib.FSM.Enum;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "TransitionSO",menuName = "SeungyungLib/FSM/TransitionSO", order = 2)]
    public class TransitionSO : ScriptableObject
    {
        [field: SerializeField] public StateType TransitionTarget { get; private set; }
        [field: SerializeField] public AbstractConditionSO AbstractConditionSo { get; private set; }

        public Transition Create(IModuleOwner owner)
        {
            return new Transition(TransitionTarget, AbstractConditionSo.Create(owner));
        }
    }
}
