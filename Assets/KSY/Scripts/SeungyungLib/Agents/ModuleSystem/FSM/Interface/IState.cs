using System;

namespace SeungyungLib.FSM.Interface
{
    public interface IState : IDisposable
    {
        ITransition[] Transitions { get; }
        
        public void Enter();
        public void Update();
        public void Exit();
    }
}
