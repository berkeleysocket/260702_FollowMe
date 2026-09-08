#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Start → Stage Select → StageN 씬 플로우 세팅 + Build Settings 등록.
    /// Live: unity command menu-flow-setup
    /// </summary>
    public static class MenuFlowSetupCli
    {
        private const string SceneDir = "Assets/00.Work/KDS/01.Scene";
        private const string StartScenePath = SceneDir + "/Start Scene.unity";
        private const string SelectScenePath = SceneDir + "/Stage Select Scene.unity";
        private const string TitleCanvasPrefab =
            "Assets/00.Work/YHW/YHW/Prefabs/UI/Title/TitleCanvas.prefab";

        [CliCommand("menu-flow-setup", "Wire Start→StageSelect→Stage scenes + Build Settings")]
        public static int SetupFromPipeline() => SetupAll() ? 0 : 1;

        [MenuItem("FollowMe/KDS/Setup Menu Flow (Start→Select→Stages)")]
        public static void SetupFromMenu() => SetupAll();

        public static bool SetupAll()
        {
            if (!File.Exists(Path.GetFullPath(StartScenePath)))
            {
                Debug.LogError("[MenuFlowSetupCli] missing Start Scene");
                return false;
            }

            EnsureStageSelectScene();
            WireStartScene();
            WireStageSelectScene();
            UpdateBuildSettings();

            AssetDatabase.SaveAssets();
            Debug.Log("[MenuFlowSetupCli] Done. Start → Stage Select → Stage1~16 Build Settings.");
            return true;
        }

        private static void EnsureStageSelectScene()
        {
            if (File.Exists(Path.GetFullPath(SelectScenePath)))
            {
                Debug.Log("[MenuFlowSetupCli] Stage Select Scene exists — rewiring");
                return;
            }

            // Start Scene을 복사해 Stage Select용으로 사용
            AssetDatabase.CopyAsset(StartScenePath, SelectScenePath);
            AssetDatabase.Refresh();
        }

        private static void WireStartScene()
        {
            var scene = EditorSceneManager.OpenScene(StartScenePath, OpenSceneMode.Single);
            EnsureBridge<StartMenuFlowBridge>("MenuFlow");
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void WireStageSelectScene()
        {
            var scene = EditorSceneManager.OpenScene(SelectScenePath, OpenSceneMode.Single);

            // TitleCanvas 없으면 배치
            if (Object.FindFirstObjectByType<YHW.UI.TitleMenuController>() == null)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TitleCanvasPrefab);
                if (prefab != null)
                    PrefabUtility.InstantiatePrefab(prefab);
            }

            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            EnsureBridge<StageSelectFlowBridge>("MenuFlow");
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void EnsureBridge<T>(string hostName) where T : Component
        {
            var existing = Object.FindFirstObjectByType<T>();
            if (existing != null) return;

            var host = GameObject.Find(hostName);
            if (host == null)
                host = new GameObject(hostName);
            host.AddComponent<T>();
        }

        private static void UpdateBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>();

            void Add(string path)
            {
                if (!File.Exists(Path.GetFullPath(path)))
                {
                    Debug.LogWarning("[MenuFlowSetupCli] skip missing " + path);
                    return;
                }

                scenes.Add(new EditorBuildSettingsScene(path, true));
            }

            Add(StartScenePath);
            Add(SelectScenePath);
            for (int i = 1; i <= StageSceneCatalog.StageCount; i++)
                Add($"{SceneDir}/Stage{i} Scene.unity");

            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"[MenuFlowSetupCli] Build Settings scenes={scenes.Count}");
        }
    }
}
#endif
