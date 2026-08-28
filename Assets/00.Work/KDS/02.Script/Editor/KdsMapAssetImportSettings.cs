#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// KDS 맵 작업용 에셋(Cafe S4-S8 · Park S9-S11) 임포트 규칙 — PPU32 · Point.
    /// HUD/플레이어/이모티콘 폴더는 대상外.
    /// </summary>
    public static class KdsMapAssetImportSettings
    {
        private static readonly string[] MapAssetFolders =
        {
            "Assets/00.Work/KDS/05.Asset/City_Cafe",
            "Assets/00.Work/KDS/05.Asset/City_Park",
            "Assets/00.Work/KDS/05.Asset/VFX_Fireworks/Stealthix_Fireworks"
        };

        [MenuItem("FollowMe/KDS/Apply Map Asset Import (Cafe S4-S8 + Park S9-S11)")]
        public static void ApplyMapAssetImport()
        {
            int count = 0;
            foreach (string folder in MapAssetFolders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                    continue;

                foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { folder }))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (Apply(path))
                        count++;
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[KdsMapAssetImport] PPU32/Point/Uncompressed 적용: {count} textures (Cafe+Park+Fireworks)");
        }

        private static bool Apply(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                return false;

            bool dirty = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                dirty = true;
            }

            if (Mathf.Abs(importer.spritePixelsPerUnit - 32f) > 0.01f)
            {
                importer.spritePixelsPerUnit = 32f;
                dirty = true;
            }

            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                dirty = true;
            }

            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                dirty = true;
            }

            if (dirty)
                importer.SaveAndReimport();

            return dirty;
        }
    }
}
#endif
