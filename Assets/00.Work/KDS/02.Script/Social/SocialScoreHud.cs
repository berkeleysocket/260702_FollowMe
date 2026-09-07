using UnityEngine;
using UnityEngine.InputSystem;

namespace FollowMe.KDS
{
    /// <summary>
    /// KDS 프로토타입 HUD (임시 IMGUI). 릴리즈 UI는 YHW 담당.
    /// Stage2: 인스타형 포토 카드 연출 (시안 A).
    /// </summary>
    public class SocialScoreHud : MonoBehaviour
    {
        private static bool ShowDebugHud =>
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            true;
#else
            false;
#endif

        [SerializeField] private SocialScoreService _score;

        [Header("Photo Result Timing")]
        [SerializeField] private float _flashSeconds = 0.18f;
        [SerializeField] private float _cardSeconds = 6f;
        [SerializeField] private float _cardPopSeconds = 0.4f;
        [SerializeField] private float _dismissEnableAfter = 0.35f;

        private string _toast;
        private float _toastUntil;

        private string _photoTitle;
        private string _photoHashtags;
        private Sprite _photoSprite;
        private long _photoLikes;
        private long _photoFollows;
        private float _photoStart = -1f;
        private float _photoUntil;
        private float _flashUntil;

        private void OnEnable()
        {
            BindScore();
        }

        private void OnDisable()
        {
            UnbindScore();
        }

        private void Start()
        {
            BindScore();
        }

        private void Update()
        {
            if (_photoStart < 0f || Time.unscaledTime < _photoStart + _dismissEnableAfter)
                return;

            if (WasAnyDismissInput())
                DismissPhotoCard();
        }

        private void BindScore()
        {
            if (_score == null)
                _score = SocialScoreService.Instance;

            if (_score == null) return;

            _score.ScoreChanged -= OnScoreChanged;
            _score.PhotoTaken -= OnPhotoTaken;
            _score.ScoreChanged += OnScoreChanged;
            _score.PhotoTaken += OnPhotoTaken;
        }

        private void UnbindScore()
        {
            if (_score == null) return;
            _score.ScoreChanged -= OnScoreChanged;
            _score.PhotoTaken -= OnPhotoTaken;
        }

        private void OnScoreChanged(long likes, long follows) { }

        private void OnPhotoTaken(string pointId, long likes, long follows)
        {
            _toast = $"사진 업로드! +♡{likes:N0}  +Follow {follows:N0}";
            _toastUntil = Time.unscaledTime + 2.5f;

            ResolvePhotoCopy(pointId, out _photoTitle, out _photoHashtags, out _photoSprite);
            _photoLikes = likes;
            _photoFollows = follows;
            _photoStart = Time.unscaledTime;
            _flashUntil = _photoStart + _flashSeconds;
            _photoUntil = _photoStart + _cardSeconds;
        }

        private void DismissPhotoCard()
        {
            _photoStart = -1f;
            _photoUntil = 0f;
            _flashUntil = 0f;
        }

        private static bool WasAnyDismissInput()
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                return true;
            if (Mouse.current != null &&
                (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame))
                return true;
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                return true;
            return false;
        }

        private static void ResolvePhotoCopy(
            string pointId, out string title, out string hashtags, out Sprite photo)
        {
            var points = Object.FindObjectsByType<PhotoPoint>(FindObjectsSortMode.None);
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == null || points[i].PointId != pointId)
                    continue;

