using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem;
using SeungyungLib.FSM.Interface;

namespace SeungyungLib.Agents
{
    public class Agent : AbstractModuleOwner
    {
        private IStateMachineModule _stateMachineModule;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _stateMachineModule = GetModule<IStateMachineModule>();
            
            DebugLogger.Assert(_stateMachineModule != null, $"[{this.GetType().Name}]: State Machine Module is null]");
        }

        private void Update()
        {
            _stateMachineModule?.Update();
        }
    }
}