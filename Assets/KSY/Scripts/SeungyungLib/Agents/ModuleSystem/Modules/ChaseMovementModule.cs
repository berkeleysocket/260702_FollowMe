using System;
using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.Template.Modules
{
    public class ChaseMovementModule : IMovementModule
    {
        public void Initialize(IModuleOwner owner)
        {
        }

        public bool IsJumpKeyPressed { get; set; }
        public event Action<float> OnChangeAxis;
        public float Axis { get; }
        public bool IsMoving { get; }
        public bool IsJumping { get; }
        public bool IsFall { get; }
        
        public void MoveToDirection(float axis)
        {
            throw new NotImplementedException();
        }
    }
}
