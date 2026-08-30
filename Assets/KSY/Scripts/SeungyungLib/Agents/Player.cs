using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.Core.ParameterSO;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.ModuleSystem.Modules;
using SeungyungLib.Template.EventChannels;

using UnityEngine;

namespace SeungyungLib.Agents
{
    public class Player : Agent
    {
        [SerializeField] private AssetNameSO dustParticleName;
        [SerializeField] private AssetNameSO smokeParticleName;
        [SerializeField] private EventChannelSO controlEventChannel;
        [SerializeField] private EventChannelSO playerEventChannel;

        private IBodyModule _bodyModule;
        private IControllableMovementModule _movementModule;
        private IGroundCheckModule _groundChecker;
        private IRenderModule _renderModule;
        private IVfxModule _vfxModule;

        #region Initialization
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this._bodyModule = GetModule<IBodyModule>();
            this._groundChecker = GetModule<IGroundCheckModule>();
            this._vfxModule = GetModule<IVfxModule>();
            this._movementModule = GetModule<IControllableMovementModule>();
            this._renderModule = GetModule<IRenderModule>();

            DebugLogger.Assert(controlEventChannel != null, "[AgentMovementModule]: controlEventChannel is null.");
            DebugLogger.Assert(playerEventChannel != null, "[AgentMovementModule]: playerEventChannel is null.");
            
            DebugLogger.Assert(_bodyModule != null, "[AgentMovementModule]: _bodyModule is null.");
            DebugLogger.Assert(_groundChecker != null, "[AgentMovementModule]: _groundChecker is null.");
            DebugLogger.Assert(_vfxModule != null, "[AgentMovementModule]: _vfxModule is null.");
            DebugLogger.Assert(_movementModule != null, "[AgentMovementModule]: _movementModule is null.");
            DebugLogger.Assert(_renderModule != null, "[AgentMovementModule]: _renderModule is null.");

            SubscribeEventHandlers();
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            
            UnsubscribeEventHandlers();
        }

        private void SubscribeEventHandlers()
        {
            _groundChecker.NotifyIsGround.OnChanged += HandleGroundCheck;
            _bodyModule.OnTakeDamage += HandleTakeDamage;
            controlEventChannel.AddListener<MoveInputEvent>(HandleMoveInput);
            controlEventChannel.AddListener<JumpInputEvent>(HandleJumpInput);
        }

        private void UnsubscribeEventHandlers()
        {
            _groundChecker.NotifyIsGround.OnChanged -= HandleGroundCheck;
            controlEventChannel.RemoveListener<MoveInputEvent>(HandleMoveInput);
            controlEventChannel.RemoveListener<JumpInputEvent>(HandleJumpInput);
        }
        #endregion

        #region Event Handlers
        private void HandleTakeDamage(int damage, int currentHealth)
        {
            PlayerEvents.PlayerHitEvent.Initialize(damage, currentHealth);
            playerEventChannel.RaiseEvent(PlayerEvents.PlayerHitEvent);
        }
        
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
        
        private void HandleMoveInput(MoveInputEvent evt)
        {
            float axis = evt.Axis;
            
            _movementModule.MoveToDirection(axis);

            if (axis != 0)
            {
                bool isFlip = axis < 0f;
                
                _renderModule.FlipX(isFlip);
                            
                if (_groundChecker.NotifyIsGround.Value)
                    _vfxModule.PlayVfx(dustParticleName.Hash, isFlip);
                else
                    _vfxModule.StopVfx(dustParticleName.Hash);
            }
            else
                _vfxModule.StopVfx(dustParticleName.Hash);
        }

        private void HandleJumpInput(JumpInputEvent evt) => _movementModule.IsJumpKeyPressed = evt.JumpKeyPressed;
        #endregion
    }
}
