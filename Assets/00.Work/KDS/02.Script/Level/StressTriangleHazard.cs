using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 임시 삼각형 장애물. 플레이어가 닿으면 스트레스가 오르고 한 번 발동되면 비활성화된다.
    /// 비주얼은 절차적으로 생성한 삼각형 스프라이트 — 추후 아트로 교체 예정.
    /// Width/Height는 transform 스케일로 적용된다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(PolygonCollider2D))]
    public class StressTriangleHazard : MonoBehaviour
    {
        private static readonly Vector2[] UnitTrianglePoints =
        {
            new Vector2(-0.5f, -0.5f),
            new Vector2(0.5f, -0.5f),
            new Vector2(0f, 0.5f),
        };

        private static Sprite _sharedSprite;

        [SerializeField] private float _stressPenalty = 12f; // 아이템 픽업 시 감소량의 3배
        [SerializeField] private float _width = 1f;
        [SerializeField] private float _height = 1f;
        [SerializeField] private Color _color = new Color(0.85f, 0.1f, 0.1f);
        [SerializeField] private bool _logHit = true;

        private void Awake()
        {
            MapTriggerLayer.Apply(gameObject);

            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == null)
                spriteRenderer.sprite = GetOrCreateTriangleSprite();
            spriteRenderer.color = _color;

            transform.localScale = new Vector3(Mathf.Max(0.01f, _width), Mathf.Max(0.01f, _height), 1f);

            var col = GetComponent<PolygonCollider2D>();
            col.points = UnitTrianglePoints;
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PlayerTriggerUtility.IsPlayer(other))
                return;

            SocialScoreService.Instance?.ApplyStress(_stressPenalty);

            if (_logHit)
                Debug.Log($"[StressTriangleHazard] 피격 → 스트레스 +{_stressPenalty}", this);

            gameObject.SetActive(false);
        }

        private static Sprite GetOrCreateTriangleSprite()
        {
            if (_sharedSprite != null)
                return _sharedSprite;

            const int size = 64;
            Vector2 a = new Vector2(size * 0.5f, size - 1);
            Vector2 b = new Vector2(0f, 0f);
            Vector2 c = new Vector2(size - 1, 0f);

            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool inside = PointInTriangle(new Vector2(x, y), a, b, c);
                    pixels[y * size + x] = inside ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0);
                }
            }

            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
            };
            texture.SetPixels32(pixels);
            texture.Apply();

            _sharedSprite = Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size);

            return _sharedSprite;
        }

        private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float d1 = Sign(p, a, b);
            float d2 = Sign(p, b, c);
            float d3 = Sign(p, c, a);

            bool hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
            bool hasPos = d1 > 0 || d2 > 0 || d3 > 0;

            return !(hasNeg && hasPos);
        }

        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }
    }
}
