using SeungyungLib.Agents.FSM.Interface;
using SeungyungLib.Agents.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.Agents.FSM
{
    public abstract class AbstractCondition : ICondition
    {
        [SerializeField] private bool isNot = false;
        protected IModuleOwner _owner;

        public void Initialize(IModuleOwner owner)
        {
            _owner = owner;
            OnInitialize();
        }
        public bool CheckCondition()
        {
            return isNot ^ HandleCheckCondition();
        }
        
        protected abstract void OnInitialize();
        protected abstract bool HandleCheckCondition(); 
    }
}