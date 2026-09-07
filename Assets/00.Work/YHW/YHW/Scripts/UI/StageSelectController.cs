using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace YHW.UI
{
    /// <summary>
    /// Drives the stage-select carousel: drag or Prev/Next/slider input smoothly
    /// snaps the track to whichever stage card is centered.
    /// </summary>
    public class StageSelectController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Panel")]
        [SerializeField] private CanvasGroup panelGroup;
        [SerializeField] private RectTransform panelCard;

        [Header("Carousel")]
        [SerializeField] private RectTransform track;
        [SerializeField] private StageCardView[] cards;
        [SerializeField] private float cardSpacing = 680f;
        [SerializeField] private float moveDuration = 0.45f;

        [Header("Controls")]
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Slider stageSlider;
        [SerializeField] private Image[] pageDots;
        [SerializeField] private Color dotActiveColor = Color.white;
        [SerializeField] private Color dotInactiveColor = new Color(1f, 1f, 1f, 0.3f);

        [Header("Scenes")]
        [SerializeField] private string[] stageSceneNames = new string[10];
        [SerializeField] private string fallbackSceneName = "HWTestScene 1";

        public event Action<int> StageConfirmed;

        private int _currentIndex;
        private Tween _trackTween;
        private bool _dragging;
        private Action _onClose;

        private void Awake()
        {
            if (prevButton != null) prevButton.onClick.AddListener(GoPrev);
            if (nextButton != null) nextButton.onClick.AddListener(GoNext);
            if (playButton != null) playButton.onClick.AddListener(ConfirmSelection);
            if (backButton != null) backButton.onClick.AddListener(Close);

            if (stageSlider != null)
            {
                stageSlider.wholeNumbers = true;
                stageSlider.minValue = 0;
                stageSlider.maxValue = Mathf.Max(0, cards.Length - 1);
                stageSlider.onValueChanged.AddListener(OnSliderChanged);
            }

            if (panelGroup != null)
            {
                panelGroup.alpha = 0f;
                panelGroup.interactable = false;
                panelGroup.blocksRaycasts = false;
            }
        }

        private void OnDestroy()
        {
            _trackTween?.Kill();
        }

        public void Open(Action onClose)
        {
            _onClose = onClose;

            for (int i = 0; i < cards.Length; i++)
                cards[i].Setup(i + 1, StageProgressStore.GetPercent(i));

            gameObject.SetActive(true);
            GoTo(_currentIndex, true);

            if (panelGroup != null)
            {
                panelGroup.interactable = true;
                panelGroup.blocksRaycasts = true;
                panelGroup.alpha = 0f;
                panelGroup.DOFade(1f, 0.25f);
            }

            if (panelCard != null)
            {
                panelCard.localScale = Vector3.one * 0.92f;
                panelCard.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            }
        }

        private void Close()
        {
            if (panelGroup == null)
            {
                gameObject.SetActive(false);
                _onClose?.Invoke();
                _onClose = null;
                return;
            }

            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
            panelGroup.DOFade(0f, 0.2f);

            RectTransform target = panelCard != null ? panelCard : panelGroup.GetComponent<RectTransform>();
            target.DOScale(0.92f, 0.2f).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    _onClose?.Invoke();
                    _onClose = null;
                });
        }

        private void GoNext() => GoTo(_currentIndex + 1, false);

        private void GoPrev() => GoTo(_currentIndex - 1, false);

        private void OnSliderChanged(float value) => GoTo(Mathf.RoundToInt(value), false);

        private void GoTo(int index, bool instant)
        {
            if (cards == null || cards.Length == 0) return;

            index = Mathf.Clamp(index, 0, cards.Length - 1);
            bool changed = index != _currentIndex || instant;
            _currentIndex = index;

            if (stageSlider != null)
                stageSlider.SetValueWithoutNotify(_currentIndex);

            for (int i = 0; i < cards.Length; i++)
                cards[i].SetFocused(i == _currentIndex, instant);

            UpdateDots();
            UpdateNavButtons();

            if (track != null && changed)
            {
                float targetX = -_currentIndex * cardSpacing;
                _trackTween?.Kill();

                if (instant)
                {
                    Vector2 pos = track.anchoredPosition;
                    pos.x = targetX;
                    track.anchoredPosition = pos;
                }
                else
                {
                    _trackTween = track.DOAnchorPosX(targetX, moveDuration).SetEase(Ease.OutCubic);
                }
            }
        }

        private void UpdateDots()
        {
            if (pageDots == null) return;
            for (int i = 0; i < pageDots.Length; i++)
            {
                if (pageDots[i] == null) continue;
                pageDots[i].color = i == _currentIndex ? dotActiveColor : dotInactiveColor;
            }
        }

        private void UpdateNavButtons()
        {
            if (prevButton != null) prevButton.interactable = _currentIndex > 0;
            if (nextButton != null) nextButton.interactable = _currentIndex < cards.Length - 1;
        }

        private void ConfirmSelection()
        {
            StageConfirmed?.Invoke(_currentIndex);

            string sceneName = null;
            if (stageSceneNames != null && _currentIndex < stageSceneNames.Length)
                sceneName = stageSceneNames[_currentIndex];

            if (string.IsNullOrEmpty(sceneName))
                sceneName = fallbackSceneName;

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("[StageSelectController] No scene configured for stage " + (_currentIndex + 1));
                return;
            }

            if (panelGroup != null)
            {
                panelGroup.interactable = false;
                panelGroup.blocksRaycasts = false;
                panelGroup.DOFade(0f, 0.3f).OnComplete(() => SceneManager.LoadScene(sceneName));
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (track == null) return;
            _dragging = true;
            _trackTween?.Kill();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragging || track == null) return;
            Vector2 pos = track.anchoredPosition;
            pos.x += eventData.delta.x;
            track.anchoredPosition = pos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_dragging || track == null) return;
            _dragging = false;

            int nearest = Mathf.RoundToInt(-track.anchoredPosition.x / cardSpacing);
            GoTo(nearest, false);
        }

        [ContextMenu("Debug/Randomize Progress")]
        private void DebugRandomizeProgress()
        {
            for (int i = 0; i < cards.Length; i++)
            {
                float percent = UnityEngine.Random.Range(0, 101);
                StageProgressStore.DebugSetPercent(i, percent);
                cards[i].Setup(i + 1, percent);
            }
            if (_currentIndex >= 0 && _currentIndex < cards.Length)
                cards[_currentIndex].SetFocused(true, true);
        }
    }
}
