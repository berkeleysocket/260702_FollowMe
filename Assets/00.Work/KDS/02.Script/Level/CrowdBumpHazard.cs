using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 인파 밀림 장애물. 스트레스 + 약한 뒤로 밀림.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CrowdBumpHazard : MonoBehaviour
    {
        [SerializeField] private float _stressPenalty = 18f;
        [SerializeField] private float _knockbackX = -2.5f;
        [SerializeField] private float _hitCooldown = 0.7f;

        private float _nextHitAllowedTime;

        private void Awake()
        {
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

            var body = other.attachedRigidbody;
            if (body == null)
            {
                var t = PlayerTriggerUtility.GetPlayerTransform(other);
                if (t != null)
                    body = t.GetComponent<Rigidbody2D>();
            }

            if (body != null)
            {
                var v = body.linearVelocity;
                v.x = _knockbackX;
                body.linearVelocity = v;
            }

            Debug.Log($"[CrowdBumpHazard] 피격 → 스트레스 +{_stressPenalty}", this);
        }
    }
}
