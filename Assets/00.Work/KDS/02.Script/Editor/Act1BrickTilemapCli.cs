using System.Collections.Generic;
using System.IO;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Batch: unity run . -- -executeMethod FollowMe.KDS.Editor.Act1BrickTilemapCli.Setup
    /// Live editor: unity command act1-brick-setup
    /// </summary>
    public static class Act1BrickTilemapCli
    {
        private const string BaseDir = "Assets/00.Work/KDS/05.Asset/City_Modern/Act1_Tiles";
        private const string GroundSheet = BaseDir + "/Act1_Brick_Ground_32x32.png";
        private const string SlopeSheet = BaseDir + "/Act1_Brick_Slope_32x32.png";
        private const string TileDir = BaseDir + "/Tiles";
        private const string RuleDir = BaseDir + "/RuleTiles";

        public static void Setup()
        {
            if (!RebuildBrickTiles())
            {
                EditorApplication.Exit(1);
                return;
            }

            EditorApplication.Exit(0);
        }

        [CliCommand("act1-brick-setup", "Rebuild Act1 brick ground/slope tiles and RuleTile assets")]
        public static int SetupFromPipeline()
        {
            return RebuildBrickTiles() ? 0 : 1;
        }

        private static bool RebuildBrickTiles()
        {
            SliceSheet(GroundSheet, 4, 3, 12);
            SliceSheet(SlopeSheet, 4, 4, 15);

            var ground = LoadSprites(GroundSheet);
            var slope = LoadSprites(SlopeSheet);
            if (ground.Length < 12 || slope.Length < 15)
            {
                Debug.LogError($"[Act1BrickTilemapCli] sprite count ground={ground.Length} slope={slope.Length}");
                return false;
            }

            EnsureDir(TileDir);
            EnsureDir(RuleDir);

            CreateGroundRuleTile(ground);
            CreateTile(TileDir + "/Act1_Brick_Ground_Left.asset", ground[3]);
            CreateTile(TileDir + "/Act1_Brick_Ground_Right.asset", ground[4]);
            CreateTile(TileDir + "/Act1_Brick_VerticalFill.asset", ground[5]);
            CreateTile(TileDir + "/Act1_Brick_Column.asset", ground[6]);
            CreateTile(TileDir + "/Act1_Brick_Edge_Left.asset", ground[8]);
            CreateTile(TileDir + "/Act1_Brick_Edge_Right.asset", ground[9]);
            CreateTile(TileDir + "/Act1_Brick_Edge_LeftBot.asset", ground[10]);
            CreateTile(TileDir + "/Act1_Brick_Edge_RightBot.asset", ground[11]);

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
            return true;
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

            // Neighbor order: UL, Up, UR, Left, Right, DL, Down, DR
            // A=Don'tCare T=This N=NotThis
            const int A = 0, T = 1, N = 2;
            var surfaceFills = new[] { sprites[0], sprites[1], sprites[2], sprites[7] };

            rule.m_DefaultSprite = sprites[0];
            rule.m_DefaultColliderType = Tile.ColliderType.Sprite;
            rule.m_TilingRules = new List<RuleTile.TilingRule>
            {
                // Left column open (UL / Left / DL) — top / mid / bot
                MakeRule(sprites[3], N, N, A, N, T, N, A, A),
                MakeRule(sprites[8], N, T, A, N, T, N, T, A),
                MakeRule(sprites[10], N, T, A, N, T, N, N, A),
                // Right column open (UR / Right / DR)
                MakeRule(sprites[4], A, N, N, T, N, A, A, N),
                MakeRule(sprites[9], A, T, N, T, N, A, T, N),
                MakeRule(sprites[11], A, T, N, T, N, A, N, N),
                // Softer left/right caps (only cardinal left/right empty)
                MakeRule(sprites[3], A, N, A, N, T, A, A, A),
                MakeRule(sprites[4], A, N, A, T, N, A, A, A),
                MakeRule(sprites[8], A, T, A, N, T, A, T, A),
                MakeRule(sprites[9], A, T, A, T, N, A, T, A),
                // Interior — surrounded on all four cardinals
                MakeRule(sprites[5], A, T, A, T, T, A, T, A),
                // Stacked body — tile above, open below
                MakeRule(sprites[5], A, T, A, T, T, A, N, A),
                // Bottom edge under platform
                MakeRule(sprites[5], A, T, A, N, T, A, N, A),
                MakeRule(sprites[5], A, T, A, T, N, A, N, A),
                // Walk surface row — open above
                MakeRuleRandom(surfaceFills, A, N, A, T, T, A, A, A),
                // Horizontal fill fallback
                MakeRuleRandom(surfaceFills, A, A, A, T, T, A, A, A)
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
