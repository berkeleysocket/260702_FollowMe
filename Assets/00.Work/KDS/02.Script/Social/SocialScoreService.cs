using System;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 핵심 루프 목표 — 100만 달성 후 회귀, 이후 1,000만.
    /// </summary>
    public static class SocialGoal
    {
        public const long FirstTargetLikes = 1_000_000;
        public const long SecondTargetLikes = 10_000_000;
    }

    /// <summary>
    /// KDS 맵 프로토타입용 좋아요/팔로우 점수.
    /// 팀 공용 시스템이 생기면 이벤트로 이관할 것.
    /// </summary>
    public class SocialScoreService : MonoBehaviour
    {
        public static SocialScoreService Instance { get; private set; }

        [SerializeField] private long _likes;
        [SerializeField] private long _follows;
        [SerializeField] private bool _secondCycle;

        public long Likes => _likes;
        public long Follows => _follows;
        public bool IsSecondCycle => _secondCycle;

        public long GoalLikes => _secondCycle
            ? SocialGoal.SecondTargetLikes
            : SocialGoal.FirstTargetLikes;

        public float GoalProgress => GoalLikes <= 0
            ? 0f
            : Mathf.Clamp01(_likes / (float)GoalLikes);

        public bool IsGoalReached => _likes >= GoalLikes;

        public event Action<long, long> ScoreChanged;
        public event Action<string, long, long> PhotoTaken;
        public event Action<int> CycleChanged;

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

        /// <summary>
        /// 100만 달성 후 회귀 — 좋아요 리셋, 목표 1,000만.
        /// </summary>
        public void EnterSecondCycle()
        {
            if (_secondCycle) return;

            _secondCycle = true;
            _likes = 0;
            ScoreChanged?.Invoke(_likes, _follows);
            CycleChanged?.Invoke(2);
        }
    }
}
