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
    /// 스트레스 사용 스테이지(S3+)에서는 게이지 0%일 때만 점수가 오른다.
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

        /// <summary>S1~S2 또는 스트레스 0%일 때 true.</summary>
        public bool CanGainScore =>
            !StageStressPolicy.UsesStressInActiveScene() || IsStressAtZeroPercent();

        public event Action<long, long> ScoreChanged;
        public event Action<string, long, long> PhotoTaken;
        public event Action<int> CycleChanged;
        /// <summary>점수 상승이 막혔을 때 (토스트용 메시지).</summary>
        public event Action<string> ScoreGainBlocked;

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
            if (amount > 0 && !TryAllowScoreGain())
                return;

            AddLikesUnchecked(amount);
        }

        public void AddFollows(long amount)
        {
            if (amount == 0) return;
            if (amount > 0 && !TryAllowScoreGain())
                return;

            AddFollowsUnchecked(amount);
        }

        /// <summary>좋아요 수집 — 스트레스는 항상 감소, 점수는 0%일 때만.</summary>
        public bool CollectLike(long likeAmount)
        {
            RelieveStress(_stressReducePerLikePickup);
            if (!TryAllowScoreGain())
                return false;

            AddLikesUnchecked(likeAmount);
            return true;
        }

        /// <summary>팔로우 수집 — 스트레스는 항상 감소, 점수는 0%일 때만.</summary>
        public bool CollectFollow(long followAmount)
        {
            RelieveStress(_stressReducePerFollowPickup);
            if (!TryAllowScoreGain())
                return false;

            AddFollowsUnchecked(followAmount);
            return true;
        }

        /// <summary>포토 보상. 스트레스 0%가 아니면 실패(소모·점수 없음).</summary>
        public bool ApplyPhotoReward(string pointId, long likeBonus, long followBonus)
        {
            if (!TryAllowScoreGain())
                return false;

            AddLikesUnchecked(likeBonus);
            AddFollowsUnchecked(followBonus);
            RelieveStress(_stressReducePerPhoto);
            PhotoTaken?.Invoke(pointId, likeBonus, followBonus);
            return true;
        }

        public void RelieveStress(float amount)
        {
            if (amount <= 0f) return;
            if (!StageStressPolicy.UsesStressInActiveScene()) return;

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
            if (!StageStressPolicy.UsesStressInActiveScene()) return;

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

        private bool TryAllowScoreGain()
        {
            if (CanGainScore)
                return true;

            ScoreGainBlocked?.Invoke("스트레스가 0%일 때만 점수가 올라갑니다");
            return false;
        }

        private bool IsStressAtZeroPercent()
        {
            var meter = ResolveStressMeter();
            if (meter == null)
                return true;

            return meter.CurrentPercent <= 0 || meter.CurrentValue <= 0.01f;
        }

        private void AddLikesUnchecked(long amount)
        {
            if (amount == 0) return;
            _likes = Math.Max(0, _likes + amount);
            ScoreChanged?.Invoke(_likes, _follows);
            GameProgressSave.CaptureSocial(_likes, _follows, _secondCycle);
        }

        private void AddFollowsUnchecked(long amount)
        {
            if (amount == 0) return;
            _follows = Math.Max(0, _follows + amount);
            ScoreChanged?.Invoke(_likes, _follows);
            GameProgressSave.CaptureSocial(_likes, _follows, _secondCycle);
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
