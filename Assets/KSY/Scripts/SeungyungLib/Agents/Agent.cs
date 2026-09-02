using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Core;

namespace SeungyungLib.Agents
{
    public class Agent : AbstractModuleOwner
    {
        private IStateMachineModule _stateMachineModule;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _stateMachineModule = GetModule<IStateMachineModule>();
            
            DebugLogger.Assert(_stateMachineModule != null, $"[{this.GetType().Name}]: StateMachineModule is null");
        }

        private void Update()
        {
            _stateMachineModule?.Update();
        }
    }
}