using System;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeungyungLib.FSM
{
    public class StateMachineModule : MonoBehaviour, IStateMachineModule
    {
        [SerializeField] private StateMachineSo stateMachineSo;
        
        public bool IsActive { get; private set; }

        private Dictionary<StateType, IState> _stateList = new Dictionary<StateType, IState>();
        private Dictionary<ConditionType, ICondition> _conditionInstances = new Dictionary<ConditionType, ICondition>();
        private IState _currentState;

        #region Initialization
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
        #endregion

        #region Unity Events
        public void Update()
        {
            _currentState?.Update();            
        }
        
        private void OnDestroy()
        {
            foreach (IState state in _stateList.Values)
                state?.Dispose();
        }
        #endregion
        
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

        public ICondition[] GetConditionInstances(ConditionType needConditions)
        {
            foreach (ConditionType key in _conditionInstances.Keys)
            {
                if ((key & needConditions) == 1)
                {
                    
                }
            }
        }
    }
}