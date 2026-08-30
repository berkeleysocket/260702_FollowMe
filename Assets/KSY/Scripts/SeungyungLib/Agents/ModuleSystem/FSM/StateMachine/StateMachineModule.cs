using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.Core.CustomDebug;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeungyungLib.FSM
{
    public class StateMachineModule : MonoBehaviour, IStateMachineModule
    {
        [SerializeField] private StateMachineSO stateMachineSo;
        
        private Dictionary<StateType, IState> _stateList = new Dictionary<StateType, IState>();
        private IState _currentState;

        public void Initialize(IModuleOwner owner)
        {
            if (stateMachineSo != null)
            {
                this._stateList = stateMachineSo.StateList.ToDictionary(
                    key => (StateType)key.Type,
                    value => (IState)value.Create(
                        (StateMachineModule)this,
                        (IModuleOwner)owner)
                );
                
                ChangeState(stateMachineSo.StartState);
            }
            
            DebugLogger.Assert(stateMachineSo != null, "[StateModule]: stateMachineSO is null");
        }

        public void Update()
        {
            _currentState?.Update();            
        }

        public void ChangeState(StateType stateType)
        {
            DebugLogger.Log("[StateMachineModule]: Changing state: " + stateType.ToString(), Color.yellow);
            if (_stateList.TryGetValue(stateType, out IState state))
            {
                _currentState?.Exit();
                _currentState = state;
                _currentState.Enter();
            }
            else
                DebugLogger.LogError("[StateMachineModule]: State not found: " + stateType.ToString());
        }
    }
}