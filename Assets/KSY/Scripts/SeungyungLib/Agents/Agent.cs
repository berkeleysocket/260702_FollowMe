using SeungyungLib.ModuleSystem;
using SeungyungLib.FSM.Interface;

namespace SeungyungLib.Agents
{
    public class Agent : AbstractModuleOwner
    {
        private IStateModule _stateModule;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _stateModule = GetModule<IStateModule>();
        }

        private void Update()
        {
            _stateModule?.Update();
        }
    }
}