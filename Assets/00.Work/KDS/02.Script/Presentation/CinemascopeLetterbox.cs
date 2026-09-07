using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FollowMe.KDS
{
    /// <summary>
    /// 컷씬 재생 시 2.39:1 시네마스코프 레터박스(상·하단 검은 바)를 표시한다.
    /// </summary>
    public class CinemascopeLetterbox : MonoBehaviour
    {
        public static CinemascopeLetterbox Instance { get; private set; }

        [SerializeField] private float _aspectRatio = 2.39f;
        [SerializeField] private float _animateSeconds = 0.35f;
        [SerializeField] private Color _barColor = Color.black;
        [SerializeField] private int _sortOrder = 900;

        private Canvas _canvas;
        private RectTransform _topBar;
        private RectTransform _bottomBar;
        private Coroutine _animateRoutine;
        private float _shownBarFraction;

        public bool IsVisible => _shownBarFraction > 0.001f;
        public float TargetBarFraction => CalculateBarFraction();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            EnsureVisuals();
            ApplyBarFraction(0f, instant: true);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!IsVisible)
                return;

            ApplyBarFraction(_shownBarFraction, instant: true);
        }

        public void Show(bool instant = false)
        {
            AnimateTo(CalculateBarFraction(), instant);
        }

        public void Hide(bool instant = false)
        {
            AnimateTo(0f, instant);
        }

        private void AnimateTo(float target, bool instant)
        {
            if (_animateRoutine != null)
            {
                StopCoroutine(_animateRoutine);
                _animateRoutine = null;
            }

            if (instant || _animateSeconds <= 0f)
            {
                ApplyBarFraction(target, instant: true);
                return;
            }

            _animateRoutine = StartCoroutine(AnimateRoutine(target));
        }

        private IEnumerator AnimateRoutine(float target)
        {
            float start = _shownBarFraction;
            float elapsed = 0f;

            while (elapsed < _animateSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / _animateSeconds);
                t = t * t * (3f - 2f * t);
                ApplyBarFraction(Mathf.Lerp(start, target, t), instant: false);
                yield return null;
            }

            ApplyBarFraction(target, instant: true);
            _animateRoutine = null;
        }

        private float CalculateBarFraction()
        {
            if (Screen.height <= 0)
                return 0f;

            float windowAspect = (float)Screen.width / Screen.height;
            float visibleHeight = windowAspect / Mathf.Max(0.01f, _aspectRatio);
            if (visibleHeight >= 1f)
                return 0f;

            return (1f - visibleHeight) * 0.5f;
        }

        private void ApplyBarFraction(float fraction, bool instant)
        {
            _shownBarFraction = Mathf.Clamp01(fraction);
            EnsureVisuals();

            bool visible = _shownBarFraction > 0.001f;
            _canvas.enabled = visible;

            if (_topBar != null)
            {
                _topBar.anchorMin = new Vector2(0f, 1f - _shownBarFraction);
                _topBar.anchorMax = Vector2.one;
                _topBar.offsetMin = Vector2.zero;
                _topBar.offsetMax = Vector2.zero;
            }

            if (_bottomBar != null)
            {
                _bottomBar.anchorMin = Vector2.zero;
                _bottomBar.anchorMax = new Vector2(1f, _shownBarFraction);
                _bottomBar.offsetMin = Vector2.zero;
                _bottomBar.offsetMax = Vector2.zero;
            }

            if (!instant)
                return;
        }

        private void EnsureVisuals()
        {
            if (_canvas != null && _topBar != null && _bottomBar != null)
                return;

            _canvas = GetComponent<Canvas>();
            if (_canvas == null)
            {
                _canvas = gameObject.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.sortingOrder = _sortOrder;
                var scaler = gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                gameObject.AddComponent<GraphicRaycaster>();
            }

            _topBar = CreateBar("TopBar");
            _bottomBar = CreateBar("BottomBar");
        }

        private RectTransform CreateBar(string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(transform, false);

            var rect = go.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 0.5f);
            go.GetComponent<Image>().color = _barColor;
            go.GetComponent<Image>().raycastTarget = false;
            return rect;
        }
    }
}
