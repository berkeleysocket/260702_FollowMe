#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEngine;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Act3 촬영 명소(PhotoPoint) 프리팹 생성 — 불꽃축제.
    /// Live: unity command act3-photo-spots
    /// </summary>
    public static class Act3PhotoSpotPrefabCli
    {
        private const string ArtDir = "Assets/00.Work/KDS/05.Asset/City_Modern/Act3_Fireworks";
        private const string PrefabDir = "Assets/00.Work/KDS/06.Prefab/Act3_PhotoSpot";
        private const string RewardDir = "Assets/00.Work/KDS/08.SO/PhotoPoint";
        private const string CameraSheetPath =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Act1_Tiles/Act1_PhotoCamera_32x32.png";
        private const string OldPropPrefabDir = "Assets/00.Work/KDS/06.Prefab/Act3_Cafe";

        private struct SpotDef
        {
            public string Id;
            public string DisplayName;
            public string SubjectFile;
            public string StillFile;
            public int CellW;
            public int CellH;
            public long Like;
            public long Follow;
            public string[] Hashtags;
        }

        private static readonly SpotDef[] Spots =
        {
            new SpotDef
            {
                Id = "Photo_Act3_BridgeView",
                DisplayName = "한강 다리 난간",
                SubjectFile = "Act3_PhotoSubject_BridgeView_96x64.png",
                StillFile = "Act3_PhotoStill_BridgeView.png",
                CellW = 96, CellH = 64, Like = 14000, Follow = 550,
                Hashtags = new[] { "#한강다리", "#불꽃축제", "#야경" }
            },
            new SpotDef
            {
                Id = "Photo_Act3_FireworkPeak",
                DisplayName = "불꽃 피크 순간",
                SubjectFile = "Act3_PhotoSubject_FireworkPeak_64x96.png",
                StillFile = "Act3_PhotoStill_FireworkPeak.png",
                CellW = 64, CellH = 96, Like = 20000, Follow = 800,
                Hashtags = new[] { "#불꽃축제", "#절정", "#인생샷" }
            },
            new SpotDef
            {
                Id = "Photo_Act3_PhoneCrowd",
                DisplayName = "폰으로 보는 인파",
                SubjectFile = "Act3_PhotoSubject_PhoneCrowd_96x64.png",
                StillFile = "Act3_PhotoStill_PhoneCrowd.png",
                CellW = 96, CellH = 64, Like = 12000, Follow = 480,
                Hashtags = new[] { "#폰속축제", "#인파", "#팔로우미" }
            },
            new SpotDef
            {
                Id = "Photo_Act3_FoodTruck",
                DisplayName = "축제 푸드트럭",
                SubjectFile = "Act3_PhotoSubject_FoodTruck_96x64.png",
                StillFile = "Act3_PhotoStill_FoodTruck.png",
                CellW = 96, CellH = 64, Like = 11000, Follow = 420,
                Hashtags = new[] { "#야시장", "#푸드트럭", "#축제먹거리" }
            },
            new SpotDef
            {
                Id = "Photo_Act3_DroneShow",
                DisplayName = "드론 라이트쇼",
                SubjectFile = "Act3_PhotoSubject_DroneShow_96x64.png",
                StillFile = "Act3_PhotoStill_DroneShow.png",
                CellW = 96, CellH = 64, Like = 15000, Follow = 600,
                Hashtags = new[] { "#드론쇼", "#라이트쇼", "#한강" }
            },
            new SpotDef
            {
                Id = "Photo_Act3_PhotoBooth",
                DisplayName = "셀프캠 포토부스",
                SubjectFile = "Act3_PhotoSubject_PhotoBooth_64x96.png",
                StillFile = "Act3_PhotoStill_PhotoBooth.png",
                CellW = 64, CellH = 96, Like = 10000, Follow = 400,
                Hashtags = new[] { "#포토부스", "#셀프캠", "#인증샷" }
            },
            new SpotDef
            {
                Id = "Photo_Act3_PierBoat",
                DisplayName = "선착장 유람선",
                SubjectFile = "Act3_PhotoSubject_PierBoat_96x64.png",
                StillFile = "Act3_PhotoStill_PierBoat.png",
                CellW = 96, CellH = 64, Like = 13000, Follow = 500,
                Hashtags = new[] { "#유람선", "#선착장", "#강바람" }
            },
            new SpotDef
            {
                Id = "Photo_Act3_EmptyRiver",
                DisplayName = "축제 끝난 강변",
                SubjectFile = "Act3_PhotoSubject_EmptyRiver_96x64.png",
                StillFile = "Act3_PhotoStill_EmptyRiver.png",
                CellW = 96, CellH = 64, Like = 9000, Follow = 350,
                Hashtags = new[] { "#끝난축제", "#공허", "#강변" }
            },
            new SpotDef
            {
                Id = "Photo_Act3_BridgeLights",
                DisplayName = "다리 야간 조명",
                SubjectFile = "Act3_PhotoSubject_BridgeLights_96x64.png",
                StillFile = "Act3_PhotoStill_BridgeLights.png",
                CellW = 96, CellH = 64, Like = 12500, Follow = 470,
                Hashtags = new[] { "#다리조명", "#네온야경", "#한강" }
            },
            new SpotDef
            {
                Id = "Photo_Act3_WaterReflect",
                DisplayName = "강물 불꽃 반사",
                SubjectFile = "Act3_PhotoSubject_WaterReflect_96x64.png",
                StillFile = "Act3_PhotoStill_WaterReflect.png",
                CellW = 96, CellH = 64, Like = 16000, Follow = 650,
                Hashtags = new[] { "#강물반사", "#불꽃", "#거울샷" }
            },
        };

        [CliCommand("act3-photo-spots", "Build Act3 photo landmark prefabs (PhotoPoint + subject + reward)")]
        public static int BuildFromPipeline()
        {
            return Build() ? 0 : 1;
        }

        [MenuItem("FollowMe/KDS/Build Act3 Photo Spots")]
        public static void BuildFromMenu()
        {
            Build();
        }

        public static bool Build()
        {
            EnsureFolder("Assets/00.Work/KDS/06.Prefab");
            EnsureFolder(PrefabDir);
            EnsureFolder(RewardDir);

            // Act3_Cafe 잔재가 있을 때만 정리 (Act2와 동일 패턴, 존재하지 않으면 no-op)
            if (AssetDatabase.IsValidFolder(OldPropPrefabDir))
            {
                AssetDatabase.DeleteAsset(OldPropPrefabDir);
                Debug.Log("[Act3PhotoSpotPrefabCli] removed old Act3_Cafe prop prefabs");
            }

            if (!ImportArt())
                return false;

            Sprite[] camFrames = LoadSprites(CameraSheetPath);
            int ok = 0;

            foreach (var spot in Spots)
            {
                string subjectPath = $"{ArtDir}/{spot.SubjectFile}";
                string stillPath = $"{ArtDir}/{spot.StillFile}";
                Sprite[] frames = LoadSprites(subjectPath);
                Sprite[] stills = LoadSprites(stillPath);
                if (frames.Length == 0)
                {
                    Debug.LogWarning("[Act3PhotoSpotPrefabCli] no frames: " + subjectPath);
                    continue;
                }

                PhotoPointRewardSO reward = EnsureReward(spot, stills.Length > 0 ? stills[0] : null);
                string prefabPath = $"{PrefabDir}/{spot.Id}.prefab";

                var root = new GameObject(spot.Id);
                try
                {
                    var col = root.AddComponent<CircleCollider2D>();
                    col.isTrigger = true;
                    col.radius = 1.35f;

                    var photo = root.AddComponent<PhotoPoint>();
                    var available = CreateCameraVisual(root.transform, "Available", camFrames, Color.white, 2.35f);
                    var used = CreateCameraVisual(root.transform, "Used", camFrames, new Color(0.55f, 0.55f, 0.6f, 0.55f), 2.35f);
                    used.SetActive(false);
                    var prompt = CreateMarker(root.transform, "Prompt", new Color(1f, 0.45f, 0.72f, 0.85f), 0.35f);
                    prompt.SetActive(false);

                    var subject = new GameObject("Subject");
                    subject.transform.SetParent(root.transform, false);
                    subject.transform.localPosition = new Vector3(0f, -0.15f, 0f);
                    var sr = subject.AddComponent<SpriteRenderer>();
                    sr.sprite = frames[0];
                    sr.sortingOrder = 4;
                    var anim = subject.AddComponent<PhotoSubjectAnimator>();
                    float fps = 6.5f;
                    if (spot.Id.Contains("Firework") || spot.Id.Contains("Drone") || spot.Id.Contains("BridgeLights"))
                        fps = 7.5f;
                    else if (spot.Id.Contains("Empty"))
                        fps = 4.5f;
                    else if (spot.Id.Contains("PhotoBooth") || spot.Id.Contains("WaterReflect"))
                        fps = 7f;
                    AssignAnimatorFrames(anim, frames, fps);

                    var so = new SerializedObject(photo);
                    so.FindProperty("_pointId").stringValue = spot.Id;
                    so.FindProperty("_reward").objectReferenceValue = reward;
                    so.FindProperty("_availableVisual").objectReferenceValue = available;
                    so.FindProperty("_usedVisual").objectReferenceValue = used;
                    so.FindProperty("_promptVisual").objectReferenceValue = prompt;
                    so.ApplyModifiedPropertiesWithoutUndo();

                    root.AddComponent<PhotoPointFx>();

                    PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                    ok++;
                }
                finally
                {
                    Object.DestroyImmediate(root);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Act3PhotoSpotPrefabCli] built {ok}/{Spots.Length} photo spots → {PrefabDir}");
            return ok == Spots.Length;
        }

        private static bool ImportArt()
        {
            foreach (var spot in Spots)
            {
                string subjectPath = $"{ArtDir}/{spot.SubjectFile}";
                if (!File.Exists(Path.GetFullPath(subjectPath)))
                {
                    Debug.LogError("[Act3PhotoSpotPrefabCli] missing subject — run gen_act3_photo_spots.py: " + subjectPath);
                    return false;
                }

                SliceSheet(subjectPath, spot.CellW, spot.CellH, 4);

                string stillPath = $"{ArtDir}/{spot.StillFile}";
                if (!File.Exists(Path.GetFullPath(stillPath)))
                {
                    Debug.LogError("[Act3PhotoSpotPrefabCli] missing still: " + stillPath);
                    return false;
                }

                ImportSingleSprite(stillPath);
            }

            if (File.Exists(Path.GetFullPath(CameraSheetPath)))
                SliceSheet(CameraSheetPath, 32, 32, 2);

            AssetDatabase.Refresh();
            return true;
        }

        private static PhotoPointRewardSO EnsureReward(SpotDef spot, Sprite still)
        {
            string path = $"{RewardDir}/PhotoReward_{spot.Id.Replace("Photo_Act3_", "")}.asset";
            var reward = AssetDatabase.LoadAssetAtPath<PhotoPointRewardSO>(path);
            if (reward == null)
            {
                reward = ScriptableObject.CreateInstance<PhotoPointRewardSO>();
                AssetDatabase.CreateAsset(reward, path);
            }

            var so = new SerializedObject(reward);
            so.FindProperty("_displayName").stringValue = spot.DisplayName;
            so.FindProperty("_likeBonus").longValue = spot.Like;
            so.FindProperty("_followBonus").longValue = spot.Follow;
            so.FindProperty("_holdSeconds").floatValue = 0.9f;
            so.FindProperty("_oneShot").boolValue = true;
            if (still != null)
                so.FindProperty("_photoSprite").objectReferenceValue = still;

            var tags = so.FindProperty("_hashtags");
            tags.arraySize = spot.Hashtags.Length;
            for (int i = 0; i < spot.Hashtags.Length; i++)
                tags.GetArrayElementAtIndex(i).stringValue = spot.Hashtags[i];

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(reward);
            return reward;
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
            so.FindProperty("_bobAmount").floatValue = 0.06f;
            so.FindProperty("_bobSpeed").floatValue = 2.6f;
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
            sr.sprite = Texture2D.whiteTexture != null
                ? Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f)
                : null;
            sr.color = color;
            sr.sortingOrder = 9;
            return go;
        }

        private static void SliceSheet(string path, int cellW, int cellH, int count)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;

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

            var metas = new List<SpriteMetaData>();
            string prefix = Path.GetFileNameWithoutExtension(path);
            for (int i = 0; i < count; i++)
            {
                metas.Add(new SpriteMetaData
                {
                    name = $"{prefix}_{i}",
                    rect = new Rect(i * cellW, 0, cellW, cellH),
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
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spritePivot = new Vector2(0.5f, 0.5f);
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

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            string name = Path.GetFileName(path);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name)) return;
            if (!AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
