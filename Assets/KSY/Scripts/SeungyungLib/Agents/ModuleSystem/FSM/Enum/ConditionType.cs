using System;

namespace SeungyungLib.FSM.Enum
{
    [Flags]
    public enum ConditionType : int
    {
        None = 0,
        IsExpired,
        IsFall,
        IsGrounded,
        IsHit,
        IsJumping,
        IsMoving
    }
}
