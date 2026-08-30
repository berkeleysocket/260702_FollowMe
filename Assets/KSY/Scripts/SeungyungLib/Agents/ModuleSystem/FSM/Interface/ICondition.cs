using System;

namespace SeungyungLib.FSM.Interface
{
    public interface ICondition : IDisposable
    {
        public bool Check();
    }
}
