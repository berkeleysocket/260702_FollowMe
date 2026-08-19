using System;

namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IAgentMovementModule : IModule
    {
        public event Action<float> OnChangeAxis;
        public void SetMovementVelocity(float axis);
        public void Jump();
    }
}