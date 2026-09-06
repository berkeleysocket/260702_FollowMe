#if UNITY_EDITOR
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Stage1 수집 = YHW Prefabs/Items 인스턴스 배치.
    /// Live: unity command stage1-collectibles
    /// </summary>
    public static class Stage1CollectibleCli
    {
        private const string ScenePath = "Assets/00.Work/KDS/01.Scene/Stage1 Scene.unity";
        private const string HeartPrefab =
            "Assets/00.Work/YHW/YHW/Prefabs/Items/Pickup_heart.prefab";

        private static readonly string[] EmojiPrefabs =
        {
            "Assets/00.Work/YHW/YHW/Prefabs/Items/Pickup_emoji_smile.prefab",
            "Assets/00.Work/YHW/YHW/Prefabs/Items/Pickup_emoji_love.prefab",
            "Assets/00.Work/YHW/YHW/Prefabs/Items/Pickup_emoji_cool.prefab",
            "Assets/00.Work/YHW/YHW/Prefabs/Items/Pickup_emoji_tongue.prefab",
            "Assets/00.Work/YHW/YHW/Prefabs/Items/Pickup_emoji_ghost.prefab",
            "Assets/00.Work/YHW/YHW/Prefabs/Items/Pickup_emoji_cry.prefab",
            "Assets/00.Work/YHW/YHW/Prefabs/Items/Pickup_emoji_devil.prefab",
            "Assets/00.Work/YHW/YHW/Prefabs/Items/Pickup_emoji_angry.prefab",
        };

        [CliCommand("stage1-collectibles", "Rebuild Stage1 using YHW Pickup_heart / Pickup_emoji prefabs")]
        public static int RebuildFromPipeline()
        {
            return Rebuild() ? 0 : 1;
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
            Debug.Log($"[Stage1CollectibleCli] YHW prefabs Follow={followN} Like={likeN}");
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
