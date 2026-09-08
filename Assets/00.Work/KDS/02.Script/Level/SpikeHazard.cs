using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 가시 장애물. 플레이어가 닿으면 스트레스가 크게 오르고 체크포인트로 리스폰한다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SpikeHazard : MonoBehaviour
    {
        [SerializeField] private float _stressPenalty = 50f;
        [SerializeField] private bool _respawnOnHit = true;
        [SerializeField] private float _hitCooldown = 0.6f;
        [SerializeField] private bool _logHit = true;

        private float _nextHitAllowedTime;

        public float StressPenalty => _stressPenalty;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PlayerTriggerUtility.IsPlayer(other))
                return;

            if (Time.unscaledTime < _nextHitAllowedTime)
                return;

            _nextHitAllowedTime = Time.unscaledTime + Mathf.Max(0.05f, _hitCooldown);

            SocialScoreService.Instance?.ApplyStress(_stressPenalty);

            if (_logHit)
                Debug.Log($"[SpikeHazard] 피격 → 스트레스 +{_stressPenalty}", this);

            if (_respawnOnHit && CheckpointService.Instance != null)
                CheckpointService.Instance.RespawnPlayer("Spike");
        }
    }
}
