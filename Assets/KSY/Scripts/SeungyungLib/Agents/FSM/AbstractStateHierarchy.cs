using SeungyungLib.Agents.FSM.Enum;
using SeungyungLib.Agents.FSM.Interface;
using SeungyungLib.Agents.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.Agents.FSM
{
    public class AbstractStateHierarchy : IStateHierarchy
    {
        [SerializeReference] private StateHierarchyTransition[] transitions;
        [SerializeReference] private StateTransition[] transition;
        
        private IState _currentState;

        public void Initialize(IModuleOwner stateOwner)
        {
            foreach (var transition in transitions)
                transition.Initialize(stateOwner);
        }

        public StateHierarchyType CheckCondition()
        {
            StateHierarchyType stateHierarchy = StateHierarchyType.None;
            foreach (var transition in transitions)
            {
                stateHierarchy = transition.Try();
                if (stateHierarchy != StateHierarchyType.None) break;
            }
            return stateHierarchy;
        }

        public void Enter()
        {
            _currentState?.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void Exit()
        {
            _currentState?.Exit();
        }
    }
}