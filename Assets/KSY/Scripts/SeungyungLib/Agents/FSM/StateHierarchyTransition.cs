using SeungyungLib.Agents.FSM.Enum;
using SeungyungLib.Agents.FSM.Interface;
using SeungyungLib.Agents.ModuleSystem.Interface;

using System;
using UnityEngine;

namespace SeungyungLib.Agents.FSM
{
    [Serializable]
    public class StateHierarchyTransition
    {
        [SerializeReference] private StateHierarchyType transitionStateHierarchy;
        [SerializeReference] private ICondition condition;
        private IModuleOwner _owner;

        public void Initialize(IModuleOwner owner)
        {
            this._owner = owner;
        }

        public StateHierarchyType Try()
        {
            // if (condition.CheckCondition(_owner))
                return transitionStateHierarchy;

            return StateHierarchyType.None;
        }
    }
}