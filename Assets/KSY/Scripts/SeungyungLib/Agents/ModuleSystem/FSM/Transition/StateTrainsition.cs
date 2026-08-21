using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;

namespace SeungyungLib.FSM
{
    public class Transition
    {
        public readonly StateType TransitionTarget;
        
        private readonly ICondition[] _conditions;
        
        public Transition(StateType transitionTarget, ICondition[] conditions)
        {
            this.TransitionTarget = transitionTarget;
            this._conditions = conditions;
        }
        
        public bool Check()
        {
            bool isSuccess = true;
            
            foreach (ICondition condition in _conditions)
                isSuccess &= condition.Check();

            return isSuccess;
        }
    }
}