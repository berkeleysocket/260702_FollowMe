using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 플레이어 진입 시 맵 모드를 변경. 레벨 디자이너가 구간마다 배치.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MapModeZone : MonoBehaviour
    {
        [SerializeField] private MapMode _targetMode = MapMode.Stable;
        [SerializeField] private bool _forceTransition;
        [SerializeField] private bool _oneShot = true;

        private bool _consumed;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PlayerTriggerUtility.IsPlayer(other))
                return;

            if (_oneShot && _consumed)
                return;

            if (MapModeService.Instance == null)
            {
                Debug.LogWarning($"[MapModeZone] MapModeService 없음: {name}", this);
                return;
            }

            if (MapModeService.Instance.TrySetMode(_targetMode, _forceTransition))
            {
                if (_oneShot)
                    _consumed = true;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _targetMode switch
            {
                MapMode.Stable => new Color(0.4f, 0.85f, 0.5f, 0.25f),
                MapMode.Warning => new Color(1f, 0.75f, 0.2f, 0.3f),
                MapMode.Chase => new Color(1f, 0.25f, 0.25f, 0.3f),
                MapMode.Recovery => new Color(0.35f, 0.75f, 1f, 0.28f),
                MapMode.Torment => new Color(0.15f, 0.1f, 0.2f, 0.35f),
                _ => Color.white
            };

            var col = GetComponent<Collider2D>();
            if (col == null) return;

            Gizmos.matrix = transform.localToWorldMatrix;
            if (col is BoxCollider2D box)
                Gizmos.DrawCube(box.offset, box.size);
            else if (col is CircleCollider2D circle)
                Gizmos.DrawSphere(circle.offset, circle.radius);
        }
    }
}
