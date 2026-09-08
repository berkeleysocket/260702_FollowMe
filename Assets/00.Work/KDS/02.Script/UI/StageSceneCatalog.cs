using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 스테이지 번호(1-based) ↔ 씬 이름 매핑.
    /// </summary>
    public static class StageSceneCatalog
    {
        public const int StageCount = 16;
        public const string StartSceneName = "Start Scene";
        public const string StageSelectSceneName = "Stage Select Scene";

        public static string GetStageSceneName(int stageNumber1Based)
        {
            int s = Mathf.Clamp(stageNumber1Based, 1, StageCount);
            return $"Stage{s} Scene";
        }

        public static string[] BuildStageSceneNames(int count)
        {
            count = Mathf.Clamp(count, 1, StageCount);
            var names = new string[count];
            for (int i = 0; i < count; i++)
                names[i] = GetStageSceneName(i + 1);
            return names;
        }
    }
}
