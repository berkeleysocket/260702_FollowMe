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
        private IAgentMovementModule _movementModule;
        private bool _isMoving;
        
        public IsMovingCondition(IModuleOwner owner, bool isNot) : base(owner, isNot)
        {
            _movementModule = owner.GetModule<IAgentMovementModule>();

            if (_movementModule != null)
            {
                _movementModule.OnChangeAxis += (float axis) =>
                {
                    Debug.Log(axis);
                    
                    if (Mathf.Abs(axis) > 0.01f)
                        _isMoving = !isNot;
                    else
                        _isMoving = isNot;
                };
            }
            else
                DebugLogger.LogError("[IsMovingCondition] Movement Module is null");
        }

        public override bool Check()
        {
            DebugLogger.Log($"isNot : {IsNot} / isMoving : {_isMoving}");
            return _isMoving;
        }
    }
}
