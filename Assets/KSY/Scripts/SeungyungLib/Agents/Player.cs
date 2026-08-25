using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.Core.ParameterSO;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.Template.EventChannels;

using UnityEngine;

namespace SeungyungLib.Agents
{
    public class Player : Agent
    {
        [SerializeField] private AssetNameSO dustParticleName;
        [SerializeField] private AssetNameSO smokeParticleName;
        [SerializeField] private EventChannelSO playerEventChannel;
        
        private IGroundCheckModule _groundChecker;
        private IVfxModule _vfxModule;
        private IControllableMovementModule _movementModule;

        #region Initialization
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this._groundChecker = GetModule<IGroundCheckModule>();
            this._vfxModule = GetModule<IVfxModule>();
            this._movementModule = GetModule<IControllableMovementModule>();

            DebugLogger.Assert(playerEventChannel != null, "[AgentMovementModule]: playerEventChannel is null.");
            DebugLogger.Assert(_groundChecker != null, "[AgentMovementModule]: _groundChecker is null.");
            DebugLogger.Assert(_vfxModule != null, "[AgentMovementModule]: _vfxModule is null.");

            RegisterEventHandlers();
        }
        
        private void RegisterEventHandlers()
        {
            _groundChecker.NotifyIsGround.OnChanged += GroundCheckerCallback;
            playerEventChannel.AddListener<MoveInputEvent>(MoveInputCallback);
            playerEventChannel.AddListener<JumpInputEvent>(JumpInputCallback);
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
        
        private void MoveInputCallback(MoveInputEvent evt)
        {
            float axis = evt.Axis;
            
            _movementModule.MoveToDirection(axis);
            MovementVfxCallback(axis);
        }

        private void JumpInputCallback(JumpInputEvent evt) => _movementModule.IsJumpKeyPressed = evt.JumpKeyPressed;
        #endregion
    }
}
