using System;

namespace SeungyungLib.ModuleSystem.Enum
{
    [Flags]
    public enum ColliderModuleOption : byte
    {
        None = 0,
        Collision,
        Trigger,
        Enter,
        Stay,
        Exit
    }
}
