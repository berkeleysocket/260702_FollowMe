using SeungyungLib.Agents.FSM.Interface;

using System;
using SeungyungLib.Agents.ModuleSystem.Interface;
using UnityEngine;

namespace SeungyungLib.Agents.FSM
{
    [Serializable]
    public class StateTransition
    {
        [SerializeField] private StateDataSO transitionState;
        [SerializeReference] private ICondition[] conditions;
        private IModuleOwner _owner;

        public void Initialize(IModuleOwner owner)
        {
            this._owner = owner;
        }

        public IState Try()
        {
            foreach(ICondition condition in conditions)
            {
                // if (condition.CheckCondition(_owner))
                    return transitionState.State;
            }

            return null;
        }
    }
}