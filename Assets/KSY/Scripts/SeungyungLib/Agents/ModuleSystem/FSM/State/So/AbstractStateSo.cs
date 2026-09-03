using System.Linq;
using SeungyungLib.Core.FlyweightService;
using SeungyungLib.Core.ParameterSO;
using SeungyungLib.Core.ReadOnlyAttribute;
using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    public abstract class AbstractStateSo : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public StateType Type { get; private set; }
        [field: SerializeField] public AnimParamSO EnterAnimParam { get; private set; }
        [field: SerializeField] public TransitionSO[] Transitions { get; private set; }

        #region Unity Events
        private void OnValidate()
        {
            string typeName = this.GetType().Name.Replace("StateSo", "");
            if (System.Enum.TryParse<StateType>(typeName, true, out StateType conditionType))
                Type = conditionType;
        }
        #endregion

        public IState Create(IModuleOwner owner, IFlyweightFactory<ConditionType, ICondition> conditionFactory)
        {
            ITransition[] transitions = Transitions.Select(transition => transition.Create(owner, conditionFactory)).ToArray();
            return OnCreate(owner, EnterAnimParam.Hash, transitions);
        }
        
        protected abstract IState OnCreate(IModuleOwner owner, int enterAnimHash, ITransition[] transitions);
    }
}