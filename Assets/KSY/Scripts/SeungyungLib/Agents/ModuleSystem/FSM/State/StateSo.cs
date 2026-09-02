using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.ParameterSO;
using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using System;
using System.Linq;
using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "StateSo",menuName = "SeungyungLib/FSM/StateSo", order = 1)]
    public class StateSo : ScriptableObject
    {
        [field: SerializeField] public StateType Type { get; private set; }
        [field: SerializeField] public AnimParamSO AnimationHash { get; private set; }
        [field: SerializeField] public TransitionSo[] StateTransitions { get; private set; }

        private void OnValidate()
        {
            if (Type == StateType.None)
                DebugLogger.LogError($"[StateSO]: {this.name}'s Type is none.");
            if (AnimationHash == null)
                DebugLogger.LogError($"[StateSO]: {this.name}'s AnimationHash is null.");
            if (StateTransitions == null || StateTransitions.Length == 0)
                DebugLogger.LogError($"[StateSO]: {this.name}'s StateTransitions is null or empty.");
        }

        public IState Create(IStateMachineModule stateMachineModule, IModuleOwner owner)
        {
            if (stateMachineModule == null)
            {
                DebugLogger.LogError($"[StateSO]: {this.name}'s StateMachine is null.");
                return null;
            }
            if (owner == null)
            {
                DebugLogger.LogError($"[StateSO]: {this.name}'s Owner is null.");
                return null;
            }
            if (StateTransitions == null)
            {
                DebugLogger.LogError($"[StateSO]: {this.name}'s StateTransitions is null.");
                return null;
            }
            if (AnimationHash == null)
            {
                DebugLogger.LogError($"[StateSO]: {this.name}'s AnimationHash is null.");
                return null;
            }
            
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .FirstOrDefault(t =>
                typeof(IState).IsAssignableFrom(t)
                  && !t.IsAbstract
                  && !t.IsInterface
                  && t.Name.Replace("State", "") == Type.ToString());
            
            if (type == null) DebugLogger.LogError($"[StateSO]: {Type.ToString() + "State"} is not found.");

            Transition[] transitions = StateTransitions.Select(x=> x.Create(owner)).ToArray();

            if (transitions.Length == 0)
            {
                DebugLogger.LogError($"[StateSO]: {this.name}'s Transitions is null.");
                return null;
            }
            
            IState state = Activator.CreateInstance(
                type, 
                stateMachineModule, 
                owner, 
                AnimationHash.Hash, 
                transitions) 
                as IState;
            
            return state;
        }
    }
}