using UnityEngine;
using UnityEngine.UI;

namespace FollowMe.KDS
{
    /// <summary>
    /// 산나비식 말풍선: 화자의 월드 좌표를 화면으로 변환해 머리 위에 붙인다.
    /// </summary>
    public class SpeechBubbleView : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private RectTransform _bubbleRoot;
        [SerializeField] private RectTransform _tail;
        [SerializeField] private Text _bodyText;
        [SerializeField] private GameObject _advancePrompt;
        [SerializeField] private Vector2 _screenOffset = new Vector2(0f, 140f);
        [SerializeField] private Vector2 _padding = new Vector2(48f, 36f);
        [SerializeField] private Vector2 _minSize = new Vector2(280f, 72f);
        [SerializeField] private Vector2 _maxSize = new Vector2(720f, 220f);
        [SerializeField] private float _screenMargin = 24f;

        private Transform _follow;
        private Canvas _canvas;

        public bool IsVisible => _bubbleRoot != null && _bubbleRoot.gameObject.activeSelf;

        private void Awake()
        {
            if (_camera == null)
                _camera = Camera.main;

            if (_bubbleRoot == null)
                BuildDefaultVisuals();

            Hide();
        }

        public void Show(string text, Transform follow, bool showAdvancePrompt)
        {
            _follow = follow;

            if (_bubbleRoot == null)
                BuildDefaultVisuals();

            if (_bodyText != null)
                _bodyText.text = text ?? string.Empty;

            if (_advancePrompt != null)
                _advancePrompt.SetActive(showAdvancePrompt);

            _bubbleRoot.gameObject.SetActive(true);
            RefreshLayout();
            UpdatePosition();
        }

        public void Hide()
        {
            _follow = null;
            if (_bubbleRoot != null)
                _bubbleRoot.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!IsVisible)
                return;

            UpdatePosition();
        }

        private void RefreshLayout()
        {
            if (_bodyText == null || _bubbleRoot == null)
                return;

            var settings = _bodyText.GetGenerationSettings(new Vector2(_maxSize.x - _padding.x, 0f));
            settings.horizontalOverflow = HorizontalWrapMode.Wrap;
            settings.verticalOverflow = VerticalWrapMode.Overflow;

            var generator = new TextGenerator();
            generator.Populate(_bodyText.text, settings);

            float textW = Mathf.Min(generator.GetPreferredWidth(_bodyText.text, settings), _maxSize.x - _padding.x);
            float textH = generator.GetPreferredHeight(_bodyText.text, settings);

            float width = Mathf.Clamp(textW + _padding.x, _minSize.x, _maxSize.x);
            float height = Mathf.Clamp(textH + _padding.y, _minSize.y, _maxSize.y);
            _bubbleRoot.sizeDelta = new Vector2(width, height);
        }

        private void UpdatePosition()
        {
            if (_bubbleRoot == null)
                return;

            if (_camera == null)
                _camera = Camera.main;

            Vector2 screenPoint;
            if (_follow != null && _camera != null)
            {
                Vector3 world = _follow.position;
                screenPoint = _camera.WorldToScreenPoint(world);
                screenPoint += _screenOffset;
            }
            else
            {
                screenPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.72f);
            }

            RectTransform canvasRect = _canvas != null
                ? _canvas.transform as RectTransform
                : _bubbleRoot.parent as RectTransform;

            if (canvasRect == null)
                return;

            Camera eventCam = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPoint, eventCam, out Vector2 local);

            Vector2 half = _bubbleRoot.sizeDelta * 0.5f;
            float maxX = (canvasRect.rect.width * 0.5f) - half.x - _screenMargin;
            float maxY = (canvasRect.rect.height * 0.5f) - half.y - _screenMargin;
            local.x = Mathf.Clamp(local.x, -maxX, maxX);
            local.y = Mathf.Clamp(local.y, -maxY, maxY);

            _bubbleRoot.anchoredPosition = local;

            if (_tail != null && _follow != null && _camera != null)
            {
                Vector2 speakerScreen = _camera.WorldToScreenPoint(_follow.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _bubbleRoot, speakerScreen, eventCam, out Vector2 tailLocal);

                float x = Mathf.Clamp(tailLocal.x, -half.x + 18f, half.x - 18f);
                _tail.anchoredPosition = new Vector2(x, -half.y + 2f);
            }
        }

        private void BuildDefaultVisuals()
        {
            _canvas = GetComponent<Canvas>();
            if (_canvas == null)
            {
                _canvas = gameObject.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.sortingOrder = 100;
                gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                gameObject.AddComponent<GraphicRaycaster>();
            }

            var bubbleGo = new GameObject("Bubble", typeof(RectTransform), typeof(Image));
            bubbleGo.transform.SetParent(transform, false);
            _bubbleRoot = bubbleGo.GetComponent<RectTransform>();
            _bubbleRoot.anchorMin = new Vector2(0.5f, 0.5f);
            _bubbleRoot.anchorMax = new Vector2(0.5f, 0.5f);
            _bubbleRoot.pivot = new Vector2(0.5f, 0.5f);
            _bubbleRoot.sizeDelta = _minSize;

            var bubbleImage = bubbleGo.GetComponent<Image>();
            bubbleImage.color = Color.white;

            var tailGo = new GameObject("Tail", typeof(RectTransform), typeof(Image));
            tailGo.transform.SetParent(_bubbleRoot, false);
            _tail = tailGo.GetComponent<RectTransform>();
            _tail.anchorMin = new Vector2(0.5f, 0f);
            _tail.anchorMax = new Vector2(0.5f, 0f);
            _tail.pivot = new Vector2(0.5f, 1f);
            _tail.sizeDelta = new Vector2(22f, 16f);
            _tail.localRotation = Quaternion.Euler(0f, 0f, 45f);
            _tail.anchoredPosition = new Vector2(0f, 8f);
            tailGo.GetComponent<Image>().color = Color.white;

            var textGo = new GameObject("Body", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(_bubbleRoot, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(20f, 16f);
            textRect.offsetMax = new Vector2(-20f, -16f);

            _bodyText = textGo.GetComponent<Text>();
            _bodyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_bodyText.font == null)
                _bodyText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            _bodyText.fontSize = 26;
            _bodyText.color = Color.black;
            _bodyText.alignment = TextAnchor.MiddleCenter;
            _bodyText.horizontalOverflow = HorizontalWrapMode.Wrap;
            _bodyText.verticalOverflow = VerticalWrapMode.Overflow;

            var promptGo = new GameObject("AdvancePrompt", typeof(RectTransform), typeof(Image));
            promptGo.transform.SetParent(_bubbleRoot, false);
            var promptRect = promptGo.GetComponent<RectTransform>();
            promptRect.anchorMin = new Vector2(1f, 0f);
            promptRect.anchorMax = new Vector2(1f, 0f);
            promptRect.pivot = new Vector2(1f, 0f);
            promptRect.anchoredPosition = new Vector2(-12f, 10f);
            promptRect.sizeDelta = new Vector2(14f, 14f);
            promptGo.GetComponent<Image>().color = new Color(0.25f, 0.55f, 1f, 1f);
            _advancePrompt = promptGo;
        }
    }
}
