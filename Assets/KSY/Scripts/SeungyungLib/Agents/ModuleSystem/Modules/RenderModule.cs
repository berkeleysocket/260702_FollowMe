using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;
using DG.Tweening;
using SeungyungLib.Template.EventChannels;

namespace SeungyungLib.ModuleSystem.Modules
{
    public class RenderModule : MonoBehaviour, IRenderModule
    {
        [SerializeField] private EventChannelSO playerEvtChannel;
        public bool IsActive { get; private set; }
        
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        public void Initialize(IModuleOwner owner)
        {
            this._animator = GetComponent<Animator>();
            this._spriteRenderer = GetComponent<SpriteRenderer>();

            DebugLogger.Assert(_animator != null, "[RenderModule]: _animator is null]");
            DebugLogger.Assert(_spriteRenderer != null, "[RenderModule]: _spriteRenderer is null]");
            
            this.playerEvtChannel.AddListener<PlayerHitEvent>(PlayHitShake);
        }
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void PlayClip(int stateHashName, float fixedTransitionDuration, float fixedTimeOffset, float normalizedTransitionTime, int layer = -1) 
            => _animator.CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);

        public void FlipX(bool flip)
            => _spriteRenderer.flipX = flip;
        
        private void PlayHitShake(PlayerHitEvent evt)
        {
            transform.DOShakePosition(0.15f, strength: 0.2f, vibrato: 20);
        }
    }
}