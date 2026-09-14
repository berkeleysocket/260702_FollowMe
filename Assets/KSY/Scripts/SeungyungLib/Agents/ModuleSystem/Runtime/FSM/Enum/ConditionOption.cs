using System;

namespace SeungyungLib.FSM.Enum
{
    [Flags]
    public enum ConditionOption : int
    {
        None = 0,
        IsNot
    }
}