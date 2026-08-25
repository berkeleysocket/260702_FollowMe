using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.ParameterSO;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.Agents
{
    public class Enemy : Agent
    {
        [SerializeField] private AssetNameSO dustParticleName;
        [SerializeField] private AssetNameSO smokeParticleName;
        
        private IGroundCheckModule _groundChecker;
        private IVfxModule _vfxModule;
        private IMovementModule _movementModule;

        #region Initialization
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this._groundChecker = GetModule<IGroundCheckModule>();
            this._vfxModule = GetModule<IVfxModule>();
            this._movementModule = GetModule<IMovementModule>();

            DebugLogger.Assert(_groundChecker != null, "[AgentMovementModule]: _groundChecker is null.");
            DebugLogger.Assert(_vfxModule != null, "[AgentMovementModule]: _vfxModule is null.");

            RegisterEventHandlers();
        }
        
        private void RegisterEventHandlers()
        {
            _groundChecker.NotifyIsGround.OnChanged += GroundCheckerCallback;
        }
        #endregion

        #region Event Handlers
        private void GroundCheckerCallback(bool isGround)
        {
            if (isGround)
            {
                float axis = _movementModule.Axis;
                    
                _vfxModule.PlayVfx(smokeParticleName.Hash, 
                    new Vector2(transform.position.x, transform.position.y - 0.5f), 
                    Quaternion.identity);
                    
                if (axis != 0)
                {
                    bool isFlip = axis < 0f;
                    _vfxModule.PlayVfx(dustParticleName.Hash, isFlip);
                }
                else
                    _vfxModule.StopVfx(dustParticleName.Hash);
            }
            else
                _vfxModule.StopVfx(dustParticleName.Hash);
        }

        private void MovementVfxCallback(float axis)
        {
            if (axis != 0 && _groundChecker.NotifyIsGround.Value)
            {
                bool isFlip = axis < 0f;
                _vfxModule.PlayVfx(dustParticleName.Hash, isFlip);
            }
            else
                _vfxModule.StopVfx(dustParticleName.Hash);
        }
        
        // private void MoveInputCallback(MoveInputEvent evt)
        // {
        //     float axis = evt.Axis;
        //     
        //     _movementModule.MoveToDirection(axis);
        //     MovementVfxCallback(axis);
        // }
        //
        // private void JumpInputCallback(JumpInputEvent evt) => _movementModule.IsJumpKeyPressed = evt.JumpKeyPressed;
        #endregion
    }
}
