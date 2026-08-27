#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// StageMapDatabase 템플릿(T1~T4)·Act5 채도↓에 맞춰
    /// 카메라/패럴랙스/타일맵 틴트·배경 스프라이트·소품을 일괄 적용.
    /// </summary>
    public static class StageVisualThemeApplier
    {
        private const string SceneFolder = "Assets/00.Work/KDS/01.Scene";

        private const string CitySky =
            "Assets/00.Work/KDS/05.Asset/City_Modern/GandalfHardcore City Tiles/City background sky.png";
        private const string CityFar =
            "Assets/00.Work/KDS/05.Asset/City_Modern/GandalfHardcore City Tiles/City background layer2.png";
        private const string CityNear =
            "Assets/00.Work/KDS/05.Asset/City_Modern/GandalfHardcore City Tiles/City background layer1.png";

        private const string NeonSkyline =
            "Assets/00.Work/KDS/05.Asset/City_Neon/WarpedCity_ansimuz/warped city files/ENVIRONMENT/background/skyline-a.png";
        private const string NeonBuildings =
            "Assets/00.Work/KDS/05.Asset/City_Neon/WarpedCity_ansimuz/warped city files/ENVIRONMENT/background/buildings-bg.png";
        private const string NeonNear =
            "Assets/00.Work/KDS/05.Asset/City_Neon/WarpedCity_ansimuz/warped city files/ENVIRONMENT/background/near-buildings-bg.png";
        private const string NeonBanner =
            "Assets/00.Work/KDS/05.Asset/City_Neon/WarpedCity_ansimuz/warped city files/ENVIRONMENT/props/banner-neon/banner-neon-1.png";

        private const string CafeSheet =
            "Assets/00.Work/KDS/05.Asset/City_Cafe/coffeeshopstuff.png";
        private const string FireworkSprite =
            "Assets/00.Work/KDS/05.Asset/VFX_Fireworks/Stealthix_Fires/Small_Fireball_10x26.png";

        private const float ParallaxStep = 32f;

        [MenuItem("FollowMe/KDS/Apply Act Visual Themes (S1-S16)")]
        public static void ApplyAllStageVisuals()
        {
            ApplyStages(1, 16, "[StageVisualTheme] S1~S16 비주얼 테마 적용 시작");
        }

        [MenuItem("FollowMe/KDS/Apply Cafe Street Themes (S4-S8)")]
        public static void ApplyCafeStreetThemes()
        {
            ApplyStages(4, 8, "[StageVisualTheme] S4~S8 City_Cafe 거리 테마 적용");
        }

        private static void ApplyStages(int from, int to, string header)
        {
            var log = new System.Text.StringBuilder();
            log.AppendLine(header);

            for (int stage = from; stage <= to; stage++)
            {
                try
                {
                    string path = $"{SceneFolder}/Stage{stage} Scene.unity";
                    if (!File.Exists(path))
                    {
                        Debug.LogError($"[StageVisualTheme] 씬 없음: {path}");
                        continue;
                    }

                    EditorSceneManager.OpenScene(path);
                    StageMapSpec spec = StageMapDatabase.Get(stage);
                    ApplyToOpenScene(spec);
                    var scene = SceneManager.GetActiveScene();
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene, path);
                    log.AppendLine($"  ✓ S{stage:D2} {spec.Template} X={spec.LengthX}");
                    Debug.Log($"[StageVisualTheme] Stage {stage} 완료 ({spec.Template})");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[StageVisualTheme] Stage {stage} 실패: {ex.Message}\n{ex.StackTrace}");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log(log.ToString());
        }

        public static void ApplyToOpenScene(StageMapSpec spec)
        {
            EnsurePixelImportSettings();
            Theme theme = BuildTheme(spec);
            ApplyCamera(theme);
            Transform bg = EnsureBackgroundRoot(spec);
            RebuildParallax(bg, spec, theme);
            ApplyTilemapTint(theme);
            RebuildThemeProps(spec, theme);
        }

        private static Theme BuildTheme(StageMapSpec spec)
        {
            switch (spec.Template)
            {
                case StageTemplate.CafeAlley:
                {
                    // Near는 City 건물 끄고 City_Cafe 파사드로 거리 구성
                    var sky = LoadFirstSprite(CitySky);
                    var far = LoadFirstSprite(CityFar);
                    return new Theme
                    {
                        CamBg = Hex("FFE2B0"),
                        SkyTint = new Color(1f, 0.94f, 0.8f, 1f),
                        FarTint = new Color(1f, 0.9f, 0.78f, 0.85f),
                        NearTint = Color.white,
                        TileTint = new Color(1f, 0.93f, 0.84f, 1f),
                        SkySprite = sky,
                        FarSprite = far,
                        NearSprite = null,
                        SkyScale = 1.15f,
                        FarScale = 1.05f,
                        NearScale = 1f,
                        SkyY = 4.5f,
                        FarY = 1.4f,
                        NearY = 0.2f,
                        PropMode = PropMode.Cafe,
                        HideSky = false
                    };
                }

                case StageTemplate.Fireworks:
                {
                    var sky = LoadFirstSprite(NeonSkyline) ?? LoadFirstSprite(CitySky);
                    var far = LoadFirstSprite(NeonBuildings) ?? LoadFirstSprite(CityFar);
                    var near = LoadFirstSprite(NeonNear) ?? LoadFirstSprite(CityNear);
                    return new Theme
                    {
                        CamBg = Hex("0C1024"),
                        SkyTint = new Color(0.55f, 0.45f, 0.85f, 1f),
                        FarTint = new Color(0.75f, 0.6f, 0.95f, 1f),
                        NearTint = Color.white,
                        TileTint = new Color(0.75f, 0.7f, 0.85f, 1f),
                        SkySprite = sky,
                        FarSprite = far,
                        NearSprite = near,
                        SkyScale = FitScale(sky, 34f, 8f),
                        FarScale = FitScale(far, 34f, 8f),
                        NearScale = FitScale(near, 32f, 8f),
                        SkyY = 3.5f,
                        FarY = 1.5f,
                        NearY = 0.4f,
                        PropMode = PropMode.Fireworks,
                        HideSky = false
                    };
                }

                case StageTemplate.Subway:
                {
                    // 네온시티(T3)와 구분 — City_Modern을 강하게 암화 + 천장 암막
                    var far = LoadFirstSprite(CityFar);
                    var near = LoadFirstSprite(CityNear);
                    return new Theme
                    {
                        CamBg = Hex("0A0E14"),
                        SkyTint = new Color(0.08f, 0.1f, 0.12f, 1f),
                        FarTint = new Color(0.22f, 0.26f, 0.32f, 1f),
                        NearTint = new Color(0.28f, 0.32f, 0.36f, 1f),
                        TileTint = new Color(0.4f, 0.45f, 0.5f, 1f),
                        SkySprite = null,
                        FarSprite = far,
                        NearSprite = near,
                        SkyScale = 1f,
                        FarScale = 1.05f,
                        NearScale = 1f,
                        SkyY = 4.5f,
                        FarY = 1.2f,
                        NearY = 0.2f,
                        PropMode = PropMode.Subway,
                        HideSky = true
                    };
                }

                default:
                {
                    bool desat = spec.Stage >= 15;
                    var sky = LoadFirstSprite(CitySky);
                    var far = LoadFirstSprite(CityFar);
                    var near = LoadFirstSprite(CityNear);
                    return new Theme
                    {
                        CamBg = desat ? Hex("9BB0C0") : Hex("7EC8E3"),
                        SkyTint = desat ? new Color(0.78f, 0.82f, 0.86f, 1f) : Color.white,
                        FarTint = desat ? new Color(0.72f, 0.76f, 0.8f, 1f) : Color.white,
                        NearTint = desat ? new Color(0.7f, 0.74f, 0.78f, 1f) : Color.white,
                        TileTint = desat ? new Color(0.78f, 0.8f, 0.82f, 1f) : Color.white,
                        SkySprite = sky,
                        FarSprite = far,
                        NearSprite = near,
                        SkyScale = 1.15f,
                        FarScale = 1.05f,
                        NearScale = 1f,
                        SkyY = 4.5f,
                        FarY = 1.2f,
                        NearY = 0.2f,
                        PropMode = desat ? PropMode.Sparse : PropMode.City,
                        HideSky = false
                    };
                }
            }
        }

        private static void ApplyCamera(Theme theme)
        {
            var cam = Camera.main;
            if (cam == null) return;
            cam.backgroundColor = theme.CamBg;
            cam.clearFlags = CameraClearFlags.SolidColor;
        }

        private static Transform EnsureBackgroundRoot(StageMapSpec spec)
        {
            var level = GameObject.Find($"Level_S{spec.Stage}");
            if (level == null)
                level = GameObject.Find("Level_S1") ?? new GameObject($"Level_S{spec.Stage}");

            var bg = level.transform.Find("Background");
            if (bg != null) return bg;

            var go = new GameObject("Background");
            go.transform.SetParent(level.transform, false);
            return go.transform;
        }

        private static void EnsurePixelImportSettings()
        {
            string[] paths =
            {
                NeonSkyline, NeonBuildings, NeonNear, NeonBanner, CafeSheet
            };

            foreach (var path in paths)
            {
                if (!File.Exists(path)) continue;
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool dirty = false;
                if (Mathf.Abs(importer.spritePixelsPerUnit - 32f) > 0.01f)
                {
                    importer.spritePixelsPerUnit = 32f;
                    dirty = true;
                }

                if (importer.filterMode != FilterMode.Point)
                {
                    importer.filterMode = FilterMode.Point;
                    dirty = true;
                }

                if (dirty)
                    importer.SaveAndReimport();
            }
        }

        private static void RebuildParallax(Transform bg, StageMapSpec spec, Theme theme)
        {
            ClearChildren(bg);

            float start = -8f;
            float end = Mathf.Max(spec.LengthX + 8f, 120f);
            float step = ParallaxStep;
            if (theme.FarSprite != null)
            {
                float farW = theme.FarSprite.rect.width / theme.FarSprite.pixelsPerUnit * theme.FarScale;
                if (farW > 1f && farW < step)
                    step = farW * 0.92f;
            }

            int count = Mathf.CeilToInt((end - start) / step) + 1;

            for (int i = 0; i < count; i++)
            {
                float x = start + i * step;
                if (!theme.HideSky && theme.SkySprite != null)
                    CreateLayer(bg, $"Sky_{x:0}", theme.SkySprite, new Vector3(x, theme.SkyY, 0f), theme.SkyTint, -30, theme.SkyScale);
                if (theme.FarSprite != null)
                    CreateLayer(bg, $"Far_{x:0}", theme.FarSprite, new Vector3(x, theme.FarY, 0f), theme.FarTint, -20, theme.FarScale);
                if (theme.NearSprite != null)
                    CreateLayer(bg, $"Near_{x:0}", theme.NearSprite, new Vector3(x, theme.NearY, 0f), theme.NearTint, -10, theme.NearScale);
            }
        }

        private static float FitScale(Sprite sprite, float targetWidth, float fallback)
        {
            if (sprite == null) return fallback;
            float worldW = sprite.rect.width / sprite.pixelsPerUnit;
            float worldH = sprite.rect.height / sprite.pixelsPerUnit;
            if (worldW < 0.01f || worldH < 0.01f) return fallback;

            const float targetH = 11.5f;
            float byW = targetWidth / worldW;
            float byH = targetH / worldH;
            return Mathf.Min(byW, byH);
        }

        private static void CreateLayer(
            Transform parent, string name, Sprite sprite, Vector3 pos, Color tint, int order, float scale)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(scale, scale, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = tint;
            sr.sortingOrder = order;
        }

        private static void ApplyTilemapTint(Theme theme)
        {
            var maps = UnityEngine.Object.FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
            foreach (var map in maps)
                map.color = theme.TileTint;
        }

        private static void RebuildThemeProps(StageMapSpec spec, Theme theme)
        {
            Transform level = GameObject.Find($"Level_S{spec.Stage}")?.transform;
            if (level == null) return;

            Transform props = level.Find("ThemeProps");
            if (props == null)
            {
                var go = new GameObject("ThemeProps");
                go.transform.SetParent(level, false);
                props = go.transform;
            }

            ClearChildren(props);

            switch (theme.PropMode)
            {
                case PropMode.Cafe:
                    PlaceCafeStreet(props, spec);
                    PlaceAlleyShade(props, spec);
                    break;
                case PropMode.Fireworks:
                    PlaceNeonBanners(props, spec);
                    PlaceFireworks(props, spec);
                    break;
                case PropMode.Subway:
                    PlaceSubwayDarkness(props, spec);
                    break;
                case PropMode.Sparse:
                    break;
                default:
                    PlaceCityNeonAccent(props, spec);
                    break;
            }
        }

        /// <summary>
        /// City_Cafe 시트로 구간별 카페거리 구성.
        /// Intro/Breath/Goal=밝은 가게 · Pressure/Setpiece=골목(벽·그늘·가게↓) · Teach=플랫폼.
        /// </summary>
        private static void PlaceCafeStreet(Transform parent, StageMapSpec spec)
        {
            Sprite[] cafe = LoadAllSprites(CafeSheet);
            if (cafe == null || cafe.Length < 4)
            {
                Debug.LogWarning("[StageVisualTheme] City_Cafe 시트 부족 — 카페거리 스킵");
                return;
            }

            Sprite facade = cafe[0];
            Sprite teaBar = cafe[1];
            Sprite lampA = cafe[2];
            Sprite lampB = cafe.Length > 3 ? cafe[3] : cafe[2];
            Sprite brick = cafe.Length > 36 ? cafe[36] : null;
            Sprite table = cafe.Length > 15 ? cafe[15] : null;
            Sprite stool = cafe.Length > 7 ? cafe[7] : null;
            Sprite fence = cafe.Length > 13 ? cafe[13] : null;
            Sprite trash = cafe.Length > 23 ? cafe[23] : null;
            Sprite fridge = cafe.Length > 39 ? cafe[39] : null;

            // 스테이지별 밀도 오프셋 (S4 여유 → S8 빽빽)
            float gapExtra = Mathf.Lerp(4.2f, 2.6f, (spec.Stage - 4) / 4f);

            PlaceCafeShopsInRange(parent, spec, facade, teaBar, lampA, lampB, brick, table, stool, trash,
                8f, spec.TeachEnd, gapExtra, bright: true, startIndex: 0);
            PlaceCafeShopsInRange(parent, spec, facade, teaBar, lampA, lampB, brick, table, stool, trash,
                spec.BreathEnd, spec.GoalEnd, gapExtra + 0.4f, bright: true, startIndex: 100);

            // Pressure~Setpiece: 골목 — 벽돌 벽 밀집 + 가게 거의 없음
            PlaceCafeAlley(parent, spec, brick, lampA, fridge);

            // Teach: 점프 플랫폼 (벽돌 비주얼)
            PlaceCafePlatforms(parent, spec, brick);

            // 포토존 장식 (스펙 좌표)
            PlaceCafePhotoDressing(parent, spec, facade, teaBar, table, fridge);

            if (fence != null)
            {
                float fx = 4f;
                int fi = 0;
                float fScale = 2.5f;
                float fStep = SpriteHalfWidth(fence, fScale) * 2f * 0.95f;
                while (fx < spec.TeachEnd)
                {
                    CreateProp(parent, $"CafeFence_{fi}", fence,
                        new Vector3(fx, GroundedY(fence, fScale) * 0.85f, 0f),
                        new Color(1f, 0.96f, 0.92f, 0.95f), -4, fScale);
                    fx += Mathf.Max(1.2f, fStep);
                    fi++;
                }
            }
        }

        private static void PlaceCafeShopsInRange(
            Transform parent, StageMapSpec spec,
            Sprite facade, Sprite teaBar, Sprite lampA, Sprite lampB,
            Sprite brick, Sprite table, Sprite stool, Sprite trash,
            float xStart, float xEnd, float gapExtra, bool bright, int startIndex)
        {
            const float facadeScale = 2.35f;
            const float teaScale = 2.1f;
            const float lampScale = 2.4f;
            const float propScale = 2.2f;

            float x = xStart;
            int shopIndex = startIndex;
            while (x < xEnd - 6f && x < spec.LengthX - 4f)
            {
                bool useTea = shopIndex % 3 == 1;
                Sprite shop = useTea ? teaBar : facade;
                float scale = useTea ? teaScale : facadeScale;
                Color tint = bright ? Color.white : new Color(0.75f, 0.78f, 0.82f, 1f);

                CreateProp(parent, $"CafeShop_{shopIndex}", shop,
                    new Vector3(x, GroundedY(shop, scale), 0f), tint, -8, scale);

                if (brick != null && shopIndex % 2 == 0)
                {
                    float wallX = x + SpriteHalfWidth(shop, scale) + 1.1f;
                    if (wallX < xEnd)
                    {
                        CreateProp(parent, $"CafeBrick_{shopIndex}", brick,
                            new Vector3(wallX, GroundedY(brick, 2.6f), 0f),
                            new Color(1f, 0.95f, 0.9f, 1f), -9, 2.6f);
                    }
                }

                float lampX = x + SpriteHalfWidth(shop, scale) + 0.35f;
                Sprite lamp = shopIndex % 2 == 0 ? lampA : lampB;
                CreateProp(parent, $"CafeLamp_{shopIndex}", lamp,
                    new Vector3(lampX, GroundedY(lamp, lampScale), 0f), tint, -3, lampScale);

                if (table != null)
                {
                    CreateProp(parent, $"CafeTable_{shopIndex}", table,
                        new Vector3(x - SpriteHalfWidth(shop, scale) * 0.3f, GroundedY(table, propScale), 0f),
                        tint, -2, propScale);
                }

                if (stool != null)
                {
                    CreateProp(parent, $"CafeStool_{shopIndex}", stool,
                        new Vector3(x + 0.7f, GroundedY(stool, propScale), 0f),
                        tint, -1, propScale);
                }

                if (trash != null && shopIndex % 2 == 1)
                {
                    CreateProp(parent, $"CafeTrash_{shopIndex}", trash,
                        new Vector3(lampX + 0.65f, GroundedY(trash, propScale), 0f),
                        tint, -2, propScale);
                }

                x += SpriteHalfWidth(shop, scale) * 2f + gapExtra;
                shopIndex++;
            }
        }

        private static void PlaceCafeAlley(Transform parent, StageMapSpec spec, Sprite brick, Sprite lamp, Sprite fridge)
        {
            float start = spec.TeachEnd;
            float end = Mathf.Max(spec.PressureEnd, spec.SetpieceEnd);
            if (end <= start + 2f) return;

            // 골목 벽 반복
            if (brick != null)
            {
                float bx = start + 1f;
                int i = 0;
                float scale = 3.2f;
                float step = SpriteHalfWidth(brick, scale) * 2f * 0.9f;
                while (bx < end - 1f)
                {
                    Color wallTint = new Color(0.55f, 0.58f, 0.62f, 1f);
                    CreateProp(parent, $"AlleyWall_L_{i}", brick,
                        new Vector3(bx, GroundedY(brick, scale) + 0.8f, 0f), wallTint, -9, scale);
                    // 위쪽 한 단 더 (좁은 골목 높이감)
                    CreateProp(parent, $"AlleyWall_H_{i}", brick,
                        new Vector3(bx, GroundedY(brick, scale) + 2.6f, 0f),
                        new Color(0.45f, 0.48f, 0.52f, 1f), -10, scale);
                    bx += Mathf.Max(1.5f, step);
                    i++;
                }
            }

            // 골목 가로등 (드문드문, 어두움)
            if (lamp != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    float lx = Mathf.Lerp(start + 3f, end - 3f, (i + 1f) / 4f);
                    CreateProp(parent, $"AlleyLamp_{i}", lamp,
                        new Vector3(lx, GroundedY(lamp, 2.2f), 0f),
                        new Color(0.7f, 0.75f, 0.85f, 1f), -3, 2.2f);
                }
            }

            // Setpiece 끝쪽 소품 1 (탈출 느낌)
            if (fridge != null)
            {
                CreateProp(parent, "AlleyProp_End", fridge,
                    new Vector3(spec.SetpieceEnd - 4f, GroundedY(fridge, 2f), 0f),
                    new Color(0.8f, 0.82f, 0.85f, 1f), -2, 2f);
            }
        }

        private static void PlaceCafePlatforms(Transform parent, StageMapSpec spec, Sprite brick)
        {
            // Teach 구간 중~고 플랫폼 (동선 꺾기용 그레이박스)
            float[] xs =
            {
                Mathf.Lerp(spec.IntroEnd, spec.TeachEnd, 0.25f),
                Mathf.Lerp(spec.IntroEnd, spec.TeachEnd, 0.5f),
                Mathf.Lerp(spec.IntroEnd, spec.TeachEnd, 0.75f)
            };
            float[] ys = { 2.2f, 3.4f, 2.6f };

            for (int i = 0; i < xs.Length; i++)
            {
                var go = new GameObject($"CafePlatform_{i + 1}");
                go.transform.SetParent(parent, false);
                go.transform.position = new Vector3(xs[i], ys[i], 0f);
                go.transform.localScale = new Vector3(3.2f, 0.55f, 1f);

                var col = go.AddComponent<BoxCollider2D>();
                col.size = Vector2.one;

                if (brick != null)
                {
                    var visual = new GameObject("Visual");
                    visual.transform.SetParent(go.transform, false);
                    visual.transform.localPosition = Vector3.zero;
                    visual.transform.localScale = new Vector3(1.1f, 1.4f, 1f);
                    var sr = visual.AddComponent<SpriteRenderer>();
                    sr.sprite = brick;
                    sr.color = new Color(1f, 0.92f, 0.85f, 1f);
                    sr.sortingOrder = -1;
                }
            }
        }

        private static void PlaceCafePhotoDressing(
            Transform parent, StageMapSpec spec,
            Sprite facade, Sprite teaBar, Sprite table, Sprite fridge)
        {
            float[] photos = StageMapDatabase.GetPhotoPositions(spec);
            for (int i = 0; i < photos.Length; i++)
            {
                float px = photos[i];
                Sprite dress = i == 0 ? facade : (i == 1 ? teaBar : (fridge ?? facade));
                float scale = i == 0 ? 2.5f : 2.2f;
                CreateProp(parent, $"PhotoDress_{i + 1}", dress,
                    new Vector3(px, GroundedY(dress, scale), 0f),
                    Color.white, -7, scale);

                if (table != null)
                {
                    CreateProp(parent, $"PhotoDressTable_{i + 1}", table,
                        new Vector3(px - 1.2f, GroundedY(table, 2.3f), 0f),
                        Color.white, -2, 2.3f);
                }
            }
        }

        private static float SpriteHalfWidth(Sprite sprite, float scale)
        {
            if (sprite == null) return 1f;
            return sprite.rect.width / sprite.pixelsPerUnit * scale * 0.5f;
        }

        private static float GroundedY(Sprite sprite, float scale)
        {
            if (sprite == null) return 1f;
            // 피벗 중앙 가정 — 바닥(Y≈0)에 맞춤
            return sprite.rect.height / sprite.pixelsPerUnit * scale * 0.5f;
        }

        private static void PlaceAlleyShade(Transform parent, StageMapSpec spec)
        {
            float start = spec.TeachEnd;
            float end = Mathf.Min(spec.PressureEnd + 10f, spec.LengthX);
            if (end <= start) return;

            var tex = Texture2D.whiteTexture;
            var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1f);
            float center = (start + end) * 0.5f;
            float width = end - start;
            var go = new GameObject("AlleyShade");
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(center, 3f, 0f);
            go.transform.localScale = new Vector3(width, 8f, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.color = new Color(0f, 0f, 0f, 0.38f);
            sr.sortingOrder = -6;
        }

        private static void PlaceNeonBanners(Transform parent, StageMapSpec spec)
        {
            var banner = LoadFirstSprite(NeonBanner);
            if (banner == null) return;
            float[] xs = { 30f, 70f, 110f, 140f };
            for (int i = 0; i < xs.Length; i++)
            {
                if (xs[i] > spec.LengthX - 5f) break;
                CreateProp(parent, $"NeonBanner_{i + 1}", banner, new Vector3(xs[i], 3.2f, 0f),
                    new Color(1f, 0.7f, 1f, 1f), -4, 1.5f);
            }
        }

        private static void PlaceFireworks(Transform parent, StageMapSpec spec)
        {
            var fire = LoadFirstSprite(FireworkSprite);
            if (fire == null) return;
            float teach = spec.TeachEnd;
            for (int i = 0; i < 6; i++)
            {
                float x = Mathf.Lerp(teach * 0.4f, spec.SetpieceEnd, i / 5f);
                float y = 3.5f + (i % 3) * 1.2f;
                CreateProp(parent, $"Firework_{i + 1}", fire, new Vector3(x, y, 0f),
                    new Color(1f, 0.85f, 0.5f, 0.95f), 5, 2.5f);
            }
        }

        private static void PlaceSubwayDarkness(Transform parent, StageMapSpec spec)
        {
            var tex = Texture2D.whiteTexture;
            var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1f);

            // 천장 암막
            var ceiling = new GameObject("TunnelCeiling");
            ceiling.transform.SetParent(parent, false);
            ceiling.transform.position = new Vector3(spec.LengthX * 0.5f, 6.5f, 0f);
            ceiling.transform.localScale = new Vector3(spec.LengthX + 40f, 4f, 1f);
            var csr = ceiling.AddComponent<SpriteRenderer>();
            csr.sprite = spr;
            csr.color = new Color(0.05f, 0.06f, 0.08f, 0.92f);
            csr.sortingOrder = -7;

            // Pressure 구간 추가 암전
            float center = (spec.TeachEnd + spec.PressureEnd) * 0.5f;
            float width = Mathf.Max(10f, spec.PressureEnd - spec.TeachEnd);
            var dark = new GameObject("TunnelDarkZone");
            dark.transform.SetParent(parent, false);
            dark.transform.position = new Vector3(center, 2.5f, 0f);
            dark.transform.localScale = new Vector3(width, 7f, 1f);
            var dsr = dark.AddComponent<SpriteRenderer>();
            dsr.sprite = spr;
            dsr.color = new Color(0f, 0f, 0f, 0.45f);
            dsr.sortingOrder = -5;
        }

        private static void PlaceCityNeonAccent(Transform parent, StageMapSpec spec)
        {
            var banner = LoadFirstSprite(NeonBanner);
            if (banner == null) return;
            CreateProp(parent, "NeonAccent_1", banner, new Vector3(85f, 3.5f, 0f), Color.white, -4, 1.2f);
            if (spec.LengthX > 120f)
                CreateProp(parent, "NeonAccent_2", banner, new Vector3(110f, 3.2f, 0f), Color.white, -4, 1.1f);
        }

        private static void CreateProp(
            Transform parent, string name, Sprite sprite, Vector3 pos, Color tint, int order, float scale)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * scale;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = tint;
            sr.sortingOrder = order;
        }

        private static Sprite LoadFirstSprite(string assetPath)
        {
            var sprites = LoadAllSprites(assetPath);
            return sprites != null && sprites.Length > 0 ? sprites[0] : null;
        }

        private static Sprite[] LoadAllSprites(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || !File.Exists(assetPath))
                return Array.Empty<Sprite>();

            var objs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            var list = new List<Sprite>();
            foreach (var o in objs)
            {
                if (o is Sprite s)
                    list.Add(s);
            }

            list.Sort((a, b) =>
            {
                int ia = ExtractSpriteIndex(a.name);
                int ib = ExtractSpriteIndex(b.name);
                if (ia != ib) return ia.CompareTo(ib);
                return string.CompareOrdinal(a.name, b.name);
            });

            return list.ToArray();
        }

        private static int ExtractSpriteIndex(string name)
        {
            int us = name.LastIndexOf('_');
            if (us < 0 || us >= name.Length - 1) return int.MaxValue;
            return int.TryParse(name.Substring(us + 1), out int n) ? n : int.MaxValue;
        }

        private static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
                UnityEngine.Object.DestroyImmediate(parent.GetChild(i).gameObject);
        }

        private static Color Hex(string hex)
        {
            if (ColorUtility.TryParseHtmlString("#" + hex, out var c))
                return c;
            return Color.cyan;
        }

        private static Color Desat(Color c, float amount)
        {
            float gray = c.r * 0.299f + c.g * 0.587f + c.b * 0.114f;
            return Color.Lerp(c, new Color(gray, gray, gray, c.a), amount);
        }

        private struct Theme
        {
            public Color CamBg;
            public Color SkyTint;
            public Color FarTint;
            public Color NearTint;
            public Color TileTint;
            public Sprite SkySprite;
            public Sprite FarSprite;
            public Sprite NearSprite;
            public float SkyScale;
            public float FarScale;
            public float NearScale;
            public float SkyY;
            public float FarY;
            public float NearY;
            public PropMode PropMode;
            public bool HideSky;
        }

        private enum PropMode
        {
            City,
            Cafe,
            Fireworks,
            Subway,
            Sparse
        }
    }
}
#endif
