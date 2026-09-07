using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// KDS 맵 시스템용 리스폰 훅. KSY Player 프리팹 루트에 부착한다.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerRespawn : MonoBehaviour
    {
        private Rigidbody2D _rb;

        private void Awake()
        {
            CacheRigidbody();
            if (!gameObject.CompareTag("Player"))
                gameObject.tag = "Player";
        }

        private void CacheRigidbody()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
                _rb = GetComponentInChildren<Rigidbody2D>();
        }

        public void RespawnAt(Vector3 worldPosition)
        {
            CacheRigidbody();
            transform.position = worldPosition;
            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0f;
            }
        }

        public static PlayerRespawn FindInScene()
        {
            return FindFirstObjectByType<PlayerRespawn>();
        }
    }
}
