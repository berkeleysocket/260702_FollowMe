using UnityEngine;

namespace FollowMe.KDS
{
    [CreateAssetMenu(fileName = "PhotoPointReward", menuName = "FollowMe/KDS/Photo Point Reward")]
    public class PhotoPointRewardSO : ScriptableObject
    {
        [SerializeField] private string _displayName = "포토존";
        [SerializeField] private long _likeBonus = 5000;
        [SerializeField] private long _followBonus = 200;
        [SerializeField] private bool _oneShot = true;

        public string DisplayName => _displayName;
        public long LikeBonus => _likeBonus;
        public long FollowBonus => _followBonus;
        public bool OneShot => _oneShot;
    }
}
