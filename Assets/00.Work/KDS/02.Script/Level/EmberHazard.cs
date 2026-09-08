using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 불티 장애물. 닿으면 스트레스 상승 (리스폰 없음 — 짧은 화상 느낌).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class EmberHazard : MonoBehaviour
    {
        [SerializeField] private float _stressPenalty = 22f;
        [SerializeField] private float _hitCooldown = 0.45f;
        [SerializeField] private bool _destroyOnHit;

        private float _nextHitAllowedTime;

        private void Awake()
        {
            MapTriggerLayer.Apply(gameObject);
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PlayerTriggerUtility.IsPlayer(other))
                return;
            if (Time.unscaledTime < _nextHitAllowedTime)
                return;

            _nextHitAllowedTime = Time.unscaledTime + Mathf.Max(0.05f, _hitCooldown);
            SocialScoreService.Instance?.ApplyStress(_stressPenalty);
            Debug.Log($"[EmberHazard] 피격 → 스트레스 +{_stressPenalty}", this);

            if (_destroyOnHit)
                Destroy(gameObject);
        }
    }
}
