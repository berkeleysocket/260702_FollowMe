using System.Collections;
using DG.Tweening; // DOTween 네임스페이스 추가
using KSY.System;
using SeungyungLib.CollectSystem;
using UnityEngine;

namespace KSY.Item
{
    public class Item : MonoBehaviour, ICollectable
    {
        [field: SerializeField] public ItemSO ItemData { get; private set; }
        [SerializeField] private SpriteRenderer spRenderer;
        [SerializeField] private ParticleSystem collectParticle;
        
        [Header("Collect Animation Settings")]
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float punchScaleFactor = 1.3f; // 획득 시 커질 크기 비율
        
        public CollectableSO Data => ItemData;
        
        private bool _isCollected = false;
        
        private void OnValidate()
        {
            if (ItemData != null && spRenderer != null)
                spRenderer.sprite = ItemData.Icon;
        }

        public void Collect(ICollector collector)
        {
            if (_isCollected) return;
            _isCollected = true;

            ScoreManager.AddScore(ItemData.Score);
            
            if (collectParticle != null)
            {
                collectParticle.Play();
            }

            // DOTween 연출 실행
            PlayCollectSequence();
        }

        private void PlayCollectSequence()
        {
            // 여러 트윈을 하나로 묶어 실행할 Sequence 생성
            Sequence collectSequence = DOTween.Sequence();

            if (spRenderer != null)
            {
                // 1. 살짝 커졌다가 0으로 줄어드는 스케일 연출
                Vector3 targetScale = transform.localScale * punchScaleFactor;
                
                // 크기가 순간적으로 커졌다가 완전히 0으로 줄어듦
                collectSequence.Append(transform.DOScale(targetScale, fadeDuration * 0.4f).SetEase(Ease.OutQuad))
                              .Append(transform.DOScale(Vector3.zero, fadeDuration * 0.6f).SetEase(Ease.InQuad));

                // 2. 스케일 애니메이션과 동시에 Alpha Fade-Out 진행
                collectSequence.Join(spRenderer.DOFade(0f, fadeDuration));
            }

            // 모든 연출이 끝난 후 게임 오브젝트 파괴
            collectSequence.OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}