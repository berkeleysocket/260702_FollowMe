using SeungyungLib.Core.FlyweightService;
using SeungyungLib.FSM.Enum;

using System;

namespace SeungyungLib.FSM.Interface
{
    public interface ICondition : IDisposable, IFlyweight
    {
        ConditionType Type { get; }
        bool Check();
    }
}
