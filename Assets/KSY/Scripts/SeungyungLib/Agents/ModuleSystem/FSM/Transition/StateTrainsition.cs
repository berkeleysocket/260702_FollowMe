using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;

using System;

namespace SeungyungLib.FSM
{
    public class Transition : IDisposable
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
                isSuccess &= condition?.Check() ?? false;

            return isSuccess;
        }

        
        public void Dispose()
        {
            foreach(ICondition condition in _conditions)
                condition?.Dispose();
        }
    }
}