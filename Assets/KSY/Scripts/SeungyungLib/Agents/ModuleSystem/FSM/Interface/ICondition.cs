using System;
using SeungyungLib.FSM.Enum;

namespace SeungyungLib.FSM.Interface
{
    public interface ICondition : IDisposable
    {
        ConditionType Type { get; }
        bool Check();
    }
}
