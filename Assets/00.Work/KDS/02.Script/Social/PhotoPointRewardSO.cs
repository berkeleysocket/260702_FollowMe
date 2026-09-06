using System.Text;
using UnityEngine;

namespace FollowMe.KDS
{
    [CreateAssetMenu(fileName = "PhotoPointReward", menuName = "FollowMe/KDS/Photo Point Reward")]
    public class PhotoPointRewardSO : ScriptableObject
    {
        [SerializeField] private string _displayName = "포토존";
        [SerializeField] private Sprite _photoSprite;
        [SerializeField] private long _likeBonus = 5000;
        [SerializeField] private long _followBonus = 200;
        [SerializeField] private float _holdSeconds = 0.85f;
        [SerializeField] private bool _oneShot = true;

        [Header("SNS")]
        [Tooltip("# 없이 적어도 자동으로 #이 붙습니다.")]
        [SerializeField] private string[] _hashtags =
        {
            "FollowMe",
            "인증샷"
        };

        public string DisplayName => _displayName;
        public Sprite PhotoSprite => _photoSprite;
        public long LikeBonus => _likeBonus;
        public long FollowBonus => _followBonus;
        public float HoldSeconds => Mathf.Max(0.05f, _holdSeconds);
        public bool OneShot => _oneShot;
        public string[] Hashtags => _hashtags;

        /// <summary>UI용: "#홍대 #버스킹" 형태.</summary>
        public string HashtagLine => FormatHashtagLine(_hashtags);

        public static string FormatHashtagLine(string[] tags)
        {
            if (tags == null || tags.Length == 0)
                return string.Empty;

            var sb = new StringBuilder(tags.Length * 12);
            for (int i = 0; i < tags.Length; i++)
            {
                string tag = NormalizeHashtag(tags[i]);
                if (string.IsNullOrEmpty(tag))
                    continue;

                if (sb.Length > 0)
                    sb.Append(' ');
                sb.Append(tag);
            }

            return sb.ToString();
        }

        public static string NormalizeHashtag(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;

            string t = raw.Trim().Replace(' ', '_');
            if (t[0] != '#')
                t = "#" + t;
            return t;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_hashtags == null) return;
            for (int i = 0; i < _hashtags.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(_hashtags[i]))
                    continue;
                _hashtags[i] = NormalizeHashtag(_hashtags[i]);
            }
        }
#endif
    }
}
