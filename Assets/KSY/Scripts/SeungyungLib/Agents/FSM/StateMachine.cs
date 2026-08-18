using SeungyungLib.Agents.FSM.Interface;
using SeungyungLib.Agents.FSM;
using SeungyungLib.Agents.ModuleSystem.Interface;
using UnityEngine;

namespace SeungyungLib.FSM
{
    public class StateMachine : IStateMachine
    {
        private StateOwnerDataSO stateOwnerData;

        private IStateHierarchy CurrentHierarchy
        {
            get
            {
                return _currentHierarchy;
            }
            set
            {
                if(_currentHierarchy != value)
                    _currentHierarchy.Exit();
                _currentHierarchy = value;
            }
        }
        private IStateHierarchy[] _stateHierarchies;
        private IStateHierarchy _currentHierarchy;

        public void Initialize(IModuleOwner stateOwner, StateOwnerDataSO stateOwnerDataSO)
        {
            this.stateOwnerData = stateOwnerDataSO;
            int hierarchyCount = stateOwnerData.StateHierarchies.Length;
            this._stateHierarchies = new IStateHierarchy[hierarchyCount];
            for (int i = 0; i < hierarchyCount; i++)
            {
                IStateHierarchy hierarchy = _stateHierarchies[i] = stateOwnerData.StateHierarchies[i].HierarchyInstance;
                hierarchy?.Initialize(stateOwner);
            }

            _currentHierarchy = _stateHierarchies[stateOwnerData.StartHierarchyIndex];
        }
    }
}