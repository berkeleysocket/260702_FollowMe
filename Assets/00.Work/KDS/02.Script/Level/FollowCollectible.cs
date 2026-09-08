using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// ♡ 하트 — 팔로우 증가 + 스트레스 감소. Stage1 핵심 수집.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class FollowCollectible : MonoBehaviour
    {
        [SerializeField] private long _followValue = 50;

        private void Awake()
        {
            MapTriggerLayer.Apply(gameObject);
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PlayerTriggerUtility.IsPlayer(other))
                return;

            bool gained = true;
            if (SocialScoreService.Instance != null)
                gained = SocialScoreService.Instance.CollectFollow(_followValue);

            if (gained)
                StageRunStats.Instance?.RegisterFollowPickup();
            Destroy(gameObject);
        }
    }
}
