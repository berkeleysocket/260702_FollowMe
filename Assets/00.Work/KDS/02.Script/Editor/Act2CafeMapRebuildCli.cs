#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using YHW.Stats;
using YHW.UI;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Act2(S4~S8) 카페 맵을 기존 레이아웃 폐기 후 길게·플랫폼 밀집으로 재생성.
    /// Live: unity command act2-cafe-maps
    /// </summary>
    public static class Act2CafeMapRebuildCli
    {
        private const string SceneDir = "Assets/00.Work/KDS/01.Scene";
        private const string RuleTilePath =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act1_Tiles/RuleTiles/Act1_Brick_Ground_RuleTile.asset";
        private const string PlatLeftPath =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act1_Tiles/Tiles/Act1_Brick_Platform_Left.asset";
        private const string PlatCenterPath =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act1_Tiles/Tiles/Act1_Brick_Platform_Center.asset";
        private const string PlatRightPath =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act1_Tiles/Tiles/Act1_Brick_Platform_Right.asset";
        private const string ImogeDir = "Assets/00.Work/KDS/06.Prefab/Like_Imoge";
        private const string HeartPrefab = ImogeDir + "/Follow_13.prefab";
        private const string StressBarPrefab = "Assets/00.Work/KDS/06.Prefab/StressBarCanvas.prefab";
        private const string PhotoPrefabDir = "Assets/00.Work/KDS/06.Prefab/Act2_PhotoSpot";
        private const string PropSpriteDir = "Assets/00.Work/KDS/05.Asset/City_Modern/Act2_Cafe";
        private const int GroundLayer = 10;

        private static readonly string[] EmojiPrefabs =
        {
            ImogeDir + "/Like_00.prefab",
            ImogeDir + "/Like_01.prefab",
            ImogeDir + "/Like_02.prefab",
            ImogeDir + "/Like_03.prefab",
            ImogeDir + "/Like_04.prefab",
            ImogeDir + "/Like_05.prefab",
            ImogeDir + "/Like_06.prefab",
            ImogeDir + "/Like_07.prefab",
        };

        private static readonly string[] PhotoPrefabNames =
        {
            "Photo_Act2_LatteArt",
            "Photo_Act2_Macaron",
            "Photo_Act2_DonutWall",
            "Photo_Act2_NeonCafe",
            "Photo_Act2_Terrace",
            "Photo_Act2_PlantCafe",
            "Photo_Act2_MirrorRoom",
            "Photo_Act2_DessertCart",
            "Photo_Act2_RoundWindow",
            "Photo_Act2_NightWindow",
        };

        private static readonly string[] PropSpriteNames =
        {
            "Act2_Prop_CafeSign.png",
            "Act2_Prop_LatteTable.png",
            "Act2_Prop_PatioUmbrella.png",
            "Act2_Prop_DessertCart.png",
            "Act2_Prop_StringLights.png",
            "Act2_Prop_BrickAlleyDoor.png",
            "Act2_Prop_RoundWindow.png",
            "Act2_Prop_Greenhouse.png",
            "Act2_Prop_BookWall.png",
            "Act2_Prop_ClosedShutter.png",
        };

        [CliCommand("act2-cafe-maps", "Discard and rebuild Act2 cafe stages S4-S8 (longer + more platforms)")]
        public static int RebuildFromPipeline() => RebuildAll() ? 0 : 1;

        [MenuItem("FollowMe/KDS/Rebuild Cafe Stages (S4-S8) From Scratch")]
        public static void RebuildFromMenu() => RebuildAll();

        public static bool RebuildAll()
        {
            var rule = AssetDatabase.LoadAssetAtPath<TileBase>(RuleTilePath);
            var left = AssetDatabase.LoadAssetAtPath<TileBase>(PlatLeftPath);
            var center = AssetDatabase.LoadAssetAtPath<TileBase>(PlatCenterPath);
            var right = AssetDatabase.LoadAssetAtPath<TileBase>(PlatRightPath);
            if (rule == null || left == null || center == null || right == null)
            {
                Debug.LogError("[Act2CafeMapRebuildCli] missing brick ground/platform tiles");
                return false;
            }

            var heart = AssetDatabase.LoadAssetAtPath<GameObject>(HeartPrefab);
            if (heart == null)
            {
                Debug.LogError("[Act2CafeMapRebuildCli] missing " + HeartPrefab);
                return false;
            }

            var emojis = new GameObject[EmojiPrefabs.Length];
            for (int i = 0; i < EmojiPrefabs.Length; i++)
            {
                emojis[i] = AssetDatabase.LoadAssetAtPath<GameObject>(EmojiPrefabs[i]);
                if (emojis[i] == null)
                {
                    Debug.LogError("[Act2CafeMapRebuildCli] missing " + EmojiPrefabs[i]);
                    return false;
                }
            }

            var sb = new System.Text.StringBuilder();
            for (int stage = 4; stage <= 8; stage++)
            {
                if (!RebuildStage(stage, rule, left, center, right, heart, emojis, sb))
                    return false;
            }

            Debug.Log("[Act2CafeMapRebuildCli]\n" + sb);
            return true;
        }

        private static bool RebuildStage(
            int stage,
            TileBase rule,
            TileBase left,
            TileBase center,
            TileBase right,
            GameObject heart,
            GameObject[] emojis,
            System.Text.StringBuilder results)
        {
            string scenePath = $"{SceneDir}/Stage{stage} Scene.unity";
            if (!File.Exists(Path.GetFullPath(scenePath)))
            {
                Debug.LogError("[Act2CafeMapRebuildCli] missing scene " + scenePath);
                return false;
            }

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var spec = StageMapDatabase.Get(stage);
            string levelName = $"Level_S{stage}";
            var level = GameObject.Find(levelName);
            if (level == null)
            {
                // rename leftovers
                for (int s = 1; s <= 8; s++)
                {
                    var found = GameObject.Find($"Level_S{s}");
                    if (found != null)
                    {
                        found.name = levelName;
                        level = found;
                        break;
                    }
                }
            }

            if (level == null)
            {
                level = new GameObject(levelName);
            }

            var grid = GameObject.Find("Grid");
            if (grid == null)
            {
                grid = new GameObject("Grid", typeof(Grid));
            }

            DiscardLegacyMapContent(level.transform, stage);
            EnsureSystems(stage);

            var ground = EnsureTilemap(grid.transform, "Tilemap_Ground", sortingOrder: 0);
            // 타일맵 플랫폼은 일부 씬에서 SetTile이 유실되어 GO 플랫폼 사용
            var platMap = grid.transform.Find("Tilemap_Platform");
            if (platMap != null)
            {
                var tm = platMap.GetComponent<Tilemap>();
                if (tm != null) tm.ClearAllTiles();
            }

            RebuildGround(ground, rule, spec);
            int platCount = RebuildPlatforms(level.transform, left, center, right, spec);
            RebuildBackground(level.transform, spec);
            RebuildForks(level.transform, spec);
            RebuildCheckpoints(level.transform, spec);
            RebuildGoal(level.transform, spec);
            RebuildCollectibles(level.transform, spec, heart, emojis);
            RebuildThemeProps(level.transform, spec);
            RebuildPhotoPoints(level.transform, spec);
            PlacePlayerStart();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            results.AppendLine(
                $"S{stage}: LengthX={spec.LengthX:0} platforms={platCount} goalX={spec.LengthX - 4f:0} saved");
            return true;
        }

        private static void DiscardLegacyMapContent(Transform level, int stage)
        {
            string[] wipeChildren =
            {
                "Platforms", "Collectibles", "ThemeProps", "Goals", "Forks",
                "Checkpoints", "Hazards", "PhotoPoints", "Background"
            };
            foreach (var name in wipeChildren)
            {
                var child = level.Find(name);
                if (child != null)
                    Object.DestroyImmediate(child.gameObject);
            }

            // root-level leftover photo spots / old goals
            var roots = sceneRoots();
            foreach (var go in roots)
            {
                if (go == null) continue;
                if (go.name.StartsWith("Photo_Act2_") ||
                    go.name == "Zone_Goal" ||
                    go.name == "StageGoal" ||
                    go.name.StartsWith("CafePlatform_") ||
                    go.name.StartsWith("ForkPlatform_"))
                {
                    Object.DestroyImmediate(go);
                }
            }

            // wipe platform tilemap content later via Ensure
            var plat = GameObject.Find("Grid/Tilemap_Platform");
            if (plat != null)
            {
                var tm = plat.GetComponent<Tilemap>();
                if (tm != null) tm.ClearAllTiles();
            }

            var ground = GameObject.Find("Grid/Tilemap_Ground");
            if (ground != null)
            {
                var tm = ground.GetComponent<Tilemap>();
                if (tm != null) tm.ClearAllTiles();
            }

            // remove duplicate Level_S* besides current
            foreach (var go in roots)
            {
                if (go == null) continue;
                if (go.name.StartsWith("Level_S") && go.name != $"Level_S{stage}")
                    Object.DestroyImmediate(go);
            }
        }

        private static GameObject[] sceneRoots()
        {
            var scene = EditorSceneManager.GetActiveScene();
            return scene.GetRootGameObjects();
        }

        private static Tilemap EnsureTilemap(Transform grid, string name, int sortingOrder)
        {
            var t = grid.Find(name);
            GameObject go;
            if (t == null)
            {
                go = new GameObject(name);
                go.transform.SetParent(grid, false);
                go.AddComponent<Tilemap>();
                go.AddComponent<TilemapRenderer>();
            }
            else
            {
                go = t.gameObject;
            }

            go.layer = GroundLayer;
            var map = go.GetComponent<Tilemap>();
            var renderer = go.GetComponent<TilemapRenderer>();
            renderer.sortingOrder = sortingOrder;

            var rb = go.GetComponent<Rigidbody2D>();
            if (rb == null) rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            var tmc = go.GetComponent<TilemapCollider2D>();
            if (tmc == null) tmc = go.AddComponent<TilemapCollider2D>();

            var cc = go.GetComponent<CompositeCollider2D>();
            if (cc == null) cc = go.AddComponent<CompositeCollider2D>();
            cc.geometryType = CompositeCollider2D.GeometryType.Outlines;
            tmc.compositeOperation = Collider2D.CompositeOperation.Merge;

            map.ClearAllTiles();
            return map;
        }

        private static void RebuildGround(Tilemap ground, TileBase rule, StageMapSpec spec)
        {
            int x0 = -8;
            int x1 = Mathf.CeilToInt(spec.LengthX) + 12;
            for (int x = x0; x <= x1; x++)
            {
                for (int y = -5; y <= 0; y++)
                    ground.SetTile(new Vector3Int(x, y, 0), rule);
            }
        }

        private static int RebuildPlatforms(
            Transform level, TileBase left, TileBase center, TileBase right, StageMapSpec spec)
        {
            // left/center/right kept for sprite source
            Sprite platSprite = null;
            if (center is Tile cTile && cTile.sprite != null)
                platSprite = cTile.sprite;
            else if (left is Tile lTile && lTile.sprite != null)
                platSprite = lTile.sprite;

            var old = level.Find("Platforms");
            if (old != null)
                Object.DestroyImmediate(old.gameObject);

            var root = new GameObject("Platforms");
            root.transform.SetParent(level, false);

            int count = 0;
            float x = spec.IntroEnd + 4f;
            int step = 0;
            while (x < spec.SetpieceEnd - 8f)
            {
                float w = 4f + (step % 3);
                float[] heights = { 2.2f, 3.0f, 3.8f, 3.0f, 2.4f, 4.2f };
                float y = heights[step % heights.Length];
                if (x > spec.PressureEnd && step % 2 == 0)
                    y += 0.6f;

                AddPlatform(root.transform, platSprite, x + w * 0.5f, y, w, 0.45f, $"RunPlat_{count:00}");
                count++;
                step++;
                x += 10f + (step % 4);
            }

            var forks = StageCollectibleLayout.GetForkXs(spec);
            for (int i = 0; i < forks.Length; i++)
            {
                float fx = forks[i];
                AddPlatform(root.transform, platSprite, fx, 3.1f, 12f, 0.5f, $"ForkPlatA_{i + 1}");
                AddPlatform(root.transform, platSprite, fx + 6f, 4.2f, 7f, 0.45f, $"ForkPlatB_{i + 1}");
                count += 2;
            }

            float chainX = spec.TeachEnd + 6f;
            int chain = 0;
            while (chainX < spec.BreathEnd - 4f && chain < 8 + spec.Stage)
            {
                float y = 2.2f + (chain % 3) * 0.8f;
                AddPlatform(root.transform, platSprite, chainX, y, 3.2f, 0.4f, $"ChainPlat_{chain:00}");
                count++;
                chain++;
                chainX += 7.5f;
            }

            AddPlatform(root.transform, platSprite, spec.LengthX - 18f, 2.2f, 6f, 0.45f, "GoalPlat_A");
            AddPlatform(root.transform, platSprite, spec.LengthX - 12f, 3.1f, 5f, 0.45f, "GoalPlat_B");
            count += 2;

            var rng = new System.Random(1000 + spec.Stage * 97);
            int extras = 8 + (spec.Stage - 4) * 4;
            for (int i = 0; i < extras; i++)
            {
                float px = rng.Next(Mathf.RoundToInt(spec.IntroEnd + 8f), Mathf.RoundToInt(spec.LengthX - 20f));
                float py = 2.0f + rng.Next(0, 5) * 0.7f;
                float pw = 3f + rng.Next(0, 4);
                AddPlatform(root.transform, platSprite, px, py, pw, 0.4f, $"ExtraPlat_{i:00}");
                count++;
            }

            return count;
        }

        private static void AddPlatform(
            Transform parent, Sprite sprite, float x, float y, float width, float height, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(x, y, 0f);
            go.layer = GroundLayer;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = new Vector2(width, height);
            sr.color = Color.white;
            sr.sortingOrder = 5;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(width, height);
        }

        private static void PlacePlatform(
            Tilemap map, TileBase left, TileBase center, TileBase right, int startX, int y, int width)
        {
            // legacy helper kept unused — GO platforms preferred
            width = Mathf.Max(2, width);
            for (int i = 0; i < width; i++)
            {
                TileBase tile = center;
                if (i == 0) tile = left;
                else if (i == width - 1) tile = right;
                map.SetTile(new Vector3Int(startX + i, y, 0), tile);
            }
        }

        private static void RebuildBackground(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("Background");
            root.transform.SetParent(level, false);

            // simple sky slabs every 32 units
            var white = Texture2D.whiteTexture;
            var sprite = Sprite.Create(white, new Rect(0, 0, white.width, white.height), new Vector2(0.5f, 0.5f), 4f);
            for (float x = -8f; x < spec.LengthX + 16f; x += 32f)
            {
                var sky = new GameObject($"Sky_{Mathf.RoundToInt(x)}");
                sky.transform.SetParent(root.transform, false);
                sky.transform.position = new Vector3(x, 8f, 0f);
                sky.transform.localScale = new Vector3(34f, 18f, 1f);
                var sr = sky.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.color = new Color(0.55f, 0.82f, 0.95f, 0.55f);
                sr.sortingOrder = -20;
            }
        }

        private static void RebuildForks(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("Forks");
            root.transform.SetParent(level, false);
            var forks = StageCollectibleLayout.GetForkXs(spec);
            for (int i = 0; i < forks.Length; i++)
            {
                var go = new GameObject($"Fork_{i + 1}");
                go.transform.SetParent(root.transform, false);
                go.transform.position = new Vector3(forks[i], 2f, 0f);
            }
        }

        private static void RebuildCheckpoints(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("Checkpoints");
            root.transform.SetParent(level, false);

            int n = Mathf.Max(1, spec.Checkpoints);
            for (int i = 0; i < n; i++)
            {
                float t = (i + 1f) / (n + 1f);
                float x = Mathf.Lerp(spec.IntroEnd, spec.SetpieceEnd, t);
                var go = new GameObject($"CP_{i + 1}");
                go.transform.SetParent(root.transform, false);
                go.transform.position = new Vector3(x, 1.5f, 0f);
                var box = go.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                box.size = new Vector2(2.5f, 4f);
                var cp = go.AddComponent<Checkpoint>();
                var so = new SerializedObject(cp);
                so.FindProperty("_checkpointId").stringValue = $"CP_S{spec.Stage}_{i + 1}";
                so.FindProperty("_registerOnStart").boolValue = i == 0;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void RebuildGoal(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("Goals");
            root.transform.SetParent(level, false);

            var go = new GameObject("Zone_Goal");
            go.transform.SetParent(root.transform, false);
            go.transform.position = new Vector3(spec.LengthX - 4f, 1.5f, 0f);
            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(3.5f, 5f);
            var goal = go.AddComponent<StageGoal>();
            var so = new SerializedObject(goal);
            so.FindProperty("_stageNumber").intValue = spec.Stage;
            so.FindProperty("_pauseOnClear").boolValue = true;
            so.FindProperty("_ensureVisual").boolValue = true;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void RebuildCollectibles(
            Transform level, StageMapSpec spec, GameObject heart, GameObject[] emojis)
        {
            var root = new GameObject("Collectibles");
            root.transform.SetParent(level, false);

            var spawns = StageCollectibleLayout.Build(spec);
            int followN = 0, likeN = 0;
            for (int i = 0; i < spawns.Count; i++)
            {
                var s = spawns[i];
                if (s.Kind == CollectibleKind.Follow)
                {
                    Spawn(heart, root.transform, s.X, s.Y, $"Follow_{followN:00}");
                    followN++;
                }
                else if (s.Kind == CollectibleKind.Like)
                {
                    // 높은 좋아요는 플랫폼 높이로 살짝 보정
                    float y = s.Y;
                    if (y >= StageCollectibleLayout.GetHighLikeY(spec) - 0.1f)
                        y = StageCollectibleLayout.GetPlatformY(spec) + 1.1f;
                    Spawn(emojis[likeN % emojis.Length], root.transform, s.X, y, $"Like_{likeN:00}");
                    likeN++;
                }
            }
        }

        private static void Spawn(GameObject prefab, Transform parent, float x, float y, string name)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.name = name;
            go.transform.position = new Vector3(x, y, 0f);
        }

        private static void RebuildThemeProps(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("ThemeProps");
            root.transform.SetParent(level, false);

            float x = 12f;
            int i = 0;
            while (x < spec.LengthX - 10f)
            {
                string file = PropSpriteNames[i % PropSpriteNames.Length];
                string path = $"{PropSpriteDir}/{file}";
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite == null)
                {
                    foreach (var a in AssetDatabase.LoadAllAssetsAtPath(path))
                    {
                        if (a is Sprite s)
                        {
                            sprite = s;
                            break;
                        }
                    }
                }

                if (sprite != null)
                {
                    var go = new GameObject(Path.GetFileNameWithoutExtension(file) + $"_{i:00}");
                    go.transform.SetParent(root.transform, false);
                    go.transform.position = new Vector3(x, 0f, 0f);
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = sprite;
                    sr.sortingOrder = -4;
                }

                i++;
                x += 16f + (i % 5);
            }
        }

        private static void RebuildPhotoPoints(Transform level, StageMapSpec spec)
        {
            // remove root leftovers already done; place under level/PhotoPoints
            var root = new GameObject("PhotoPoints");
            root.transform.SetParent(level, false);

            float[] xs = StageMapDatabase.GetPhotoPositions(spec);
            for (int i = 0; i < xs.Length; i++)
            {
                string prefabName = PhotoPrefabNames[i % PhotoPrefabNames.Length];
                // stage-based offset so S4~S8 use different spots
                int idx = (spec.Stage - 4 + i * 2) % PhotoPrefabNames.Length;
                prefabName = PhotoPrefabNames[idx];
                string path = $"{PhotoPrefabDir}/{prefabName}.prefab";
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;

                var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.transform);
                go.name = prefabName;
                float y = StageCollectibleLayout.GetPlatformY(spec) + 0.4f;
                go.transform.position = new Vector3(xs[i], y, 0f);
            }
        }

        private static void PlacePlayerStart()
        {
            var player = GameObject.Find("Player");
            if (player != null)
                player.transform.position = new Vector3(-2f, 1f, 0f);
        }

        private static void EnsureSystems(int stage)
        {
            var systems = GameObject.Find("SocialSystems");
            if (systems == null)
                systems = new GameObject("SocialSystems");

            if (systems.GetComponent<SocialScoreService>() == null)
                systems.AddComponent<SocialScoreService>();
            if (systems.GetComponent<SocialScoreHud>() == null)
                systems.AddComponent<SocialScoreHud>();
            if (systems.GetComponent<SocialItemScoreBridge>() == null)
                systems.AddComponent<SocialItemScoreBridge>();

            var meter = systems.GetComponent<StressMeter>();
            if (meter == null)
                meter = systems.AddComponent<StressMeter>();

            var boot = systems.GetComponent<StressMeterBootstrap>();
            if (boot == null)
                boot = systems.AddComponent<StressMeterBootstrap>();

            var bootSo = new SerializedObject(boot);
            bootSo.FindProperty("_meter").objectReferenceValue = meter;
            bootSo.FindProperty("_startFull").boolValue = true;
            bootSo.ApplyModifiedPropertiesWithoutUndo();

            var scoreSo = new SerializedObject(systems.GetComponent<SocialScoreService>());
            var meterProp = scoreSo.FindProperty("_stressMeter");
            if (meterProp != null)
            {
                meterProp.objectReferenceValue = meter;
                scoreSo.ApplyModifiedPropertiesWithoutUndo();
            }

            var bar = Object.FindFirstObjectByType<StressBar>();
            if (bar == null)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(StressBarPrefab);
                if (prefab != null)
                {
                    var canvas = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    canvas.name = "StressBarCanvas";
                    bar = canvas.GetComponentInChildren<StressBar>(true);
                }
            }

            if (bar != null)
            {
                var so = new SerializedObject(bar);
                so.FindProperty("meter").objectReferenceValue = meter;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            var levelSystems = GameObject.Find("LevelSystems");
            if (levelSystems == null)
                levelSystems = new GameObject("LevelSystems");
            if (levelSystems.GetComponent<CheckpointService>() == null)
                levelSystems.AddComponent<CheckpointService>();
            if (levelSystems.GetComponent<StageRunStats>() == null)
                levelSystems.AddComponent<StageRunStats>();

            levelSystems.GetComponent<StageRunStats>().ConfigureStage(stage);
        }
    }
}
#endif
