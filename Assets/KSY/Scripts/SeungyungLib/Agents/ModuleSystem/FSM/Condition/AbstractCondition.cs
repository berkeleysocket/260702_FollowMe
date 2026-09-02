using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.ModuleSystem.Modules;

using UnityEngine;

namespace SeungyungLib.FSM
{
    public abstract class AbstractCondition : ICondition
    {
        public ConditionType Type { get; private set; }
        
        private readonly bool _isNot = false;   

        #region Initialization
        protected AbstractCondition(IModuleOwner owner, ConditionType type, bool isNot)
        {
            this.Type = type;
            this._isNot = isNot;
        }
        #endregion

        public bool Check() => OnCheck() ^ _isNot;
        protected abstract bool OnCheck();

        #region IDisposable
        public void Dispose()
        {
            OnDispose();
        }
        protected virtual void OnDispose() {}
        #endregion
    }

    public class IsMovingCondition : AbstractCondition
    {
        private readonly IMovementModule _movementModule;
        
        public IsMovingCondition(IModuleOwner owner, ConditionType type, bool isNot) : base(owner, type, isNot)
        {
            _movementModule = owner.GetModule<IMovementModule>();
            
            DebugLogger.Assert(_movementModule != null, "[IsMovingCondition] _movementModule is null.");
        }

        protected override bool OnCheck() => _movementModule?.IsMoving ?? false;
    }

    public class IsJumpingCondition : AbstractCondition
    {
        private readonly IMovementModule _movementModule;
        
        public IsJumpingCondition(IModuleOwner owner, ConditionType type, bool isNot) : base(owner, type, isNot)
        {
            _movementModule = owner.GetModule<IMovementModule>();
            
            DebugLogger.Assert(_movementModule != null, "[IsJumpingCondition] _movementModule is null.");
        }

        protected override bool OnCheck() => _movementModule?.IsJumping ?? false;
    }

    public class IsGroundedCondition : AbstractCondition
    {
        private readonly IGroundCheckModule _groundChecker;

        public IsGroundedCondition(IModuleOwner owner, ConditionType type, bool isNot) : base(owner, type, isNot)
        {
            _groundChecker = owner.GetModule<IGroundCheckModule>();
            
            DebugLogger.Assert(_groundChecker != null, "[IsGroundCondition] _groundChecker is null.");
        }
        
        protected override bool OnCheck() => _groundChecker?.NotifyIsGround.Value ?? false;
    }

    public class IsFallCondition : AbstractCondition
    {
        private readonly IMovementModule _movementModule;

        public IsFallCondition(IModuleOwner owner, ConditionType type, bool isNot) : base(owner, type, isNot)
        {
            _movementModule = owner.GetModule<IMovementModule>();
            
            DebugLogger.Assert(_movementModule != null, "[IsFallCondition] _movementModule is null.");
        }
        
        protected override bool OnCheck() => _movementModule?.IsFall ?? false;
    }
    
    public class IsHitCondition : AbstractCondition
    {
        private readonly EventChannelSO _playerEvtChannel;
        private readonly IBodyModule _bodyModule;
        
        private bool _isHit = false;
        
        public IsHitCondition(IModuleOwner owner, ConditionType type, bool isNot) : base(owner, type, isNot)
        {
            this._bodyModule = owner.GetModule<IBodyModule>();

            _bodyModule.OnTakeDamage += HandlePlayerHitEvent;
        }
        
        protected override bool OnCheck()
        {
            if (_isHit)
            {
                DebugLogger.Log("OnCheck : Hit!");
                _isHit = false;
                return true;
            }
            
            return false;
        }

        private void HandlePlayerHitEvent(int damage, int currentHp) => _isHit = true;
    }

    public class IsExpiredCondition : AbstractCondition
    {
        private readonly float _duration;
        
        private float _startTime = -1f;
        
        public IsExpiredCondition(IModuleOwner owner, ConditionType type, 
            bool isNot, float duration) : base(owner, type, isNot)
        {
            this._duration = duration;
        }

        protected override bool OnCheck()
        {
            if (_startTime <= -1f)
                _startTime = Time.time;

            if (Time.time - _startTime >= _duration)
            {
                _startTime = -1f;
                return true;
            }
            else
                return false;
        }
    }
}