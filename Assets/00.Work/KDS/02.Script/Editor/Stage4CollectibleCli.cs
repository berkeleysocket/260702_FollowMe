#if UNITY_EDITOR
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using YHW.Stats;
using YHW.UI;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Stage4 = Like_Imoge 수집 배치 + 스트레스/별점 시스템 보장.
    /// Live: unity command stage4-collectibles
    /// </summary>
    public static class Stage4CollectibleCli
    {
        private const string ScenePath = "Assets/00.Work/KDS/01.Scene/Stage4 Scene.unity";
        private const string ImogeDir = "Assets/00.Work/KDS/06.Prefab/Like_Imoge";
        private const string HeartPrefab = ImogeDir + "/Follow_13.prefab";
        private const string StressBarPrefab = "Assets/00.Work/KDS/06.Prefab/StressBarCanvas.prefab";

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

        [CliCommand("stage4-collectibles", "Rebuild Stage4 Like_Imoge collectibles")]
        public static int RebuildFromPipeline() => Rebuild() ? 0 : 1;

        [MenuItem("FollowMe/KDS/Rebuild Stage4 Collectibles (Like_Imoge)")]
        public static void RebuildFromMenu() => Rebuild();

        private static bool Rebuild()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (scene.path != ScenePath)
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var level = GameObject.Find("Level_S4");
            if (level == null)
            {
                Debug.LogError("[Stage4CollectibleCli] Level_S4 missing");
                return false;
            }

            var heart = AssetDatabase.LoadAssetAtPath<GameObject>(HeartPrefab);
            if (heart == null)
            {
                Debug.LogError("[Stage4CollectibleCli] missing " + HeartPrefab);
                return false;
            }

            var emojis = new GameObject[EmojiPrefabs.Length];
            for (int i = 0; i < EmojiPrefabs.Length; i++)
            {
                emojis[i] = AssetDatabase.LoadAssetAtPath<GameObject>(EmojiPrefabs[i]);
                if (emojis[i] == null)
                {
                    Debug.LogError("[Stage4CollectibleCli] missing " + EmojiPrefabs[i]);
                    return false;
                }
            }

            var existing = level.transform.Find("Collectibles");
            if (existing != null)
                Object.DestroyImmediate(existing.gameObject);

            var root = new GameObject("Collectibles");
            root.transform.SetParent(level.transform, false);

            EnsureSystems();

            var spec = StageMapDatabase.Get(4);
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
                    Spawn(emojis[likeN % emojis.Length], root.transform, s.X, s.Y, $"Like_{likeN:00}");
                    likeN++;
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[Stage4CollectibleCli] Follow={followN} Like={likeN}");
            return true;
        }

        private static void Spawn(GameObject prefab, Transform parent, float x, float y, string name)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.name = name;
            go.transform.position = new Vector3(x, y, 0f);
        }

        private static void EnsureSystems()
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

            var run = levelSystems.GetComponent<StageRunStats>();
            run.ConfigureStage(4);
        }
    }
}
#endif
