using System;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 체크포인트 등록·플레이어 리스폰.
    /// </summary>
    public class CheckpointService : MonoBehaviour
    {
        public static CheckpointService Instance { get; private set; }

        [Header("Spawn")]
        [SerializeField] private Transform _defaultSpawn;
        [SerializeField] private PlayerRespawn _playerOverride;
        [SerializeField] private float _fallRespawnY = -8f;

        [Header("Debug")]
        [SerializeField] private bool _logRespawn = true;

        private Vector3 _lastSpawnPosition;
        private string _lastCheckpointId;

        public string LastCheckpointId => _lastCheckpointId;
        public Vector3 LastSpawnPosition => _lastSpawnPosition;
        public bool HasCheckpoint => _lastCheckpointId != null;

        public event Action<string, Vector3> CheckpointUpdated;
        public event Action Respawned;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _lastSpawnPosition = ResolveDefaultSpawnPosition();
        }

        private void Start()
        {
            if (_playerOverride == null)
                _playerOverride = PlayerRespawn.FindInScene();

            if (_lastCheckpointId == "Default")
            {
                _lastSpawnPosition = ResolveDefaultSpawnPosition();
                RegisterCheckpoint("Spawn", _lastSpawnPosition);
            }
        }

        private void Update()
        {
            if (_playerOverride == null) return;
            if (_playerOverride.transform.position.y < _fallRespawnY)
                RespawnPlayer("Fall");
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void RegisterCheckpoint(string checkpointId, Vector3 spawnPosition)
        {
            if (string.IsNullOrEmpty(checkpointId))
                checkpointId = "Checkpoint";

            _lastCheckpointId = checkpointId;
            _lastSpawnPosition = spawnPosition;
            CheckpointUpdated?.Invoke(_lastCheckpointId, _lastSpawnPosition);

            if (_logRespawn)
                Debug.Log($"[CheckpointService] 등록: {checkpointId} @ {spawnPosition}", this);
        }

        public void RespawnPlayer(string reason = "Manual")
        {
            PlayerRespawn player = _playerOverride != null
                ? _playerOverride
                : PlayerRespawn.FindInScene();

            if (player == null)
            {
                Debug.LogWarning("[CheckpointService] PlayerRespawn 없음.", this);
                return;
            }

            player.RespawnAt(_lastSpawnPosition);

            if (_logRespawn)
                Debug.Log($"[CheckpointService] 리스폰 ({reason}) → {_lastCheckpointId} @ {_lastSpawnPosition}", this);

            Respawned?.Invoke();
        }

        private Vector3 ResolveDefaultSpawnPosition()
        {
            if (_defaultSpawn != null)
                return _defaultSpawn.position;

            if (_playerOverride != null)
                return _playerOverride.transform.position;

            var player = PlayerRespawn.FindInScene();
            if (player != null)
                return player.transform.position;

            return new Vector3(-2f, 1f, 0f);
        }
    }
}
