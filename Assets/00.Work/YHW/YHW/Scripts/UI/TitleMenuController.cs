using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace YHW.UI
{
    public class TitleMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private CanvasGroup titleGroup;
        [SerializeField] private OptionsMenuController optionsMenu;
        [SerializeField] private StageSelectController stageSelectMenu;

        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;

        [Header("Title Fx")]
        [SerializeField] private RectTransform titleLogo;
        [SerializeField] private float logoFloatDistance = 12f;
        [SerializeField] private float logoFloatDuration = 1.6f;

        private Vector2 _logoStartPos;

        private void Awake()
        {
            startButton.onClick.AddListener(OnStartClicked);
            optionsButton.onClick.AddListener(OnOptionsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void Start()
        {
            titleGroup.alpha = 0f;
            titleGroup.DOFade(1f, 0.8f).SetEase(Ease.OutQuad);

            if (titleLogo != null)
            {
                _logoStartPos = titleLogo.anchoredPosition;
                titleLogo.DOAnchorPosY(_logoStartPos.y + logoFloatDistance, logoFloatDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(titleLogo);
            DOTween.Kill(titleGroup);
        }

        private void OnStartClicked()
        {
            SetInteractable(false);
            stageSelectMenu.Open(() => SetInteractable(true));
        }

        private void OnOptionsClicked()
        {
            SetInteractable(false);
            optionsMenu.Open(() => SetInteractable(true));
        }

        private void OnQuitClicked()
        {
            QuitGame();
        }

        private void SetInteractable(bool value)
        {
            startButton.interactable = value;
            optionsButton.interactable = value;
            quitButton.interactable = value;
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
