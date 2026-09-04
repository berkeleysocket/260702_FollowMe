using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

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


        protected override void OnEnter()
        {
            base.OnEnter();
            DebugLogger.Log("RunState OnEnter!!!");
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
        public KnockdownState(IModuleOwner owner, int enterAnimHash, ITransition[] transitions) : base(owner, enterAnimHash, transitions)
        {
        }
    }
    
    public class HitState : AbstractState
    {
        public HitState(IModuleOwner owner, int enterAnimHash, ITransition[] transitions) : base(owner, enterAnimHash, transitions)
        {
        }
    }
}