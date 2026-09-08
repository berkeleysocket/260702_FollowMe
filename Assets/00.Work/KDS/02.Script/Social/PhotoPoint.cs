using UnityEngine;
using UnityEngine.InputSystem;

namespace FollowMe.KDS
{
    /// <summary>
    /// 포토존: 범위 안 + Interact(E) 홀드로 촬영 → 좋아요/팔로우 대폭 상승.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PhotoPoint : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private string _pointId = "PhotoPoint";
        [SerializeField] private PhotoPointRewardSO _reward;

        [Header("Fallback Reward")]
        [SerializeField] private long _fallbackLikeBonus = 5000;
        [SerializeField] private long _fallbackFollowBonus = 200;
        [SerializeField] private float _fallbackHoldSeconds = 0.85f;
        [SerializeField] private bool _oneShot = true;

        [Header("Visuals")]
        [SerializeField] private GameObject _availableVisual;
        [SerializeField] private GameObject _usedVisual;
        [SerializeField] private GameObject _promptVisual;

        private bool _playerInside;
        private bool _used;
        private float _holdTimer;
        private InputAction _interactAction;

        /// <summary>현재 홀드 중인 포토존(HUD용).</summary>
        public static PhotoPoint Active { get; private set; }

        /// <summary>플레이어가 들어와 있는 포토존(프롬프트용).</summary>
        public static PhotoPoint Nearby { get; private set; }

        public bool IsUsed => _used;
        public bool IsPlayerInside => _playerInside;
        public bool IsHolding => Active == this && _holdTimer > 0f && !_used;
        public float HoldProgress { get; private set; }
        public string PointId => string.IsNullOrEmpty(_pointId) ? gameObject.name : _pointId;
        public string DisplayName => _reward != null ? _reward.DisplayName : PointId;
        public string HashtagLine => _reward != null ? _reward.HashtagLine : string.Empty;
        public Sprite PhotoSprite => _reward != null ? _reward.PhotoSprite : null;
        public long PreviewLikeBonus => _reward != null ? _reward.LikeBonus : _fallbackLikeBonus;
        public long PreviewFollowBonus => _reward != null ? _reward.FollowBonus : _fallbackFollowBonus;

        private float RequiredHoldSeconds =>
            _reward != null ? _reward.HoldSeconds : Mathf.Max(0.05f, _fallbackHoldSeconds);

        private void Awake()
        {
            MapTriggerLayer.Apply(gameObject);

            var col = GetComponent<Collider2D>();
            col.isTrigger = true;

            // 프로젝트 Interact 액션은 Hold interaction이 붙어 있어
            // 포토존 자체 홀드 타이머와 겹친다 → E/F 전용 액션 사용
            _interactAction = new InputAction("PhotoInteract", InputActionType.Button);
            _interactAction.AddBinding("<Keyboard>/e");
            _interactAction.AddBinding("<Keyboard>/f");
            _interactAction.AddBinding("<Gamepad>/buttonNorth");

            RefreshVisuals();
        }

        private void OnEnable()
        {
            _interactAction?.Enable();
        }

        private void OnDisable()
        {
            CancelHold();
            if (Active == this)
                Active = null;
            if (Nearby == this)
                Nearby = null;
            _interactAction?.Disable();
        }

        private void OnDestroy()
        {
            if (_interactAction == null) return;
            _interactAction.Dispose();
            _interactAction = null;
        }

        private void FixedUpdate()
        {
            // 레이어/트리거 누락 대비: 존 안이면 Inside로 간주
            RefreshInsideByOverlap();
        }

