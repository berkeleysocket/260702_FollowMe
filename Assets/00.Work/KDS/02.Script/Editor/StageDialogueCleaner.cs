#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// 스테이지 씬에 배치된 DialogueTrigger / DialogueSystem 등을 제거.
    /// </summary>
    public static class StageDialogueCleaner
    {
        private const string SceneFolder = "Assets/00.Work/KDS/01.Scene";

        [MenuItem("FollowMe/KDS/Remove All Dialogue From Stage Maps (S1-S16)")]
        public static void RemoveAllFromStageMaps()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("[StageDialogueCleaner] Play 모드 중에는 실행할 수 없습니다.");
                return;
            }

            var log = new StringBuilder();
            log.AppendLine("[StageDialogueCleaner] S1~S16 다이얼로그 제거 시작");

            for (int stage = 1; stage <= 16; stage++)
            {
                string path = GetScenePath(stage);
                if (!File.Exists(path))
                {
                    log.AppendLine($"  - S{stage:D2} 씬 없음");
                    continue;
                }

                EditorSceneManager.OpenScene(path);
                int removed = RemoveFromOpenScene();
                var scene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene, path);
                log.AppendLine($"  ✓ S{stage:D2} 제거 {removed}개");
            }

            AssetDatabase.Refresh();
            Debug.Log(log.ToString());
        }

        public static int RemoveFromOpenScene()
        {
            int removed = 0;

            var triggers = Object.FindObjectsByType<DialogueTrigger>(FindObjectsSortMode.None);
            foreach (var trigger in triggers)
            {
                if (trigger == null) continue;
                Object.DestroyImmediate(trigger.gameObject);
                removed++;
            }

            var players = Object.FindObjectsByType<DialoguePlayer>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                if (player == null) continue;
                Object.DestroyImmediate(player.gameObject);
                removed++;
            }

            var speakers = Object.FindObjectsByType<DialogueSpeaker>(FindObjectsSortMode.None);
            foreach (var speaker in speakers)
            {
                if (speaker == null) continue;
                Object.DestroyImmediate(speaker.gameObject);
                removed++;
            }

            return removed;
        }

        private static string GetScenePath(int stage) =>
            $"{SceneFolder}/Stage{stage} Scene.unity";
    }
}
#endif
