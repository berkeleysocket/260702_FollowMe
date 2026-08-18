using System.IO;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 대사 JSON 저장 위치 (KDS 작업 폴더).
    /// Assets/00.Work/KDS/08.SO/Dialogues/
    /// </summary>
    public static class DialogueJsonPaths
    {
        public const string FolderRelativeToAssets = "00.Work/KDS/08.SO/Dialogues";

        public static string GetFolderFullPath()
        {
            return Path.Combine(Application.dataPath, FolderRelativeToAssets);
        }

        public static string GetFullPath(string jsonFileName)
        {
            return Path.Combine(GetFolderFullPath(), $"{jsonFileName}.json");
        }

        public static string GetAssetPath(string jsonFileName)
        {
            return $"Assets/{FolderRelativeToAssets}/{jsonFileName}.json";
        }
    }
}
