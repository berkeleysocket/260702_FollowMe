using SeungyungLib.Agents.ModuleSystem.Interface;

namespace SeungyungLib.Agents.FSM.Interface
{   
    public interface ICondition
    {
        public void Initialize(IModuleOwner owner);
        public bool CheckCondition();
    }
}