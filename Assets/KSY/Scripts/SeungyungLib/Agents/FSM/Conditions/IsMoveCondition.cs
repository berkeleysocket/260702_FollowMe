using SeungyungLib.Agents.ModuleSystem.Interface;

using System;

namespace SeungyungLib.Agents.FSM
{
    [Serializable]
    public class IsMoveCondition : AbstractCondition
    {
        private IAgentMovementModule _movementModule;

        protected override void OnInitialize()  
        {
            this._movementModule = _owner.GetModule<IAgentMovementModule>();
        }

        protected override bool HandleCheckCondition()
        {
            throw new NotImplementedException();
        }
    }
}
