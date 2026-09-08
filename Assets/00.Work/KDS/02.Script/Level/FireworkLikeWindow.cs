using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 불꽃 타이밍 윈도우. 활성 구간에 플레이어가 들어오면 좋아요 보너스·스트레스 완화.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class FireworkLikeWindow : MonoBehaviour
    {
        [SerializeField] private float _likeBonus = 3f;
        [SerializeField] private float _stressRelief = 12f;
        [SerializeField] private float _activeDuration = 2.5f;
        [SerializeField] private float _cooldown = 6f;
        [SerializeField] private bool _autoPulse = true;
        [SerializeField] private float _pulseInterval = 8f;
        [SerializeField] private SpriteRenderer _visual;
        [SerializeField] private Color _activeColor = new Color(1f, 0.85f, 0.4f, 0.85f);
        [SerializeField] private Color _idleColor = new Color(1f, 1f, 1f, 0.25f);

        private bool _windowOpen;
        private bool _claimed;
        private float _windowEndTime;
        private float _nextPulseTime;
        private float _nextClaimTime;

        public bool IsOpen => _windowOpen;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
            if (_visual == null)
                _visual = GetComponent<SpriteRenderer>();
            SetVisualIdle();
            _nextPulseTime = Time.time + 1.5f;
        }

        private void Update()
        {
            if (_windowOpen && Time.time >= _windowEndTime)
                CloseWindow();

            if (_autoPulse && !_windowOpen && Time.time >= _nextPulseTime)
                OpenWindow();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_windowOpen || _claimed)
                return;
            if (Time.time < _nextClaimTime)
                return;
            if (!PlayerTriggerUtility.IsPlayer(other))
                return;

            _claimed = true;
            _nextClaimTime = Time.time + _cooldown;

            if (SocialScoreService.Instance != null)
            {
                long likes = Mathf.Max(1, Mathf.RoundToInt(_likeBonus));
                SocialScoreService.Instance.AddLikes(likes);
                SocialScoreService.Instance.RelieveStress(_stressRelief);
            }

            Debug.Log($"[FireworkLikeWindow] 타이밍 성공 Like+{_likeBonus} Stress-{_stressRelief}", this);
            CloseWindow();
        }

        public void OpenWindow()
        {
            _windowOpen = true;
            _claimed = false;
            _windowEndTime = Time.time + Mathf.Max(0.4f, _activeDuration);
            if (_visual != null)
                _visual.color = _activeColor;
        }

        public void CloseWindow()
        {
            _windowOpen = false;
            _nextPulseTime = Time.time + Mathf.Max(_cooldown, _pulseInterval);
            SetVisualIdle();
        }

        private void SetVisualIdle()
        {
            if (_visual != null)
                _visual.color = _idleColor;
        }
    }
}
