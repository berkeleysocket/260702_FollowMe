using UnityEngine;

namespace SeungyungLib.ModuleSystem.Core
{
    public interface IControllableMovementModule : IMovementModule
    {
        bool IsControlling => IsActive && Axis != 0; 
        bool IsJumpKeyPressed { get; set; }
    }
}
