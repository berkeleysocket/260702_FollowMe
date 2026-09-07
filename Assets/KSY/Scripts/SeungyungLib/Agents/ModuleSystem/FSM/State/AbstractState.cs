using SeungyungLib.FSM.Interface;
using SeungyungLib.Core.Timer;
using SeungyungLib.ModuleSystem.Core;

namespace  SeungyungLib.FSM
{
    public abstract class AbstractState : IState
    {
        public ITransition[] Transitions { get; }
        
        private readonly IStateMachineModule _stateMachineModule;
        private readonly IRenderModule _renderModule;
        private readonly int _enterAnimHash;
        
        protected AbstractState( 
            IModuleOwner owner, 
            int enterAnimHash, 
            ITransition[] transitions)
        {
            this._stateMachineModule = owner.GetModule<IStateMachineModule>();
            this._renderModule = owner.GetModule<IRenderModule>();
            this._enterAnimHash = enterAnimHash;
            this.Transitions = transitions;
        }
        
        public void Enter()
        {
            _renderModule.PlayClip(_enterAnimHash, 0f, 0f, 0f);
            
            OnEnter();
        }
        protected virtual void OnEnter() {}

        public void Update()
        {
            foreach (ITransition transition in Transitions)
            {
                if (transition.ConditionCheck())
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
    }

    public class IdleState : AbstractState
    {
        public IdleState(IModuleOwner owner, int enterAnimHash, ITransition[] transitions) : base(owner, enterAnimHash, transitions)
        {
        }
    }
    
    public class RunState : AbstractState
    {
        public RunState(IModuleOwner owner, int enterAnimHash, ITransition[] transitions) : base(owner, enterAnimHash, transitions)
        {
        }
    }
    
    public class JumpState : AbstractState
    {
        public JumpState(IModuleOwner owner, int enterAnimHash, ITransition[] transitions) : base(owner, enterAnimHash, transitions)
        {
        }
    }
    
    public class FallState : AbstractState
    {
        public FallState(IModuleOwner owner, int enterAnimHash, ITransition[] transitions) : base(owner, enterAnimHash, transitions)
        {
        }
    }
    
    public class KnockdownState : AbstractState
    {
        private readonly IMovementModule _movementModule;
        private readonly IRenderModule _renderModule;
        private readonly UnityTimer _timer = new UnityTimer();
        private readonly IBodyModule _body;
        private readonly float _exitTime;
        
        public KnockdownState(IModuleOwner owner, int enterAnimHash, ITransition[] transitions, float exitTime) : base(owner, enterAnimHash, transitions)
        {
            this._movementModule = owner.GetModule<IMovementModule>();
            this._renderModule = owner.GetModule<IRenderModule>();
            this._body = owner.GetModule<IBodyModule>();
            this._exitTime = exitTime;
        }

        protected override void OnEnter()
        {
            _movementModule.Deactivate();
            _timer.Initialize(_exitTime);
            _timer.Start();
        }

        protected override void OnUpdate()
        {
            if (_timer.Check())
                _body.Recovery();
        }

        protected override void OnExit()
        {
            _movementModule.Activate();
        }
    }
    
    public class HitState : AbstractState
    {
        private readonly UnityTimer _timer = new UnityTimer();
        private readonly IBodyModule _body;
        private readonly float _exitTime;
        
        public HitState(IModuleOwner owner, int enterAnimHash, ITransition[] transitions, float exitTime) : base(owner, enterAnimHash, transitions)
        {
            this._exitTime = exitTime;
            this._body = owner.GetModule<IBodyModule>();
        }

        protected override void OnEnter()
        {
            _timer.Initialize(_exitTime);
            _timer.Start();
        }

        protected override void OnUpdate()
        {
            if (_timer.Check() && !_body.IsKnockdown)
                _body.Knockdown();
        }
    }
}