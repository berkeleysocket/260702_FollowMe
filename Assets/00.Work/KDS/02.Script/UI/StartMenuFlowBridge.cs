using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YHW.UI;

namespace FollowMe.KDS
{
    /// <summary>
    /// Start Scene: 게임 시작 버튼 → Stage Select Scene.
    /// YHW TitleMenuController는 수정하지 않고 버튼 리스너만 교체한다.
    /// </summary>
    public class StartMenuFlowBridge : MonoBehaviour
    {
        [SerializeField] private string _stageSelectSceneName = StageSceneCatalog.StageSelectSceneName;
        [SerializeField] private Button _startButtonOverride;

        private void Start()
        {
            WireStartButton();
        }

        private void WireStartButton()
        {
            Button start = _startButtonOverride;
            if (start == null)
            {
                var title = FindFirstObjectByType<TitleMenuController>();
                if (title != null)
                {
                    var field = typeof(TitleMenuController).GetField(
                        "startButton",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                        start = field.GetValue(title) as Button;
                }
            }

            if (start == null)
            {
                var buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
                for (int i = 0; i < buttons.Length; i++)
                {
                    string n = buttons[i].gameObject.name.ToLowerInvariant();
                    if (n.Contains("start") || n.Contains("게임시작") || n == "startbutton")
                    {
                        start = buttons[i];
                        break;
                    }
                }
            }

            if (start == null)
            {
                Debug.LogWarning("[StartMenuFlowBridge] Start 버튼을 찾지 못했습니다.");
                return;
            }

            start.onClick.RemoveAllListeners();
            start.onClick.AddListener(GoToStageSelect);
            Debug.Log("[StartMenuFlowBridge] Start → " + _stageSelectSceneName);
        }

        public void GoToStageSelect()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_stageSelectSceneName);
        }
    }
}
