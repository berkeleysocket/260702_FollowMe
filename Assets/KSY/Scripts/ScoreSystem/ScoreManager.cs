using System;

namespace KSY.System
{
    public static class ScoreManager
    {
        public static event Action<int> OnChangedScore;
        public static int Score { get; private set; }

        public static void AddScore(int score)
        {
            Score += score;
            OnChangedScore?.Invoke(Score);
        }
    }
}