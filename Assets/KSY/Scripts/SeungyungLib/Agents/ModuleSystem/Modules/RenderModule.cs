using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.Template.EventChannels;

using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class RenderModule : MonoBehaviour, IRenderModule
    {
        [SerializeField] private EventChannelSO playerEventChannel;
        
        private IModuleOwner _owner;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        public void Initialize(IModuleOwner owner)
        {
            this._owner = owner;
            this._animator = GetComponent<Animator>();
            this._spriteRenderer = GetComponent<SpriteRenderer>();

            DebugLogger.Assert(playerEventChannel != null, "[AgentRenderModule]: playerEventChannel is null]");
            DebugLogger.Assert(_owner != null, "[AgentRenderModule]: _owner is null]");
            DebugLogger.Assert(_animator != null, "[AgentRenderModule]: _animator is null]");
            DebugLogger.Assert(_spriteRenderer != null, "[AgentRenderModule]: _spriteRenderer is null]");
            
            playerEventChannel.AddListener<MoveInputEvent>(HandleMoveInput);
        }

        private void HandleMoveInput(MoveInputEvent evt)
        {
            float axis = evt.Axis;

            if (axis != 0f)
                FlipX(axis < 1f);
        }

        public void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0) 
            => _animator.CrossFadeInFixedTime(clipHash, crossFadeDuration, layerIndex, normalizedTime);

        public void FlipX(bool flip)
            => _spriteRenderer.flipX = flip;
    }
}