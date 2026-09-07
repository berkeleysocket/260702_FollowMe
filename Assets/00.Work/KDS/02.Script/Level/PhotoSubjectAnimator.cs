using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 포토존 피사체: 스프라이트 프레임 루프 + 약한 보빙.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PhotoSubjectAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Sprite[] _frames;
        [SerializeField] private float _fps = 4f;
        [SerializeField] private float _bobAmount = 0.04f;
        [SerializeField] private float _bobSpeed = 2.2f;
        [SerializeField] private bool _bob = true;

        private Vector3 _basePos;
        private float _timer;
        private int _index;

        private void Awake()
        {
            if (_renderer == null)
                _renderer = GetComponent<SpriteRenderer>();
            _basePos = transform.localPosition;
            if (_frames != null && _frames.Length > 0 && _renderer != null)
                _renderer.sprite = _frames[0];
        }

        private void OnEnable()
        {
            _basePos = transform.localPosition;
        }

        private void Update()
        {
            if (_frames != null && _frames.Length > 1 && _fps > 0f)
            {
                _timer += Time.deltaTime;
                float step = 1f / _fps;
                if (_timer >= step)
                {
                    _timer -= step;
                    _index = (_index + 1) % _frames.Length;
                    if (_renderer != null)
                        _renderer.sprite = _frames[_index];
                }
            }

            if (_bob)
            {
                float y = Mathf.Sin(Time.time * _bobSpeed) * _bobAmount;
                transform.localPosition = _basePos + new Vector3(0f, y, 0f);
            }
        }

        public void SetFrames(Sprite[] frames, float fps = 4f)
        {
            _frames = frames;
            _fps = fps;
            _index = 0;
            _timer = 0f;
            if (_renderer == null)
                _renderer = GetComponent<SpriteRenderer>();
            if (_frames != null && _frames.Length > 0 && _renderer != null)
                _renderer.sprite = _frames[0];
        }
    }
}
