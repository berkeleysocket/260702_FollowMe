using System;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// KDS 맵 프로토타입용 좋아요/팔로우 점수.
    /// 팀 공용 시스템이 생기면 이벤트로 이관할 것.
    /// </summary>
    public class SocialScoreService : MonoBehaviour
    {
        public static SocialScoreService Instance { get; private set; }

        [SerializeField] private long _likes;
        [SerializeField] private long _follows;

        public long Likes => _likes;
        public long Follows => _follows;

        public event Action<long, long> ScoreChanged;
        public event Action<string, long, long> PhotoTaken;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void AddLikes(long amount)
        {
            if (amount == 0) return;
            _likes = Math.Max(0, _likes + amount);
            ScoreChanged?.Invoke(_likes, _follows);
        }

        public void AddFollows(long amount)
        {
            if (amount == 0) return;
            _follows = Math.Max(0, _follows + amount);
            ScoreChanged?.Invoke(_likes, _follows);
        }

        public void ApplyPhotoReward(string pointId, long likeBonus, long followBonus)
        {
            AddLikes(likeBonus);
            AddFollows(followBonus);
            PhotoTaken?.Invoke(pointId, likeBonus, followBonus);
        }
    }
}
