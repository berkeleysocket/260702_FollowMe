using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM.Interface
{
    public interface IStateMachineModule : IModule
    {
        public void Update();
    }
}