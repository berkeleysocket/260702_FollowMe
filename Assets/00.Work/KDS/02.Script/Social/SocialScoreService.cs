using System;
using UnityEngine;
using YHW.Stats;

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
        [SerializeField] private StressMeter _stressMeter;
        [SerializeField] private float _stressReducePerLikePickup = 8f;
        [SerializeField] private float _stressReducePerFollowPickup = 8f;
        [SerializeField] private float _stressReducePerPhoto = 20f;
        [SerializeField] private float _stressAddPerFall = 35f;

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
            GameProgressSave.ApplySocialTo(this);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                PersistSocial();
        }

        private void OnApplicationQuit()
        {
            PersistSocial();
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
            GameProgressSave.CaptureSocial(_likes, _follows, _secondCycle);
        }

        public void AddFollows(long amount)
        {
            if (amount == 0) return;
            _follows = Math.Max(0, _follows + amount);
            ScoreChanged?.Invoke(_likes, _follows);
            GameProgressSave.CaptureSocial(_likes, _follows, _secondCycle);
        }

        /// <summary>좋아요 수집 1회 — 점수 + 스트레스 감소.</summary>
        public void CollectLike(long likeAmount)
        {
            AddLikes(likeAmount);
            RelieveStress(_stressReducePerLikePickup);
        }

        /// <summary>팔로우 수집 1회 — 점수 + 스트레스 감소.</summary>
        public void CollectFollow(long followAmount)
        {
            AddFollows(followAmount);
            RelieveStress(_stressReducePerFollowPickup);
        }

        public void ApplyPhotoReward(string pointId, long likeBonus, long followBonus)
        {
            AddLikes(likeBonus);
            AddFollows(followBonus);
            RelieveStress(_stressReducePerPhoto);
            PhotoTaken?.Invoke(pointId, likeBonus, followBonus);
        }

        public void RelieveStress(float amount)
        {
            if (amount <= 0f) return;

            var meter = ResolveStressMeter();
            if (meter == null) return;

            meter.ReduceStress(amount);
        }

        /// <summary>낙사·함정 등 — 스트레스 크게 증가.</summary>
        public void ApplyFallStress()
        {
            ApplyStress(_stressAddPerFall);
        }

        public void ApplyStress(float amount)
        {
            if (amount <= 0f) return;

            var meter = ResolveStressMeter();
            if (meter == null) return;

            meter.AddStress(amount);
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
            GameProgressSave.CaptureSocial(_likes, _follows, _secondCycle);
            GameProgressSave.FlushIfDirty();
        }

        /// <summary>세이브에서 누적 점수 복원.</summary>
        public void LoadFromSave(long likes, long follows, bool secondCycle)
        {
            _likes = Math.Max(0, likes);
            _follows = Math.Max(0, follows);
            _secondCycle = secondCycle;
            ScoreChanged?.Invoke(_likes, _follows);
            if (_secondCycle)
                CycleChanged?.Invoke(2);
        }

        private void PersistSocial()
        {
            GameProgressSave.CaptureSocial(_likes, _follows, _secondCycle);
            GameProgressSave.FlushIfDirty();
        }

        private StressMeter ResolveStressMeter()
        {
            if (_stressMeter != null)
                return _stressMeter;

            _stressMeter = FindFirstObjectByType<StressMeter>();
            return _stressMeter;
        }
    }
}
