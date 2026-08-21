using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    public abstract class AbstractCondition : ICondition
    {
        protected bool IsNot { get; private set; }

        public AbstractCondition(IModuleOwner owner, bool isNot)
        {
            this.IsNot = isNot;
        }

        public abstract bool Check();
    }

    public class IsMovingCondition : AbstractCondition
    {
        private readonly IAgentMovementModule _movementModule;
        // private bool _isMoving;
        
        public IsMovingCondition(IModuleOwner owner, bool isNot) : base(owner, isNot)
        {
            _movementModule = owner.GetModule<IAgentMovementModule>();

            // if (_movementModule != null)
            // {
            //     _movementModule.OnChangeAxis += (float axis) =>
            //     {
            //         if (Mathf.Abs(axis) > 0.01f)
            //             _isMoving = !isNot;
            //         else
            //             _isMoving = isNot;
            //     };
            // }
            // else
            //     DebugLogger.LogError("[IsMovingCondition] _movementModule is null");
            DebugLogger.Assert(_movementModule != null, "[IsMovingCondition] _movementModule is null.");
        }

        // public override bool Check() => _isMoving;
        public override bool Check()
        {
            if (IsNot)
                return !_movementModule.IsMoving;
            return _movementModule.IsMoving;
        }
    }

    public class IsJumpingCondition : AbstractCondition
    {
        private readonly IAgentMovementModule _movementModule;
        
        public IsJumpingCondition(IModuleOwner owner, bool isNot) : base(owner, isNot)
        {
            _movementModule = owner.GetModule<IAgentMovementModule>();
            
            DebugLogger.Assert(_movementModule != null, "[IsJumpingCondition] _movementModule is null.");
        }

        public override bool Check()
        {
            if (IsNot)
                return !_movementModule.IsJumping;
            return _movementModule.IsJumping;
        }
    }

    public class IsGroundCondition : AbstractCondition
    {
        private readonly IAgentGroundCheckModule _groundChecker;

        public IsGroundCondition(IModuleOwner owner, bool isNot) : base(owner, isNot)
        {
            _groundChecker = owner.GetModule<IAgentGroundCheckModule>();
            
            DebugLogger.Assert(_groundChecker != null, "[IsGroundCondition] _groundChecker is null.");
        }
        
        public override bool Check()
        {
            if (IsNot)
                return !_groundChecker.NotifyIsGround.Value;
            return _groundChecker.NotifyIsGround.Value;
        }
    }

    public class IsFallCondition : AbstractCondition
    {
        private readonly IAgentMovementModule _movementModule;

        public IsFallCondition(IModuleOwner owner, bool isNot) : base(owner, isNot)
        {
            _movementModule = owner.GetModule<IAgentMovementModule>();
            
            DebugLogger.Assert(_movementModule != null, "[IsFallCondition] _movementModule is null.");
        }
        
        public override bool Check()
        {
            if (IsNot)
                return !_movementModule.IsFall;
            return _movementModule.IsFall;
        }
    }
}
