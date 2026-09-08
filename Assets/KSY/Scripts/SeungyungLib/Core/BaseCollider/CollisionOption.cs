using System;

namespace SeungyungLib.Core.BaseCollider
{
    [Flags]
    public enum CollisionOption : byte
    {
        None = 0,
        Collision,
        Trigger,
        Enter,
        Stay,
        Exit
    }
}
