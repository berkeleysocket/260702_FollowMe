using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using System.Collections.Generic;
using System.Linq;
using SeungyungLib.Core.CustomDebug;
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
            if (stateMachineSO != null)
            {
                this._stateList = stateMachineSO.StateList.ToDictionary(
                    key => (StateType)key.Type,
                    value => (IState)value.Create(
                        (StateModule)this,
                        (IModuleOwner)owner)
                );
                
                ChangeState(stateMachineSO.StartState);
            }
            
            DebugLogger.Assert(stateMachineSO != null, "[StateModule]: stateMachineSO is null");
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