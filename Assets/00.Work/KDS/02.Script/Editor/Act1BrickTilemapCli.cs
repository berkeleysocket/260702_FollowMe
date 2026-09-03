using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowMe.KDS.Editor
{
    /// <summary>Unity CLI: unity run . -- -executeMethod FollowMe.KDS.Editor.Act1BrickTilemapCli.Setup</summary>
    public static class Act1BrickTilemapCli
    {
        private const string BaseDir = "Assets/00.Work/KDS/05.Asset/City_Modern/Act1_Tiles";
        private const string GroundSheet = BaseDir + "/Act1_Brick_Ground_32x32.png";
        private const string SlopeSheet = BaseDir + "/Act1_Brick_Slope_32x32.png";
        private const string TileDir = BaseDir + "/Tiles";
        private const string RuleDir = BaseDir + "/RuleTiles";

        public static void Setup()
        {
            SliceSheet(GroundSheet, 4, 2, 8);
            SliceSheet(SlopeSheet, 4, 4, 15);

            var ground = LoadSprites(GroundSheet);
            var slope = LoadSprites(SlopeSheet);
            if (ground.Length < 5 || slope.Length < 15)
            {
                Debug.LogError($"[Act1BrickTilemapCli] sprite count ground={ground.Length} slope={slope.Length}");
                EditorApplication.Exit(1);
                return;
            }

            EnsureDir(TileDir);
            EnsureDir(RuleDir);

            CreateGroundRuleTile(ground);
            CreateTile(TileDir + "/Act1_Brick_VerticalFill.asset", ground[5]);
            CreateTile(TileDir + "/Act1_Brick_Column.asset", ground[6]);

            string[] slopeNames =
            {
                "Act1_Brick_Slope_SteepUp", "Act1_Brick_Slope_SteepDown",
                "Act1_Brick_Slope_SteepUpLeft", "Act1_Brick_Slope_SteepDownLeft",
                "Act1_Brick_Slope_GentleUp", "Act1_Brick_Slope_GentleDown",
                "Act1_Brick_Step_Up", "Act1_Brick_Step_Down",
                "Act1_Brick_Corner_Outer", "Act1_Brick_Corner_Inner",
                "Act1_Brick_Platform_Left", "Act1_Brick_Platform_Center", "Act1_Brick_Platform_Right",
                "Act1_Brick_Underside", "Act1_Brick_FloatingSlab"
            };

            for (int i = 0; i < slopeNames.Length; i++)
                CreateTile(TileDir + "/" + slopeNames[i] + ".asset", slope[i]);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Act1BrickTilemapCli] Setup complete.");
            EditorApplication.Exit(0);
        }

        private static void EnsureDir(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static void SliceSheet(string path, int cols, int rows, int count)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError("[Act1BrickTilemapCli] missing texture: " + path);
                return;
            }

            const int size = 32;
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
            settings.spriteAlignment = (int)SpriteAlignment.BottomLeft;
            settings.spritePivot = Vector2.zero;
            importer.SetTextureSettings(settings);

            var metas = new List<SpriteMetaData>();
            string prefix = Path.GetFileNameWithoutExtension(path);
            for (int i = 0; i < count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                metas.Add(new SpriteMetaData
                {
                    name = $"{prefix}_{i}",
                    rect = new Rect(col * size, (rows - 1 - row) * size, size, size),
                    alignment = (int)SpriteAlignment.BottomLeft,
                    pivot = Vector2.zero
                });
            }

            importer.spritesheet = metas.ToArray();
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

        private static void CreateGroundRuleTile(Sprite[] sprites)
        {
            string assetPath = RuleDir + "/Act1_Brick_Ground_RuleTile.asset";
            var rule = AssetDatabase.LoadAssetAtPath<RuleTile>(assetPath);
            if (rule == null)
            {
                rule = ScriptableObject.CreateInstance<RuleTile>();
                AssetDatabase.CreateAsset(rule, assetPath);
            }

            const int A = 0, T = 1, N = 2;
            rule.m_DefaultSprite = sprites[0];
            rule.m_DefaultColliderType = Tile.ColliderType.Sprite;
            rule.m_TilingRules = new List<RuleTile.TilingRule>
            {
                MakeRule(sprites[3], A, A, A, N, A, A, A, A),
                MakeRule(sprites[4], A, A, A, A, N, A, A, A),
                MakeRuleRandom(new[] { sprites[0], sprites[1], sprites[2], sprites[7] }, A, A, A, T, T, A, A, A)
            };
            EditorUtility.SetDirty(rule);
        }

        private static RuleTile.TilingRule MakeRule(Sprite sprite, params int[] neighbors)
        {
            var rule = new RuleTile.TilingRule
            {
                m_Sprites = new[] { sprite },
                m_ColliderType = Tile.ColliderType.Sprite,
                m_Output = RuleTile.TilingRule.OutputSprite.Single,
                m_Neighbors = new List<int>(neighbors)
            };
            return rule;
        }

        private static RuleTile.TilingRule MakeRuleRandom(Sprite[] sprites, params int[] neighbors)
        {
            var rule = new RuleTile.TilingRule
            {
                m_Sprites = sprites,
                m_ColliderType = Tile.ColliderType.Sprite,
                m_Output = RuleTile.TilingRule.OutputSprite.Random,
                m_Neighbors = new List<int>(neighbors)
            };
            return rule;
        }

        private static void CreateTile(string path, Sprite sprite)
        {
            var tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, path);
            }

            tile.sprite = sprite;
            tile.colliderType = Tile.ColliderType.Sprite;
            EditorUtility.SetDirty(tile);
        }
    }
}
