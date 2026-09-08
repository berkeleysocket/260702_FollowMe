using UnityEngine;
using UnityEngine.SceneManagement;

namespace FollowMe.KDS
{
    /// <summary>
    /// 맵 엔드포인트. 도달 시 별점 정산·저장 후 스테이지를 멈춘다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class StageGoal : MonoBehaviour
    {
        [SerializeField] private int _stageNumber = 1;
        [SerializeField] private bool _pauseOnClear = true;
        [SerializeField] private bool _ensureVisual = true;
        [SerializeField] private Vector2 _triggerSize = new Vector2(4f, 6f);

        private bool _cleared;
        private static Sprite _whiteSprite;

        public int StageNumber => _stageNumber;
        public int LastStars { get; private set; }
        public bool IsCleared => _cleared;

        private void Awake()
        {
            MapTriggerLayer.Apply(gameObject);
            EnsureTriggerCollider();
            EnsureRunStats();
            if (_ensureVisual)
                EnsureEndpointVisual();
        }

        private void FixedUpdate()
        {
            // 물리 레이어/트리거 누락 대비: 골 근처 X 도달 시 클리어
            if (_cleared) return;
            var player = PlayerRespawn.FindInScene();
            if (player == null) return;

            Vector3 p = player.transform.position;
            float halfW = _triggerSize.x * 0.6f;
            float top = transform.position.y + _triggerSize.y;
            float bottom = transform.position.y - 1.5f;
            if (p.x >= transform.position.x - halfW &&
                p.x <= transform.position.x + halfW &&
                p.y >= bottom &&
                p.y <= top)
            {
                CompleteClear();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) => TryClear(other);

        private void OnTriggerStay2D(Collider2D other) => TryClear(other);

        private void TryClear(Collider2D other)
        {
            if (_cleared || !PlayerTriggerUtility.IsPlayer(other))
                return;
            CompleteClear();
        }

        private void CompleteClear()
        {
            if (_cleared) return;
            _cleared = true;
            var stats = EnsureRunStats();
            stats.ConfigureStage(_stageNumber);
            LastStars = stats.ClearStage();

            GameProgressSave.RecordStageClear(_stageNumber, LastStars, stats.FollowRatio);
            if (SocialScoreService.Instance != null)
            {
                GameProgressSave.CaptureSocial(
                    SocialScoreService.Instance.Likes,
                    SocialScoreService.Instance.Follows,
                    SocialScoreService.Instance.IsSecondCycle);
                GameProgressSave.FlushIfDirty();
            }

            var hud = FindFirstObjectByType<StageClearStarsHud>();
            if (hud == null)
            {
                var go = new GameObject("StageClearStarsHud");
                hud = go.AddComponent<StageClearStarsHud>();
            }

            hud.Show(_stageNumber, LastStars, stats.FollowsCollected, stats.FollowsTotal);

            if (_pauseOnClear)
                Time.timeScale = 0f;

            Debug.Log($"[StageGoal] Stage {_stageNumber} 클리어 ★{LastStars} (엔드포인트 도달)", this);
        }

        private void EnsureTriggerCollider()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;

            // 스케일 꼬임 방지: 로컬 스케일 1 + 명시적 트리거 크기
            transform.localScale = Vector3.one;
            if (col is BoxCollider2D box)
            {
                box.size = _triggerSize;
                box.offset = new Vector2(0f, _triggerSize.y * 0.15f);
            }

            // 트리거 안정화용 Static RB
            var rb = GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            rb.simulated = true;
        }

        private StageRunStats EnsureRunStats()
        {
            if (StageRunStats.Instance != null)
                return StageRunStats.Instance;

            var systems = GameObject.Find("SocialSystems");
            if (systems == null)
                systems = new GameObject("SocialSystems");

            var stats = systems.GetComponent<StageRunStats>();
            if (stats == null)
                stats = systems.AddComponent<StageRunStats>();

            stats.ConfigureStage(_stageNumber);
            return stats;
        }

        private void EnsureEndpointVisual()
        {
            if (transform.Find("EndPole") != null)
                return;

            var white = GetWhiteSprite();

            var pole = new GameObject("EndPole");
            pole.transform.SetParent(transform, false);
            pole.transform.localPosition = Vector3.zero;
            pole.transform.localScale = new Vector3(0.35f, 3.2f, 1f);
            var poleSr = pole.AddComponent<SpriteRenderer>();
            poleSr.sprite = white;
            poleSr.color = new Color(0.95f, 0.95f, 0.98f, 0.95f);
            poleSr.sortingOrder = 20;

            var flag = new GameObject("EndFlag");
            flag.transform.SetParent(transform, false);
            flag.transform.localPosition = new Vector3(0.85f, 2.3f, 0f);
            flag.transform.localScale = new Vector3(1.8f, 1.1f, 1f);
            var flagSr = flag.AddComponent<SpriteRenderer>();
            flagSr.sprite = white;
            flagSr.color = new Color(0.25f, 0.95f, 0.55f, 0.9f);
            flagSr.sortingOrder = 21;

            var glow = new GameObject("EndZoneGlow");
            glow.transform.SetParent(transform, false);
            glow.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            glow.transform.localScale = new Vector3(3.2f, 3.6f, 1f);
            var glowSr = glow.AddComponent<SpriteRenderer>();
            glowSr.sprite = white;
            glowSr.color = new Color(0.3f, 0.95f, 0.55f, 0.22f);
            glowSr.sortingOrder = 19;
        }

        private static Sprite GetWhiteSprite()
        {
            if (_whiteSprite != null)
                return _whiteSprite;

            var tex = Texture2D.whiteTexture;
            _whiteSprite = Sprite.Create(
                tex,
                new Rect(0f, 0f, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                4f);
            return _whiteSprite;
        }
    }
}
