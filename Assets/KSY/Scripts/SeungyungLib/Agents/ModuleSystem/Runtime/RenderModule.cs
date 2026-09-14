using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.ModuleSystem.Core;

using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace SeungyungLib.ModuleSystem.Modules
{
    public class RenderModule : MonoBehaviour, IRenderModule
    {
        [SerializeField] private EventChannelSO playerEvtChannel;
        
        //AI 코드로 깜박거리는 효과를 구현함.
        //나중에 SpriteEffect라는 추상 클래스로 묶어서 이펙트들을 재사용할 수 있고 분리할 수 있게 구현할 것.
        [SerializeField] private float blinkInterval = 0.1f;         // 깜빡이는 주기
        
        public bool IsActive { get; private set; }
        
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private bool _isPlayingInvincibility;

        public void Initialize(IModuleOwner owner)
        {
            this._animator = GetComponent<Animator>();
            this._spriteRenderer = GetComponent<SpriteRenderer>();

            DebugLogger.Assert(_animator != null, "[RenderModule]: _animator is null]");
            DebugLogger.Assert(_spriteRenderer != null, "[RenderModule]: _spriteRenderer is null]");

        }
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void PlayClip(int stateHashName, float fixedTransitionDuration, float fixedTimeOffset, float normalizedTransitionTime, int layer = -1) 
            => _animator.CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);

        public void FlipX(bool flip)
            => _spriteRenderer.flipX = flip;

        #region AI Invincible Effect Code
        public void PlayInvincibilityEffect(bool isActive)
        {
            _isPlayingInvincibility = isActive;
            
            if (_isPlayingInvincibility)
                StartCoroutine(InvincibilityRoutine());
        }

        private IEnumerator InvincibilityRoutine()
        {
            Color originalColor = _spriteRenderer.color;
            Color blinkColor = originalColor;
            blinkColor.a = 0.2f; 

            while (_isPlayingInvincibility)
            {
                _spriteRenderer.color = (_spriteRenderer.color.a == originalColor.a) ? blinkColor : originalColor;
                yield return new WaitForSeconds(blinkInterval);
            }
            
            _spriteRenderer.color = originalColor;
        }
        #endregion
    }
}