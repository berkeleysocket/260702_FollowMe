using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.FSM
{
    public class HitState : AbstractState
    {
        public HitState(IStateMachineModule stateMachineModule, IModuleOwner owner, int animationNameHash, Transition[] transitions) : base(stateMachineModule, owner, animationNameHash, transitions)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            DebugLogger.Log("Hit Enter!!!!!!");
        }
    }
}