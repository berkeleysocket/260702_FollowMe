#if UNITY_EDITOR
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Stage1 수집 = KDS Like_Imoge 프리팹 배치.
    /// Live: unity command stage1-collectibles
    /// </summary>
    public static class Stage1CollectibleCli
    {
        private const string ScenePath = "Assets/00.Work/KDS/01.Scene/Stage1 Scene.unity";
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

        [CliCommand("stage1-collectibles", "Rebuild Stage1 using KDS Like_Imoge like/follow prefabs")]
        public static int RebuildFromPipeline()
        {
            return Rebuild() ? 0 : 1;
        }

        [MenuItem("FollowMe/KDS/Rebuild Stage1 Collectibles (Like_Imoge)")]
        public static void RebuildFromMenu()
        {
            Rebuild();
        }

        private static bool Rebuild()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (scene.path != ScenePath)
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var level = GameObject.Find("Level_S1");
            if (level == null)
            {
                Debug.LogError("[Stage1CollectibleCli] Level_S1 missing");
                return false;
            }

            var heart = AssetDatabase.LoadAssetAtPath<GameObject>(HeartPrefab);
            if (heart == null)
            {
                Debug.LogError("[Stage1CollectibleCli] missing " + HeartPrefab);
                return false;
            }

            var emojis = new GameObject[EmojiPrefabs.Length];
            for (int i = 0; i < EmojiPrefabs.Length; i++)
            {
                emojis[i] = AssetDatabase.LoadAssetAtPath<GameObject>(EmojiPrefabs[i]);
                if (emojis[i] == null)
                {
                    Debug.LogError("[Stage1CollectibleCli] missing " + EmojiPrefabs[i]);
                    return false;
                }
            }

            foreach (var photo in Object.FindObjectsByType<PhotoPoint>(FindObjectsSortMode.None))
            {
                if (photo != null)
                    Object.DestroyImmediate(photo.gameObject);
            }

            var existing = level.transform.Find("Collectibles");
            if (existing != null)
                Object.DestroyImmediate(existing.gameObject);

            var root = new GameObject("Collectibles");
            root.transform.SetParent(level.transform, false);

            EnsureScoreBridge();

            var spec = StageMapDatabase.Get(1);
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
                    var prefab = emojis[likeN % emojis.Length];
                    Spawn(prefab, root.transform, s.X, s.Y, $"Like_{likeN:00}");
                    likeN++;
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[Stage1CollectibleCli] Like_Imoge Follow={followN} Like={likeN}");
            return true;
        }

        private static void EnsureScoreBridge()
        {
            var systems = GameObject.Find("SocialSystems");
            if (systems == null)
            {
                systems = new GameObject("SocialSystems");
            }

            if (systems.GetComponent<SocialItemScoreBridge>() == null)
                systems.AddComponent<SocialItemScoreBridge>();
        }

        private static void Spawn(GameObject prefab, Transform parent, float x, float y, string name)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.name = name;
            go.transform.position = new Vector3(x, y, 0f);
        }
    }
}
#endif