        private void Update()
        {
            if (_used || !_playerInside || _interactAction == null)
            {
                CancelHold();
                return;
            }

            bool holding = _interactAction.IsPressed()
                           || (Keyboard.current != null && Keyboard.current.eKey.isPressed)
                           || (Keyboard.current != null && Keyboard.current.fKey.isPressed);

            if (holding)
            {
                if (Active != null && Active != this)
                    return;

                Active = this;
                _holdTimer += Time.deltaTime;
                HoldProgress = Mathf.Clamp01(_holdTimer / RequiredHoldSeconds);

                if (_holdTimer >= RequiredHoldSeconds)
                    TryTakePhoto();
            }
            else
            {
                CancelHold();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsPlayer(other)) return;
            SetPlayerInside(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsPlayer(other)) return;
            // Overlap 폴백이 있으면 FixedUpdate가 다시 잡음
            SetPlayerInside(false);
        }

        private void RefreshInsideByOverlap()
        {
            if (_used) return;

            var col = GetComponent<Collider2D>();
            if (col == null) return;

            var filter = new ContactFilter2D();
            filter.NoFilter();
            filter.useTriggers = true;

            var hits = new Collider2D[8];
            int count = col.Overlap(filter, hits);
            bool inside = false;
            for (int i = 0; i < count; i++)
            {
                if (IsPlayer(hits[i]))
                {
                    inside = true;
                    break;
                }
            }

            // Physics2D 레이어 무시 시 Overlap도 실패할 수 있어 거리 폴백
            if (!inside)
            {
                var player = PlayerRespawn.FindInScene();
                if (player != null)
                {
                    Bounds b = col.bounds;
                    b.Expand(0.35f);
                    inside = b.Contains(player.transform.position);
                }
            }

            if (inside != _playerInside)
                SetPlayerInside(inside);
        }

        private void SetPlayerInside(bool inside)
        {
            _playerInside = inside;
            if (inside)
            {
                if (!_used)
                    Nearby = this;
            }
            else
            {
                if (Nearby == this)
                    Nearby = null;
                CancelHold();
            }

            RefreshVisuals();
        }

        public bool TryTakePhoto()
        {
            if (_used) return false;
            if (!_playerInside) return false;

            long likes = PreviewLikeBonus;
            long follows = PreviewFollowBonus;
            bool oneShot = _reward != null ? _reward.OneShot : _oneShot;
            string id = PointId;

            if (SocialScoreService.Instance == null)
            {
                Debug.LogWarning("[PhotoPoint] SocialScoreService 없음. 씬에 배치하세요.", this);
                CancelHold();
                return false;
            }

            if (!SocialScoreService.Instance.ApplyPhotoReward(id, likes, follows))
            {
                CancelHold();
                Debug.Log($"[PhotoPoint] {id} 촬영 실패 — 스트레스가 0%가 아님", this);
                return false;
            }

            if (oneShot)
                _used = true;

            CancelHold();
            RefreshVisuals();
            Debug.Log($"[PhotoPoint] {id} 촬영! +좋아요 {likes}, +팔로우 {follows}");
            return true;
        }

        private void CancelHold()
        {
            _holdTimer = 0f;
            HoldProgress = 0f;
            if (Active == this)
                Active = null;
        }

        private static bool IsPlayer(Collider2D other)
        {
            return PlayerTriggerUtility.IsPlayer(other);
        }

        private void RefreshVisuals()
        {
            if (_availableVisual != null)
                _availableVisual.SetActive(!_used);
            if (_usedVisual != null)
                _usedVisual.SetActive(_used);
            if (_promptVisual != null)
                _promptVisual.SetActive(_playerInside && !_used);
        }

        private void OnDrawGizmos()
        {
            var col = GetComponent<Collider2D>();
            if (col == null) return;

            Gizmos.color = _used
                ? new Color(0.5f, 0.5f, 0.5f, 0.25f)
                : new Color(1f, 0.45f, 0.85f, 0.28f);
            Gizmos.matrix = transform.localToWorldMatrix;

            if (col is BoxCollider2D box)
                Gizmos.DrawCube(box.offset, box.size);
            else if (col is CircleCollider2D circle)
                Gizmos.DrawSphere(circle.offset, circle.radius);
        }
    }
}
