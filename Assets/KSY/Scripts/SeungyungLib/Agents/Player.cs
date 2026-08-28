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
        [SerializeField] private EventChannelSO controlEventChannel;
        
        private IGroundCheckModule _groundChecker;
        private IVfxModule _vfxModule;
        private IControllableMovementModule _movementModule;
        private IRenderModule _renderModule;

        #region Initialization
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this._groundChecker = GetModule<IGroundCheckModule>();
            this._vfxModule = GetModule<IVfxModule>();
            this._movementModule = GetModule<IControllableMovementModule>();
            this._renderModule = GetModule<IRenderModule>();

            DebugLogger.Assert(controlEventChannel != null, "[AgentMovementModule]: playerEventChannel is null.");
            DebugLogger.Assert(_groundChecker != null, "[AgentMovementModule]: _groundChecker is null.");
            DebugLogger.Assert(_vfxModule != null, "[AgentMovementModule]: _vfxModule is null.");
            DebugLogger.Assert(_renderModule != null, "[AgentMovementModule]: _renderModule is null.");

            RegisterEventHandlers();
        }
        
        private void RegisterEventHandlers()
        {
            _groundChecker.NotifyIsGround.OnChanged += HandleGroundCheck;
            controlEventChannel.AddListener<MoveInputEvent>(HandleMoveInput);
            controlEventChannel.AddListener<JumpInputEvent>(HandleJumpInput);
        }
        #endregion

        #region Event Handlers
        private void HandleGroundCheck(bool isGround)
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
        
        private void HandleMoveInput(MoveInputEvent evt)
        {
            float axis = evt.Axis;
            bool isFlip = axis < 0f;
            
            _movementModule.MoveToDirection(axis);
            _renderModule.FlipX(isFlip);
            MovementVfxCallback(axis);
        }

        private void HandleJumpInput(JumpInputEvent evt) => _movementModule.IsJumpKeyPressed = evt.JumpKeyPressed;
        #endregion
    }
}
