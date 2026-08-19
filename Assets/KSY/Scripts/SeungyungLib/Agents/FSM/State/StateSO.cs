using SeungyungLib.Core.ParameterSO;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using System;
using System.Linq;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "StateSO",menuName = "SeungyungLib/FSM/StateSO", order = 1)]
    public class StateSO : ScriptableObject
    {
        [field: SerializeField] public StateType Type { get; private set; }
        [field: SerializeField] public AnimParamSO AnimationHash { get; private set; }
        [field: SerializeField] public TransitionSO[] StateTransitions { get; private set; }

        private void OnValidate()
        {
            if (Type == StateType.None)
                DebugLogger.LogError($"[StateSO]: {this.name}'s Type is none.");
            if (AnimationHash == null)
                DebugLogger.LogError($"[StateSO]: {this.name}'s AnimationHash is null.");
            if (StateTransitions == null || StateTransitions.Length == 0)
                DebugLogger.LogError($"[StateSO]: {this.name}'s StateTransitions is null or empty.");
        }

        public IState Create(IStateModule stateModule, IModuleOwner owner)
        {
            if (stateModule == null)
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
            
            Type t = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .FirstOrDefault(t =>
                typeof(IState).IsAssignableFrom(t)
                  && !t.IsAbstract
                  && !t.IsInterface
                  && t.Name.Replace("State", "") == Type.ToString());
            
            if (t == null) DebugLogger.LogError($"[StateSO]: {Type.ToString() + "State"} is not found.");

            IAgentRenderModule renderModule = owner.GetModule<IAgentRenderModule>();
            Transition[] transitions = StateTransitions.Select(x=> x.Create(owner)).ToArray();

            if (renderModule == null)
            {
                DebugLogger.LogError($"[StateSO]: {this.name}'s This Owner has not RendererModule.");
                return null;
            }
            if (transitions.Length == 0)
            {
                DebugLogger.LogError($"[StateSO]: {this.name}'s Transitions is null.");
                return null;
            }
            
            IState state = Activator.CreateInstance(
                t, 
                stateModule, 
                renderModule, 
                AnimationHash.Hash, 
                transitions) 
                as IState;
            
            return state;
        }
    }
}