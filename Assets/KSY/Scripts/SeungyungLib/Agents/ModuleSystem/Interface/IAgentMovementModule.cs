using System;

namespace SeungyungLib.Agents.ModuleSystem.Interface
{
    public interface IAgentMovementModule : IModule
    {
        public event Action<float> OnChangeAxis;
        public void SetMovementVelocity(float axis);
        public void Jump();
    }
}