#if UNITY_EDITOR
using System.IO;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Act3(S9~S11) 불꽃축제 맵 — 지면 + 플랫폼만 재생성 (구조물/수집/포토 제외).
    /// Live: unity command act3-terrain
    /// </summary>
    public static class Act3FireworksMapRebuildCli
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
        private const string BridgeDeckSprite =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act3_Fireworks/Act3_Tile_BridgeDeck.png";
        private const string NightPathSprite =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act3_Fireworks/Act3_Tile_NightPath.png";
        private const int GroundLayer = 10;

        [CliCommand("act3-terrain", "Rebuild Act3 S9-S11 ground + platforms only")]
        public static int RebuildFromPipeline() => RebuildAll() ? 0 : 1;

        [MenuItem("FollowMe/KDS/Rebuild Fireworks Stages Terrain (S9-S11 Ground+Platforms)")]
        public static void RebuildFromMenu() => RebuildAll();

        public static bool RebuildAll()
        {
            var rule = AssetDatabase.LoadAssetAtPath<TileBase>(RuleTilePath);
            var left = AssetDatabase.LoadAssetAtPath<TileBase>(PlatLeftPath);
            var center = AssetDatabase.LoadAssetAtPath<TileBase>(PlatCenterPath);
            var right = AssetDatabase.LoadAssetAtPath<TileBase>(PlatRightPath);
            if (rule == null)
            {
                Debug.LogError("[Act3FireworksMapRebuildCli] missing ground RuleTile");
                return false;
            }

            Sprite platSprite = LoadSprite(BridgeDeckSprite) ?? LoadSprite(NightPathSprite);
            if (platSprite == null && center is Tile cTile)
                platSprite = cTile.sprite;
            if (platSprite == null && left is Tile lTile)
                platSprite = lTile.sprite;

            var sb = new System.Text.StringBuilder();
            for (int stage = 9; stage <= 11; stage++)
            {
                if (!RebuildStage(stage, rule, platSprite, sb))
                    return false;
            }

            Debug.Log("[Act3FireworksMapRebuildCli]\n" + sb);
            return true;
        }

        private static bool RebuildStage(int stage, TileBase rule, Sprite platSprite, System.Text.StringBuilder results)
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

            var ground = EnsureTilemap(grid.transform, "Tilemap_Ground", 0);
            var platMap = grid.transform.Find("Tilemap_Platform");
            if (platMap != null)
            {
                var tm = platMap.GetComponent<Tilemap>();
                if (tm != null) tm.ClearAllTiles();
            }

            RebuildGround(ground, rule, spec);
            // S11 후반 공허: 지면 일부 구간을 끊고 플랫폼 루트만 남김
            if (stage == 11)
                CarveVoidGaps(ground, spec);

            int plats = RebuildPlatforms(level.transform, platSprite, spec);
            PlaceInvisibleGoal(level.transform, spec);
            PlacePlayerStart();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            results.AppendLine($"S{stage}: LengthX={spec.LengthX:0} platforms={plats} goalX={spec.LengthX - 4f:0}");
            return true;
        }

        private static void DiscardDecorAndLegacy(Transform level, int stage)
        {
            string[] wipe =
            {
                "Platforms", "Collectibles", "ThemeProps", "Goals", "Forks",
                "Checkpoints", "Hazards", "PhotoPoints", "Background", "Zones",
                "Monsters", "Triggers"
            };
            foreach (var name in wipe)
            {
                // 직계 + 깊은 자식 모두 제거 (레거시 Zones 잔존 방지)
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

            // Level 아래 남은 체크포인트/존 마커 정리
            if (level != null)
            {
                var stale = new System.Collections.Generic.List<GameObject>();
                CollectStale(level, stale);
                foreach (var go in stale)
                {
                    if (go != null)
                        Object.DestroyImmediate(go);
                }
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

        private static void CollectStale(Transform root, System.Collections.Generic.List<GameObject> into)
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

        private static Tilemap EnsureTilemap(Transform grid, string name, int sortingOrder)
        {
            // S9~S11은 예전 공원 맵이라 Grid/타일맵이 없거나 깨져 있을 수 있음 → 통째로 재생성
            var existing = grid.Find(name);
            if (existing != null)
                Object.DestroyImmediate(existing.gameObject);

            var go = new GameObject(name);
            go.transform.SetParent(grid, false);
            go.layer = GroundLayer;
            go.AddComponent<Tilemap>();
            var renderer = go.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = sortingOrder;

            // Composite/Rigidbody 없이 TilemapCollider만 (Act3 지형 생성용)
            go.AddComponent<TilemapCollider2D>();

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
                for (int y = -5; y <= 0; y++)
                    ground.SetTile(new Vector3Int(x, y, 0), rule);
            }
        }

        /// <summary>S11 공허: Setpiece 중후반에 지면 구멍 — 플랫폼으로만 전진.</summary>
        private static void CarveVoidGaps(Tilemap ground, StageMapSpec spec)
        {
            int[][] gaps =
            {
                new[] { Mathf.RoundToInt(spec.BreathEnd + 8f), Mathf.RoundToInt(spec.BreathEnd + 18f) },
                new[] { Mathf.RoundToInt(spec.SetpieceEnd - 40f), Mathf.RoundToInt(spec.SetpieceEnd - 28f) },
                new[] { Mathf.RoundToInt(spec.SetpieceEnd - 18f), Mathf.RoundToInt(spec.SetpieceEnd - 8f) },
            };

            foreach (var g in gaps)
            {
                for (int x = g[0]; x <= g[1]; x++)
                {
                    for (int y = -5; y <= 0; y++)
                        ground.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }

        private static int RebuildPlatforms(Transform level, Sprite platSprite, StageMapSpec spec)
        {
            var old = level.Find("Platforms");
            if (old != null)
                Object.DestroyImmediate(old.gameObject);

            var root = new GameObject("Platforms");
            root.transform.SetParent(level, false);

            int count = 0;
            var rng = new System.Random(3000 + spec.Stage * 41);

            // 1) 러닝 리듬 플랫폼 — Teach~Setpiece
            float x = spec.IntroEnd + 6f;
            int step = 0;
            while (x < spec.SetpieceEnd - 10f)
            {
                float w = 3.5f + (step % 4) * 0.7f;
                // 불꽃 테마: 높낮이 리듬이 더 큼
                float[] heights = { 4.2f, 5.2f, 6.2f, 7.4f, 5.8f, 4.8f, 8.0f };
                float y = heights[step % heights.Length];
                if (x > spec.PressureEnd)
                    y += 0.6f + (step % 3) * 0.35f;

                AddPlatform(root.transform, platSprite, x + w * 0.5f, y, w, 0.42f, $"RunPlat_{count:00}");
                count++;
                step++;
                // Act3: Act2보다 약간 촘촘
                x += 8.5f + (step % 3);
            }

            // 2) 갈림 상단 루트
            var forks = StageCollectibleLayout.GetForkXs(spec);
            for (int i = 0; i < forks.Length; i++)
            {
                float fx = forks[i];
                AddPlatform(root.transform, platSprite, fx, 5.6f, 14f, 0.48f, $"ForkPlatA_{i + 1}");
                AddPlatform(root.transform, platSprite, fx + 7f, 7.0f, 8f, 0.42f, $"ForkPlatB_{i + 1}");
                AddPlatform(root.transform, platSprite, fx - 4f, 4.4f, 5f, 0.4f, $"ForkPlatC_{i + 1}");
                count += 3;
            }

            // 3) 점프 체인 — Pressure~Breath (타이밍 연습용 짧은 발판)
            float chainX = spec.TeachEnd + 4f;
            int chain = 0;
            int chainMax = 10 + (spec.Stage - 9) * 4;
            while (chainX < spec.BreathEnd - 2f && chain < chainMax)
            {
                float y = 4.2f + (chain % 4) * 1.0f;
                AddPlatform(root.transform, platSprite, chainX, y, 2.8f, 0.38f, $"ChainPlat_{chain:00}");
                count++;
                chain++;
                chainX += 6.8f;
            }

            // 4) 세트피스 직선 위 하이 플랫폼 (불꽃 윈도우용 자리)
            float setX = spec.BreathEnd + 10f;
            int setN = 0;
            while (setX < spec.SetpieceEnd - 12f)
            {
                AddPlatform(root.transform, platSprite, setX, 7.2f, 5.5f, 0.4f, $"SetHigh_{setN:00}");
                AddPlatform(root.transform, platSprite, setX + 8f, 5.6f, 4f, 0.4f, $"SetMid_{setN:00}");
                count += 2;
                setN++;
                setX += 16f;
            }

            // 5) Goal 직전
            AddPlatform(root.transform, platSprite, spec.LengthX - 22f, 4.4f, 7f, 0.42f, "GoalPlat_A");
            AddPlatform(root.transform, platSprite, spec.LengthX - 14f, 5.8f, 6f, 0.42f, "GoalPlat_B");
            AddPlatform(root.transform, platSprite, spec.LengthX - 8f, 4.2f, 4f, 0.4f, "GoalPlat_C");
            count += 3;

            // 6) S11 공허 구멍 위 징검다리
            if (spec.Stage == 11)
            {
                float[] voidXs =
                {
                    spec.BreathEnd + 10f, spec.BreathEnd + 14f, spec.BreathEnd + 18f,
                    spec.SetpieceEnd - 36f, spec.SetpieceEnd - 32f, spec.SetpieceEnd - 28f,
                    spec.SetpieceEnd - 14f, spec.SetpieceEnd - 10f
                };
                for (int i = 0; i < voidXs.Length; i++)
                {
                    float y = 4.0f + (i % 3) * 1.1f;
                    AddPlatform(root.transform, platSprite, voidXs[i], y, 3.0f, 0.4f, $"VoidPlat_{i:00}");
                    count++;
                }
            }

            // 7) 스테이지별 추가 밀도
            int extras = 10 + (spec.Stage - 9) * 5;
            for (int i = 0; i < extras; i++)
            {
                float px = rng.Next(Mathf.RoundToInt(spec.IntroEnd + 10f), Mathf.RoundToInt(spec.LengthX - 24f));
                float py = 3.8f + rng.Next(0, 6) * 0.85f;
                float pw = 2.8f + rng.Next(0, 5) * 0.6f;
                AddPlatform(root.transform, platSprite, px, py, pw, 0.38f, $"ExtraPlat_{i:00}");
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
            sr.color = new Color(0.85f, 0.78f, 0.95f, 1f); // 밤 축제 톤
            sr.sortingOrder = 5;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(width, height);
        }

        private static void PlaceInvisibleGoal(Transform level, StageMapSpec spec)
        {
            // 구조물 비주얼 없이 엔드 트리거만 (길이 확인용)
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
            var ensure = so.FindProperty("_ensureVisual");
            if (ensure != null)
                ensure.boolValue = false; // 구조물 비주얼 끔
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void PlacePlayerStart()
        {
            var player = GameObject.Find("Player");
            if (player != null)
                player.transform.position = new Vector3(-2f, 1f, 0f);
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
