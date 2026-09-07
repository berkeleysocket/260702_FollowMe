using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace YHW.UI
{
    /// <summary>
    /// One card in the stage-select carousel: stage number, best-score text
    /// and a 3-star progress readout (1 star at 33%, 2 at 66%, 3 at 100%).
    /// </summary>
    public class StageCardView : MonoBehaviour
    {
        [SerializeField] private Text stageNumberText;
        [SerializeField] private Text percentText;
        [SerializeField] private Image[] stars = new Image[3];
        [SerializeField] private Image cardBackground;

        [Header("Star Colors")]
        [SerializeField] private Color filledStarColor = new Color(1f, 0.85f, 0.2f);
        [SerializeField] private Color emptyStarColor = new Color(1f, 1f, 1f, 0.2f);

        [Header("Focus Look")]
        [SerializeField] private Color focusedBackgroundColor = new Color(1f, 1f, 1f, 0.14f);
        [SerializeField] private Color unfocusedBackgroundColor = new Color(1f, 1f, 1f, 0.05f);
        [SerializeField] private float focusedScale = 1f;
        [SerializeField] private float unfocusedScale = 0.8f;
        [SerializeField] private float transitionDuration = 0.4f;

        private float _percent;
        private CanvasGroup _canvasGroup;
        private Tween _scaleTween;
        private Tween _fadeTween;
        private Tween _bgTween;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public void Setup(int stageNumber, float percent)
        {
            _percent = Mathf.Clamp(percent, 0f, 100f);

            if (stageNumberText != null)
                stageNumberText.text = "STAGE " + stageNumber;

            if (percentText != null)
                percentText.text = Mathf.RoundToInt(_percent) + "% CLEAR";

            int starCount = StageProgressStore.GetStarCount(_percent);
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] == null) continue;
                stars[i].color = i < starCount ? filledStarColor : emptyStarColor;
                stars[i].transform.localScale = Vector3.one;
            }
        }

        public void PlayStarRevealAnimation()
        {
            int starCount = StageProgressStore.GetStarCount(_percent);
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] == null || i >= starCount) continue;

                Transform t = stars[i].transform;
                t.DOKill();
                t.localScale = Vector3.zero;
                t.DOScale(1f, 0.35f).SetEase(Ease.OutBack).SetDelay(i * 0.08f);
            }
        }

        public void SetFocused(bool focused, bool instant)
        {
            float targetScale = focused ? focusedScale : unfocusedScale;
            float targetAlpha = focused ? 1f : 0.55f;
            Color targetBg = focused ? focusedBackgroundColor : unfocusedBackgroundColor;

            _scaleTween?.Kill();
            _fadeTween?.Kill();
            _bgTween?.Kill();

            if (instant)
            {
                transform.localScale = Vector3.one * targetScale;
                if (_canvasGroup != null) _canvasGroup.alpha = targetAlpha;
                if (cardBackground != null) cardBackground.color = targetBg;
            }
            else
            {
                _scaleTween = transform.DOScale(targetScale, transitionDuration).SetEase(Ease.OutCubic);
                if (_canvasGroup != null)
                    _fadeTween = _canvasGroup.DOFade(targetAlpha, transitionDuration);
                if (cardBackground != null)
                    _bgTween = cardBackground.DOColor(targetBg, transitionDuration);
            }

            if (focused)
                PlayStarRevealAnimation();
        }
    }
}
