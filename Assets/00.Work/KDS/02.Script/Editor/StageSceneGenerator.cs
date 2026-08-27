#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Stage1 Scene을 템플릿으로 S1~S16 맵 씬을 일괄 생성·갱신.
    /// </summary>
    public static class StageSceneGenerator
    {
        private const string TemplateScenePath = "Assets/00.Work/KDS/01.Scene/Stage1 Scene.unity";
        private const string OutputFolder = "Assets/00.Work/KDS/01.Scene";

        [MenuItem("FollowMe/KDS/Generate All Stage Maps (S1-S16)")]
        public static void GenerateAllStageMaps()
        {
            if (!File.Exists(TemplateScenePath))
            {
                Debug.LogError($"[StageSceneGenerator] 템플릿 없음: {TemplateScenePath}");
                return;
            }

            Directory.CreateDirectory(OutputFolder);
            var results = new System.Text.StringBuilder();
            results.AppendLine("[StageSceneGenerator] S1~S16 맵 생성 시작");

            for (int stage = 1; stage <= 16; stage++)
            {
                try
                {
                    string scenePath = GetScenePath(stage);
                    StageMapSpec spec = StageMapDatabase.Get(stage);

                    if (stage == 1)
                    {
                        EditorSceneManager.OpenScene(TemplateScenePath);
                    }
                    else
                    {
                        if (File.Exists(scenePath))
                            AssetDatabase.DeleteAsset(scenePath);

                        if (!AssetDatabase.CopyAsset(TemplateScenePath, scenePath))
                        {
                            Debug.LogError($"[StageSceneGenerator] S{stage} 복제 실패");
                            continue;
                        }

                        AssetDatabase.SaveAssets();
                        EditorSceneManager.OpenScene(scenePath);
                    }

                    ApplySpecToOpenScene(spec);
                    var scene = SceneManager.GetActiveScene();
                    EditorSceneManager.MarkSceneDirty(scene);
                    if (!EditorSceneManager.SaveScene(scene, scenePath))
                        Debug.LogError($"[StageSceneGenerator] S{stage} 저장 실패: {scenePath}");

                    results.AppendLine($"  ✓ Stage {stage:D2} ({spec.ActTitle}, {spec.Template}, X={spec.LengthX}) → {scenePath}");
                    Debug.Log($"[StageSceneGenerator] Stage {stage} 완료");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[StageSceneGenerator] Stage {stage} 실패: {ex.Message}\n{ex.StackTrace}");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log(results.ToString());
        }

        private static string GetScenePath(int stage) =>
            $"{OutputFolder}/Stage{stage} Scene.unity";

        private static void ApplySpecToOpenScene(StageMapSpec spec)
        {
            RenameLevelRoot(spec);
            ConfigureGround(spec);
            ConfigureCamera(spec);
            ConfigurePlayerSpawn();
            RebuildZones(spec);
            RebuildCheckpoints(spec);
            RebuildPhotoPoints(spec);
            RebuildMapModeZones(spec);
            RebuildMonsterPlaceholders(spec);
            RebuildForkMarkers(spec);
            EnsureSystems();
        }

        private static void RenameLevelRoot(StageMapSpec spec)
        {
            var level = GameObject.Find("Level_S1") ?? FindChildByPrefix("Level_S");
            if (level == null)
            {
                level = new GameObject($"Level_S{spec.Stage}");
                return;
            }

            level.name = $"Level_S{spec.Stage}";
        }

        private static GameObject FindChildByPrefix(string prefix)
        {
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (root.name.StartsWith(prefix))
                    return root;
            }

            return null;
        }

        private static void ConfigureGround(StageMapSpec spec)
        {
            var ground = GameObject.Find("TempGround");
            if (ground == null) return;

            float width = spec.LengthX + 20f;
            ground.transform.position = new Vector3(spec.LengthX * 0.5f - 10f, -0.5f, 0f);
            ground.transform.localScale = new Vector3(width, 1f, 1f);
        }

        private static void ConfigureCamera(StageMapSpec spec)
        {
            var cam = Camera.main;
            if (cam == null) return;

            var follow = cam.GetComponent<SimpleCameraFollow>();
            if (follow == null) return;

            var so = new SerializedObject(follow);
            var maxX = so.FindProperty("_maxX");
            var minX = so.FindProperty("_minX");
            if (maxX == null || minX == null)
            {
                Debug.LogWarning("[StageSceneGenerator] SimpleCameraFollow _maxX/_minX 없음");
                return;
            }

            maxX.floatValue = spec.LengthX - 5f;
            minX.floatValue = -2f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigurePlayerSpawn()
        {
            var player = UnityEngine.Object.FindFirstObjectByType<PhotoProbePlayer>();
            if (player == null) return;
            player.transform.position = new Vector3(-2f, 1f, 0f);
        }

        private static Transform EnsureChild(Transform parent, string name)
        {
            var existing = parent.Find(name);
            if (existing != null)
                return existing;

            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static Transform GetOrCreateLevelRoot(StageMapSpec spec)
        {
            var level = GameObject.Find($"Level_S{spec.Stage}");
            if (level != null) return level.transform;

            var go = new GameObject($"Level_S{spec.Stage}");
            return go.transform;
        }

        private static void RebuildZones(StageMapSpec spec)
        {
            Transform level = GetOrCreateLevelRoot(spec);
            Transform zones = EnsureChild(level, "Zones");

            ClearChildren(zones);
            CreateZoneMarker(zones, "Intro", 0f, spec.IntroEnd);
            CreateZoneMarker(zones, "Teach", spec.IntroEnd, spec.TeachEnd);
            if (spec.HasPressure)
                CreateZoneMarker(zones, "Pressure", spec.TeachEnd, spec.PressureEnd);
            CreateZoneMarker(zones, "Breath", spec.HasPressure ? spec.PressureEnd : spec.TeachEnd, spec.BreathEnd);
            CreateZoneMarker(zones, "Setpiece", spec.BreathEnd, spec.SetpieceEnd);
            CreateZoneMarker(zones, "Goal", spec.SetpieceEnd, spec.GoalEnd);
        }

        private static void CreateZoneMarker(Transform parent, string name, float xStart, float xEnd)
        {
            float center = (xStart + xEnd) * 0.5f;
            float width = Mathf.Max(1f, xEnd - xStart);
            var go = new GameObject($"Zone_{name}");
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(center, 0.5f, 0f);
            go.transform.localScale = new Vector3(width, 0.1f, 1f);
        }

        private static void RebuildCheckpoints(StageMapSpec spec)
        {
            Transform level = GetOrCreateLevelRoot(spec);
            Transform triggers = EnsureChild(level, "Triggers");
            ClearChildrenNamed(triggers, "Checkpoint_");

            float[] xs = spec.Checkpoints switch
            {
                1 => new[] { 8f },
                2 => new[] { 8f, spec.BreathEnd * 0.5f },
                _ => new[] { 8f, spec.BreathEnd * 0.5f, spec.TeachEnd + 5f }
            };

            for (int i = 0; i < spec.Checkpoints && i < xs.Length; i++)
            {
                string id = i == 0 ? "CP_Intro" : $"CP_{i + 1}";
                CreateCheckpoint(triggers, $"Checkpoint_{id}", id, xs[i], i == 0);
            }
        }

        private static void CreateCheckpoint(Transform parent, string goName, string id, float x, bool registerOnStart)
        {
            var go = new GameObject(goName);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(x, 1f, 0f);
            go.transform.localScale = new Vector3(6f, 3f, 1f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            var cp = go.AddComponent<Checkpoint>();
            var so = new SerializedObject(cp);
            so.FindProperty("_checkpointId").stringValue = id;
            so.FindProperty("_registerOnStart").boolValue = registerOnStart;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void RebuildPhotoPoints(StageMapSpec spec)
        {
            Transform level = GetOrCreateLevelRoot(spec);
            Transform photoRoot = EnsureChild(level, "PhotoPoints");
            ClearChildren(photoRoot);

            float[] xs = StageMapDatabase.GetPhotoPositions(spec);
            for (int i = 0; i < xs.Length; i++)
            {
                var go = new GameObject($"Photo_S{spec.Stage}_{i + 1}");
                go.transform.SetParent(photoRoot, false);
                go.transform.position = new Vector3(xs[i], 1.5f, 0f);
                go.transform.localScale = new Vector3(4f, 3f, 1f);

                var col = go.AddComponent<BoxCollider2D>();
                col.isTrigger = true;

                var point = go.AddComponent<PhotoPoint>();
                var so = new SerializedObject(point);
                so.FindProperty("_pointId").stringValue = go.name;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void RebuildMapModeZones(StageMapSpec spec)
        {
            Transform level = GetOrCreateLevelRoot(spec);
            Transform modes = EnsureChild(level, "MapModeZones");
            ClearChildren(modes);

            if (!spec.HasPressure && !spec.TormentMode)
            {
                CreateMapModeZone(modes, "Stable_All", MapMode.Stable, 0f, spec.LengthX, false);
                return;
            }

            if (spec.TormentMode)
            {
                CreateMapModeZone(modes, "Stable_Intro", MapMode.Stable, 0f, spec.TeachEnd, true);
                CreateMapModeZone(modes, "Torment_Pressure", MapMode.Torment, spec.TeachEnd, spec.SetpieceEnd, true);
                CreateMapModeZone(modes, "Stable_Ending", MapMode.Stable, spec.SetpieceEnd, spec.LengthX, true);
                return;
            }

            float warnStart = spec.TeachEnd;
            float warnEnd = warnStart + Mathf.Min(15f, (spec.PressureEnd - spec.TeachEnd) * 0.4f);
            CreateMapModeZone(modes, "Stable_Teach", MapMode.Stable, 0f, warnStart, true);
            CreateMapModeZone(modes, "Warning", MapMode.Warning, warnStart, warnEnd, true);
            CreateMapModeZone(modes, "Chase", MapMode.Chase, warnEnd, spec.PressureEnd, true);
            CreateMapModeZone(modes, "Recovery", MapMode.Recovery, spec.PressureEnd, spec.BreathEnd, true);
            CreateMapModeZone(modes, "Stable_Setpiece", MapMode.Stable, spec.BreathEnd, spec.LengthX, true);
        }

        private static void CreateMapModeZone(Transform parent, string name, MapMode mode, float xStart, float xEnd, bool oneShot)
        {
            if (xEnd <= xStart) return;

            float center = (xStart + xEnd) * 0.5f;
            float width = xEnd - xStart;
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(center, 2f, 0f);
            go.transform.localScale = new Vector3(width, 4f, 1f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            var zone = go.AddComponent<MapModeZone>();
            var so = new SerializedObject(zone);
            so.FindProperty("_targetMode").enumValueIndex = (int)mode;
            so.FindProperty("_oneShot").boolValue = oneShot;
            so.FindProperty("_forceTransition").boolValue = mode == MapMode.Torment;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void RebuildMonsterPlaceholders(StageMapSpec spec)
        {
            Transform level = GetOrCreateLevelRoot(spec);
            Transform monsters = EnsureChild(level, "Monsters");
            ClearChildren(monsters);

            if (spec.Monsters <= 0) return;

            for (int i = 0; i < spec.Monsters; i++)
            {
                float x = spec.HasPressure
                    ? Mathf.Lerp(spec.TeachEnd + 5f, spec.PressureEnd - 5f, (i + 1f) / (spec.Monsters + 1f))
                    : spec.SetpieceEnd * 0.5f;

                var slot = new GameObject($"MonsterSlot_{i + 1}");
                slot.transform.SetParent(monsters, false);
                slot.transform.position = new Vector3(x, 1f, 0f);

                var body = new GameObject("Body");
                body.transform.SetParent(slot.transform, false);
                body.transform.localPosition = Vector3.zero;
                body.transform.localScale = new Vector3(1.5f, 2f, 1f);

                var modeObj = slot.AddComponent<MapModeObject>();
                var so = new SerializedObject(modeObj);
                var modes = so.FindProperty("_activeInModes");
                modes.arraySize = spec.TormentMode ? 1 : 1;
                modes.GetArrayElementAtIndex(0).enumValueIndex = spec.TormentMode
                    ? (int)MapMode.Torment
                    : (int)MapMode.Chase;
                var targets = so.FindProperty("_targets");
                targets.arraySize = 1;
                targets.GetArrayElementAtIndex(0).objectReferenceValue = body;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void RebuildForkMarkers(StageMapSpec spec)
        {
            if (spec.Forks <= 0) return;

            Transform level = GetOrCreateLevelRoot(spec);
            Transform forks = EnsureChild(level, "Forks");
            ClearChildren(forks);

            float[] xs = spec.Forks switch
            {
                1 => new[] { spec.BreathEnd * 0.7f },
                _ => new[] { spec.BreathEnd * 0.55f, spec.SetpieceEnd * 0.85f }
            };

            for (int i = 0; i < xs.Length && i < spec.Forks; i++)
            {
                var go = new GameObject($"Fork_{i + 1}");
                go.transform.SetParent(forks, false);
                go.transform.position = new Vector3(xs[i], 2f, 0f);
            }
        }

        private static void EnsureSystems()
        {
            if (UnityEngine.Object.FindFirstObjectByType<MapModeService>() == null)
            {
                var go = new GameObject("LevelSystems");
                go.AddComponent<MapModeService>();
                go.AddComponent<CheckpointService>();
            }

            if (UnityEngine.Object.FindFirstObjectByType<SocialScoreService>() == null)
            {
                var go = new GameObject("SocialSystems");
                go.AddComponent<SocialScoreService>();
                go.AddComponent<SocialScoreHud>();
            }

            if (UnityEngine.Object.FindFirstObjectByType<PhotoProbePlayer>() == null)
            {
                var go = new GameObject("PhotoProbePlayer");
                go.tag = "Player";
                go.AddComponent<Rigidbody2D>();
                go.AddComponent<BoxCollider2D>();
                go.AddComponent<PhotoProbePlayer>();
                go.transform.position = new Vector3(-2f, 1f, 0f);
            }

            var player = UnityEngine.Object.FindFirstObjectByType<PhotoProbePlayer>();
            var checkpoint = UnityEngine.Object.FindFirstObjectByType<CheckpointService>();
            if (checkpoint != null && player != null)
            {
                var so = new SerializedObject(checkpoint);
                so.FindProperty("_playerOverride").objectReferenceValue = player;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            var cam = Camera.main;
            if (cam != null && player != null)
            {
                var follow = cam.GetComponent<SimpleCameraFollow>();
                if (follow != null)
                {
                    var so = new SerializedObject(follow);
                    so.FindProperty("_target").objectReferenceValue = player.transform;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }
        }

        private static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
                UnityEngine.Object.DestroyImmediate(parent.GetChild(i).gameObject);
        }

        private static void ClearChildrenNamed(Transform parent, string prefix)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
                if (child.name.StartsWith(prefix))
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
            }
        }
    }
}
#endif
