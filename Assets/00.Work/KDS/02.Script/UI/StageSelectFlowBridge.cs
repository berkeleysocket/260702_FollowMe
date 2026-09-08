using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YHW.UI;

namespace FollowMe.KDS
{
    /// <summary>
    /// Stage Select Scene: 씬 이름 연결 + 이전 스테이지 미클리어 시 잠금.
    /// YHW StageSelectController / StageCardView는 수정하지 않고 훅만 한다.
    /// </summary>
    public class StageSelectFlowBridge : MonoBehaviour
    {
        [SerializeField] private StageSelectController _select;
        [SerializeField] private bool _autoOpenOnStart = true;
        [SerializeField] private string _startSceneName = StageSceneCatalog.StartSceneName;
        [SerializeField] private Color _lockedCardColor = new Color(0.35f, 0.35f, 0.4f, 0.55f);

        private Button _playButton;
        private Button _backButton;
        private StageCardView[] _cards;
        private FieldInfo _indexField;
        private FieldInfo _sceneNamesField;

        private void Awake()
        {
            if (_select == null)
                _select = FindFirstObjectByType<StageSelectController>(FindObjectsInactive.Include);

            if (_select == null)
            {
                Debug.LogError("[StageSelectFlowBridge] StageSelectController 없음");
                return;
            }

            CacheFields();
            ApplyStageSceneNames();
            CacheButtonsAndCards();
            RemapButtons();
        }

        private void Start()
        {
            GameProgressSave.Load();

            if (_autoOpenOnStart && _select != null)
            {
                // 타이틀 패널 숨기고 스테이지 선택만 표시
                HideTitleChrome();
                _select.gameObject.SetActive(true);
                _select.Open(ReturnToStart);
            }

            // Open()이 Setup을 호출한 뒤 잠금 오버레이 적용
            ApplyLockVisuals();
            RefreshPlayInteractable();
        }

        private void Update()
        {
            // 캐러셀 이동 시 Play 버튼 잠금 상태 갱신
            RefreshPlayInteractable();
        }

        private void CacheFields()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            _indexField = typeof(StageSelectController).GetField("_currentIndex", flags);
            _sceneNamesField = typeof(StageSelectController).GetField("stageSceneNames", flags);
            var cardsField = typeof(StageSelectController).GetField("cards", flags);
            if (cardsField != null)
                _cards = cardsField.GetValue(_select) as StageCardView[];

            var playField = typeof(StageSelectController).GetField("playButton", flags);
            var backField = typeof(StageSelectController).GetField("backButton", flags);
            if (playField != null) _playButton = playField.GetValue(_select) as Button;
            if (backField != null) _backButton = backField.GetValue(_select) as Button;
        }

        private void CacheButtonsAndCards()
        {
            // already in CacheFields
        }

        private void ApplyStageSceneNames()
        {
            if (_sceneNamesField == null) return;
            int count = 10;
            if (_cards != null && _cards.Length > 0)
                count = _cards.Length;

            string[] names = StageSceneCatalog.BuildStageSceneNames(count);
            _sceneNamesField.SetValue(_select, names);

            var fallbackField = typeof(StageSelectController).GetField(
                "fallbackSceneName", BindingFlags.Instance | BindingFlags.NonPublic);
            fallbackField?.SetValue(_select, StageSceneCatalog.GetStageSceneName(1));
        }

        private void RemapButtons()
        {
            if (_playButton != null)
            {
                _playButton.onClick.RemoveAllListeners();
                _playButton.onClick.AddListener(OnPlayPressed);
            }

            if (_backButton != null)
            {
                _backButton.onClick.RemoveAllListeners();
                _backButton.onClick.AddListener(ReturnToStart);
            }
        }

        private void OnPlayPressed()
        {
            int index = GetCurrentIndex();
            int stage = index + 1;

            if (!GameProgressSave.IsStageUnlocked(stage))
            {
                Debug.Log($"[StageSelectFlowBridge] Stage {stage} 잠금 — 이전 스테이지를 클리어하세요.");
                return;
            }

            string sceneName = StageSceneCatalog.GetStageSceneName(stage);
            GameProgressSave.SetLastPlayedStage(stage);
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        private void ReturnToStart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_startSceneName);
        }

        private int GetCurrentIndex()
        {
            if (_indexField == null || _select == null) return 0;
            return (int)_indexField.GetValue(_select);
        }

        private void RefreshPlayInteractable()
        {
            if (_playButton == null) return;
            int stage = GetCurrentIndex() + 1;
            bool unlocked = GameProgressSave.IsStageUnlocked(stage);
            _playButton.interactable = unlocked;
        }

        private void ApplyLockVisuals()
        {
            if (_cards == null) return;

            for (int i = 0; i < _cards.Length; i++)
            {
                var card = _cards[i];
                if (card == null) continue;

                int stage = i + 1;
                bool unlocked = GameProgressSave.IsStageUnlocked(stage);
                float percent = GameProgressSave.GetBestFollowPercent(stage);
                int stars = GameProgressSave.GetBestStars(stage);
                if (stars > 0)
                {
                    float starPercent = stars >= 3 ? 100f : stars >= 2 ? 66f : 33f;
                    percent = Mathf.Max(percent, starPercent);
                }

                // YHW Setup으로 퍼센트 표시 갱신
                card.Setup(stage, unlocked ? percent : 0f);

                var cg = card.GetComponent<CanvasGroup>();
                if (cg == null) cg = card.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = unlocked ? 1f : 0.45f;

                // 잠금 텍스트
                var texts = card.GetComponentsInChildren<Text>(true);
                for (int t = 0; t < texts.Length; t++)
                {
                    if (texts[t] != null && texts[t].name.Contains("Percent"))
                    {
                        if (!unlocked)
                            texts[t].text = "LOCKED";
                        break;
                    }
                }

                var bgField = typeof(StageCardView).GetField(
                    "cardBackground", BindingFlags.Instance | BindingFlags.NonPublic);
                if (!unlocked && bgField != null)
                {
                    var bg = bgField.GetValue(card) as Image;
                    if (bg != null)
                        bg.color = _lockedCardColor;
                }
            }
        }

        private void HideTitleChrome()
        {
            var title = FindFirstObjectByType<TitleMenuController>();
            if (title == null) return;

            // 타이틀 그룹만 끄고 스테이지 선택 패널은 유지
            var titleGroupField = typeof(TitleMenuController).GetField(
                "titleGroup", BindingFlags.Instance | BindingFlags.NonPublic);
            var titleGroup = titleGroupField?.GetValue(title) as CanvasGroup;
            if (titleGroup != null)
            {
                titleGroup.alpha = 0f;
                titleGroup.interactable = false;
                titleGroup.blocksRaycasts = false;
            }

            // Start/Options/Quit 비활성
            DisableTitleButton("startButton");
            DisableTitleButton("optionsButton");
            DisableTitleButton("quitButton");

            void DisableTitleButton(string fieldName)
            {
                var f = typeof(TitleMenuController).GetField(
                    fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                var btn = f?.GetValue(title) as Button;
                if (btn != null)
                    btn.gameObject.SetActive(false);
            }
        }
    }
}
