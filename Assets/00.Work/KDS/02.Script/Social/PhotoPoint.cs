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
        public long PreviewLikeBonus => _reward != null ? _reward.LikeBonus : _fallbackLikeBonus;
        public long PreviewFollowBonus => _reward != null ? _reward.FollowBonus : _fallbackFollowBonus;

        private float RequiredHoldSeconds =>
            _reward != null ? _reward.HoldSeconds : Mathf.Max(0.05f, _fallbackHoldSeconds);

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;

            _interactAction = InputSystem.actions != null
                ? InputSystem.actions.FindAction("Player/Interact", throwIfNotFound: false)
                : null;

            if (_interactAction == null)
            {
                _interactAction = new InputAction("PhotoInteract", InputActionType.Button);
                _interactAction.AddBinding("<Keyboard>/e");
                _interactAction.AddBinding("<Keyboard>/f");
                _interactAction.AddBinding("<Gamepad>/buttonNorth");
            }

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
        }

        private void Update()
        {
            if (_used || !_playerInside || _interactAction == null)
            {
                CancelHold();
                return;
            }

            if (_interactAction.IsPressed())
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
            _playerInside = true;
            if (!_used)
                Nearby = this;
            RefreshVisuals();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsPlayer(other)) return;
            _playerInside = false;
            if (Nearby == this)
                Nearby = null;
            CancelHold();
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

            SocialScoreService.Instance.ApplyPhotoReward(id, likes, follows);

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