                title = points[i].DisplayName;
                hashtags = points[i].HashtagLine;
                photo = points[i].PhotoSprite;
                return;
            }

            title = string.IsNullOrEmpty(pointId) ? "오늘의 사진" : pointId;
            hashtags = string.Empty;
            photo = null;
        }

        private void OnGUI()
        {
            if (CutscenePlayer.Instance != null && CutscenePlayer.Instance.IsPlaying)
                return;

            long likes = _score != null ? _score.Likes : 0;
            long follows = _score != null ? _score.Follows : 0;
            long goal = _score != null ? _score.GoalLikes : SocialGoal.FirstTargetLikes;
            float progress = _score != null ? _score.GoalProgress : 0f;
            string cycleLabel = _score != null && _score.IsSecondCycle ? "2차 목표" : "목표";

            // Hide normal HUD chrome while photo card is up
            bool photoOpen = _photoStart >= 0f && Time.unscaledTime < _photoUntil;
            if (!photoOpen)
            {
                const float panelW = 280f;
                float panelH = ShowDebugHud ? 118f : 74f;
                GUI.Box(new Rect(12, 12, panelW, panelH), "");
                GUI.Label(new Rect(24, 18, panelW - 24, 18), $"{cycleLabel}  ♡ {likes:N0} / {goal:N0}");
                DrawProgressBar(new Rect(24, 38, panelW - 36, 12), progress);
                GUI.Label(new Rect(24, 54, panelW - 24, 18), $"팔로우  {follows:N0}");

                if (ShowDebugHud)
                {
                    if (MapModeService.Instance != null)
                    {
                        GUI.Label(new Rect(12, 76, panelW, 20),
                            $"맵 모드  {MapModeService.GetDisplayName(MapModeService.Instance.CurrentMode)}");
                    }

                    if (CheckpointService.Instance != null)
                    {
                        GUI.Label(new Rect(12, 96, panelW + 40, 20),
                            $"CP  {CheckpointService.Instance.LastCheckpointId}");
                    }
                }

                PhotoPoint active = PhotoPoint.Active;
                if (active != null && !active.IsUsed)
                {
                    DrawCaptureHoldUi(active);
                }
                else if (PhotoPoint.Nearby != null && !PhotoPoint.Nearby.IsUsed)
                {
                    DrawCapturePromptUi(PhotoPoint.Nearby);
                }

                if (!string.IsNullOrEmpty(_toast) && Time.unscaledTime < _toastUntil)
                {
                    float w = 420f;
                    GUI.Box(new Rect((Screen.width - w) * 0.5f, Screen.height * 0.14f, w, 40f), _toast);
                }

                if (ShowDebugHud)
                {
                    GUI.Label(new Rect(12, Screen.height - 28, 520, 24),
                        "하트=팔로우 · 이모지=좋아요 · 포토존=E홀드(사진)");
                }
            }

            DrawPhotoResult();
        }

        private static void DrawCapturePromptUi(PhotoPoint nearby)
        {
            float bob = Mathf.Sin(Time.unscaledTime * 5f) * 5f;
            float w = 360f;
            float h = 62f;
            float x = (Screen.width - w) * 0.5f;
            float y = Screen.height * 0.76f + bob;

            Color prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.18f);
            GUI.DrawTexture(new Rect(x + 3f, y + 4f, w, h), Texture2D.whiteTexture);

            GUI.color = new Color(1f, 1f, 1f, 0.96f);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);

            // pink key chip
            GUI.color = new Color(1f, 0.45f, 0.72f, 1f);
            GUI.DrawTexture(new Rect(x + 12f, y + 13f, 36f, 36f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(x + 12f, y + 13f, 36f, 36f), "E",
                new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    fontSize = 18
                });

            GUI.color = new Color(0.25f, 0.22f, 0.26f, 1f);
            GUI.Label(new Rect(x + 58f, y + 10f, w - 74f, 24f), nearby.DisplayName,
                new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 16 });
            GUI.color = new Color(0.45f, 0.45f, 0.5f, 1f);
            GUI.Label(new Rect(x + 58f, y + 34f, w - 74f, 20f),
                $"홀드하여 촬영  ·  +♡{nearby.PreviewLikeBonus:N0}",
                new GUIStyle(GUI.skin.label) { fontSize = 14 });
            GUI.color = prev;
        }

        private static void DrawCaptureHoldUi(PhotoPoint active)
        {
            float p = Mathf.Clamp01(active.HoldProgress);
            float pulse = 1f + Mathf.Sin(Time.unscaledTime * 16f) * 0.02f * p;
            float shake = Mathf.Sin(Time.unscaledTime * 36f) * 2.5f * p;

            float ring = 88f * pulse;
            float panelW = 300f;
            float panelH = 208f;
            float cx = Screen.width * 0.5f + shake;
            float cy = Screen.height * 0.6f;
            float x = cx - panelW * 0.5f;
            float y = cy - panelH * 0.5f;

            Color prev = GUI.color;

            GUI.color = new Color(0f, 0f, 0f, 0.18f);
            GUI.DrawTexture(new Rect(x + 3f, y + 5f, panelW, panelH), Texture2D.whiteTexture);
            GUI.color = new Color(1f, 1f, 1f, 0.98f);
            GUI.DrawTexture(new Rect(x, y, panelW, panelH), Texture2D.whiteTexture);

            // top accent bar (Instagram-card tone)
            GUI.color = new Color(1f, 0.45f, 0.72f, 1f);
            GUI.DrawTexture(new Rect(x, y, panelW, 3f), Texture2D.whiteTexture);

            GUI.color = new Color(0.55f, 0.52f, 0.58f, 1f);
            GUI.Label(new Rect(x + 16f, y + 14f, panelW - 32f, 20f), "촬영 중",
                new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14
                });
            GUI.color = new Color(0.22f, 0.2f, 0.24f, 1f);
            GUI.Label(new Rect(x + 16f, y + 34f, panelW - 32f, 24f), active.DisplayName,
                new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 17,
                    fontStyle = FontStyle.Bold
                });

            float rx = cx - ring * 0.5f;
            float ry = y + 66f;
            DrawRingGauge(new Rect(rx, ry, ring, ring), p);

            float chip = 46f;
            float chipX = cx - chip * 0.5f;
            float chipY = ry + ring * 0.5f - chip * 0.5f;
            GUI.color = Color.Lerp(new Color(1f, 0.72f, 0.86f, 1f), new Color(1f, 0.38f, 0.66f, 1f), p);
            GUI.DrawTexture(new Rect(chipX, chipY, chip, chip), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(chipX, chipY, chip, chip),
                Mathf.RoundToInt(p * 100f) + "%",
                new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    fontSize = 14
                });

            // divider
            GUI.color = new Color(0.9f, 0.9f, 0.93f, 1f);
            GUI.DrawTexture(new Rect(x + 24f, y + panelH - 42f, panelW - 48f, 1f), Texture2D.whiteTexture);

            GUI.color = new Color(0.85f, 0.25f, 0.45f, 1f);
            GUI.Label(new Rect(x + 16f, y + panelH - 34f, panelW * 0.5f - 20f, 24f),
                $"+♡ {active.PreviewLikeBonus:N0}",
                new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 14, fontStyle = FontStyle.Bold });
            GUI.color = new Color(0.25f, 0.45f, 0.85f, 1f);
            GUI.Label(new Rect(x + panelW * 0.5f + 4f, y + panelH - 34f, panelW * 0.5f - 20f, 24f),
                $"+Follow {active.PreviewFollowBonus:N0}",
                new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 14, fontStyle = FontStyle.Bold });

            GUI.color = prev;
        }

        private static void DrawRingGauge(Rect rect, float progress)
        {
            Color prev = GUI.color;
            int segments = 56;
            int filled = Mathf.RoundToInt(segments * Mathf.Clamp01(progress));
            float cx = rect.x + rect.width * 0.5f;
            float cy = rect.y + rect.height * 0.5f;
            float radius = rect.width * 0.5f - 6f;

            for (int i = 0; i < segments; i++)
            {
                float tm = (i / (float)segments) * Mathf.PI * 2f - Mathf.PI * 0.5f;
                float px = cx + Mathf.Cos(tm) * radius;
                float py = cy + Mathf.Sin(tm) * radius;
                bool on = i < filled;
                GUI.color = on
                    ? Color.Lerp(new Color(1f, 0.6f, 0.82f, 1f), new Color(1f, 0.32f, 0.62f, 1f), i / (float)segments)
                    : new Color(0.88f, 0.88f, 0.91f, 1f);
                float s = on ? 7f : 5.5f;
                GUI.DrawTexture(new Rect(px - s * 0.5f, py - s * 0.5f, s, s), Texture2D.whiteTexture);
            }

            GUI.color = prev;
        }

        private void DrawPhotoResult()
        {
            if (_photoStart < 0f) return;
            if (Time.unscaledTime >= _photoUntil && Time.unscaledTime >= _flashUntil)
            {
                _photoStart = -1f;
                return;
            }

            float elapsed = Time.unscaledTime - _photoStart;
            float fadeOut = Mathf.Clamp01((_photoUntil - Time.unscaledTime) / 0.35f);

            // Dim backdrop
            if (Time.unscaledTime < _photoUntil)
            {
                Color prevDim = GUI.color;
                GUI.color = new Color(0.08f, 0.07f, 0.1f, 0.45f * fadeOut);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                GUI.color = prevDim;
            }

            // Flash
            if (Time.unscaledTime < _flashUntil)
            {
                float flashU = 1f - Mathf.Clamp01(elapsed / _flashSeconds);
                Color prev = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, 0.88f * flashU);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                GUI.color = prev;
            }

            if (Time.unscaledTime >= _photoUntil)
                return;

            float pop = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, _cardPopSeconds));
            float ease = EaseOutBack(pop);

            float cardW = Mathf.Min(420f, Screen.width * 0.72f) * Mathf.Lerp(0.82f, 1f, ease);
            float photoSize = cardW - 28f;
            float headerH = 48f;
            float footerH = 54f;
            float textBlock = 78f;
            float cardH = headerH + photoSize + textBlock + footerH;
            float x = (Screen.width - cardW) * 0.5f;
            float y = (Screen.height - cardH) * 0.5f - 10f + Mathf.Lerp(48f, 0f, ease);

            // Soft shadow
            Color prevC = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.22f * fadeOut);
            GUI.DrawTexture(new Rect(x + 6f, y + 8f, cardW, cardH), Texture2D.whiteTexture);

            // Card body
            GUI.color = new Color(1f, 1f, 1f, fadeOut);
            GUI.DrawTexture(new Rect(x, y, cardW, cardH), Texture2D.whiteTexture);

            // Header
            float hx = x + 14f;
            float hy = y + 10f;
            GUI.color = new Color(1f, 0.45f, 0.72f, fadeOut);
            GUI.DrawTexture(new Rect(hx, hy, 28f, 28f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(hx + 6f, hy + 4f, 20f, 20f), "📷");

            var brandStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 15,
                alignment = TextAnchor.MiddleLeft
            };
            GUI.color = new Color(0.22f, 0.2f, 0.24f, fadeOut);
            GUI.Label(new Rect(hx + 36f, hy, 160f, 28f), "Follow Me", brandStyle);
            GUI.color = new Color(0.55f, 0.55f, 0.6f, fadeOut);
            GUI.Label(new Rect(x + cardW - 40f, hy, 28f, 28f), "···",
                new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 18 });

            // 1:1 Photo with develop wipe
            float develop = Mathf.Clamp01((elapsed - 0.1f) / 0.45f);
            var photoRect = new Rect(x + 14f, y + headerH, photoSize, photoSize);
            GUI.color = new Color(0.93f, 0.93f, 0.95f, fadeOut);
            GUI.DrawTexture(photoRect, Texture2D.whiteTexture);

            float revealH = photoRect.height * Mathf.Max(0.02f, develop);
            GUI.BeginGroup(new Rect(photoRect.x, photoRect.y, photoRect.width, revealH));
            if (_photoSprite != null && _photoSprite.texture != null)
            {
                GUI.color = new Color(1f, 1f, 1f, fadeOut);
                DrawSprite(new Rect(0f, 0f, photoRect.width, photoRect.height), _photoSprite, ScaleMode.ScaleAndCrop);
            }
            else
            {
                GUI.color = new Color(1f, 0.55f, 0.85f, 0.55f * fadeOut);
                GUI.DrawTexture(new Rect(0f, 0f, photoRect.width, photoRect.height), Texture2D.whiteTexture);
            }

            GUI.EndGroup();

            // Title + hashtags
            float ty = photoRect.yMax + 10f;
            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 17
            };
            GUI.color = new Color(0.28f, 0.22f, 0.2f, fadeOut);
            GUI.Label(new Rect(x + 12f, ty, cardW - 24f, 26f), _photoTitle, titleStyle);

            if (!string.IsNullOrEmpty(_photoHashtags))
            {
                GUI.color = new Color(0.35f, 0.55f, 0.95f, fadeOut);
                GUI.Label(new Rect(x + 12f, ty + 26f, cardW - 24f, 22f), _photoHashtags,
                    new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 13,
                        wordWrap = true
                    });
            }

            // Divider
            float dy = y + headerH + photoSize + textBlock - 8f;
            GUI.color = new Color(0.85f, 0.85f, 0.88f, fadeOut);
            GUI.DrawTexture(new Rect(x + 16f, dy, cardW - 32f, 1f), Texture2D.whiteTexture);

            // Split reward footer
            float fy = y + cardH - footerH;
            float half = (cardW - 2f) * 0.5f;
            GUI.color = new Color(1f, 0.92f, 0.95f, fadeOut);
            GUI.DrawTexture(new Rect(x + 1f, fy, half, footerH - 1f), Texture2D.whiteTexture);
            GUI.color = new Color(0.9f, 0.95f, 1f, fadeOut);
            GUI.DrawTexture(new Rect(x + 1f + half, fy, half, footerH - 1f), Texture2D.whiteTexture);

            GUI.color = new Color(0.82f, 0.84f, 0.88f, fadeOut);
            GUI.DrawTexture(new Rect(x + half, fy + 8f, 1f, footerH - 16f), Texture2D.whiteTexture);

            var rewardStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 14
            };
            GUI.color = new Color(0.85f, 0.25f, 0.45f, fadeOut);
            GUI.Label(new Rect(x + 1f, fy, half, footerH - 1f), $"+ ♡ {_photoLikes:N0}", rewardStyle);
            GUI.color = new Color(0.25f, 0.45f, 0.85f, fadeOut);
            GUI.Label(new Rect(x + 1f + half, fy, half, footerH - 1f), $"+ Follow {_photoFollows:N0}", rewardStyle);

            // Dismiss hint
            GUI.color = new Color(1f, 1f, 1f, 0.85f * fadeOut);
            GUI.Label(
                new Rect((Screen.width - 280f) * 0.5f, y + cardH + 14f, 280f, 22f),
                "아무 키나 눌러 닫기",
                new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 13 });

            GUI.color = prevC;
        }

        private static void DrawSprite(Rect screenRect, Sprite sprite, ScaleMode scaleMode)
        {
            Texture2D tex = sprite.texture;
            Rect tr = sprite.textureRect;
            var uv = new Rect(
                tr.x / tex.width,
                tr.y / tex.height,
                tr.width / tex.width,
                tr.height / tex.height);

            if (scaleMode == ScaleMode.ScaleAndCrop)
            {
                float spriteAspect = tr.width / tr.height;
                float rectAspect = screenRect.width / Mathf.Max(0.01f, screenRect.height);
                Rect drawUv = uv;

                if (spriteAspect > rectAspect)
                {
                    float visible = rectAspect / spriteAspect;
                    float u0 = (1f - visible) * 0.5f;
                    drawUv = new Rect(uv.x + uv.width * u0, uv.y, uv.width * visible, uv.height);
                }
                else
                {
                    float visible = spriteAspect / rectAspect;
                    float v0 = (1f - visible) * 0.5f;
                    drawUv = new Rect(uv.x, uv.y + uv.height * v0, uv.width, uv.height * visible);
                }

                GUI.DrawTextureWithTexCoords(screenRect, tex, drawUv);
            }
            else
            {
                GUI.DrawTextureWithTexCoords(screenRect, tex, uv);
            }
        }

        private static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            float inv = x - 1f;
            return 1f + c3 * inv * inv * inv + c1 * inv * inv;
        }

        private static void DrawProgressBar(Rect rect, float progress)
        {
            Color prev = GUI.color;
            GUI.color = new Color(0.15f, 0.15f, 0.18f, 0.9f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = new Color(1f, 0.45f, 0.85f, 1f);
            var fill = new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(progress), rect.height);
            GUI.DrawTexture(fill, Texture2D.whiteTexture);
            GUI.color = prev;
        }
    }
}
