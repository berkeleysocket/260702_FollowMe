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
    /// Act3(S9~S11) 불꽃축제 맵 — 지면 + 플랫폼 + 골 + 시스템 (구조물/수집/포토는 act3-decorate).
    /// 기획(Stage_All_LevelDesign.md Act3) 모듈:
    ///   Intro 강변 평지 → Teach 불꽃 발사대(윈도우) → Pressure 경고·추격(S11 공허 갭)
    ///   → Breath 회복·갈림1 → Setpiece 다리 데크 직선 → Goal 갈림2·클리어
    /// Live: unity command act3-terrain
    /// </summary>
    public static class Act3FireworksMapRebuildCli
    {
        private const string SceneDir = "Assets/00.Work/KDS/01.Scene";
        private const string RuleTilePath =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act1_Tiles/RuleTiles/Act1_Brick_Ground_RuleTile.asset";
        private const string PlatCenterPath =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act1_Tiles/Tiles/Act1_Brick_Platform_Center.asset";
        private const string BridgeDeckSprite =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act3_Fireworks/Act3_Tile_BridgeDeck.png";
        private const string NightPathSprite =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act3_Fireworks/Act3_Tile_NightPath.png";
        private const string StressBarPrefab = "Assets/00.Work/KDS/06.Prefab/StressBarCanvas.prefab";
        private const int GroundLayer = 10;

        private static readonly Color NightGroundTint = new Color(0.62f, 0.66f, 0.86f, 1f);
        private static readonly Color DarkGroundTint = new Color(0.42f, 0.44f, 0.58f, 1f);
        private static readonly Color FestivalPlat = new Color(0.96f, 0.92f, 1f, 1f);
        private static readonly Color DarkPlat = new Color(0.62f, 0.62f, 0.78f, 1f);

        [CliCommand("act3-terrain", "Rebuild Act3 S9-S11 ground + platforms + goal (concept: launch pads / void / bridge deck)")]
        public static int RebuildFromPipeline() => RebuildAll() ? 0 : 1;

        [MenuItem("FollowMe/KDS/Rebuild Fireworks Stages Terrain (S9-S11 Ground+Platforms)")]
        public static void RebuildFromMenu() => RebuildAll();

        public static bool RebuildAll()
        {
            var rule = AssetDatabase.LoadAssetAtPath<TileBase>(RuleTilePath);
            if (rule == null)
            {
                Debug.LogError("[Act3FireworksMapRebuildCli] missing ground RuleTile");
                return false;
            }

            // 데크 = Act3 BridgeDeck(다리 상판) / 일반 발판 = Act1 브릭 플랫폼(Act2와 동일 결) → 없으면 NightPath
            Sprite deckSprite = LoadSprite(BridgeDeckSprite);
            Sprite pathSprite = null;
            var center = AssetDatabase.LoadAssetAtPath<TileBase>(PlatCenterPath);
            if (center is Tile cTile && cTile.sprite != null) pathSprite = cTile.sprite;
            if (pathSprite == null) pathSprite = LoadSprite(NightPathSprite);
            if (deckSprite == null) deckSprite = pathSprite;
            if (pathSprite == null) pathSprite = deckSprite;

            var sb = new System.Text.StringBuilder();
            for (int stage = 9; stage <= 11; stage++)
            {
                if (!RebuildStage(stage, rule, deckSprite, pathSprite, sb))
                    return false;
            }

            Debug.Log("[Act3FireworksMapRebuildCli]\n" + sb);
            return true;
        }

        private static bool RebuildStage(
            int stage, TileBase rule, Sprite deckSprite, Sprite pathSprite, System.Text.StringBuilder results)
        {
            string scenePath = $"{SceneDir}/Stage{stage} Scene.unity";
            if (!File.Exists(Path.GetFullPath(scenePath)))
            {
                Debug.LogError("[Act3FireworksMapRebuildCli] missing " + scenePath);
                return false;
            }

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var spec = StageMapDatabase.Get(stage);
            string levelName = $"Level_S{stage}";

            var level = GameObject.Find(levelName);
            if (level == null)
            {
                for (int s = 1; s <= 16; s++)
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
                level = new GameObject(levelName);

            var grid = GameObject.Find("Grid");
            if (grid == null)
                grid = new GameObject("Grid", typeof(Grid));

            DiscardDecorAndLegacy(level.transform, stage);
            EnsureSystems(stage);

            // 레거시 공원 타일맵(Grid/Tilemap 등) 제거 — 갭 아래로 옛 지면이 비치는 것 방지
            for (int i = grid.transform.childCount - 1; i >= 0; i--)
            {
                var c = grid.transform.GetChild(i);
                if (c.name != "Tilemap_Ground")
                    Object.DestroyImmediate(c.gameObject);
            }

            var ground = EnsureTilemap(grid.transform, "Tilemap_Ground", 0);
            ground.color = stage == 11 ? DarkGroundTint : NightGroundTint;

            RebuildGround(ground, rule, spec);
            int plats = RebuildPlatforms(level.transform, deckSprite, pathSprite, spec);
            float groundTop = ground.layoutGrid.CellToWorld(new Vector3Int(0, 1, 0)).y; // 최상단 셀(y=0) 윗면
            RebuildGoal(level.transform, spec, groundTop);
            PlacePlayerStart();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            results.AppendLine(
                $"S{stage}: LengthX={spec.LengthX:0} platforms={plats} gaps={Act3FireworksLayout.GetPressureGaps(spec).Count} goalX={spec.LengthX - 4f:0}");
            return true;
        }

        private static void DiscardDecorAndLegacy(Transform level, int stage)
        {
            string[] wipe =
            {
                "Platforms", "Collectibles", "ThemeProps", "Goals", "Forks",
                "Checkpoints", "Hazards", "PhotoPoints", "Background", "Zones",
                "Monsters", "Triggers", "FireworkWindows"
            };
            foreach (var name in wipe)
            {
                Transform child;
                while ((child = FindDeep(level, name)) != null)
                    Object.DestroyImmediate(child.gameObject);
            }

            foreach (var go in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (go == null) continue;
                if (go.name.StartsWith("Photo_Act") ||
                    go.name == "Zone_Goal" ||
                    go.name == "StageGoal" ||
                    go.name.StartsWith("CafePlatform_") ||
                    go.name.StartsWith("ForkPlatform_") ||
                    go.name.StartsWith("RunPlat_") ||
                    go.name.StartsWith("Spike_") ||
                    go.name.StartsWith("Checkpoint_"))
                {
                    Object.DestroyImmediate(go);
                }
            }

            var stale = new List<GameObject>();
            CollectStale(level, stale);
            foreach (var go in stale)
            {
                if (go != null)
                    Object.DestroyImmediate(go);
            }

            var ground = GameObject.Find("Grid/Tilemap_Ground");
            if (ground != null)
            {
                var tm = ground.GetComponent<Tilemap>();
                if (tm != null) tm.ClearAllTiles();
            }

            foreach (var go in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (go != null && go.name.StartsWith("Level_S") && go.name != $"Level_S{stage}")
                    Object.DestroyImmediate(go);
            }
        }

        private static void CollectStale(Transform root, List<GameObject> into)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                var c = root.GetChild(i);
                string n = c.name;
                if (n.StartsWith("Checkpoint_") ||
                    n.StartsWith("Zone_") ||
                    n == "Monsters" ||
                    n == "Zones")
                {
                    into.Add(c.gameObject);
                    continue;
                }
                CollectStale(c, into);
            }
        }

        private static Transform FindDeep(Transform root, string name)
        {
            if (root == null) return null;
            if (root.name == name) return root;
            for (int i = 0; i < root.childCount; i++)
            {
                var found = FindDeep(root.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }

        /// <summary>Act2와 동일 구성: Static RB + TilemapCollider + Composite(Outlines).</summary>
        private static Tilemap EnsureTilemap(Transform grid, string name, int sortingOrder)
        {
            var existing = grid.Find(name);
            if (existing != null)
                Object.DestroyImmediate(existing.gameObject);

            var go = new GameObject(name);
            go.transform.SetParent(grid, false);
            go.layer = GroundLayer;
            go.AddComponent<Tilemap>();
            var renderer = go.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = sortingOrder;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            var tmc = go.AddComponent<TilemapCollider2D>();
            var cc = go.AddComponent<CompositeCollider2D>();
            cc.geometryType = CompositeCollider2D.GeometryType.Outlines;
            tmc.compositeOperation = Collider2D.CompositeOperation.Merge;

            var map = go.GetComponent<Tilemap>();
            map.ClearAllTiles();
            return map;
        }

        private static void RebuildGround(Tilemap ground, TileBase rule, StageMapSpec spec)
        {
            int x0 = -8;
            int x1 = Mathf.CeilToInt(spec.LengthX) + 12;
            for (int x = x0; x <= x1; x++)
            {
                if (!Act3FireworksLayout.HasGround(spec, x))
                    continue;
                for (int y = -5; y <= 0; y++)
                    ground.SetTile(new Vector3Int(x, y, 0), rule);
            }
        }

        private static int RebuildPlatforms(Transform level, Sprite deckSprite, Sprite pathSprite, StageMapSpec spec)
        {
            var root = new GameObject("Platforms");
            root.transform.SetParent(level, false);
            int count = 0;

            // 1) Teach — 불꽃 발사대: 낮은 발판 → 높은 발판 → 윈도우(장식 단계에서 배치)
            var teachXs = Act3FireworksLayout.GetTeachWindowXs(spec);
            for (int i = 0; i < teachXs.Length; i++)
            {
                float wx = teachXs[i];
                AddPlatform(root.transform, pathSprite, wx - 4f, Act3FireworksLayout.LaunchLowY, 3f, 0.42f,
                    FestivalPlat, $"Launch_Low_{i:00}");
                AddPlatform(root.transform, deckSprite, wx, Act3FireworksLayout.LaunchHighY, 4.5f, 1.0f,
                    FestivalPlat, $"Launch_High_{i:00}");
                count += 2;
            }

            // 2) Pressure — 잿불 위 점프 발판 + (S10/S11) 갭 징검다리
            bool dark = spec.Stage == 11;
            foreach (var hx in Act3FireworksLayout.GetPressureHopXs(spec))
            {
                AddPlatform(root.transform, pathSprite, hx, 3.6f, 3.2f, 0.4f,
                    dark ? DarkPlat : FestivalPlat, $"Hop_{count:00}");
                count++;
            }

            int stoneN = 0;
            foreach (var s in Act3FireworksLayout.GetGapStones(spec))
            {
                AddPlatform(root.transform, pathSprite, s.x, s.y, s.z, 0.4f,
                    dark ? DarkPlat : FestivalPlat, $"Stone_{stoneN:00}");
                stoneN++;
                count++;
            }

            // 3) 갈림 상단 루트 (Breath = Fork1, Goal = Fork2)
            var forks = Act3FireworksLayout.GetForkXs(spec);
            for (int i = 0; i < forks.Length; i++)
            {
                float fx = forks[i];
                var c = Act3FireworksLayout.IsDark(spec, fx) ? DarkPlat : FestivalPlat;
                AddPlatform(root.transform, pathSprite, fx - 5f, 3.6f, 4f, 0.4f, c, $"ForkPlatC_{i + 1}");
                AddPlatform(root.transform, deckSprite, fx + 1f, 5.6f, 12f, 1.0f, c, $"ForkPlatA_{i + 1}");
                AddPlatform(root.transform, deckSprite, fx + 9f, 7.0f, 7f, 1.0f, c, $"ForkPlatB_{i + 1}");
                count += 3;
            }

            // 4) Setpiece — 한강 다리 데크 직선 (S11은 짧은 세그먼트·긴 간격 = 끊긴 강변)
            Act3FireworksLayout.GetDeckRhythm(spec, out float seg, out float gap);
            float deckStart = Act3FireworksLayout.DeckStart(spec);
            float deckEnd = Act3FireworksLayout.DeckEnd(spec);
            AddPlatform(root.transform, pathSprite, deckStart - 5f, 3.6f, 4f, 0.4f,
                dark ? DarkPlat : FestivalPlat, "Deck_Ramp");
            count++;
            float dx = deckStart;
            int di = 0;
            while (dx + seg <= deckEnd + 0.01f)
            {
                AddPlatform(root.transform, deckSprite, dx + seg * 0.5f, Act3FireworksLayout.DeckY, seg, 1.0f,
                    dark ? DarkPlat : FestivalPlat, $"Deck_{di:00}");
                count++;
                di++;
                dx += seg + gap;
            }

            // 5) Goal 직전 마무리 발판
            var gc = dark ? DarkPlat : FestivalPlat;
            AddPlatform(root.transform, pathSprite, spec.LengthX - 11f, 3.6f, 4f, 0.42f, gc, "GoalPlat_A");
            count++;

            return count;
        }

        private static void AddPlatform(
            Transform parent, Sprite sprite, float x, float y, float width, float height, Color color, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(x, y, 0f);
            go.layer = GroundLayer;

            // 비주얼은 자식으로 — 스프라이트 피벗(BottomCenter 등)과 무관하게 콜라이더 박스에 정확히 맞춘다
            var visual = new GameObject("Visual");
            visual.transform.SetParent(go.transform, false);
            float pivotNormY = sprite != null && sprite.rect.height > 0f ? sprite.pivot.y / sprite.rect.height : 0.5f;
            visual.transform.localPosition = new Vector3(0f, -height * 0.5f + pivotNormY * height, 0f);

            var sr = visual.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.tileMode = SpriteTileMode.Continuous;
            sr.size = new Vector2(width, height);
            sr.color = color;
            sr.sortingOrder = 5;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(width, height);
        }

        private static void RebuildGoal(Transform level, StageMapSpec spec, float groundTop)
        {
            var root = new GameObject("Goals");
            root.transform.SetParent(level, false);
            var go = new GameObject("Zone_Goal");
            go.transform.SetParent(root.transform, false);
            // StageGoal 엔드 폴(높이 3.2, 중심 기준) 하단이 지면 윗면 근처에 오도록
            go.transform.position = new Vector3(spec.LengthX - 4f, groundTop + 1.5f, 0f);
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

        private static void PlacePlayerStart()
        {
            var player = GameObject.Find("Player");
            if (player != null)
                player.transform.position = new Vector3(-2f, 1f, 0f);
        }

        /// <summary>Act2와 동일 시스템 + MapModeService(경고→추격→회복 존용).</summary>
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
            if (levelSystems.GetComponent<MapModeService>() == null)
                levelSystems.AddComponent<MapModeService>();

            levelSystems.GetComponent<StageRunStats>().ConfigureStage(stage);
        }

        private static Sprite LoadSprite(string path)
        {
            var sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sp != null) return sp;
            foreach (var a in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (a is Sprite s)
                    return s;
            }

            return null;
        }
    }
}
#endif

