using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FollowMe.KDS
{
    /// <summary>
    /// 스테이지 플레이 중 Esc로 여는 일시정지·설정 창.
    /// YHW 타이틀 옵션과 같은 Prefs를 쓴다. (YHW 폴더 수정 없음)
    /// </summary>
    public class InGamePauseMenu : MonoBehaviour
    {
        private bool _open;
        private float _savedTimeScale = 1f;

        private float _master = 1f;
        private float _bgm = 0.8f;
        private float _sfx = 0.8f;
        private bool _fullscreen = true;
        private int _qualityIndex;
        private int _resolutionIndex;
        private List<Resolution> _resolutions = new List<Resolution>();
        private string[] _resolutionLabels = System.Array.Empty<string>();
        private Vector2 _scroll;

        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;
        private bool _stylesReady;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoSpawn()
        {
            if (!IsPlayableStageScene(SceneManager.GetActiveScene().name))
                return;

            if (Object.FindFirstObjectByType<InGamePauseMenu>() != null)
                return;

            GameSettingsStore.ApplyBootSettings();

            var go = new GameObject("InGamePauseMenu");
            go.AddComponent<InGamePauseMenu>();
        }

        public static bool IsPlayableStageScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return false;
            if (sceneName.IndexOf("Select", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return false;
            // "Stage1 Scene" ~ "Stage16 Scene"
            if (!sceneName.StartsWith("Stage", System.StringComparison.Ordinal))
                return false;
            return sceneName.IndexOf(" Scene", System.StringComparison.Ordinal) >= 0;
        }

        public bool IsOpen => _open;

        private void Update()
        {
            if (IsStageClearBlocking())
            {
                if (_open)
                    Close(resumeTime: false);
                return;
            }

            if (WasTogglePressed())
            {
                if (_open) Close(resumeTime: true);
                else Open();
            }
        }

        private static bool WasTogglePressed()
        {
            var kb = Keyboard.current;
            if (kb != null && (kb.escapeKey.wasPressedThisFrame || kb.pKey.wasPressedThisFrame))
                return true;

            var pad = Gamepad.current;
            if (pad != null && pad.startButton.wasPressedThisFrame)
                return true;

            return false;
        }

        private bool IsStageClearBlocking()
        {
            var goal = Object.FindFirstObjectByType<StageGoal>();
            return goal != null && goal.IsCleared;
        }

        private void Open()
        {
            if (_open) return;

            _savedTimeScale = Time.timeScale > 0.01f ? Time.timeScale : 1f;
            Time.timeScale = 0f;
            _open = true;
            LoadFromPrefs();
            BuildResolutionList();
        }

        private void Close(bool resumeTime)
        {
            if (!_open) return;
            _open = false;
            if (resumeTime)
                Time.timeScale = _savedTimeScale > 0.01f ? _savedTimeScale : 1f;
        }

        private void LoadFromPrefs()
        {
            _master = GameSettingsStore.MasterVolume;
            _bgm = GameSettingsStore.BgmVolume;
            _sfx = GameSettingsStore.SfxVolume;
            _fullscreen = Screen.fullScreen;
            _qualityIndex = Mathf.Clamp(
                PlayerPrefs.GetInt(GameSettingsStore.PrefQuality, QualitySettings.GetQualityLevel()),
                0,
                Mathf.Max(0, QualitySettings.names.Length - 1));
        }

        private void BuildResolutionList()
        {
            _resolutions.Clear();
            var labels = new List<string>();
            var seen = new HashSet<string>();
            int current = 0;

            var all = Screen.resolutions;
            for (int i = 0; i < all.Length; i++)
            {
                var r = all[i];
                string key = r.width + "x" + r.height;
                if (!seen.Add(key)) continue;

                _resolutions.Add(r);
                labels.Add(r.width + " x " + r.height);
                if (r.width == Screen.width && r.height == Screen.height)
                    current = _resolutions.Count - 1;
            }

            _resolutionLabels = labels.ToArray();
            int saved = PlayerPrefs.GetInt(GameSettingsStore.PrefResolutionIndex, current);
            _resolutionIndex = Mathf.Clamp(saved, 0, Mathf.Max(0, _resolutions.Count - 1));
            if (_resolutions.Count == 0)
                _resolutionIndex = 0;
        }

        private void EnsureStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 26,
                fontStyle = FontStyle.Bold
            };
            _titleStyle.normal.textColor = Color.white;

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleLeft
            };
            _labelStyle.normal.textColor = new Color(0.9f, 0.92f, 0.98f);

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };
        }

        private void OnGUI()
        {
            if (!_open) return;

            EnsureStyles();

            // Esc 토글이 Update에 있으므로 여기선 그리기만
            float alpha = 1f;
            var prev = GUI.color;

            GUI.color = new Color(0f, 0f, 0f, 0.62f * alpha);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);

            float w = 460f;
            float h = Mathf.Min(560f, Screen.height * 0.88f);
            float x = (Screen.width - w) * 0.5f;
            float y = (Screen.height - h) * 0.5f;

            GUI.color = new Color(0.09f, 0.1f, 0.14f, 0.96f);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);

            GUI.color = Color.white;
            GUI.Label(new Rect(x, y + 16f, w, 36f), "설정", _titleStyle);

            var contentRect = new Rect(x + 24f, y + 60f, w - 48f, h - 200f);
            var viewRect = new Rect(0f, 0f, contentRect.width - 16f, 380f);
            _scroll = GUI.BeginScrollView(contentRect, _scroll, viewRect);

            float row = 0f;
            float rowH = 28f;
            float gap = 10f;

            DrawSliderRow(ref row, viewRect.width, "마스터 볼륨", ref _master);
            row += gap;
            DrawSliderRow(ref row, viewRect.width, "BGM 볼륨", ref _bgm);
            row += gap;
            DrawSliderRow(ref row, viewRect.width, "SFX 볼륨", ref _sfx);
            row += gap * 1.5f;

            GUI.Label(new Rect(0f, row, viewRect.width, rowH), "전체화면", _labelStyle);
            _fullscreen = GUI.Toggle(new Rect(viewRect.width - 28f, row + 2f, 24f, 24f), _fullscreen, GUIContent.none);
            row += rowH + gap;

            GUI.Label(new Rect(0f, row, viewRect.width, rowH), "그래픽 품질", _labelStyle);
            row += rowH;
            DrawCycleRow(ref row, viewRect.width, QualitySettings.names, ref _qualityIndex);
            row += gap;

            if (_resolutionLabels.Length > 0)
            {
                GUI.Label(new Rect(0f, row, viewRect.width, rowH), "해상도", _labelStyle);
                row += rowH;
                DrawCycleRow(ref row, viewRect.width, _resolutionLabels, ref _resolutionIndex);
            }

            GUI.EndScrollView();

            float bx = x + 24f;
            float bw = w - 48f;
            float bh = 36f;
            float by = y + h - 120f;

            if (GUI.Button(new Rect(bx, by, bw * 0.48f, bh), "적용", _buttonStyle))
                ApplySettings();

            if (GUI.Button(new Rect(bx + bw * 0.52f, by, bw * 0.48f, bh), "기본값", _buttonStyle))
                ResetDefaults();

            by += bh + 8f;
            if (GUI.Button(new Rect(bx, by, bw, bh), "계속하기 (Esc)", _buttonStyle))
                Close(resumeTime: true);

            by += bh + 8f;
            float half = bw * 0.48f;
            if (GUI.Button(new Rect(bx, by, half, bh), "스테이지 선택", _buttonStyle))
                GoToScene(StageSceneCatalog.StageSelectSceneName);

            if (GUI.Button(new Rect(bx + bw * 0.52f, by, half, bh), "타이틀로", _buttonStyle))
                GoToScene(StageSceneCatalog.StartSceneName);

            GUI.color = prev;
        }

        private void DrawSliderRow(ref float row, float width, string label, ref float value)
        {
            GUI.Label(new Rect(0f, row, width * 0.42f, 24f), $"{label}  {Mathf.RoundToInt(value * 100f)}%", _labelStyle);
            value = GUI.HorizontalSlider(new Rect(width * 0.44f, row + 6f, width * 0.56f, 18f), value, 0f, 1f);
            row += 28f;

            // 실시간 마스터 반영
            if (label.StartsWith("마스터"))
                AudioListener.volume = value;
        }

        private void DrawCycleRow(ref float row, float width, string[] options, ref int index)
        {
            if (options == null || options.Length == 0)
            {
                GUI.Label(new Rect(0f, row, width, 28f), "(없음)", _labelStyle);
                row += 32f;
                return;
            }

            index = Mathf.Clamp(index, 0, options.Length - 1);
            float btnW = 36f;
            if (GUI.Button(new Rect(0f, row, btnW, 28f), "<"))
                index = (index - 1 + options.Length) % options.Length;

            GUI.Label(
                new Rect(btnW + 8f, row, width - btnW * 2f - 16f, 28f),
                options[index],
                _labelStyle);

            if (GUI.Button(new Rect(width - btnW, row, btnW, 28f), ">"))
                index = (index + 1) % options.Length;

            row += 32f;
        }

        private void ApplySettings()
        {
            GameSettingsStore.SaveAudio(_master, _bgm, _sfx);
            GameSettingsStore.SaveDisplay(_qualityIndex, _fullscreen, _resolutionIndex);

            if (_resolutionIndex >= 0 && _resolutionIndex < _resolutions.Count)
            {
                var r = _resolutions[_resolutionIndex];
                Screen.SetResolution(r.width, r.height, _fullscreen);
            }
            else
            {
                Screen.fullScreen = _fullscreen;
            }
        }

        private void ResetDefaults()
        {
            _master = 1f;
            _bgm = 0.8f;
            _sfx = 0.8f;
            _fullscreen = true;
            _qualityIndex = Mathf.Max(0, QualitySettings.names.Length - 1);

            int native = _resolutions.FindIndex(r =>
                r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height);
            _resolutionIndex = Mathf.Max(0, native);
            AudioListener.volume = _master;
        }

        private void GoToScene(string sceneName)
        {
            Time.timeScale = 1f;
            _open = false;
            SceneManager.LoadScene(sceneName);
        }
    }
}
