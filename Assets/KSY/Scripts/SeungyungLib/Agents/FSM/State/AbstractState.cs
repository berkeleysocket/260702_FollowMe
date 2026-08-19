using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;
using UnityEngine;

namespace  SeungyungLib.FSM
{
    public abstract class AbstractState : IState
    {
        private readonly IStateModule _stateModule;
        private readonly Transition[] _transitions;
        private readonly IAgentRenderModule _renderModule;
        private readonly int _animationNameHash;

        public AbstractState(IStateModule stateModule, IAgentRenderModule renderModule, 
            int animationNameHash, Transition[] transitions)
        {
            this._stateModule = stateModule;
            this._animationNameHash = animationNameHash;
            this._renderModule = renderModule;
            this._transitions = transitions;
        }
        
        public void Enter()
        {
            _renderModule.PlayClip(_animationNameHash, 0, 0);
        }
        public virtual void OnEnter() {}

        public void Update()
        {
            foreach (Transition transition in _transitions)
            {
                if (transition.Condition.Check())
                {
                    _stateModule.ChangeState(transition.TransitionTarget);
                    break;
                }
            }
        }
        public virtual void OnUpdate() {}

        public void Exit()
        {
            OnExit();
        }
        public virtual void OnExit() {}
    }
}