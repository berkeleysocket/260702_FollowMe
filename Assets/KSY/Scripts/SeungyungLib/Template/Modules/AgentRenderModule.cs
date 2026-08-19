using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class AgentRenderModule : MonoBehaviour, IAgentRenderModule
    {
        private IModuleOwner _owner;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        public virtual void Initialize(IModuleOwner owner)
        {
            this._owner = owner;
            this._animator = GetComponent<Animator>();
        }

        public void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0)
        {
            _animator.CrossFadeInFixedTime(clipHash, crossFadeDuration, layerIndex, normalizedTime);
        }

        public void FlipX(bool flip) => _spriteRenderer.flipX = flip;
    }
}
