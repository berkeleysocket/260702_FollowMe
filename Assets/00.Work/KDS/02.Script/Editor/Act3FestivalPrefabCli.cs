#if UNITY_EDITOR
using System.IO;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEngine;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Act3 축제 소품·장애물·불꽃윈도우 스프라이트 → 프리팹.
    /// Live: unity command act3-festival-prefabs
    /// </summary>
    public static class Act3FestivalPrefabCli
    {
        private const string SpriteDir = "Assets/00.Work/KDS/05.Asset/City_Modern/Act3_Fireworks";
        private const string PropPrefabDir = "Assets/00.Work/KDS/06.Prefab/Act3_Fireworks";
        private const string HazardPrefabDir = "Assets/00.Work/KDS/06.Prefab/Hazards";

        private static readonly string[] PropFiles =
        {
            "Act3_Prop_RiverRail.png",
            "Act3_Prop_ParkBench.png",
            "Act3_Prop_FoodStall.png",
            "Act3_Prop_LanternPole.png",
            "Act3_Prop_FestivalFlag.png",
            "Act3_Prop_CrowdSilhouette.png",
            "Act3_Prop_BridgePillar.png",
            "Act3_Prop_PierPlank.png",
            "Act3_Prop_TreeAutumn.png",
            "Act3_Prop_TrashBin.png",
            "Act3_Prop_VendorCart.png",
            "Act3_Prop_StringLanterns.png",
            "Act3_Prop_SpeakerStack.png",
            "Act3_Prop_BarrierCone.png",
            "Act3_Prop_PhotoFrame.png",
            "Act3_Prop_EmptyBench.png",
            "Act3_Prop_RiverEdge.png",
            "Act3_Prop_NightBush.png",
            "Act3_Tile_NightPath.png",
            "Act3_Tile_BridgeDeck.png",
            "Act3_BG_NightSky.png",
        };

        [CliCommand("act3-festival-prefabs", "Import Act3 festival sprites and build prop/hazard prefabs")]
        public static int BuildFromPipeline() => BuildAll() ? 0 : 1;

        [MenuItem("FollowMe/KDS/Build Act3 Festival Prefabs")]
        public static void BuildFromMenu() => BuildAll();

        public static bool BuildAll()
        {
            EnsureFolder("Assets/00.Work/KDS/06.Prefab");
            EnsureFolder(PropPrefabDir);
            EnsureFolder(HazardPrefabDir);

            AssetDatabase.Refresh();

            int props = 0;
            foreach (var file in PropFiles)
            {
                if (BuildSimpleProp(file, SortingOrderFor(file)))
                    props++;
            }

            bool ember = BuildHazard(
                "Act3_Hazard_Ember.png",
                "EmberHazard",
                typeof(EmberHazard),
                new Vector2(0.45f, 0.55f),
                new Vector2(0f, 0.1f));

            bool crowd = BuildHazard(
                "Act3_Hazard_CrowdBump.png",
                "CrowdBumpHazard",
                typeof(CrowdBumpHazard),
                new Vector2(1.0f, 0.9f),
                new Vector2(0f, 0.35f));

            bool window = BuildFireworkWindow();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Act3FestivalPrefabCli] props={props} ember={ember} crowd={crowd} fireworkWindow={window}");
            return props > 0 && ember && crowd && window;
        }

        private static bool BuildSimpleProp(string file, int sortingOrder)
        {
            string spritePath = $"{SpriteDir}/{file}";
            if (!File.Exists(Path.GetFullPath(spritePath)))
            {
                Debug.LogWarning("[Act3FestivalPrefabCli] missing " + spritePath);
                return false;
            }

            ConfigureSpriteImport(spritePath, bottomCenter: true);
            Sprite sprite = LoadSprite(spritePath);
            if (sprite == null) return false;

            string prefabName = Path.GetFileNameWithoutExtension(file);
            string prefabPath = $"{PropPrefabDir}/{prefabName}.prefab";

            var go = new GameObject(prefabName);
            try
            {
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = sortingOrder;
                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                return true;
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        private static bool BuildHazard(
            string file, string prefabName, System.Type componentType, Vector2 boxSize, Vector2 boxOffset)
        {
            string spritePath = $"{SpriteDir}/{file}";
            if (!File.Exists(Path.GetFullPath(spritePath)))
            {
                Debug.LogWarning("[Act3FestivalPrefabCli] missing " + spritePath);
                return false;
            }

            ConfigureSpriteImport(spritePath, bottomCenter: true);
            Sprite sprite = LoadSprite(spritePath);
            if (sprite == null) return false;

            string prefabPath = $"{HazardPrefabDir}/{prefabName}.prefab";
            var go = new GameObject(prefabName);
            try
            {
                go.layer = 0;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = 9;

                var box = go.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                box.size = boxSize;
                box.offset = boxOffset;

                go.AddComponent(componentType);
                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                return true;
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        private static bool BuildFireworkWindow()
        {
            string spritePath = $"{SpriteDir}/Act3_Vfx_FireworkWindow.png";
            if (!File.Exists(Path.GetFullPath(spritePath)))
                return false;

            ConfigureSpriteImport(spritePath, bottomCenter: false);
            Sprite sprite = LoadSprite(spritePath);
            if (sprite == null) return false;

            string prefabPath = $"{HazardPrefabDir}/FireworkLikeWindow.prefab";
            // keep with hazards/effects folder for placement convenience
            var go = new GameObject("FireworkLikeWindow");
            try
            {
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = 12;
                sr.color = new Color(1f, 1f, 1f, 0.25f);

                var box = go.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                box.size = new Vector2(2.2f, 2.2f);

                var window = go.AddComponent<FireworkLikeWindow>();
                var so = new SerializedObject(window);
                so.FindProperty("_visual").objectReferenceValue = sr;
                so.ApplyModifiedPropertiesWithoutUndo();

                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                return true;
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        private static Sprite LoadSprite(string path)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null) return sprite;
            foreach (var a in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (a is Sprite s)
                    return s;
            }

            return null;
        }

        private static int SortingOrderFor(string file)
        {
            if (file.Contains("BG_")) return -25;
            if (file.Contains("Tile_")) return -2;
            if (file.Contains("StringLanterns") || file.Contains("FestivalFlag")) return 6;
            if (file.Contains("Crowd") || file.Contains("Tree") || file.Contains("BridgePillar")) return 1;
            if (file.Contains("FoodStall") || file.Contains("Vendor") || file.Contains("Speaker")) return 3;
            return 2;
        }

        private static void ConfigureSpriteImport(string assetPath, bool bottomCenter)
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
            settings.spriteMeshType = SpriteMeshType.FullRect;
            if (bottomCenter)
            {
                settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                settings.spritePivot = new Vector2(0.5f, 0f);
            }
            else
            {
                settings.spriteAlignment = (int)SpriteAlignment.Center;
                settings.spritePivot = new Vector2(0.5f, 0.5f);
            }

            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            string name = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);
            if (!string.IsNullOrEmpty(parent))
                AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
