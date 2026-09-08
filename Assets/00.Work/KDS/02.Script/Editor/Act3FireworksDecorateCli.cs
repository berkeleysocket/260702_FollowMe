#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Act3(S9~S11) 구조물·수집·포토·장애물·모드 존 배치 (지면/플랫폼은 act3-terrain).
    /// 기획(Stage_All_LevelDesign.md Act3):
    ///   - Teach: 불꽃 윈도우 위 "버스트" 좋아요 — 불꽃이 터지는 동안만 켜짐
    ///   - Pressure: 경고→추격→회복 존 + 추격 중에만 활성되는 잿불/인파 장애물
    ///   - S11: TeachEnd 이후 공허 — 랜턴·군중 없음, 빈 벤치·강변만
    ///   - 포토: 기획서 ID를 스테이지별로 명시 배치
    /// Live: unity command act3-decorate
    /// </summary>
    public static class Act3FireworksDecorateCli
    {
        private const string SceneDir = "Assets/00.Work/KDS/01.Scene";
        private const string PropPrefabDir = "Assets/00.Work/KDS/06.Prefab/Act3_Fireworks";
        private const string PhotoPrefabDir = "Assets/00.Work/KDS/06.Prefab/Act3_PhotoSpot";
        private const string HazardPrefabDir = "Assets/00.Work/KDS/06.Prefab/Hazards";
        private const string ImogeDir = "Assets/00.Work/KDS/06.Prefab/Like_Imoge";
        private const string HeartPrefab = ImogeDir + "/Follow_13.prefab";

        // 지면 위 오프셋 (groundTop 기준)
        private const float LowLikeAbove = 0.7f;
        private const float FollowAbove = 0.6f;
        private const float LanternOverheadY = 8.8f;

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

        // 축제 구간 지면 소품 (NPC는 폰만 보는 인파)
        private static readonly string[] FestivalGroundCycle =
        {
            "Act3_Prop_CrowdSilhouette", "Act3_Prop_LanternPole", "Act3_Prop_FoodStall",
            "Act3_Prop_CrowdSilhouette", "Act3_Prop_FestivalFlag", "Act3_Prop_VendorCart",
            "Act3_Prop_SpeakerStack", "Act3_Prop_ParkBench", "Act3_Prop_PhotoFrame",
        };

        // 추격(Pressure) 구간 — 어수선·희소
        private static readonly string[] PressureCycle =
        {
            "Act3_Prop_BarrierCone", "Act3_Prop_TrashBin", "Act3_Prop_TreeAutumn", "Act3_Prop_EmptyBench",
        };

        // S11 공허 — 축제가 끝난 강변
        private static readonly string[] DarkCycle =
        {
            "Act3_Prop_EmptyBench", "Act3_Prop_RiverEdge", "Act3_Prop_NightBush", "Act3_Prop_TrashBin",
        };

        private static readonly Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

        [CliCommand("act3-decorate", "Place Act3 S9-S11 props, firework bursts, collectibles, photos, hazards, mode zones")]
        public static int DecorateFromPipeline() => DecorateAll() ? 0 : 1;

        [MenuItem("FollowMe/KDS/Decorate Fireworks Stages (S9-S11 Props+Collectibles)")]
        public static void DecorateFromMenu() => DecorateAll();

        [CliCommand("act3-photos-refresh", "Rebuild only PhotoPoints on S9-S11 (keep props/collectibles)")]
        public static int RefreshPhotosFromPipeline() => RefreshPhotosOnly() ? 0 : 1;

        public static bool RefreshPhotosOnly()
        {
            PrefabCache.Clear();
            var sb = new System.Text.StringBuilder();
            for (int stage = 9; stage <= 11; stage++)
            {
                if (!OpenStage(stage, out var scene, out var spec, out var level))
                    return false;

                Transform photos;
                while ((photos = FindDeep(level.transform, "PhotoPoints")) != null)
                    Object.DestroyImmediate(photos.gameObject);

                foreach (var go in scene.GetRootGameObjects())
                {
                    if (go != null && go.name.StartsWith("Photo_Act3_"))
                        Object.DestroyImmediate(go);
                }

                float groundTop = Act3FireworksLayout.GetGroundTop();
                int n = RebuildPhotoPoints(level.transform, spec, groundTop);
                var photoRoot = FindDeep(level.transform, "PhotoPoints");
                if (photoRoot != null)
                    for (int i = 0; i < photoRoot.childCount; i++)
                        Act3FireworksLayout.SnapBottomToGround(photoRoot.GetChild(i), groundTop);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                sb.AppendLine($"S{stage}: photos={n}");
            }

            Debug.Log("[Act3FireworksDecorateCli] photos refresh\n" + sb);
            return true;
        }

        public static bool DecorateAll()
        {
            PrefabCache.Clear();
            var heart = AssetDatabase.LoadAssetAtPath<GameObject>(HeartPrefab);
            if (heart == null)
            {
                Debug.LogError("[Act3FireworksDecorateCli] missing " + HeartPrefab);
                return false;
            }

            var emojis = new List<GameObject>();
            foreach (var path in EmojiPrefabs)
            {
                var p = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (p != null) emojis.Add(p);
            }

            if (emojis.Count == 0)
            {
                Debug.LogError("[Act3FireworksDecorateCli] no Like_Imoge emoji prefabs");
                return false;
            }

            var sb = new System.Text.StringBuilder();
            for (int stage = 9; stage <= 11; stage++)
            {
                if (!DecorateStage(stage, heart, emojis.ToArray(), sb))
                    return false;
            }

            Debug.Log("[Act3FireworksDecorateCli]\n" + sb);
            return true;
        }

        private static bool OpenStage(
            int stage, out UnityEngine.SceneManagement.Scene scene, out StageMapSpec spec, out GameObject level)
        {
            scene = default;
            spec = default;
            level = null;
            string scenePath = $"{SceneDir}/Stage{stage} Scene.unity";
            if (!File.Exists(Path.GetFullPath(scenePath)))
            {
                Debug.LogError("[Act3FireworksDecorateCli] missing " + scenePath);
                return false;
            }

            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            spec = StageMapDatabase.Get(stage);
            level = GameObject.Find($"Level_S{stage}");
            if (level == null)
            {
                Debug.LogError($"[Act3FireworksDecorateCli] Level_S{stage} not found — run act3-terrain first");
                return false;
            }

            return true;
        }

        private static bool DecorateStage(
            int stage, GameObject heart, GameObject[] emojis, System.Text.StringBuilder results)
        {
            if (!OpenStage(stage, out var scene, out var spec, out var level))
                return false;

            WipeDecor(level.transform, scene);

            float groundTop = Act3FireworksLayout.GetGroundTop();

            var photoXs = new List<float>();
            foreach (var p in Act3FireworksLayout.GetPhotos(spec))
                photoXs.Add(p.X);

            int props = RebuildThemeProps(level.transform, spec, photoXs, groundTop);
            var likeCounter = new Counter();
            int windows = RebuildFireworkWindows(level.transform, spec, emojis, likeCounter);
            int collects = RebuildCollectibles(level.transform, spec, heart, emojis, likeCounter, groundTop);
            int hazards = RebuildHazards(level.transform, spec, groundTop);
            int zones = RebuildModeZones(level.transform, spec);
            int photos = RebuildPhotoPoints(level.transform, spec, groundTop);
            RebuildForks(level.transform, spec);
            RebuildCheckpoints(level.transform, spec);
            int snapped = SnapGroundedStructures(level.transform, groundTop);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            results.AppendLine(
                $"S{stage}: groundTop={groundTop:0.00} props={props} windows={windows} likes={likeCounter.Likes} follows={likeCounter.Follows} " +
                $"collectibles={collects} hazards={hazards} zones={zones} photos={photos} snapped={snapped}");
            return true;
        }

        /// <summary>
        /// 사이드뷰 정합: 바닥에 붙는 구조물(지면 소품·포토 피사체·지면 장애물)의 스프라이트 하단을
        /// 지면 콜라이더 윗면에 맞춘다. 공중 소품(랜턴 줄 등)·플랫폼 위 오브젝트는 건드리지 않는다.
        /// </summary>
        private static int SnapGroundedStructures(Transform level, float groundTop)
        {
            int n = 0;
            float airborne = groundTop + 1.5f;

            var props = FindDeep(level, "ThemeProps");
            if (props != null)
            {
                for (int i = 0; i < props.childCount; i++)
                {
                    var c = props.GetChild(i);
                    if (c.position.y > airborne) continue; // 머리 위 랜턴 줄
                    if (Act3FireworksLayout.SnapBottomToGround(c, groundTop)) n++;
                }
            }

            var photos = FindDeep(level, "PhotoPoints");
            if (photos != null)
            {
                for (int i = 0; i < photos.childCount; i++)
                {
                    if (Act3FireworksLayout.SnapBottomToGround(photos.GetChild(i), groundTop)) n++;
                }
            }

            var hazards = FindDeep(level, "Hazards");
            if (hazards != null)
            {
                foreach (var e in hazards.GetComponentsInChildren<EmberHazard>(true))
                    if (Act3FireworksLayout.SnapBottomToGround(e.transform, groundTop)) n++;
                foreach (var c in hazards.GetComponentsInChildren<CrowdBumpHazard>(true))
                    if (Act3FireworksLayout.SnapBottomToGround(c.transform, groundTop)) n++;
            }

            return n;
        }

        private sealed class Counter
        {
            public int Likes;
            public int Follows;
        }

        private static void WipeDecor(Transform level, UnityEngine.SceneManagement.Scene scene)
        {
            string[] wipe =
            {
                "Background", "ThemeProps", "Collectibles", "PhotoPoints", "Hazards",
                "Forks", "Checkpoints", "Triggers", "Zones", "Monsters", "FireworkWindows"
            };
            foreach (var name in wipe)
            {
                Transform child;
                while ((child = FindDeep(level, name)) != null)
                    Object.DestroyImmediate(child.gameObject);
            }

            foreach (var go in scene.GetRootGameObjects())
            {
                if (go == null) continue;
                if (go.name.StartsWith("Photo_Act3_") ||
                    go.name.StartsWith("Checkpoint_") ||
                    go.name.StartsWith("Ember_") ||
                    go.name.StartsWith("Crowd_") ||
                    go.name.StartsWith("FireworkWin_"))
                {
                    Object.DestroyImmediate(go);
                }
            }
        }

        // ───────────────────────── Props ─────────────────────────

        private static int RebuildThemeProps(Transform level, StageMapSpec spec, List<float> photoXs, float groundTop)
        {
            var root = new GameObject("ThemeProps");
            root.transform.SetParent(level, false);
            int count = 0;

            // 0) 한강 다리 실루엣 — 전체 구간 리듬 (공허 구간에도 남는 유일한 구조물)
            for (float x = 14f; x < spec.LengthX - 6f; x += 30f)
                count += PlaceProp(root.transform, "Act3_Prop_BridgePillar", x, groundTop, photoXs);

            // 0-1) 강변 난간 — 축제 구간만
            for (float x = 4f; x < spec.LengthX - 6f; x += 12f)
            {
                if (Act3FireworksLayout.IsDark(spec, x)) continue;
                count += PlaceProp(root.transform, "Act3_Prop_RiverRail", x, groundTop, photoXs);
            }

            // 1) Intro — 축제 한복판 (밀도 높음)
            count += PlaceCycle(root.transform, FestivalGroundCycle, 5f, spec.IntroEnd - 2f, 6.5f, spec, photoXs, 0, groundTop);
            for (float x = 8f; x < spec.IntroEnd; x += 9f)
                count += PlaceProp(root.transform, "Act3_Prop_StringLanterns", x, groundTop + 3.6f, photoXs);

            // 2) Teach — 발사대 사이 인파(폰만 보는 NPC) + 노점
            var teachXs = Act3FireworksLayout.GetTeachWindowXs(spec);
            for (int i = 0; i < teachXs.Length; i++)
            {
                float wx = teachXs[i];
                count += PlaceProp(root.transform, "Act3_Prop_CrowdSilhouette", wx - 8f, groundTop, photoXs);
                count += PlaceProp(root.transform,
                    i % 2 == 0 ? "Act3_Prop_FoodStall" : "Act3_Prop_VendorCart", wx + 4.5f, groundTop, photoXs);
                count += PlaceProp(root.transform, "Act3_Prop_LanternPole", wx + 8.5f, groundTop, photoXs);
            }

            // 3) Pressure — 어수선·희소. S11은 공허 세트
            var pressureCycle = spec.Stage == 11 ? DarkCycle : PressureCycle;
            count += PlaceCycle(root.transform, pressureCycle, spec.TeachEnd + 4f, spec.PressureEnd - 3f,
                spec.Stage == 11 ? 14f : 10f, spec, photoXs, 1, groundTop);

            // 4) Breath — 회복: 다시 축제 (S11은 짧고 어두움)
            if (spec.Stage == 11)
            {
                count += PlaceCycle(root.transform, DarkCycle, spec.PressureEnd + 3f, spec.BreathEnd - 3f, 12f, spec, photoXs, 2, groundTop);
            }
            else
            {
                count += PlaceCycle(root.transform, FestivalGroundCycle, spec.PressureEnd + 3f, spec.BreathEnd - 3f, 7f, spec, photoXs, 3, groundTop);
                for (float x = spec.PressureEnd + 6f; x < spec.BreathEnd - 4f; x += 10f)
                    count += PlaceProp(root.transform, "Act3_Prop_StringLanterns", x, groundTop + 3.6f, photoXs);
            }

            // 5) Setpiece — 데크 위 직선: 머리 위 랜턴 줄 + 지면 인파 (S11: 빈 강변)
            float deckStart = Act3FireworksLayout.DeckStart(spec);
            float deckEnd = Act3FireworksLayout.DeckEnd(spec);
            if (spec.Stage == 11)
            {
                count += PlaceCycle(root.transform, DarkCycle, spec.BreathEnd + 3f, spec.SetpieceEnd - 3f, 13f, spec, photoXs, 1, groundTop);
            }
            else
            {
                for (float x = deckStart + 2f; x < deckEnd; x += 6.5f)
                    count += PlaceProp(root.transform, "Act3_Prop_StringLanterns", x, LanternOverheadY, photoXs);
                string[] setCycle =
                {
                    "Act3_Prop_CrowdSilhouette", "Act3_Prop_LanternPole", "Act3_Prop_CrowdSilhouette",
                    "Act3_Prop_SpeakerStack", "Act3_Prop_PhotoFrame",
                };
                count += PlaceCycle(root.transform, setCycle, spec.BreathEnd + 4f, spec.SetpieceEnd - 3f, 9f, spec, photoXs, 0, groundTop);
            }

            // 6) Goal — 인파 흩어짐
            string[] goalCycle = spec.Stage == 11
                ? DarkCycle
                : new[] { "Act3_Prop_ParkBench", "Act3_Prop_LanternPole", "Act3_Prop_CrowdSilhouette", "Act3_Prop_TrashBin" };
            count += PlaceCycle(root.transform, goalCycle, spec.SetpieceEnd + 2f, spec.LengthX - 8f, 9f, spec, photoXs, 0, groundTop);

            return count;
        }

        private static int PlaceCycle(
            Transform parent, string[] cycle, float x0, float x1, float step, StageMapSpec spec,
            List<float> photoXs, int offset, float groundTop)
        {
            int count = 0;
            int i = offset;
            for (float x = x0; x < x1; x += step)
            {
                if (!Act3FireworksLayout.HasGround(spec, x))
                {
                    i++;
                    continue;
                }

                count += PlaceProp(parent, cycle[i % cycle.Length], x, groundTop, photoXs);
                i++;
            }

            return count;
        }

        private static int PlaceProp(Transform parent, string prefabName, float x, float y, List<float> photoXs)
        {
            // 포토 피사체 자리 비우기
            for (int i = 0; i < photoXs.Count; i++)
            {
                if (Mathf.Abs(photoXs[i] - x) < 3.2f && y < 3.5f)
                    return 0;
            }

            var prefab = LoadPrefab($"{PropPrefabDir}/{prefabName}.prefab");
            if (prefab == null) return 0;

            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.name = $"{prefabName}_{x:000}";
            go.transform.position = new Vector3(x, y, 0f);
            return 1;
        }

        // ───────────────────── Firework windows ─────────────────────

        /// <summary>
        /// Teach 발사대 윈도우 + 버스트 좋아요(불꽃 터질 때만 켜짐) / Setpiece 데크 윈도우(보너스만).
        /// </summary>
        private static int RebuildFireworkWindows(
            Transform level, StageMapSpec spec, GameObject[] emojis, Counter counter)
        {
            var root = new GameObject("FireworkWindows");
            root.transform.SetParent(level, false);

            var windowPrefab = LoadPrefab($"{HazardPrefabDir}/FireworkLikeWindow.prefab");
            if (windowPrefab == null) return 0;

            int count = 0;

            // Teach — 버스트
            int teachLikes = spec.Stage switch { 9 => 18, 10 => 20, _ => 14 };
            var teachXs = Act3FireworksLayout.GetTeachWindowXs(spec);
            int perWindow = teachXs.Length > 0 ? teachLikes / teachXs.Length : 0;
            int remainder = teachXs.Length > 0 ? teachLikes - perWindow * teachXs.Length : 0;

            for (int i = 0; i < teachXs.Length; i++)
            {
                float wx = teachXs[i];
                float wy = Act3FireworksLayout.TeachWindowY;
                var win = Spawn(windowPrefab, root.transform, wx, wy, $"FireworkWin_Teach_{i:00}");

                var burst = new GameObject($"Burst_{i:00}");
                burst.transform.SetParent(win.transform, false);
                burst.transform.position = new Vector3(wx, wy, 0f);

                int n = perWindow + (i < remainder ? 1 : 0);
                for (int k = 0; k < n; k++)
                {
                    // 위쪽 반원 + 양옆 (발판 위에서 점프로 닿는 높이)
                    float t = n == 1 ? 0.5f : k / (float)(n - 1);
                    float ang = Mathf.Lerp(12f, 168f, t) * Mathf.Deg2Rad;
                    float r = Act3FireworksLayout.BurstRadius + (k % 2) * 0.35f;
                    float lx = wx + Mathf.Cos(ang) * r;
                    float ly = wy + Mathf.Sin(ang) * r * 0.85f;
                    var like = Spawn(emojis[counter.Likes % emojis.Length], burst.transform, lx, ly,
                        $"Like_{counter.Likes:00}");
                    like.SetActive(true);
                    counter.Likes++;
                }

                burst.SetActive(false);

                var so = new SerializedObject(win.GetComponent<FireworkLikeWindow>());
                so.FindProperty("_activeDuration").floatValue = 4.5f;
                so.FindProperty("_pulseInterval").floatValue = 6.5f;
                so.FindProperty("_cooldown").floatValue = 4f;
                so.FindProperty("_likeBonus").floatValue = 4f;
                var targets = so.FindProperty("_burstTargets");
                targets.arraySize = 1;
                targets.GetArrayElementAtIndex(0).objectReferenceValue = burst;
                so.ApplyModifiedPropertiesWithoutUndo();
                count++;
            }

            // Setpiece — 데크 위 타이밍 보너스 (S11은 2개, 약함)
            var setXs = Act3FireworksLayout.GetSetpieceWindowXs(spec);
            for (int i = 0; i < setXs.Length; i++)
            {
                var win = Spawn(windowPrefab, root.transform, setXs[i], Act3FireworksLayout.DeckWindowY,
                    $"FireworkWin_Deck_{i:00}");
                var so = new SerializedObject(win.GetComponent<FireworkLikeWindow>());
                so.FindProperty("_activeDuration").floatValue = 3f;
                so.FindProperty("_pulseInterval").floatValue = spec.Stage == 11 ? 11f : 7.5f;
                so.ApplyModifiedPropertiesWithoutUndo();
                count++;
            }

            return count;
        }

        // ───────────────────── Collectibles ─────────────────────

        private static int RebuildCollectibles(
            Transform level, StageMapSpec spec, GameObject heart, GameObject[] emojis, Counter counter, float groundTop)
        {
            var root = new GameObject("Collectibles");
            root.transform.SetParent(level, false);
            int placed = 0;

            int introLikes = 4;
            int pressureLikes = 4;
            int breathLikes = spec.Stage == 11 ? 4 : 6;
            int setLikes = spec.Stage == 9 ? 6 : 8;
            int goalLikes = spec.Stage == 11 ? 6 : 0;

            var forks = Act3FireworksLayout.GetForkXs(spec);
            float fork1 = forks[0];
            float fork2 = forks[1];

            // Intro — 저지대, 첫 도파민
            foreach (var x in Act3FireworksLayout.Spread(7f, spec.IntroEnd - 5f, introLikes))
                placed += SpawnLike(root.transform, emojis, counter, x, groundTop + LowLikeAbove);

            // Pressure — 점프 발판 위 (위험 위치), 부족하면 지면
            var hops = Act3FireworksLayout.GetPressureHopXs(spec);
            for (int i = 0; i < pressureLikes; i++)
            {
                if (i < hops.Length)
                    placed += SpawnLike(root.transform, emojis, counter, hops[i], 3.6f + 1.1f);
                else
                {
                    float x = Mathf.Lerp(spec.TeachEnd + 6f, spec.PressureEnd - 6f, (i + 0.5f) / pressureLikes);
                    x = FirstGrounded(spec, x, 1f); // 공허 갭 위면 다음 지면으로
                    placed += SpawnLike(root.transform, emojis, counter, x, groundTop + LowLikeAbove);
                }
            }

            // Breath — 절반 지면(회복), 절반 갈림1 상단
            int breathLow = breathLikes / 2;
            int breathHigh = breathLikes - breathLow;
            foreach (var x in Act3FireworksLayout.Spread(spec.PressureEnd + 4f, fork1 - 9f, breathLow))
                placed += SpawnLike(root.transform, emojis, counter, x, groundTop + LowLikeAbove);
            foreach (var x in Act3FireworksLayout.Spread(fork1 - 3f, fork1 + 6f, breathHigh))
                placed += SpawnLike(root.transform, emojis, counter, x, 5.6f + 1.1f);

            // Setpiece — 데크 위 직선 (윈도우 자리는 피함)
            float deckStart = Act3FireworksLayout.DeckStart(spec);
            float deckEnd = Act3FireworksLayout.DeckEnd(spec);
            var setWindows = Act3FireworksLayout.GetSetpieceWindowXs(spec);
            foreach (var x0 in Act3FireworksLayout.Spread(deckStart + 3f, deckEnd - 3f, setLikes))
            {
                float x = x0;
                foreach (var wxw in setWindows)
                {
                    if (Mathf.Abs(wxw - x) < 2.4f)
                        x = wxw + 2.6f;
                }

                placed += SpawnLike(root.transform, emojis, counter, x, Act3FireworksLayout.DeckY + 1.1f);
            }

            // Goal — S11만 갈림2 상단에 (마지막 유혹)
            if (goalLikes > 0)
            {
                foreach (var x in Act3FireworksLayout.Spread(fork2 - 3f, fork2 + 11f, goalLikes))
                {
                    float y = x > fork2 + 5.5f ? 7.0f + 1.1f : 5.6f + 1.1f;
                    placed += SpawnLike(root.transform, emojis, counter, x, y);
                }
            }

            // Follow(♡) 6 — 모듈당 1개, 주로 지면 = 안전 루트에도 보상
            var teachXs = Act3FireworksLayout.GetTeachWindowXs(spec);
            float teachFollowX = teachXs.Length >= 2 ? (teachXs[0] + teachXs[1]) * 0.5f + 1f : spec.TeachEnd - 4f;
            float[] followXs =
            {
                spec.IntroEnd - 6f,
                teachFollowX,
                FirstGrounded(spec, spec.PressureEnd - 5f, -1f),
                fork1 + 1f,
                FirstGrounded(spec, (deckStart + deckEnd) * 0.5f, 1f),
                spec.LengthX - 9f,
            };
            for (int i = 0; i < followXs.Length; i++)
            {
                Spawn(heart, root.transform, followXs[i], groundTop + FollowAbove, $"Follow_{counter.Follows:00}");
                counter.Follows++;
                placed++;
            }

            return placed;
        }

        private static float FirstGrounded(StageMapSpec spec, float x, float dir)
        {
            for (int i = 0; i < 40; i++)
            {
                if (Act3FireworksLayout.HasGround(spec, x))
                    return x;
                x += dir;
            }

            return x;
        }

        private static int SpawnLike(Transform parent, GameObject[] emojis, Counter counter, float x, float y)
        {
            Spawn(emojis[counter.Likes % emojis.Length], parent, x, y, $"Like_{counter.Likes:00}");
            counter.Likes++;
            return 1;
        }

        // ───────────────────── Hazards (추격 중에만 활성) ─────────────────────

        private static int RebuildHazards(Transform level, StageMapSpec spec, float groundTop)
        {
            var root = new GameObject("Hazards");
            root.transform.SetParent(level, false);

            var ember = LoadPrefab($"{HazardPrefabDir}/EmberHazard.prefab");
            var crowd = LoadPrefab($"{HazardPrefabDir}/CrowdBumpHazard.prefab");
            int count = 0;

            var ranges = Act3FireworksLayout.GetChaseRanges(spec);
            for (int c = 0; c < ranges.Count; c++)
            {
                float c0 = ranges[c].x;
                float c1 = ranges[c].y;

                var set = new GameObject($"ChaseSet_{c + 1}");
                set.transform.SetParent(root.transform, false);
                var modeObj = set.AddComponent<MapModeObject>();
                var so = new SerializedObject(modeObj);
                var modes = so.FindProperty("_activeInModes");
                modes.arraySize = 1;
                modes.GetArrayElementAtIndex(0).intValue = (int)MapMode.Chase;
                so.ApplyModifiedPropertiesWithoutUndo();

                // 잿불 — 지면 위 7u 간격 (갭 제외, 발판 아래는 피함)
                if (ember != null)
                {
                    var hops = Act3FireworksLayout.GetPressureHopXs(spec);
                    int n = 0;
                    for (float x = c0 + 2f; x < c1 - 1f; x += 7f)
                    {
                        if (!Act3FireworksLayout.HasGround(spec, x)) continue;
                        bool underHop = false;
                        foreach (var h in hops)
                        {
                            if (Mathf.Abs(h - x) < 1.2f) { underHop = true; break; }
                        }

                        float ex = underHop ? x + 2.2f : x;
                        Spawn(ember, set.transform, ex, groundTop, $"Ember_{c + 1}_{n:00}");
                        count++;
                        n++;
                    }
                }

                // 인파 밀림 — 구간당 1~2
                if (crowd != null)
                {
                    float[] ts = spec.Stage == 9 ? new[] { 0.6f } : new[] { 0.35f, 0.8f };
                    for (int i = 0; i < ts.Length; i++)
                    {
                        float x = FirstGrounded(spec, Mathf.Lerp(c0, c1, ts[i]), 1f);
                        Spawn(crowd, set.transform, x, groundTop, $"Crowd_{c + 1}_{i}");
                        count++;
                    }
                }
            }

            return count;
        }

        // ───────────────────── Mode zones ─────────────────────

        private static int RebuildModeZones(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("Zones");
            root.transform.SetParent(level, false);

            int count = 0;
            var zones = Act3FireworksLayout.GetModeZones(spec);
            for (int i = 0; i < zones.Count; i++)
            {
                var z = zones[i];
                var go = new GameObject($"Zone_{z.Mode}_{i:00}");
                go.transform.SetParent(root.transform, false);
                go.transform.position = new Vector3(z.X, 4.5f, 0f);
                go.layer = MapTriggerLayer.PickupLayer;
                var box = go.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                box.size = new Vector2(1.5f, 11f);
                var zone = go.AddComponent<MapModeZone>();
                var so = new SerializedObject(zone);
                so.FindProperty("_targetMode").intValue = (int)z.Mode;
                so.FindProperty("_forceTransition").boolValue = false;
                so.FindProperty("_oneShot").boolValue = true;
                so.ApplyModifiedPropertiesWithoutUndo();
                count++;
            }

            return count;
        }

        // ───────────────────── Photos / Forks / Checkpoints ─────────────────────

        private static int RebuildPhotoPoints(Transform level, StageMapSpec spec, float groundTop)
        {
            var root = new GameObject("PhotoPoints");
            root.transform.SetParent(level, false);

            int count = 0;
            foreach (var p in Act3FireworksLayout.GetPhotos(spec))
            {
                var prefab = LoadPrefab($"{PhotoPrefabDir}/{p.Prefab}.prefab");
                if (prefab == null) continue;

                var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.transform);
                go.name = p.Prefab;
                go.transform.position = new Vector3(p.X, groundTop + 0.2f, 0f);
                count++;
            }

            return count;
        }

        private static void RebuildForks(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("Forks");
            root.transform.SetParent(level, false);
            var forks = Act3FireworksLayout.GetForkXs(spec);
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

            // 의미 있는 지점: Intro 끝(시작 등록) → 추격 뒤 → 세트피스 앞 → 골 앞
            float[] candidates =
            {
                spec.IntroEnd - 2f,
                FirstGrounded(spec, spec.PressureEnd + 1.5f, 1f),
                spec.BreathEnd + 1.5f,
                spec.SetpieceEnd + 1.5f,
            };

            int n = Mathf.Clamp(spec.Checkpoints, 1, candidates.Length);
            for (int i = 0; i < n; i++)
            {
                var go = new GameObject($"CP_{i + 1}");
                go.transform.SetParent(root.transform, false);
                go.transform.position = new Vector3(candidates[i], 1.5f, 0f);
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

        // ───────────────────── Utils ─────────────────────

        private static GameObject Spawn(GameObject prefab, Transform parent, float x, float y, string name)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.name = name;
            go.transform.position = new Vector3(x, y, 0f);
            return go;
        }

        private static GameObject LoadPrefab(string path)
        {
            if (PrefabCache.TryGetValue(path, out var cached))
                return cached;

            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null)
                Debug.LogWarning("[Act3FireworksDecorateCli] missing prefab " + path);
            PrefabCache[path] = go;
            return go;
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
    }
}
#endif


