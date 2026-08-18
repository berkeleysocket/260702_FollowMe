using SeungyungLib.Agents.FSM;
using SeungyungLib.Agents.ModuleSystem;
using SeungyungLib.FSM;

using UnityEngine;

namespace SeungyungLib.Agents
{
    public class Agent : AbstractModuleOwner
    {
        [SerializeField] private StateOwnerDataSO stateOwner;
        private StateMachine _stateMachine;

        protected override void OnAwake()
        {
            _stateMachine = new StateMachine();
            _stateMachine.Initialize(this, stateOwner);
        }
    }
}