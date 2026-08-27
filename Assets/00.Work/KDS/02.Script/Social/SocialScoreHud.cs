using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 좋아요/팔로우 HUD + 포토존 E홀드 진행 + 촬영 토스트.
    /// </summary>
    public class SocialScoreHud : MonoBehaviour
    {
        [SerializeField] private SocialScoreService _score;

        private string _toast;
        private float _toastUntil;

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
        }

        private void OnGUI()
        {
            long likes = _score != null ? _score.Likes : 0;
            long follows = _score != null ? _score.Follows : 0;

            GUI.Box(new Rect(12, 12, 260, 88), "");
            GUI.Label(new Rect(24, 20, 240, 20), $"좋아요  {likes:N0}");
            GUI.Label(new Rect(24, 40, 240, 20), $"팔로우  {follows:N0}");

            if (MapModeService.Instance != null)
            {
                GUI.Label(new Rect(12, 70, 260, 20),
                    $"맵 모드  {MapModeService.GetDisplayName(MapModeService.Instance.CurrentMode)}");
            }

            if (CheckpointService.Instance != null)
            {
                GUI.Label(new Rect(12, 88, 320, 20),
                    $"CP  {CheckpointService.Instance.LastCheckpointId}");
            }

            PhotoPoint active = PhotoPoint.Active;
            if (active != null && !active.IsUsed)
            {
                float w = 320f;
                float x = (Screen.width - w) * 0.5f;
                float y = Screen.height * 0.72f;
                GUI.Box(new Rect(x, y, w, 52), "");
                GUI.Label(new Rect(x + 12, y + 6, w - 24, 18), $"촬영 중… {active.DisplayName}");
                DrawProgressBar(new Rect(x + 12, y + 28, w - 24, 14), active.HoldProgress);
            }
            else if (PhotoPoint.Nearby != null && !PhotoPoint.Nearby.IsUsed)
            {
                var nearby = PhotoPoint.Nearby;
                GUI.Label(
                    new Rect((Screen.width - 360f) * 0.5f, Screen.height * 0.78f, 360f, 22),
                    $"E 홀드 — {nearby.DisplayName}  (+♡{nearby.PreviewLikeBonus:N0})");
            }

            if (!string.IsNullOrEmpty(_toast) && Time.unscaledTime < _toastUntil)
            {
                float w = 420f;
                GUI.Box(new Rect((Screen.width - w) * 0.5f, Screen.height * 0.2f, w, 40), _toast);
            }

            GUI.Label(new Rect(12, Screen.height - 28, 480, 24), "포토존에서 E(홀드)로 사진 촬영");
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
