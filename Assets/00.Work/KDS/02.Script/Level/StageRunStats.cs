using System;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 스테이지 런 중 갈림·일상 수집 기록 (엔딩 정산용 프로토타입).
    /// </summary>
    public class StageRunStats : MonoBehaviour
    {
        public static StageRunStats Instance { get; private set; }

        [SerializeField] private int _dailyCollected;
        [SerializeField] private int _likesCollected;

        public int DailyCollected => _dailyCollected;
        public int LikesCollected => _likesCollected;

        public event Action<int, int> StatsChanged;

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

        public void RegisterLikePickup()
        {
            _likesCollected++;
            StatsChanged?.Invoke(_likesCollected, _dailyCollected);
        }

        public void RegisterDailyPickup()
        {
            _dailyCollected++;
            StatsChanged?.Invoke(_likesCollected, _dailyCollected);
        }
    }
}
