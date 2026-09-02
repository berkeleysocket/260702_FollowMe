using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace  SeungyungLib.FSM
{
    public abstract class AbstractState : IState
    {
        public ITransition[] Transitions => _transitions; 
        
        private readonly IStateMachineModule _stateMachineModule;
        private readonly ITransition[] _transitions;
        private readonly IRenderModule _renderModule;
        private readonly int _animationNameHash;
        
        protected AbstractState(IStateMachineModule stateMachineModule, 
            IModuleOwner owner, 
            int animationNameHash, 
            ITransition[] transitions)
        {
            this._stateMachineModule = stateMachineModule;
            this._animationNameHash = animationNameHash;
            this._transitions = transitions;
            
            this._renderModule = owner.GetModule<IRenderModule>();
        }
        
        public void Enter()
        {
            _renderModule.PlayClip(_animationNameHash, 0f, 0f, 0f);

            OnEnter();
        }
        protected virtual void OnEnter() {}

        public void Update()
        {
            foreach (ITransition transition in _transitions)
            {
                if (transition.Check())
                {
                    _stateMachineModule.ChangeState(transition.TransitionTarget);
                    break;
                }
            }

            OnUpdate();
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