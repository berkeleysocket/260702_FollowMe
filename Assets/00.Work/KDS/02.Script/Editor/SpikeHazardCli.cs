#if UNITY_EDITOR
using System.IO;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// 가시 장애물 스프라이트·프리팹 생성 + 현재/지정 씬 배치.
    /// Live: unity command spike-hazard-setup
    /// </summary>
    public static class SpikeHazardCli
    {
        private const string ArtDir = "Assets/00.Work/KDS/05.Asset/City_Modern/Act2_Cafe";
        private const string SpritePath = ArtDir + "/Act2_Spike_32x32.png";
        private const string PrefabDir = "Assets/00.Work/KDS/06.Prefab/Hazards";
        private const string PrefabPath = PrefabDir + "/SpikeHazard.prefab";
        private const int GroundLayer = 10;

        [CliCommand("spike-hazard-setup", "Build spike hazard sprite/prefab and place samples on Stage4")]
        public static int SetupFromPipeline() => SetupAndPlaceStage4() ? 0 : 1;

        [MenuItem("FollowMe/KDS/Build Spike Hazard Prefab")]
        public static void BuildPrefabMenu() => BuildPrefab();

        [MenuItem("FollowMe/KDS/Place Spike Hazards On Open Scene")]
        public static void PlaceOnOpenSceneMenu()
        {
            if (!EnsurePrefab())
                return;

            var scene = EditorSceneManager.GetActiveScene();
            int stage = GuessStageNumber(scene.name);
            int n = PlaceOnActiveScene(stage);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[SpikeHazardCli] placed {n} spikes on {scene.name}");
        }

        public static bool SetupAndPlaceStage4()
        {
            if (!BuildPrefab())
                return false;

            var scene = EditorSceneManager.OpenScene(
                "Assets/00.Work/KDS/01.Scene/Stage4 Scene.unity", OpenSceneMode.Single);
            int n = PlaceOnActiveScene(4);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[SpikeHazardCli] Stage4 spikes={n}");
            return n > 0;
        }

        public static bool BuildPrefab()
        {
            EnsureFolder(ArtDir);
            EnsureFolder("Assets/00.Work/KDS/06.Prefab");
            EnsureFolder(PrefabDir);

            if (!WriteSpikeTexture(SpritePath))
                return false;

            ConfigureSpriteImport(SpritePath);
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
            if (sprite == null)
            {
                Debug.LogError("[SpikeHazardCli] sprite missing after import: " + SpritePath);
                return false;
            }

            var go = new GameObject("SpikeHazard");
            try
            {
                go.layer = GroundLayer;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = 8;
                sr.color = Color.white;

                var box = go.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                // 가시 끝만 닿게 — 하단 약간 여유
                box.size = new Vector2(0.9f, 0.55f);
                box.offset = new Vector2(0f, 0.28f);

                go.AddComponent<SpikeHazard>();

                PrefabUtility.SaveAsPrefabAsset(go, PrefabPath);
            }
            finally
            {
                Object.DestroyImmediate(go);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[SpikeHazardCli] prefab → " + PrefabPath);
            return true;
        }

        public static bool EnsurePrefab()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null)
                return true;
            return BuildPrefab();
        }

        public static int PlaceOnActiveScene(int stage)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (prefab == null)
            {
                Debug.LogError("[SpikeHazardCli] missing prefab");
                return 0;
            }

            var level = GameObject.Find($"Level_S{stage}") ?? FindAnyLevel();
            if (level == null)
            {
                Debug.LogError("[SpikeHazardCli] Level_S* missing");
                return 0;
            }

            var old = level.transform.Find("Hazards");
            if (old != null)
                Object.DestroyImmediate(old.gameObject);

            var root = new GameObject("Hazards");
            root.transform.SetParent(level.transform, false);

            var positions = BuildSpikeLayout(stage);
            for (int i = 0; i < positions.Length; i++)
            {
                var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.transform);
                go.name = $"Spike_{i:00}";
                go.transform.position = positions[i];
            }

            return positions.Length;
        }

        private static Vector3[] BuildSpikeLayout(int stage)
        {
            // Act2 기준 지면 y≈0 상단 — 발 높이 가시
            float y = 0.55f;
            if (stage <= 0) stage = 4;

            // 스테이지마다 플랫폼·러닝 리듬에 맞춘 샘플 배치
            return stage switch
            {
                4 => new[]
                {
                    new Vector3(22f, y, 0f),
                    new Vector3(36f, y, 0f),
                    new Vector3(54f, y, 0f),
                    new Vector3(72f, y, 0f),
                    new Vector3(88f, y, 0f),
                    new Vector3(118f, y, 0f),
                    new Vector3(148f, y, 0f),
                    new Vector3(175f, y, 0f),
                },
                5 => new[]
                {
                    new Vector3(24f, y, 0f), new Vector3(40f, y, 0f), new Vector3(58f, y, 0f),
                    new Vector3(78f, y, 0f), new Vector3(102f, y, 0f), new Vector3(130f, y, 0f),
                    new Vector3(158f, y, 0f), new Vector3(185f, y, 0f),
                },
                6 => new[]
                {
                    new Vector3(26f, y, 0f), new Vector3(44f, y, 0f), new Vector3(62f, y, 0f),
                    new Vector3(84f, y, 0f), new Vector3(110f, y, 0f), new Vector3(140f, y, 0f),
                    new Vector3(168f, y, 0f), new Vector3(195f, y, 0f),
                },
                7 => new[]
                {
                    new Vector3(28f, y, 0f), new Vector3(46f, y, 0f), new Vector3(66f, y, 0f),
                    new Vector3(90f, y, 0f), new Vector3(118f, y, 0f), new Vector3(148f, y, 0f),
                    new Vector3(178f, y, 0f), new Vector3(205f, y, 0f),
                },
                8 => new[]
                {
                    new Vector3(30f, y, 0f), new Vector3(48f, y, 0f), new Vector3(70f, y, 0f),
                    new Vector3(96f, y, 0f), new Vector3(124f, y, 0f), new Vector3(156f, y, 0f),
                    new Vector3(188f, y, 0f), new Vector3(218f, y, 0f),
                },
                _ => new[]
                {
                    new Vector3(20f, y, 0f),
                    new Vector3(40f, y, 0f),
                    new Vector3(60f, y, 0f),
                }
            };
        }

        private static bool WriteSpikeTexture(string assetPath)
        {
            const int w = 32;
            const int h = 32;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            var clear = new Color(0f, 0f, 0f, 0f);
            var tip = new Color(0.85f, 0.88f, 0.92f, 1f);
            var edge = new Color(0.25f, 0.22f, 0.28f, 1f);
            var body = new Color(0.55f, 0.52f, 0.58f, 1f);
            var baseCol = new Color(0.35f, 0.32f, 0.38f, 1f);

            var pixels = new Color[w * h];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = clear;

            // 3개 삼각형 가시 (바닥에서 위로)
            DrawSpike(pixels, w, h, 5, tip, edge, body, baseCol);
            DrawSpike(pixels, w, h, 16, tip, edge, body, baseCol);
            DrawSpike(pixels, w, h, 27, tip, edge, body, baseCol);

            // 받침
            for (int x = 2; x < w - 2; x++)
            {
                pixels[Index(x, 0, w)] = baseCol;
                pixels[Index(x, 1, w)] = edge;
            }

            tex.SetPixels(pixels);
            tex.Apply();

            string full = Path.GetFullPath(assetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllBytes(full, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(assetPath);
            return true;
        }

        private static void DrawSpike(
            Color[] pixels, int w, int h, int cx,
            Color tip, Color edge, Color body, Color baseCol)
        {
            int top = 28;
            int bottom = 2;
            int half = 5;
            for (int y = bottom; y <= top; y++)
            {
                float t = (y - bottom) / (float)(top - bottom);
                int halfNow = Mathf.Max(0, Mathf.RoundToInt(half * (1f - t)));
                for (int x = cx - halfNow; x <= cx + halfNow; x++)
                {
                    if (x < 0 || x >= w) continue;
                    Color c = body;
                    if (y >= top - 1) c = tip;
                    else if (x == cx - halfNow || x == cx + halfNow) c = edge;
                    else if (y <= bottom + 1) c = baseCol;
                    pixels[Index(x, y, w)] = c;
                }
            }
        }

        private static int Index(int x, int y, int w) => y * w + x;

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
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            settings.spritePivot = new Vector2(0.5f, 0f);
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }

        private static GameObject FindAnyLevel()
        {
            for (int i = 1; i <= 16; i++)
            {
                var go = GameObject.Find($"Level_S{i}");
                if (go != null) return go;
            }

            return null;
        }

        private static int GuessStageNumber(string sceneName)
        {
            for (int i = 1; i <= 16; i++)
            {
                if (sceneName.Contains($"Stage{i}"))
                    return i;
            }

            return 4;
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
