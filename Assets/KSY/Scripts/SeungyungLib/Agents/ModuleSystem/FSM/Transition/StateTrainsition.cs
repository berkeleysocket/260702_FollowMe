using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;

namespace SeungyungLib.FSM
{
    public class Transition
    {
        public Transition(StateType transitionTarget, ICondition condition)
        {
            this.TransitionTarget = transitionTarget;
            this.Condition = condition;
        }

        public readonly StateType TransitionTarget;
        public readonly ICondition Condition;
    }
}
