#if UNITY_EDITOR
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using YHW.Stats;
using YHW.UI;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Stage3 = Angry_Imoge를 일자 배치 + StressMeter 연동.
    /// Live: unity command stage3-angry
    /// </summary>
    public static class Stage3AngryCollectibleCli
    {
        private const string ScenePath = "Assets/00.Work/KDS/01.Scene/Stage3 Scene.unity";
        private const string AngryDir = "Assets/00.Work/KDS/06.Prefab/Angry_Imoge";
        private const string StressBarPrefab = "Assets/00.Work/KDS/06.Prefab/StressBarCanvas.prefab";

        private const float LineY = 1.2f;
        private const float XStart = 8f;
        private const float XEnd = 136f;
        private const int Count = 16;

        [CliCommand("stage3-angry", "Place Stage3 angry emojis in a straight line + stress meter")]
        public static int RebuildFromPipeline()
        {
            return Rebuild() ? 0 : 1;
        }

        [MenuItem("FollowMe/KDS/Rebuild Stage3 Angry Collectibles")]
        public static void RebuildFromMenu()
        {
            Rebuild();
        }

        private static bool Rebuild()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (scene.path != ScenePath)
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var level = GameObject.Find("Level_S3") ?? GameObject.Find("Level_S1");
            if (level == null)
            {
                Debug.LogError("[Stage3AngryCollectibleCli] Level_S3/S1 missing");
                return false;
            }

            if (level.name != "Level_S3")
                level.name = "Level_S3";

            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { AngryDir });
            if (prefabGuids.Length == 0)
            {
                Debug.LogError("[Stage3AngryCollectibleCli] no prefabs in " + AngryDir);
                return false;
            }

            var prefabs = new GameObject[prefabGuids.Length];
            for (int i = 0; i < prefabGuids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefabs[i] == null)
                {
                    Debug.LogError("[Stage3AngryCollectibleCli] missing " + path);
                    return false;
                }
            }

            System.Array.Sort(prefabs, (a, b) => string.CompareOrdinal(a.name, b.name));

            var existing = level.transform.Find("AngryCollectibles");
            if (existing != null)
                Object.DestroyImmediate(existing.gameObject);

            // Stage3 collectibles = angry only
            foreach (var name in new[] { "Collectibles", "PhotoPoints" })
            {
                var extra = level.transform.Find(name);
                if (extra != null)
                    Object.DestroyImmediate(extra.gameObject);
            }

            foreach (var p in Object.FindObjectsByType<PhotoPoint>(FindObjectsSortMode.None))
            {
                if (p != null)
                    Object.DestroyImmediate(p.gameObject);
            }

            var root = new GameObject("AngryCollectibles");
            root.transform.SetParent(level.transform, false);

            float span = XEnd - XStart;
            float step = Count <= 1 ? 0f : span / (Count - 1);

            for (int i = 0; i < Count; i++)
            {
                var prefab = prefabs[i % prefabs.Length];
                var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.transform);
                go.name = $"Angry_{i:00}";
                go.transform.position = new Vector3(XStart + step * i, LineY, 0f);
            }

            EnsureStressSystems();
            EnsureScoreBridge();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[Stage3AngryCollectibleCli] Angry line={Count} y={LineY} x={XStart}..{XEnd}");
            return true;
        }

        private static void EnsureScoreBridge()
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
            if (systems.GetComponent<StressMeterBootstrap>() == null)
                systems.AddComponent<StressMeterBootstrap>();
        }

        private static void EnsureStressSystems()
        {
            var systems = GameObject.Find("SocialSystems");
            if (systems == null)
                systems = new GameObject("SocialSystems");

            var meter = systems.GetComponent<StressMeter>();
            if (meter == null)
                meter = Object.FindFirstObjectByType<StressMeter>();
            if (meter == null)
                meter = systems.AddComponent<StressMeter>();

            var bar = Object.FindFirstObjectByType<StressBar>();
            if (bar == null)
            {
                var canvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(StressBarPrefab);
                if (canvasPrefab != null)
                {
                    var canvas = (GameObject)PrefabUtility.InstantiatePrefab(canvasPrefab);
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

            var bridge = systems.GetComponent<SocialItemScoreBridge>();
            if (bridge != null)
            {
                var so = new SerializedObject(bridge);
                var meterProp = so.FindProperty("_stressMeter");
                if (meterProp != null)
                {
                    meterProp.objectReferenceValue = meter;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }
        }
    }
}
#endif
