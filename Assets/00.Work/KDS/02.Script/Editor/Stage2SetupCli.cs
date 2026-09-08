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
    /// Stage2 = 포토존 튜토리얼 + 피사체 + Like_Imoge 수집 + 클리어.
    /// Live: unity command stage2-setup
    /// </summary>
    public static class Stage2SetupCli
    {
        private const string ScenePath = "Assets/00.Work/KDS/01.Scene/Stage2 Scene.unity";
        private const string TileDir = "Assets/00.Work/KDS/05.Asset/City_Modern/Act1_Tiles";
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

        private static readonly string[] PhotoRewardPaths =
        {
            "Assets/00.Work/KDS/08.SO/PhotoPoint/PhotoReward_ForkViewpoint.asset",
            "Assets/00.Work/KDS/08.SO/PhotoPoint/PhotoReward_NeonBillboard.asset",
            "Assets/00.Work/KDS/08.SO/PhotoPoint/PhotoReward_Busking.asset",
        };

        private static readonly string[] PhotoStillPaths =
        {
            TileDir + "/Act1_PhotoStill_Viewpoint.png",
            TileDir + "/Act1_PhotoStill_Neon.png",
            TileDir + "/Act1_PhotoStill_Busking.png",
        };

        private static readonly string[] PhotoIds =
        {
            "Photo_S2_Fork",
            "Photo_S2_Neon",
            "Photo_S2_Busking",
        };

        private static readonly string[] SubjectSheetPaths =
        {
            TileDir + "/Act1_PhotoSubject_Viewpoint_64x48.png",
            TileDir + "/Act1_PhotoSubject_Neon_48x64.png",
            TileDir + "/Act1_PhotoSubject_Busking_64x48.png",
        };

        private static readonly Vector2Int[] SubjectCellSizes =
        {
            new Vector2Int(64, 48),
            new Vector2Int(48, 64),
            new Vector2Int(64, 48),
        };

        private const string CameraSheetPath = TileDir + "/Act1_PhotoCamera_32x32.png";

        [CliCommand("stage2-setup", "Rebuild Stage2 photo tutorial: subjects + pickups + PhotoPoints + Goal")]
        public static int SetupFromPipeline()
        {
            return Setup() ? 0 : 1;
        }

        [CliCommand("stage2-photo-subjects", "Import photo subject sheets and place animated props on Stage2 PhotoPoints")]
        public static int SubjectsOnlyFromPipeline()
        {
            return PlaceSubjectsOnly() ? 0 : 1;
        }

        private static bool Setup()
        {
            if (!ImportPhotoArt())
                return false;

            var scene = EditorSceneManager.GetActiveScene();
            if (scene.path != ScenePath)
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var level = GameObject.Find("Level_S2") ?? GameObject.Find("Level_S1");
            if (level == null)
            {
                Debug.LogError("[Stage2SetupCli] Level_S2/S1 missing");
                return false;
            }

            if (level.name != "Level_S2")
                level.name = "Level_S2";

            var heart = AssetDatabase.LoadAssetAtPath<GameObject>(HeartPrefab);
            if (heart == null)
            {
                Debug.LogError("[Stage2SetupCli] missing " + HeartPrefab);
                return false;
            }

            var emojis = new GameObject[EmojiPrefabs.Length];
            for (int i = 0; i < EmojiPrefabs.Length; i++)
            {
                emojis[i] = AssetDatabase.LoadAssetAtPath<GameObject>(EmojiPrefabs[i]);
                if (emojis[i] == null)
                {
                    Debug.LogError("[Stage2SetupCli] missing " + EmojiPrefabs[i]);
                    return false;
                }
            }

            var rewards = LoadRewards();
            if (rewards == null) return false;

            AssignStillsToRewards(rewards);
            DisableTempGround();
            EnsureScoreBridge();
            RebuildCollectibles(level.transform, heart, emojis);
            RebuildPhotoPoints(level.transform, rewards);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Stage2SetupCli] Stage2 photo tutorial setup complete.");
            return true;
        }

        private static bool PlaceSubjectsOnly()
        {
            if (!ImportPhotoArt())
                return false;

            var scene = EditorSceneManager.GetActiveScene();
            if (scene.path != ScenePath)
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var rewards = LoadRewards();
            if (rewards == null) return false;
            AssignStillsToRewards(rewards);

            var photos = Object.FindObjectsByType<PhotoPoint>(FindObjectsSortMode.None);
            if (photos.Length == 0)
            {
                Debug.LogError("[Stage2SetupCli] no PhotoPoints — run stage2-setup first");
                return false;
            }

            var camFrames = LoadSprites(CameraSheetPath);
            for (int i = 0; i < photos.Length; i++)
            {
                var p = photos[i];
                int idx = IndexOfPhotoId(p.PointId);
                if (idx < 0) idx = Mathf.Clamp(i, 0, SubjectSheetPaths.Length - 1);

                EnsureSubjectOnPhoto(p.transform, idx, camFrames);
                if (idx < rewards.Length)
                {
                    var so = new SerializedObject(p);
                    so.FindProperty("_reward").objectReferenceValue = rewards[idx];
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[Stage2SetupCli] subjects placed on {photos.Length} PhotoPoints");
            return true;
        }

        private static PhotoPointRewardSO[] LoadRewards()
        {
            var rewards = new PhotoPointRewardSO[PhotoRewardPaths.Length];
            for (int i = 0; i < PhotoRewardPaths.Length; i++)
            {
                rewards[i] = AssetDatabase.LoadAssetAtPath<PhotoPointRewardSO>(PhotoRewardPaths[i]);
                if (rewards[i] == null)
                {
                    Debug.LogError("[Stage2SetupCli] missing " + PhotoRewardPaths[i]);
                    return null;
                }
            }

            return rewards;
        }

        private static int IndexOfPhotoId(string id)
        {
            for (int i = 0; i < PhotoIds.Length; i++)
                if (PhotoIds[i] == id) return i;
            return -1;
        }

        private static bool ImportPhotoArt()
        {
            for (int i = 0; i < SubjectSheetPaths.Length; i++)
            {
                if (!File.Exists(SubjectSheetPaths[i]))
                {
                    Debug.LogError("[Stage2SetupCli] missing sheet — run gen_photo_subjects.py: " + SubjectSheetPaths[i]);
                    return false;
                }

                SliceSheet(SubjectSheetPaths[i], SubjectCellSizes[i].x, SubjectCellSizes[i].y, 4);
            }

            if (!File.Exists(CameraSheetPath))
            {
                Debug.LogError("[Stage2SetupCli] missing " + CameraSheetPath);
                return false;
            }

            SliceSheet(CameraSheetPath, 32, 32, 2);

            for (int i = 0; i < PhotoStillPaths.Length; i++)
            {
                if (!File.Exists(PhotoStillPaths[i]))
                {
                    Debug.LogError("[Stage2SetupCli] missing still " + PhotoStillPaths[i]);
                    return false;
                }

                ImportSingleSprite(PhotoStillPaths[i]);
            }

            AssetDatabase.Refresh();
            return true;
        }

        private static void AssignStillsToRewards(PhotoPointRewardSO[] rewards)
        {
            for (int i = 0; i < rewards.Length; i++)
            {
                var sprites = LoadSprites(PhotoStillPaths[i]);
                if (sprites.Length == 0) continue;
                var so = new SerializedObject(rewards[i]);
                so.FindProperty("_photoSprite").objectReferenceValue = sprites[0];
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(rewards[i]);
            }

            AssetDatabase.SaveAssets();
        }

        private static void DisableTempGround()
        {
            var temp = GameObject.Find("TempGround");
            if (temp != null)
                temp.SetActive(false);
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
        }

        private static void RebuildCollectibles(Transform level, GameObject heart, GameObject[] emojis)
        {
            var existing = level.Find("Collectibles");
            if (existing != null)
                Object.DestroyImmediate(existing.gameObject);

            var root = new GameObject("Collectibles");
            root.transform.SetParent(level, false);

            var spec = StageMapDatabase.Get(2);
            var spawns = StageCollectibleLayout.Build(spec);
            int followN = 0, likeN = 0;
            for (int i = 0; i < spawns.Count; i++)
            {
                var s = spawns[i];
                if (s.Kind == CollectibleKind.Follow)
                {
                    SpawnPrefab(heart, root.transform, s.X, s.Y, $"Follow_{followN:00}");
                    followN++;
                }
                else if (s.Kind == CollectibleKind.Like)
                {
                    SpawnPrefab(emojis[likeN % emojis.Length], root.transform, s.X, s.Y, $"Like_{likeN:00}");
                    likeN++;
                }
            }

            Debug.Log($"[Stage2SetupCli] Collectibles Follow={followN} Like={likeN}");
        }

        private static void RebuildPhotoPoints(Transform level, PhotoPointRewardSO[] rewards)
        {
            foreach (var p in Object.FindObjectsByType<PhotoPoint>(FindObjectsSortMode.None))
            {
                if (p != null)
                    Object.DestroyImmediate(p.gameObject);
            }

            var folder = level.Find("PhotoPoints");
            if (folder != null)
                Object.DestroyImmediate(folder.gameObject);

            folder = new GameObject("PhotoPoints").transform;
            folder.SetParent(level, false);

            var spec = StageMapDatabase.Get(2);
            float[] xs = StageMapDatabase.GetPhotoPositions(spec);
            int count = Mathf.Min(xs.Length, rewards.Length, PhotoIds.Length);
            var camFrames = LoadSprites(CameraSheetPath);

            for (int i = 0; i < count; i++)
                CreatePhotoPoint(folder, xs[i], PhotoIds[i], rewards[i], i, camFrames);

            RebuildGoal(level);
            Debug.Log($"[Stage2SetupCli] PhotoPoints={count}");
        }

        private static void CreatePhotoPoint(
            Transform parent, float x, string id, PhotoPointRewardSO reward, int subjectIndex, Sprite[] camFrames)
        {
            var go = new GameObject(id);
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(x, 1.5f, 0f);

            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(4.5f, 4.5f);

            var available = CreateCameraVisual(go.transform, "Available", camFrames, new Color(1f, 1f, 1f, 1f), 2.35f);
            var used = CreateCameraVisual(go.transform, "Used", camFrames, new Color(0.55f, 0.55f, 0.6f, 0.55f), 2.35f);
            used.SetActive(false);

            var prompt = CreateMarker(go.transform, "Prompt", new Color(1f, 1f, 0.45f, 0.85f), 0.4f);
            prompt.transform.localPosition = new Vector3(0f, 2.6f, 0f);
            prompt.SetActive(false);

            var photo = go.AddComponent<PhotoPoint>();
            var so = new SerializedObject(photo);
            so.FindProperty("_pointId").stringValue = id;
            so.FindProperty("_reward").objectReferenceValue = reward;
            so.FindProperty("_availableVisual").objectReferenceValue = available;
            so.FindProperty("_usedVisual").objectReferenceValue = used;
            so.FindProperty("_promptVisual").objectReferenceValue = prompt;
            so.ApplyModifiedPropertiesWithoutUndo();

            go.AddComponent<PhotoPointFx>();
            EnsureSubjectOnPhoto(go.transform, subjectIndex, camFrames);
        }

        private static void EnsureSubjectOnPhoto(Transform photoRoot, int subjectIndex, Sprite[] camFrames)
        {
            var old = photoRoot.Find("Subject");
            if (old != null)
                Object.DestroyImmediate(old.gameObject);

            var frames = LoadSprites(SubjectSheetPaths[subjectIndex]);
            if (frames.Length == 0)
            {
                Debug.LogWarning("[Stage2SetupCli] no frames for " + SubjectSheetPaths[subjectIndex]);
                return;
            }

            var go = new GameObject("Subject");
            go.transform.SetParent(photoRoot, false);
            // sit on ground under floating camera
            go.transform.localPosition = new Vector3(0f, -1.35f, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = frames[0];
            sr.sortingOrder = 4;

            var anim = go.AddComponent<PhotoSubjectAnimator>();
            AssignAnimatorFrames(anim, frames, subjectIndex == 1 ? 3.5f : 5f);

            // refresh camera visuals if missing or sprite stripped
            if (camFrames != null && camFrames.Length > 0)
            {
                EnsureCameraVisual(photoRoot, "Available", camFrames, Color.white, 2.35f);
                EnsureCameraVisual(
                    photoRoot, "Used", camFrames, new Color(0.55f, 0.55f, 0.6f, 0.55f), 2.35f);
            }
        }

        private static void EnsureCameraVisual(
            Transform photoRoot, string name, Sprite[] frames, Color tint, float localY)
        {
            var existing = photoRoot.Find(name);
            if (existing == null)
            {
                CreateCameraVisual(photoRoot, name, frames, tint, localY);
                return;
            }

            var sr = existing.GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = existing.gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = frames[0];
            sr.color = tint;
            sr.sortingOrder = 8;
            existing.localPosition = new Vector3(0f, localY, 0f);

            if (frames.Length > 1)
            {
                var anim = existing.GetComponent<PhotoSubjectAnimator>();
                if (anim == null)
                    anim = existing.gameObject.AddComponent<PhotoSubjectAnimator>();
                AssignAnimatorFrames(anim, frames, 3f);
            }
        }

        private static void AssignAnimatorFrames(PhotoSubjectAnimator anim, Sprite[] frames, float fps)
        {
            var so = new SerializedObject(anim);
            var prop = so.FindProperty("_frames");
            prop.arraySize = frames.Length;
            for (int i = 0; i < frames.Length; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
            so.FindProperty("_fps").floatValue = fps;
            so.FindProperty("_bob").boolValue = true;
            so.ApplyModifiedPropertiesWithoutUndo();
            anim.SetFrames(frames, fps);
        }

        private static GameObject CreateCameraVisual(
            Transform parent, string name, Sprite[] frames, Color tint, float localY)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0f, localY, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = tint;
            sr.sortingOrder = 8;
            if (frames != null && frames.Length > 0)
                sr.sprite = frames[0];
            else
                sr.sprite = MakeWhiteSprite();

            if (frames != null && frames.Length > 1)
            {
                var anim = go.AddComponent<PhotoSubjectAnimator>();
                AssignAnimatorFrames(anim, frames, 3f);
            }

            return go;
        }

        private static GameObject CreateMarker(Transform parent, string name, Color color, float scale)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localScale = new Vector3(scale, scale, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeWhiteSprite();
            sr.color = color;
            sr.sortingOrder = 9;
            return go;
        }

        private static Sprite _whiteSprite;

        private static Sprite MakeWhiteSprite()
        {
            if (_whiteSprite != null) return _whiteSprite;
            var tex = Texture2D.whiteTexture;
            _whiteSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 4f);
            return _whiteSprite;
        }

        private static void RebuildGoal(Transform level)
        {
            foreach (var g in Object.FindObjectsByType<StageGoal>(FindObjectsSortMode.None))
            {
                if (g != null)
                    Object.DestroyImmediate(g.gameObject);
            }

            var goalsParent = level.Find("Goals");
            if (goalsParent == null)
            {
                var goParent = new GameObject("Goals");
                goParent.transform.SetParent(level, false);
                goalsParent = goParent.transform;
            }

            var go = new GameObject("Zone_Goal");
            go.transform.SetParent(goalsParent, false);
            go.transform.position = new Vector3(138f, 1.5f, 0f);
            go.transform.localScale = new Vector3(4f, 4f, 1f);

            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(1f, 2f);

            var goal = go.AddComponent<StageGoal>();
            var so = new SerializedObject(goal);
            so.FindProperty("_stageNumber").intValue = 2;
            so.ApplyModifiedPropertiesWithoutUndo();

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeWhiteSprite();
            sr.color = new Color(0.3f, 0.95f, 0.55f, 0.45f);
            sr.sortingOrder = 3;
        }

        private static void SpawnPrefab(GameObject prefab, Transform parent, float x, float y, string name)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            go.name = name;
            go.transform.position = new Vector3(x, y, 0f);
        }

        private static void SliceSheet(string path, int cellW, int cellH, int count)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError("[Stage2SetupCli] importer null " + path);
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.spritePixelsPerUnit = 32;
            importer.mipmapEnabled = false;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            settings.spritePivot = new Vector2(0.5f, 0f);
            importer.SetTextureSettings(settings);

            int cols = Mathf.Max(1, count);
            var metas = new List<SpriteMetaData>();
            string prefix = Path.GetFileNameWithoutExtension(path);
            for (int i = 0; i < count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                // sheets are single row
                metas.Add(new SpriteMetaData
                {
                    name = $"{prefix}_{i}",
                    rect = new Rect(col * cellW, 0, cellW, cellH),
                    alignment = (int)SpriteAlignment.BottomCenter,
                    pivot = new Vector2(0.5f, 0f)
                });
            }

            importer.spritesheet = metas.ToArray();
            importer.SaveAndReimport();
        }

        private static void ImportSingleSprite(string path)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.spritePixelsPerUnit = 32;
            importer.mipmapEnabled = false;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            settings.spritePivot = new Vector2(0.5f, 0f);
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }

        private static Sprite[] LoadSprites(string path)
        {
            var list = new List<Sprite>();
            foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (obj is Sprite sp)
                    list.Add(sp);
            }

            list.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            return list.ToArray();
        }
    }
}
#endif
