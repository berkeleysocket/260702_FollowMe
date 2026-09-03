using System;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using System.Collections.Generic;
using System.Linq;
using SeungyungLib.Core.FlyweightService;
using UnityEngine;

namespace SeungyungLib.FSM
{
    public class StateMachineModule : MonoBehaviour, IStateMachineModule
    {
        [SerializeField] private StateMachineSo stateMachineSo;
        
        public bool IsActive { get; private set; }

        private Dictionary<StateType, IState> _stateList = new Dictionary<StateType, IState>();
        private FlyweightFactory<ConditionType, ICondition> _conditionFactory = new FlyweightFactory<ConditionType, ICondition>();
        private IState _currentState;

        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            if (stateMachineSo != null)
            {
                this._stateList = stateMachineSo.StateList.ToDictionary(
                    key => (StateType)key.Type,
                    value => (IState)value.Create(owner)
                );
                
                ChangeState(stateMachineSo.StartState);
            }
            
            DebugLogger.Assert(stateMachineSo != null, "[StateModule]: stateMachineSO is null");
        }
        #endregion

        #region Unity Events
        public void Update()
        {
            _currentState?.Update();            
        }
        #endregion

        private void CheckCondition()
        {
        }
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
        
        public void ChangeState(StateType stateType)
        {
            DebugLogger.Log("[StateMachineModule]: Changing state: " + stateType.ToString(), Color.yellow);
            if (_stateList.TryGetValue(stateType, out IState state) && state != null)
            {
                _currentState.Exit();
                _currentState = state;
                _currentState.Enter();
            }
            else
                DebugLogger.LogError("[StateMachineModule]: State not found: " + stateType.ToString());
        }
    }
}