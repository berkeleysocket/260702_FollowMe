using System;

namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IMovementModule : IModule
    {
        event Action<float> OnChangeAxis;
        
        float Axis { get; }
        bool IsMoving { get; }
        bool IsJumping { get; }
        bool IsFall { get; }
        
        void MoveToDirection(float axis);
    }
}