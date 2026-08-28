using UnityEngine;

namespace FollowMe.KDS
{
    [RequireComponent(typeof(Collider2D))]
    public class LikeCollectible : MonoBehaviour
    {
        [SerializeField] private long _likeValue = 100;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PlayerTriggerUtility.IsPlayer(other))
                return;

            if (SocialScoreService.Instance != null)
                SocialScoreService.Instance.AddLikes(_likeValue);

            StageRunStats.Instance?.RegisterLikePickup();
            Destroy(gameObject);
        }
    }
}
