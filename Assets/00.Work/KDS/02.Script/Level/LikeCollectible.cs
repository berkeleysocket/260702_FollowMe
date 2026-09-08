using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 반응 이모지 — 좋아요 증가 + 스트레스 감소.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LikeCollectible : MonoBehaviour
    {
        [SerializeField] private long _likeValue = 100;

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

            if (SocialScoreService.Instance != null)
                SocialScoreService.Instance.CollectLike(_likeValue);

            StageRunStats.Instance?.RegisterLikePickup();
            Destroy(gameObject);
        }
    }
}

