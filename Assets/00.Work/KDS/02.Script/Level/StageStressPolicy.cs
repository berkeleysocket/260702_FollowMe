using UnityEngine;
using UnityEngine.SceneManagement;

namespace FollowMe.KDS
{
    /// <summary>
    /// Act1(S1~S2)은 스트레스 미사용. S3부터 활성화.
    /// </summary>
    public static class StageStressPolicy
    {
        public const int StressStartStage = 3;

        public static bool UsesStress(int stageNumber1Based) =>
            stageNumber1Based >= StressStartStage;

        public static int ResolveActiveStageNumber()
        {
            if (StageRunStats.Instance != null && StageRunStats.Instance.StageNumber > 0)
                return StageRunStats.Instance.StageNumber;

            var goal = Object.FindFirstObjectByType<StageGoal>();
            if (goal != null)
                return goal.StageNumber;

            string scene = SceneManager.GetActiveScene().name; // "Stage1 Scene"
            if (scene.StartsWith("Stage") && scene.Length > 5)
            {
                int end = 5;
                while (end < scene.Length && char.IsDigit(scene[end]))
                    end++;
                if (end > 5 && int.TryParse(scene.Substring(5, end - 5), out int n))
                    return n;
            }

            return 1;
        }

        public static bool UsesStressInActiveScene() =>
            UsesStress(ResolveActiveStageNumber());
    }
}
