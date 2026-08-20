using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeungyungLib.FSM
{
    public class StateModule : MonoBehaviour, IStateModule
    {
        [SerializeField] private StateMachineSO stateMachineSO;
        
        private Dictionary<StateType, IState> _stateList = new Dictionary<StateType, IState>();
        private IState _currentState;

        public void Initialize(IModuleOwner owner)
        {
            this._stateList = stateMachineSO.StateList.ToDictionary(
                key => (StateType)key.Type,
                value => (IState)value.Create(
                    (StateModule)this,
                    (IModuleOwner)owner)
                );

            ChangeState(stateMachineSO.StartState);
        }

        public void Update()
        {
            _currentState?.Update();            
        }

        public void ChangeState(StateType stateType)
        {
            if (_stateList.TryGetValue(stateType, out IState state))
            {
                _currentState?.Exit();
                _currentState = state;
                _currentState.Enter();
            }
        }
    }
}