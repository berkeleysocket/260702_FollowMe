using System;

namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IMovementModule : IModule
    {
        event Action<int> OnMoved;
        
        int Axis { get; }
        bool IsMoving { get; }
        bool IsJumping { get; }
        bool IsFall { get; }
        
        void MoveToDirection(int axis);
    }
}