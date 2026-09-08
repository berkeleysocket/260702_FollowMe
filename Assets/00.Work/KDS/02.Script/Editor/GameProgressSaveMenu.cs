#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FollowMe.KDS.Editor
{
    public static class GameProgressSaveMenu
    {
        [MenuItem("FollowMe/KDS/Save/Clear All Progress")]
        public static void ClearAllProgress()
        {
            if (!EditorUtility.DisplayDialog(
                    "Clear Progress",
                    "KDS 진행 저장과 YHW 스테이지 best%를 모두 지울까요?",
                    "Clear",
                    "Cancel"))
                return;

            GameProgressSave.ClearAll();
            Debug.Log("[GameProgressSaveMenu] Progress cleared.");
        }

        [MenuItem("FollowMe/KDS/Save/Log Progress")]
        public static void LogProgress()
        {
            GameProgressSave.Load();
            var d = GameProgressSave.Data;
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(
                $"[GameProgressSave] last={d.lastPlayedStage} highestCleared={d.highestClearedStage} " +
                $"likes={d.totalLikes} follows={d.totalFollows} cycle2={d.secondCycle}");
            for (int i = 0; i < GameProgressData.StageCount; i++)
            {
                if (!d.cleared[i] && d.bestStars[i] <= 0 && d.bestFollowPercent[i] <= 0f)
                    continue;
                sb.AppendLine(
                    $"  S{i + 1}: cleared={d.cleared[i]} ★{d.bestStars[i]} follow%={d.bestFollowPercent[i]:0.#} " +
                    $"unlocked={GameProgressSave.IsStageUnlocked(i + 1)}");
            }

            Debug.Log(sb.ToString());
        }
    }
}
#endif
