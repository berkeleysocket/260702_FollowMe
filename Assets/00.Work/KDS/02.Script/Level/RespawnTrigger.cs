using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 함정·낙사 등 — 체크포인트로 리스폰.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class RespawnTrigger : MonoBehaviour
    {
        [SerializeField] private bool _requirePlayerTag = true;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_requirePlayerTag && !PlayerTriggerUtility.IsPlayer(other))
                return;

            if (CheckpointService.Instance != null)
                CheckpointService.Instance.RespawnPlayer("Hazard");
        }
    }
}
