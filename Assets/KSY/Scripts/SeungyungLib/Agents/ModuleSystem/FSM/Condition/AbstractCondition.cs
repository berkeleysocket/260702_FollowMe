using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.Template.EventChannels;

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
        private readonly IMovementModule _movementModule;
        
        public IsMovingCondition(IModuleOwner owner, bool isNot) : base(owner, isNot)
        {
            _movementModule = owner.GetModule<IMovementModule>();
            
            DebugLogger.Assert(_movementModule != null, "[IsMovingCondition] _movementModule is null.");
        }

        public override bool Check()
        {
            if (IsNot)
                return !_movementModule.IsMoving;
            return _movementModule.IsMoving;
        }
    }

    public class IsJumpingCondition : AbstractCondition
    {
        private readonly IMovementModule _movementModule;
        
        public IsJumpingCondition(IModuleOwner owner, bool isNot) : base(owner, isNot)
        {
            _movementModule = owner.GetModule<IMovementModule>();
            
            DebugLogger.Assert(_movementModule != null, "[IsJumpingCondition] _movementModule is null.");
        }

        public override bool Check()
        {
            if (IsNot)
                return !_movementModule.IsJumping;
            return _movementModule.IsJumping;
        }
    }

    public class IsGroundedCondition : AbstractCondition
    {
        private readonly IGroundCheckModule _groundChecker;

        public IsGroundedCondition(IModuleOwner owner, bool isNot) : base(owner, isNot)
        {
            _groundChecker = owner.GetModule<IGroundCheckModule>();
            
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
        private readonly IMovementModule _movementModule;

        public IsFallCondition(IModuleOwner owner, bool isNot) : base(owner, isNot)
        {
            _movementModule = owner.GetModule<IMovementModule>();
            
            DebugLogger.Assert(_movementModule != null, "[IsFallCondition] _movementModule is null.");
        }
        
        public override bool Check()
        {
            if (IsNot)
                return !_movementModule.IsFall;
            return _movementModule.IsFall;
        }
    }
    
    public class IsHitCondition : AbstractCondition
    {
        private bool _isHit = false;
        
        public IsHitCondition(IModuleOwner owner, bool isNot, EventChannelSO playerEvtChannel) : base(owner, isNot)
        {
            playerEvtChannel.AddListener<PlayerHitEvent>(HandlePlayerHitEvent);
        }

        public override bool Check()
        {
            if (_isHit)
            {
                _isHit = false;
                return true;
            }
            
            return false;
        }

        private void HandlePlayerHitEvent(PlayerHitEvent playerHitEvent) => _isHit = true;
    }

    public class IsExpiredCondition : AbstractCondition
    {
        private readonly float _duration;
        private float _startTime = -1f;
        
        public IsExpiredCondition(IModuleOwner owner, bool isNot, float duration) : base(owner, isNot)
        {
            this._duration = duration;
        }

        public override bool Check()
        {
            TryStartTimer();
            return CheckTimer();
        }

        private void TryStartTimer()
        {
            if (_startTime <= -1f)
                _startTime = Time.time;
        }

        private bool CheckTimer() => Time.time - _startTime >= _duration;
    }
}