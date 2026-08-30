using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class RenderModule : MonoBehaviour, IRenderModule
    {
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        public void Initialize(IModuleOwner owner)
        {
            this._animator = GetComponent<Animator>();
            this._spriteRenderer = GetComponent<SpriteRenderer>();

            DebugLogger.Assert(_animator != null, "[RenderModule]: _animator is null]");
            DebugLogger.Assert(_spriteRenderer != null, "[RenderModule]: _spriteRenderer is null]");
        }

        public void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0) 
            => _animator.CrossFadeInFixedTime(clipHash, crossFadeDuration, layerIndex, normalizedTime);

        public void FlipX(bool flip)
            => _spriteRenderer.flipX = flip;
    }
}