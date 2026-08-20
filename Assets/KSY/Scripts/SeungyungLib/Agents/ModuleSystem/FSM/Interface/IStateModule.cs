using SeungyungLib.FSM.Enum;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM.Interface
{
    public interface IStateModule : IModule
    {
        public void Update();
        public void ChangeState(StateType stateType);
    }
}