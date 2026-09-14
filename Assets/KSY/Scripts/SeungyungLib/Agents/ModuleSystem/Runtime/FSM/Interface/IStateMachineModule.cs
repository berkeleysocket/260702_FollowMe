using SeungyungLib.FSM.Enum;

namespace SeungyungLib.ModuleSystem.Core
{
    public interface IStateMachineModule : IModule
    {
        public void Update();

        public void ChangeState(StateType target);
    }
}