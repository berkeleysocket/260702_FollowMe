using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;

using System;

namespace SeungyungLib.FSM
{
    public class Transition : ITransition, IDisposable
    {
        public StateType TransitionTarget { get; private set; }

        private readonly ICondition[] _conditions;
        
        public Transition(IStateMachineModule stateMachineModule, StateType transitionTarget, ConditionType conditionFlags)
        {
            this.TransitionTarget = transitionTarget;
            _conditions = stateMachineModule.GetConditionInstances(conditionFlags);
        }

        public bool Check()
        {
            bool isSuccess = true;
            
            foreach (ICondition condition in _conditions)
                isSuccess &= condition?.Check() ?? false;

            return isSuccess;
        }

        public void Dispose()
        {
            foreach (ICondition condition in _conditions)
                condition.Dispose();
        }
    }
}