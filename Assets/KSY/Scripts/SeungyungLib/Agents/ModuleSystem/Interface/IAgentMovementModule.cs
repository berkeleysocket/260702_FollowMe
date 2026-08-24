using System;

namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IAgentMovementModule : IModule
    {
        public bool IsJumpKeyPressed { get; set; }

        public event Action<float> OnChangeAxis;
        
        public float Axis { get; }
        public bool IsMoving { get; }
        public bool IsJumping { get; }
        public bool IsFall { get; }
        
        public void MoveToDirection(float axis);
    }
}