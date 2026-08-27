using UnityEngine;
using UnityEngine.InputSystem;

namespace FollowMe.KDS
{
    /// <summary>
    /// 맵 테스트용 간이 플레이어. 팀 플레이어가 들어오면 교체.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PhotoProbePlayer : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 7f;

        private Rigidbody2D _rb;
        private InputAction _moveAction;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 3f;
            _rb.freezeRotation = true;
            gameObject.tag = "Player";

            _moveAction = InputSystem.actions != null
                ? InputSystem.actions.FindAction("Player/Move", throwIfNotFound: false)
                : null;

            if (_moveAction == null)
            {
                _moveAction = new InputAction("ProbeMove", InputActionType.Value);
                _moveAction.AddCompositeBinding("2DVector")
                    .With("Up", "<Keyboard>/w")
                    .With("Down", "<Keyboard>/s")
                    .With("Left", "<Keyboard>/a")
                    .With("Right", "<Keyboard>/d");
                _moveAction.AddCompositeBinding("2DVector")
                    .With("Up", "<Keyboard>/upArrow")
                    .With("Down", "<Keyboard>/downArrow")
                    .With("Left", "<Keyboard>/leftArrow")
                    .With("Right", "<Keyboard>/rightArrow");
            }
        }

        private void OnEnable() => _moveAction?.Enable();

        public void RespawnAt(Vector3 worldPosition)
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();

            transform.position = worldPosition;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }

        private void FixedUpdate()
        {
            if (_moveAction == null) return;
            Vector2 input = _moveAction.ReadValue<Vector2>();
            Vector2 v = _rb.linearVelocity;
            v.x = input.x * _moveSpeed;
            // Jump optional later
            if (input.y > 0.5f && Mathf.Abs(_rb.linearVelocity.y) < 0.05f)
                v.y = 8f;
            _rb.linearVelocity = v;
        }
    }
}
