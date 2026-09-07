using System;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 스테이지 런 중 수집 기록 + 클리어 시 팔로우 비율 별점.
    /// 맵 팔로우 80%↑=3★, 70%↑=2★, 그 외=1★.
    /// </summary>
    public class StageRunStats : MonoBehaviour
    {
        public static StageRunStats Instance { get; private set; }

        [SerializeField] private int _stageNumber = 1;
        [SerializeField] private int _dailyCollected;
        [SerializeField] private int _likesCollected;
        [SerializeField] private int _followsCollected;
        [SerializeField] private int _followsTotal;
        [SerializeField] private float _threeStarRatio = 0.8f;
        [SerializeField] private float _twoStarRatio = 0.7f;

        public int StageNumber => _stageNumber;
        public int DailyCollected => _dailyCollected;
        public int LikesCollected => _likesCollected;
        public int FollowsCollected => _followsCollected;
        public int FollowsTotal => _followsTotal;
        public int Stars { get; private set; }

        public float FollowRatio =>
            _followsTotal <= 0 ? 0f : Mathf.Clamp01(_followsCollected / (float)_followsTotal);

        public event Action<int, int> StatsChanged;
        public event Action<int> StageClearedWithStars;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            RefreshFollowTotalIfNeeded();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void ConfigureStage(int stageNumber)
        {
            _stageNumber = Mathf.Max(1, stageNumber);
            RefreshFollowTotalIfNeeded();
        }

        public void RegisterLikePickup()
        {
            _likesCollected++;
            StatsChanged?.Invoke(_likesCollected, _dailyCollected);
        }

        public void RegisterFollowPickup()
        {
            _followsCollected++;
            StatsChanged?.Invoke(_likesCollected, _dailyCollected);
        }

        public void RegisterDailyPickup()
        {
            _dailyCollected++;
            StatsChanged?.Invoke(_likesCollected, _dailyCollected);
        }

        public int EvaluateStars()
        {
            // 클리어 시점: 먹은 수 + 맵에 남은 Follow_* = 원래 총량
            int remaining = CountFollowsInScene();
            int inferredTotal = _followsCollected + remaining;
            if (inferredTotal > _followsTotal)
                _followsTotal = inferredTotal;

            if (_followsTotal <= 0)
                _followsTotal = Mathf.Max(0, StageMapDatabase.GetFollowCount(_stageNumber));

            if (_followsTotal <= 0)
            {
                Stars = 1;
                return Stars;
            }

            float ratio = FollowRatio;
            if (ratio >= _threeStarRatio)
                Stars = 3;
            else if (ratio >= _twoStarRatio)
                Stars = 2;
            else
                Stars = 1;

            return Stars;
        }

        public int ClearStage()
        {
            int stars = EvaluateStars();
            StageClearedWithStars?.Invoke(stars);
            Debug.Log(
                $"[StageRunStats] Stage {_stageNumber} 클리어 ★{stars} " +
                $"(Follow {_followsCollected}/{_followsTotal} = {FollowRatio * 100f:0.#}%)",
                this);
            return stars;
        }

        private void RefreshFollowTotalIfNeeded()
        {
            if (_followsTotal > 0)
                return;

            _followsTotal = CountFollowsInScene();
            if (_followsTotal <= 0)
                _followsTotal = Mathf.Max(0, StageMapDatabase.GetFollowCount(_stageNumber));
        }

        public static int CountFollowsInScene()
        {
            int count = 0;
            var transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];
                if (t != null && t.name.StartsWith("Follow_", StringComparison.Ordinal))
                    count++;
            }

            return count;
        }
    }
}
