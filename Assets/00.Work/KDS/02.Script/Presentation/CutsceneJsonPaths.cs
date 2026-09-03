using System.IO;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 컷씬 JSON 경로. Assets/00.Work/KDS/08.SO/Cutscenes/
    /// </summary>
    public static class CutsceneJsonPaths
    {
        public const string FolderRelativeToAssets = "00.Work/KDS/08.SO/Cutscenes";

        public static string GetFolderFullPath()
        {
            return Path.Combine(Application.dataPath, FolderRelativeToAssets);
        }

        public static string GetFullPath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            if (!fileName.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
                fileName += ".json";

            return Path.Combine(GetFolderFullPath(), fileName);
        }

        public static string GetAssetPath(string fileName)
        {
            if (!fileName.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
                fileName += ".json";

            return $"Assets/{FolderRelativeToAssets}/{fileName}";
        }
    }
}
