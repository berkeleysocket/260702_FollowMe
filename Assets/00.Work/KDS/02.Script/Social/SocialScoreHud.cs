using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 좋아요/팔로우 디버그 HUD + 사진 보상 팝업.
    /// </summary>
    public class SocialScoreHud : MonoBehaviour
    {
        [SerializeField] private SocialScoreService _score;

        private string _toast;
        private float _toastUntil;

        private void OnEnable()
        {
            if (_score == null)
                _score = SocialScoreService.Instance;

            if (_score != null)
            {
                _score.ScoreChanged += OnScoreChanged;
                _score.PhotoTaken += OnPhotoTaken;
            }
        }

        private void OnDisable()
        {
            if (_score != null)
            {
                _score.ScoreChanged -= OnScoreChanged;
                _score.PhotoTaken -= OnPhotoTaken;
            }
        }

        private void Start()
        {
            if (_score == null)
                _score = SocialScoreService.Instance;
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

            GUI.Box(new Rect(12, 12, 260, 54), "");
            GUI.Label(new Rect(24, 20, 240, 20), $"좋아요  {likes:N0}");
            GUI.Label(new Rect(24, 40, 240, 20), $"팔로우  {follows:N0}");

            if (!string.IsNullOrEmpty(_toast) && Time.unscaledTime < _toastUntil)
            {
                float w = 420f;
                GUI.Box(new Rect((Screen.width - w) * 0.5f, Screen.height * 0.2f, w, 40), _toast);
            }

            GUI.Label(new Rect(12, Screen.height - 28, 480, 24), "포토존에서 E(홀드)로 사진 촬영");
        }
    }
}
