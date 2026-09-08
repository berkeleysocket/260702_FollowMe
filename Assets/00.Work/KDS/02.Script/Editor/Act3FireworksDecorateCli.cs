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
    /// Act3(S9~S11) 구조물·배경·수집·포토·장애물 배치 (지면/플랫폼은 유지).
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

        private static readonly string[] PropPrefabNames =
        {
            "Act3_Prop_RiverRail",
            "Act3_Prop_ParkBench",
            "Act3_Prop_FoodStall",
            "Act3_Prop_LanternPole",
            "Act3_Prop_FestivalFlag",
            "Act3_Prop_CrowdSilhouette",
            "Act3_Prop_BridgePillar",
            "Act3_Prop_PierPlank",
            "Act3_Prop_TreeAutumn",
            "Act3_Prop_TrashBin",
            "Act3_Prop_VendorCart",
            "Act3_Prop_StringLanterns",
            "Act3_Prop_SpeakerStack",
            "Act3_Prop_BarrierCone",
            "Act3_Prop_PhotoFrame",
            "Act3_Prop_EmptyBench",
            "Act3_Prop_RiverEdge",
            "Act3_Prop_NightBush",
        };

        private static readonly string[] PhotoPrefabNames =
        {
            "Photo_Act3_BridgeView",
            "Photo_Act3_FireworkPeak",
            "Photo_Act3_PhoneCrowd",
            "Photo_Act3_FoodTruck",
            "Photo_Act3_DroneShow",
            "Photo_Act3_PhotoBooth",
            "Photo_Act3_PierBoat",
            "Photo_Act3_EmptyRiver",
            "Photo_Act3_BridgeLights",
            "Photo_Act3_WaterReflect",
        };

        private const string BgMain = "Act3_BG_Promenade";
        private const string BgSky = "Act3_BG_SkyFireworks";
        private const string BgLights = "Act3_BG_FestivalLights";
        private const string BgVoidMain = "Act3_BG_EmptyVoid";

        [CliCommand("act3-decorate", "Place Act3 S9-S11 props, backgrounds, collectibles, photos, hazards")]
        public static int DecorateFromPipeline() => DecorateAll() ? 0 : 1;

        [MenuItem("FollowMe/KDS/Decorate Fireworks Stages (S9-S11 Props+BG+Collectibles)")]
        public static void DecorateFromMenu() => DecorateAll();

        public static bool DecorateAll()
        {
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

        private static bool DecorateStage(
            int stage, GameObject heart, GameObject[] emojis, System.Text.StringBuilder results)
        {
            string scenePath = $"{SceneDir}/Stage{stage} Scene.unity";
            if (!File.Exists(Path.GetFullPath(scenePath)))
            {
                Debug.LogError("[Act3FireworksDecorateCli] missing " + scenePath);
                return false;
            }

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var spec = StageMapDatabase.Get(stage);
            var level = GameObject.Find($"Level_S{stage}");
            if (level == null)
            {
                Debug.LogError($"[Act3FireworksDecorateCli] Level_S{stage} not found — run act3-terrain first");
                return false;
            }

            WipeDecor(level.transform);

            int bg = RebuildBackground(level.transform, spec);
            int props = RebuildThemeProps(level.transform, spec);
            int collects = RebuildCollectibles(level.transform, spec, heart, emojis);
            int photos = RebuildPhotoPoints(level.transform, spec);
            int hazards = RebuildHazards(level.transform, spec);
            RebuildForks(level.transform, spec);
            RebuildCheckpoints(level.transform, spec);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            results.AppendLine(
                $"S{stage}: bg={bg} props={props} collectibles={collects} photos={photos} hazards={hazards}");
            return true;
        }

        private static void WipeDecor(Transform level)
        {
            string[] wipe =
            {
                "Background", "ThemeProps", "Collectibles", "PhotoPoints", "Hazards",
                "Forks", "Checkpoints", "Triggers"
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

        private static int RebuildBackground(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("Background");
            root.transform.SetParent(level, false);

            // 풀씬 파노라마를 여러 장 겹치면 깨짐 → 하늘 / 메인 산책로 / 랜턴 3층만
            int count = 0;
            count += PlaceBgStrip(root.transform, BgSky, spec, y: 0.2f, scaleX: 1.6f, scaleY: 1.85f, "Sky");

            float mainY = 0f;
            float voidStart = spec.Stage == 11 ? spec.BreathEnd : float.MaxValue;

            // 축제 구간: Promenade 메인 (강변 산책로 한 장)
            count += PlaceBgStripRange(
                root.transform, BgMain, spec, -12f, voidStart, mainY, 1.5f, 1.35f, "Main");

            // S11 공허: EmptyVoid로 교체
            if (spec.Stage == 11)
            {
                count += PlaceBgStripRange(
                    root.transform, BgVoidMain, spec, voidStart, spec.LengthX + 24f, mainY, 1.5f, 1.35f, "Void");
            }

            // 상단 랜턴 오버레이 (축제 구간만)
            float lightsEnd = Mathf.Min(voidStart, spec.LengthX + 24f);
            count += PlaceBgStripRange(
                root.transform, BgLights, spec, -12f, lightsEnd, 6.5f, 1.6f, 1.0f, "Lights");

            return count;
        }

        private static int PlaceBgStrip(
            Transform parent, string prefabName, StageMapSpec spec,
            float y, float scaleX, float scaleY, string prefix)
        {
            return PlaceBgStripRange(parent, prefabName, spec, -12f, spec.LengthX + 24f, y, scaleX, scaleY, prefix);
        }

        private static int PlaceBgStripRange(
            Transform parent, string prefabName, StageMapSpec spec,
            float x0, float x1, float yBottom, float scaleX, float scaleY, string prefix)
        {
            string path = $"{PropPrefabDir}/{prefabName}.prefab";
            var prefab = LoadPrefab(path);
            if (prefab == null) return 0;

            float worldW = 16f * scaleX;
            float yCenterOffset = 0f;
            var probe = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                var sr = probe.GetComponentInChildren<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    worldW = Mathf.Max(8f, sr.sprite.bounds.size.x * scaleX);
                    float pivotNormalizedY = sr.sprite.pivot.y / Mathf.Max(1f, sr.sprite.rect.height);
                    if (pivotNormalizedY > 0.25f)
                        yCenterOffset = sr.sprite.bounds.extents.y * scaleY;
                }
            }
            finally
            {
                Object.DestroyImmediate(probe);
            }

            // 겹침 없이 타일 — 너무 조밀하면 타임아웃
            float step = worldW;
            int count = 0;
            int i = 0;
            for (float x = x0; x < x1; x += step)
            {
                var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                go.name = $"{prefix}_{i:00}";
                go.transform.position = new Vector3(x + worldW * 0.5f, yBottom + yCenterOffset, 0f);
                go.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                count++;
                i++;
                if (i > 80) break; // 안전 상한
            }

            return count;
        }

        private static int RebuildThemeProps(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("ThemeProps");
            root.transform.SetParent(level, false);

            var loaded = new List<GameObject>();
            foreach (var name in PropPrefabNames)
            {
                var p = LoadPrefab($"{PropPrefabDir}/{name}.prefab");
                if (p != null) loaded.Add(p);
            }

            if (loaded.Count == 0) return 0;

            int count = 0;
            var rng = new System.Random(4200 + spec.Stage * 17);
            float x = 8f;
            int i = 0;
            while (x < spec.LengthX - 8f)
            {
                // S11 공허 구간은 프롭 희소
                if (spec.Stage == 11 && x > spec.BreathEnd && x < spec.SetpieceEnd)
                {
                    if (rng.NextDouble() > 0.35)
                    {
                        x += 14f;
                        i++;
                        continue;
                    }
                }

                var prefab = loaded[(i + spec.Stage) % loaded.Count];
                var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.transform);
                go.name = $"{prefab.name}_{i:00}";
                float y = 0f;
                // 군중·가로등은 지면, 깃발·랜턴은 살짝 위
                if (prefab.name.Contains("StringLanterns") || prefab.name.Contains("FestivalFlag"))
                    y = 2.2f + (i % 3) * 0.4f;
                else if (prefab.name.Contains("Crowd"))
                    y = 0f;
                go.transform.position = new Vector3(x, y, 0f);
                count++;
                i++;
                x += 9f + (i % 4) * 1.5f;
            }

            // 갈림길 주변 밀집 소품
            var forks = StageCollectibleLayout.GetForkXs(spec);
            for (int f = 0; f < forks.Length; f++)
            {
                PlacePropNear(root.transform, loaded, forks[f] - 3f, 0f, $"ForkCrowdA_{f}", "Crowd");
                PlacePropNear(root.transform, loaded, forks[f] + 4f, 0f, $"ForkStall_{f}", "FoodStall");
                PlacePropNear(root.transform, loaded, forks[f] + 1f, 2.4f, $"ForkLantern_{f}", "Lantern");
                count += 3;
            }

            return count;
        }

        private static void PlacePropNear(
            Transform parent, List<GameObject> loaded, float x, float y, string name, string hint)
        {
            GameObject prefab = null;
            foreach (var p in loaded)
            {
                if (p.name.Contains(hint))
                {
                    prefab = p;
                    break;
                }
            }

            prefab ??= loaded[0];
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.name = name;
            go.transform.position = new Vector3(x, y, 0f);
        }

        private static int RebuildCollectibles(
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
                    float y = s.Y;
                    if (y >= StageCollectibleLayout.GetHighLikeY(spec) - 0.1f)
                        y = StageCollectibleLayout.GetPlatformY(spec) + 1.1f;
                    Spawn(emojis[likeN % emojis.Length], root.transform, s.X, y, $"Like_{likeN:00}");
                    likeN++;
                }
            }

            return followN + likeN;
        }

        private static int RebuildPhotoPoints(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("PhotoPoints");
            root.transform.SetParent(level, false);

            float[] xs = StageMapDatabase.GetPhotoPositions(spec);
            int count = 0;
            for (int i = 0; i < xs.Length; i++)
            {
                int idx = (spec.Stage - 9 + i * 3) % PhotoPrefabNames.Length;
                string path = $"{PhotoPrefabDir}/{PhotoPrefabNames[idx]}.prefab";
                var prefab = LoadPrefab(path);
                if (prefab == null) continue;

                var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.transform);
                go.name = PhotoPrefabNames[idx];
                float y = StageCollectibleLayout.GetPlatformY(spec) + 0.4f;
                go.transform.position = new Vector3(xs[i], y, 0f);
                count++;
            }

            return count;
        }

        private static int RebuildHazards(Transform level, StageMapSpec spec)
        {
            var root = new GameObject("Hazards");
            root.transform.SetParent(level, false);

            var ember = LoadPrefab($"{HazardPrefabDir}/EmberHazard.prefab");
            var crowd = LoadPrefab($"{HazardPrefabDir}/CrowdBumpHazard.prefab");
            var window = LoadPrefab($"{HazardPrefabDir}/FireworkLikeWindow.prefab");
            int count = 0;
            var rng = new System.Random(5100 + spec.Stage * 13);

            // 불꽃 윈도우 — 세트피스 하이 루트
            if (window != null)
            {
                float setX = spec.BreathEnd + 10f;
                int n = 0;
                while (setX < spec.SetpieceEnd - 12f)
                {
                    Spawn(window, root.transform, setX, 7.6f, $"FireworkWin_{n:00}");
                    count++;
                    n++;
                    setX += 16f;
                }
            }

            // 잿불 — Pressure~Breath
            if (ember != null)
            {
                int emberN = 4 + (spec.Stage - 9) * 2;
                for (int i = 0; i < emberN; i++)
                {
                    float x = Mathf.Lerp(spec.TeachEnd, spec.BreathEnd, (i + 0.5f) / emberN);
                    x += rng.Next(-2, 3);
                    Spawn(ember, root.transform, x, 0.15f, $"Ember_{i:00}");
                    count++;
                }
            }

            // 군중 범프 — 갈림 하단 + Pressure
            if (crowd != null)
            {
                var forks = StageCollectibleLayout.GetForkXs(spec);
                for (int i = 0; i < forks.Length; i++)
                {
                    Spawn(crowd, root.transform, forks[i] - 1.5f, 0.2f, $"Crowd_Fork_{i}");
                    count++;
                }

                float cx = spec.PressureEnd - 20f;
                for (int i = 0; i < 2 + (spec.Stage - 9); i++)
                {
                    Spawn(crowd, root.transform, cx + i * 12f, 0.2f, $"Crowd_Run_{i}");
                    count++;
                }
            }

            return count;
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

        private static void Spawn(GameObject prefab, Transform parent, float x, float y, string name)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.name = name;
            go.transform.position = new Vector3(x, y, 0f);
        }

        private static GameObject LoadPrefab(string path)
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null)
                Debug.LogWarning("[Act3FireworksDecorateCli] missing prefab " + path);
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
