using UnityEngine;

namespace FollowMe.KDS
{
    [CreateAssetMenu(fileName = "PhotoPointReward", menuName = "FollowMe/KDS/Photo Point Reward")]
    public class PhotoPointRewardSO : ScriptableObject
    {
        [SerializeField] private string _displayName = "포토존";
        [SerializeField] private long _likeBonus = 5000;
        [SerializeField] private long _followBonus = 200;
        [SerializeField] private float _holdSeconds = 0.85f;
        [SerializeField] private bool _oneShot = true;

        public string DisplayName => _displayName;
        public long LikeBonus => _likeBonus;
        public long FollowBonus => _followBonus;
        public float HoldSeconds => Mathf.Max(0.05f, _holdSeconds);
        public bool OneShot => _oneShot;
    }
}
