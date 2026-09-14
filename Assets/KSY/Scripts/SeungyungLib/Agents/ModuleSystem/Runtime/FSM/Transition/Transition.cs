using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;

namespace SeungyungLib.FSM
{
    public class Transition : ITransition
    {
        public StateType TransitionTarget { get; private set; }
        
        private readonly ICondition[] _conditions;
        
        public Transition(StateType transitionTarget, ICondition[] conditions)
        {
            this.TransitionTarget = transitionTarget;
            this._conditions = conditions;
        }
        
        public bool ConditionCheck()
        {
            bool conditionCheck = true;
            foreach (ICondition condition in _conditions)
                conditionCheck &= condition.Check();
            return conditionCheck;
        }
    }
}