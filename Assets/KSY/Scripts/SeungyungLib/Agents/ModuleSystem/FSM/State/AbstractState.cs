using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;
using UnityEngine;

namespace  SeungyungLib.FSM
{
    public abstract class AbstractState : IState
    {
        private readonly IStateMachineModule _stateMachineModule;
        private readonly Transition[] _transitions;
        private readonly IRenderModule _renderModule;
        private readonly int _animationNameHash;

        protected AbstractState(IStateMachineModule stateMachineModule, IRenderModule renderModule, 
            int animationNameHash, Transition[] transitions)
        {
            this._stateMachineModule = stateMachineModule;
            this._animationNameHash = animationNameHash;
            this._renderModule = renderModule;
            this._transitions = transitions;
        }
        
        public void Enter()
        {
            _renderModule.PlayClip(_animationNameHash, 0, 0);
        }
        protected virtual void OnEnter() {}

        public void Update()
        {
            foreach (Transition transition in _transitions)
            {
                if (transition.Check())
                {
                    _stateMachineModule.ChangeState(transition.TransitionTarget);
                    break;
                }
            }
        }
        protected virtual void OnUpdate() {}

        public void Exit()
        {
            OnExit();
        }
        protected virtual void OnExit() {}

        #region IDisposable
        public void Dispose()
        {
            foreach (Transition transition in _transitions)
                transition?.Dispose();
        }
        #endregion

    }
}