#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// 타일·수집물·Goal·갈림 플랫폼 배치 (연동 없이 맵 레이아웃만).
    /// </summary>
    public static class StageMapLayoutBuilder
    {
        private const string FloorTilePath =
            "Assets/00.Work/KDS/05.Asset/City_Modern/Tiles/Floor/GandalfHardcore_city_tiles_32x32_5.asset";

        public static void ApplyLayout(StageMapSpec spec)
        {
            RebuildGround(spec);
            RebuildCollectibles(spec);
            RebuildForkPlatforms(spec);
            RebuildGoal(spec);
        }

        private static Transform GetLevelRoot(StageMapSpec spec)
        {
            var level = GameObject.Find($"Level_S{spec.Stage}");
            return level != null ? level.transform : null;
        }

        private static void RebuildGround(StageMapSpec spec)
        {
            var ground = GameObject.Find("TempGround");
            if (ground != null)
            {
                float width = spec.LengthX + 20f;
                ground.transform.position = new Vector3(spec.LengthX * 0.5f - 10f, -0.5f, 0f);
                ground.transform.localScale = new Vector3(width, 1f, 1f);
            }

            if (spec.Template != StageTemplate.CityStreet)
                return;

            var grid = GameObject.Find("Grid");
            if (grid == null) return;

            var tilemap = grid.transform.Find("Tilemap")?.GetComponent<Tilemap>();
            if (tilemap == null)
                tilemap = grid.GetComponentInChildren<Tilemap>();
            if (tilemap == null) return;

            var tile = AssetDatabase.LoadAssetAtPath<TileBase>(FloorTilePath);
            if (tile == null) return;

            tilemap.ClearAllTiles();
            int xMin = -5;
            int xMax = Mathf.CeilToInt(spec.LengthX + 10f);
            for (int x = xMin; x <= xMax; x++)
            {
                tilemap.SetTile(new Vector3Int(x, 0, 0), tile);
                tilemap.SetTile(new Vector3Int(x, -1, 0), tile);
            }

            tilemap.RefreshAllTiles();
        }

        private static void RebuildCollectibles(StageMapSpec spec)
        {
            Transform level = GetLevelRoot(spec);
            if (level == null) return;

            Transform root = level.Find("Collectibles");
            if (root == null)
            {
                var go = new GameObject("Collectibles");
                go.transform.SetParent(level, false);
                root = go.transform;
            }

            ClearChildren(root);

            var spawns = StageCollectibleLayout.Build(spec);
            int likeIndex = 0;
            int dailyIndex = 0;

            foreach (var spawn in spawns)
            {
                if (spawn.Kind == CollectibleKind.Like)
                {
                    likeIndex++;
                    CreateLike(root, spec, likeIndex, spawn);
                }
                else
                {
                    dailyIndex++;
                    CreateDaily(root, spec, dailyIndex, spawn);
                }
            }
        }

        private static void CreateLike(Transform root, StageMapSpec spec, int index, CollectibleSpawn spawn)
        {
            var go = new GameObject($"Like_{index:D2}");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(spawn.X, spawn.Y, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateQuadSprite();
            sr.color = new Color(1f, 0.35f, 0.55f, 1f);
            sr.sortingOrder = 5;
            go.transform.localScale = new Vector3(0.45f, 0.45f, 1f);

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;
            go.AddComponent<LikeCollectible>();
        }

        private static void CreateDaily(Transform root, StageMapSpec spec, int index, CollectibleSpawn spawn)
        {
            var go = new GameObject($"Daily_{spawn.DailyKind}_{index:D2}");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(spawn.X, spawn.Y, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateQuadSprite();
            sr.color = spawn.DailyKind switch
            {
                DailyKind.Meal => new Color(0.95f, 0.75f, 0.25f, 1f),
                DailyKind.Sleep => new Color(0.55f, 0.65f, 0.95f, 1f),
                _ => new Color(0.45f, 0.9f, 0.55f, 1f)
            };
            sr.sortingOrder = 5;
            go.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.7f, 0.7f);
            var daily = go.AddComponent<DailyCollectible>();
            var so = new SerializedObject(daily);
            so.FindProperty("_kind").enumValueIndex = (int)spawn.DailyKind;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void RebuildForkPlatforms(StageMapSpec spec)
        {
            if (spec.Forks <= 0) return;

            Transform level = GetLevelRoot(spec);
            if (level == null) return;

            Transform root = level.Find("Platforms");
            if (root == null)
            {
                var go = new GameObject("Platforms");
                go.transform.SetParent(level, false);
                root = go.transform;
            }

            ClearChildren(root);

            float platformY = StageCollectibleLayout.GetPlatformY(spec);
            float[] forkXs = StageCollectibleLayout.GetForkXs(spec);

            for (int i = 0; i < forkXs.Length; i++)
            {
                float x = forkXs[i];
                var go = new GameObject($"ForkPlatform_{i + 1}");
                go.transform.SetParent(root, false);
                go.transform.position = new Vector3(x + 2f, platformY, 0f);
                go.transform.localScale = new Vector3(14f, 0.6f, 1f);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = CreateQuadSprite();
                sr.color = new Color(0.55f, 0.58f, 0.62f, 0.85f);
                sr.sortingOrder = -2;

                var col = go.AddComponent<BoxCollider2D>();
                col.size = Vector2.one;
            }
        }

        private static void RebuildGoal(StageMapSpec spec)
        {
            Transform level = GetLevelRoot(spec);
            if (level == null) return;

            Transform root = level.Find("Goals");
            if (root == null)
            {
                var go = new GameObject("Goals");
                go.transform.SetParent(level, false);
                root = go.transform;
            }

            ClearChildren(root);

            float goalX = spec.LengthX - 5f;
            var goalGo = new GameObject("StageGoal");
            goalGo.transform.SetParent(root, false);
            goalGo.transform.position = new Vector3(goalX, 1.5f, 0f);
            goalGo.transform.localScale = new Vector3(4f, 4f, 1f);

            var col = goalGo.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1f, 2f);

            var sr = goalGo.AddComponent<SpriteRenderer>();
            sr.sprite = CreateQuadSprite();
            sr.color = new Color(0.3f, 0.95f, 0.55f, 0.55f);
            sr.sortingOrder = 3;

            var goal = goalGo.AddComponent<StageGoal>();
            var so = new SerializedObject(goal);
            so.FindProperty("_stageNumber").intValue = spec.Stage;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Sprite CreateQuadSprite()
        {
            var tex = Texture2D.whiteTexture;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 16f);
        }

        private static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}
#endif
