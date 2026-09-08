using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 임시 삼각형 장애물. 플레이어가 닿으면 아이템 섭취로 스트레스가 감소하는 것과
    /// 같은 양만큼 스트레스가 오른다. 비주얼은 절차적 삼각형 메시 — 추후 아트로 교체 예정.
    /// </summary>
    [RequireComponent(typeof(PolygonCollider2D))]
    public class StressTriangleHazard : MonoBehaviour
    {
        [SerializeField] private float _stressPenalty = 8f; // 아이템 픽업 시 감소량(SocialScoreService 기본값)과 동일
        [SerializeField] private float _hitCooldown = 0.6f;
        [SerializeField] private float _width = 1f;
        [SerializeField] private float _height = 1f;
        [SerializeField] private Color _color = new Color(0.85f, 0.1f, 0.1f);
        [SerializeField] private bool _logHit = true;

        private float _nextHitAllowedTime;

        private void Reset()
        {
            BuildVisual();
            BuildCollider();
        }

        private void Awake()
        {
            MapTriggerLayer.Apply(gameObject);

            if (GetComponent<MeshFilter>() == null)
                BuildVisual();

            BuildCollider();
            GetComponent<PolygonCollider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PlayerTriggerUtility.IsPlayer(other))
                return;

            if (Time.unscaledTime < _nextHitAllowedTime)
                return;

            _nextHitAllowedTime = Time.unscaledTime + Mathf.Max(0.05f, _hitCooldown);

            SocialScoreService.Instance?.ApplyStress(_stressPenalty);

            if (_logHit)
                Debug.Log($"[StressTriangleHazard] 피격 → 스트레스 +{_stressPenalty}", this);
        }

        private Vector2[] TrianglePoints()
        {
            float halfW = _width * 0.5f;
            float halfH = _height * 0.5f;
            return new[]
            {
                new Vector2(-halfW, -halfH),
                new Vector2(halfW, -halfH),
                new Vector2(0f, halfH),
            };
        }

        private void BuildCollider()
        {
            var col = GetComponent<PolygonCollider2D>();
            col.points = TrianglePoints();
        }

        private void BuildVisual()
        {
            var points = TrianglePoints();

            var mesh = new Mesh { name = "StressTriangleHazard" };
            mesh.vertices = new Vector3[] { points[0], points[1], points[2] };
            // 양방향 삼각형 두 장을 겹쳐 와인딩 방향과 무관하게 항상 보이도록 한다.
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 1 };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            var filter = gameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            meshRenderer.sharedMaterial = new Material(shader) { color = _color };
        }
    }
}
