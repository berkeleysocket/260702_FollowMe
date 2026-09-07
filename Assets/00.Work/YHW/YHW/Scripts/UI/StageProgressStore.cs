using UnityEngine;

namespace YHW.UI
{
    /// <summary>
    /// Persists each stage's best clear percentage (0-100) in PlayerPrefs.
    /// </summary>
    public static class StageProgressStore
    {
        private const string KeyPrefix = "stage.bestPercent.";

        public static float GetPercent(int stageIndex)
        {
            return Mathf.Clamp(PlayerPrefs.GetFloat(KeyPrefix + stageIndex, 0f), 0f, 100f);
        }

        /// <summary>Raises the saved best score for a stage; never lowers it.</summary>
        public static void ReportPercent(int stageIndex, float percent)
        {
            float clamped = Mathf.Clamp(percent, 0f, 100f);
            if (clamped > GetPercent(stageIndex))
            {
                PlayerPrefs.SetFloat(KeyPrefix + stageIndex, clamped);
                PlayerPrefs.Save();
            }
        }

        /// <summary>Overwrites the saved percent regardless of the previous value. Editor/testing use.</summary>
        public static void DebugSetPercent(int stageIndex, float percent)
        {
            PlayerPrefs.SetFloat(KeyPrefix + stageIndex, Mathf.Clamp(percent, 0f, 100f));
            PlayerPrefs.Save();
        }

        public static int GetStarCount(float percent)
        {
            if (percent >= 100f) return 3;
            if (percent >= 66f) return 2;
            if (percent >= 33f) return 1;
            return 0;
        }
    }
}
