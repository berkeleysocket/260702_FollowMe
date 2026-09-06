#if UNITY_EDITOR
using System.IO;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEngine;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Act2 카페 거리 소품 스프라이트 → 06.Prefab 프리팹 생성.
    /// Live: unity command act2-cafe-prefabs
    /// </summary>
    public static class Act2CafePropPrefabCli
    {
        private const string SpriteDir = "Assets/00.Work/KDS/05.Asset/City_Modern/Act2_Cafe";
        private const string PrefabDir = "Assets/00.Work/KDS/06.Prefab/Act2_Cafe";

        private static readonly string[] PropFiles =
        {
            "Act2_Prop_LatteTable.png",
            "Act2_Prop_MacaronSculpture.png",
            "Act2_Prop_DonutWall.png",
            "Act2_Prop_CafeSign.png",
            "Act2_Prop_Terrace.png",
            "Act2_Prop_MirrorWall.png",
            "Act2_Prop_Greenhouse.png",
            "Act2_Prop_BookWall.png",
            "Act2_Prop_RoundWindow.png",
            "Act2_Prop_PatioUmbrella.png",
            "Act2_Prop_NightWindow.png",
            "Act2_Prop_DessertCart.png",
            "Act2_Prop_ClosedShutter.png",
            "Act2_Prop_PianoCorner.png",
            "Act2_Prop_PhotoSpotMark.png",
            "Act2_Prop_ParfaitTower.png",
            "Act2_Prop_StringLights.png",
            "Act2_Prop_BrickAlleyDoor.png",
            "Act2_Prop_CheckerSpot.png",
        };

        [CliCommand("act2-cafe-prefabs", "Import Act2 cafe prop sprites and build prefabs under KDS/06.Prefab")]
        public static int BuildFromPipeline()
        {
            return Build() ? 0 : 1;
        }

        [MenuItem("FollowMe/KDS/Build Act2 Cafe Prefabs")]
        public static void BuildFromMenu()
        {
            Build();
        }

        public static bool Build()
        {
            EnsureFolder("Assets/00.Work/KDS/06.Prefab");
            EnsureFolder(PrefabDir);

            AssetDatabase.Refresh();

            int ok = 0;
            foreach (var file in PropFiles)
            {
                string spritePath = $"{SpriteDir}/{file}";
                if (!File.Exists(Path.GetFullPath(spritePath)))
                {
                    Debug.LogWarning($"[Act2CafePropPrefabCli] missing sprite: {spritePath}");
                    continue;
                }

                ConfigureSpriteImport(spritePath);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (sprite == null)
                {
                    // multi-sprite fallback: take first sub-asset
                    var assets = AssetDatabase.LoadAllAssetsAtPath(spritePath);
                    foreach (var a in assets)
                    {
                        if (a is Sprite s)
                        {
                            sprite = s;
                            break;
                        }
                    }
                }

                if (sprite == null)
                {
                    Debug.LogWarning($"[Act2CafePropPrefabCli] no Sprite at {spritePath}");
                    continue;
                }

                string prefabName = Path.GetFileNameWithoutExtension(file);
                string prefabPath = $"{PrefabDir}/{prefabName}.prefab";

                var go = new GameObject(prefabName);
                try
                {
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = sprite;
                    sr.sortingOrder = SortingOrderFor(prefabName);

                    PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                    ok++;
                }
                finally
                {
                    Object.DestroyImmediate(go);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Act2CafePropPrefabCli] built {ok}/{PropFiles.Length} prefabs → {PrefabDir}");
            return ok > 0;
        }

        private static int SortingOrderFor(string name)
        {
            if (name.Contains("StringLights")) return 6;
            if (name.Contains("PhotoSpotMark") || name.Contains("CheckerSpot")) return 1;
            if (name.Contains("Sign") || name.Contains("Window") || name.Contains("Shutter") || name.Contains("Door"))
                return 2;
            if (name.Contains("Wall") || name.Contains("Terrace")) return 3;
            return 4;
        }

        private static void ConfigureSpriteImport(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 32f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            settings.spritePivot = new Vector2(0.5f, 0f);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            string name = Path.GetFileName(path);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name)) return;
            if (!AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
