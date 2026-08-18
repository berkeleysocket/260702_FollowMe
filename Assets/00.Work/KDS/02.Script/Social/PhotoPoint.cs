using UnityEngine;
using UnityEngine.InputSystem;

namespace FollowMe.KDS
{
    /// <summary>
    /// 사진 포인트: 플레이어가 범위 안에서 Interact로 촬영하면 좋아요/팔로우 대폭 상승.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PhotoPoint : MonoBehaviour
    {
        [SerializeField] private string _pointId = "PhotoPoint";
        [SerializeField] private PhotoPointRewardSO _reward;
        [SerializeField] private long _fallbackLikeBonus = 5000;
        [SerializeField] private long _fallbackFollowBonus = 200;
        [SerializeField] private bool _oneShot = true;
        [SerializeField] private GameObject _availableVisual;
        [SerializeField] private GameObject _usedVisual;
        [SerializeField] private GameObject _promptVisual;

        private bool _playerInside;
        private bool _used;
        private InputAction _interactAction;

        public bool IsUsed => _used;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;

            // Input System Player map Interact (기존 프로젝트 액션 재사용)
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
            if (_interactAction != null)
                _interactAction.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            if (_interactAction != null)
                _interactAction.performed -= OnInteractPerformed;
            // 우리가 만든 로컬 액션만 Disable 해도 됨. FindAction은 공용일 수 있음.
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsPlayer(other)) return;
            _playerInside = true;
            RefreshVisuals();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsPlayer(other)) return;
            _playerInside = false;
            RefreshVisuals();
        }

        private void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            if (!_playerInside || _used) return;
            TryTakePhoto();
        }

        public bool TryTakePhoto()
        {
            if (_used) return false;

            long likes = _reward != null ? _reward.LikeBonus : _fallbackLikeBonus;
            long follows = _reward != null ? _reward.FollowBonus : _fallbackFollowBonus;
            bool oneShot = _reward != null ? _reward.OneShot : _oneShot;
            string id = string.IsNullOrEmpty(_pointId) ? gameObject.name : _pointId;

            if (SocialScoreService.Instance == null)
            {
                Debug.LogWarning("[PhotoPoint] SocialScoreService 없음. 씬에 배치하세요.");
                return false;
            }

            SocialScoreService.Instance.ApplyPhotoReward(id, likes, follows);

            if (oneShot)
                _used = true;

            RefreshVisuals();
            Debug.Log($"[PhotoPoint] {id} 촬영! +좋아요 {likes}, +팔로우 {follows}");
            return true;
        }

        private static bool IsPlayer(Collider2D other)
        {
            return other.CompareTag("Player") || other.GetComponentInParent<PhotoProbePlayer>() != null;
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
    }
}
