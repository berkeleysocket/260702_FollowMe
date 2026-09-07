using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.ModuleSystem.Core;
using SeungyungLib.Template.EventChannels;

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
        [SerializeField] private float invincibilityDuration = 2.0f; // 무적 시간
        [SerializeField] private float blinkInterval = 0.1f;         // 깜빡이는 주기
        public bool IsInvincible { get; private set; }
        
        public bool IsActive { get; private set; }
        
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

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

        #region AI Shake Effect Code
        private void PlayHitShake(PlayerHitEvent evt)
        {
            transform.DOShakePosition(0.15f, strength: 0.2f, vibrato: 20);
        }
        #endregion

        #region AI Invincible Effect Code
        public void TriggerInvincibility()
        {
            if (IsInvincible) return;
            StartCoroutine(InvincibilityRoutine());
        }

        private IEnumerator InvincibilityRoutine()
        {
            IsInvincible = true;

            float timer = 0f;
            Color originalColor = _spriteRenderer.color;
            Color blinkColor = originalColor;
            blinkColor.a = 0.2f; // 반투명 상태

            while (timer < invincibilityDuration)
            {
                // Alpha 값 토글
                _spriteRenderer.color = (_spriteRenderer.color.a == originalColor.a) ? blinkColor : originalColor;

                yield return new WaitForSeconds(blinkInterval);
                timer += blinkInterval;
            }

            // 상태 복구
            _spriteRenderer.color = originalColor;
            IsInvincible = false;
        }
        #endregion
    }
}