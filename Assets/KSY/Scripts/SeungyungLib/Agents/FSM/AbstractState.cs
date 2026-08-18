using SeungyungLib.Agents.FSM.Interface;
using SeungyungLib.Agents.ModuleSystem.Interface;
using UnityEngine;

namespace SeungyungLib.Agents.FSM
{
    public abstract class AbstractState : IState
    {
        protected IModuleOwner _owner;
        protected IAgentMovementModule _movementModule;
        protected IAgentRenderModule _renderModule;
        private readonly int _animationHash;
        
        public AbstractState(IModuleOwner owner, int animationHash)
        {
            this._owner = owner;
            this._movementModule = owner.GetModule<IAgentMovementModule>();
            this._renderModule = owner.GetModule<IAgentRenderModule>();
            this._animationHash = animationHash;
        }

        public virtual void Enter()
        {
            _renderModule.PlayClip(_animationHash, 0f, 0.25f);
        }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}