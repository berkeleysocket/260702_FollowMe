using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 맵 엔드포인트. 도달 시 별점 정산 후 스테이지를 멈춘다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class StageGoal : MonoBehaviour
    {
        [SerializeField] private int _stageNumber = 1;
        [SerializeField] private bool _pauseOnClear = true;
        [SerializeField] private bool _ensureVisual = true;

        private bool _cleared;
        private static Sprite _whiteSprite;

        public int StageNumber => _stageNumber;
        public int LastStars { get; private set; }
        public bool IsCleared => _cleared;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
            EnsureRunStats();
            if (_ensureVisual)
                EnsureEndpointVisual();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_cleared || !PlayerTriggerUtility.IsPlayer(other))
                return;

            _cleared = true;
            var stats = EnsureRunStats();
            stats.ConfigureStage(_stageNumber);
            LastStars = stats.ClearStage();

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
