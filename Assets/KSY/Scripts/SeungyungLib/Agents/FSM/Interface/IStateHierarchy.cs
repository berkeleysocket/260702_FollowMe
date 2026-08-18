using SeungyungLib.Agents.FSM.Enum;
using SeungyungLib.Agents.ModuleSystem.Interface;

namespace SeungyungLib.Agents.FSM.Interface
{
    public interface IStateHierarchy
    {
        public void Initialize(IModuleOwner stateOwner);
        public StateHierarchyType CheckCondition();
        public void Enter();
        public void Update();
        public void Exit();
    }
}