using System;

namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IAgentMovementModule : IModule
    {
        public bool IsJumping { get; }
        public bool IsFall { get; }
        public bool IsMoving { get; }

        public event Action<float> OnChangeAxis;
        
        public void MoveToDirection(float axis);
    }
}