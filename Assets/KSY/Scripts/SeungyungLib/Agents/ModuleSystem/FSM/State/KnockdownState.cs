using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class KnockdownState : AbstractState
    {
        private readonly IMovementModule _movementModule;
        
        public KnockdownState(IStateMachineModule stateMachineModule, IModuleOwner owner, int animationNameHash, Transition[] transitions) : base(stateMachineModule, owner, animationNameHash, transitions)
        {
            this._movementModule = owner.GetModule<IMovementModule>();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            
            DebugLogger.Log("OnEnter KnockdownState");
            _movementModule.Deactivate();
        }

        protected override void OnExit()
        {
            base.OnExit();
            
            DebugLogger.Log("OnExit KnockdownState");
            _movementModule.Activate();
        }
    }
}