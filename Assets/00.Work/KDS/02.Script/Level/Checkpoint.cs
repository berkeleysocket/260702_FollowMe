using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 플레이어가 지나가면 리스폰 위치 갱신.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private string _checkpointId = "CP_Intro";
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private bool _registerOnStart;

        public string CheckpointId => string.IsNullOrEmpty(_checkpointId) ? name : _checkpointId;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;

            if (_spawnPoint == null)
                _spawnPoint = transform;
        }

        private void Start()
        {
            if (_registerOnStart)
                Register();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PlayerTriggerUtility.IsPlayer(other))
                return;

            Register();
        }

        public void Register()
        {
            if (CheckpointService.Instance == null)
            {
                Debug.LogWarning($"[Checkpoint] CheckpointService 없음: {name}", this);
                return;
            }

            Vector3 pos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            CheckpointService.Instance.RegisterCheckpoint(CheckpointId, pos);
        }

        private void OnDrawGizmos()
        {
            Transform spawn = _spawnPoint != null ? _spawnPoint : transform;
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.85f);
            Gizmos.DrawWireSphere(spawn.position, 0.35f);
        }
    }
}
