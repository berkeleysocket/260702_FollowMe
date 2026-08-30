using System;

namespace SeungyungLib.FSM.Interface
{
    public interface IState : IDisposable
    {
        public void Enter();
        public void Update();
        public void Exit();
    }
}
