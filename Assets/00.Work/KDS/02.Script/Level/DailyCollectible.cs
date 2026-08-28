using UnityEngine;

namespace FollowMe.KDS
{
    [RequireComponent(typeof(Collider2D))]
    public class DailyCollectible : MonoBehaviour
    {
        [SerializeField] private DailyKind _kind = DailyKind.Reply;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PlayerTriggerUtility.IsPlayer(other))
                return;

            StageRunStats.Instance?.RegisterDailyPickup();
            Destroy(gameObject);
        }
    }

    public enum DailyKind
    {
        Reply,
        Meal,
        Sleep
    }
}
