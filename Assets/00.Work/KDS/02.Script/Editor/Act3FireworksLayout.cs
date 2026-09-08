#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace FollowMe.KDS.Editor
{
    /// <summary>
    /// Act3(S9~S11) 불꽃축제 레이아웃 공용 좌표 — 지형(act3-terrain)과 장식(act3-decorate)이 같은 값을 쓴다.
    /// 기획: Stage_All_LevelDesign.md Act3
    ///  - Teach   : 불꽃 윈도우 = 좋아요 대량 생성 (발사대 플랫폼 + 윈도우 + 버스트 링)
    ///  - Pressure: 경고 → 추격 (S9 1회 / S10·S11 2회). S11은 공허(지면 끊김·소품 소멸)
    ///  - Breath  : 회복·포토·갈림1
    ///  - Setpiece: 불꽃 직선 — 한강 다리 데크 위 직선 러닝. S11은 어두운 강변
    ///  - Goal    : 갈림2 + 클리어
    /// </summary>
    public static class Act3FireworksLayout
    {
        public const float GroundY = 0f;
        public const float LaunchLowY = 3.4f;
        public const float LaunchHighY = 5.6f;
        public const float TeachWindowY = 7.3f;
        public const float DeckY = 6.0f;
        public const float DeckWindowY = 7.7f;
        public const float BurstRadius = 1.75f;
        public const float PhotoY = 1.2f;

        public readonly struct PhotoPlacement
        {
            public PhotoPlacement(string prefab, float x)
            {
                Prefab = prefab;
                X = x;
            }

            public string Prefab { get; }
            public float X { get; }
        }

        public readonly struct ModeZonePlacement
        {
            public ModeZonePlacement(MapMode mode, float x)
            {
                Mode = mode;
                X = x;
            }

            public MapMode Mode { get; }
            public float X { get; }
        }

        /// <summary>
        /// 지면 콜라이더 윗면 Y (열린 씬의 Grid/Tilemap_Ground 기준). 타일이 셀 -5..0에 깔리므로 보통 1.0.
        /// 바닥에 붙는 구조물은 이 값에 스프라이트 하단을 맞춘다.
        /// </summary>
        public static float GetGroundTop()
        {
            var go = GameObject.Find("Grid/Tilemap_Ground");
            var tm = go != null ? go.GetComponent<UnityEngine.Tilemaps.Tilemap>() : null;
            if (tm == null || tm.layoutGrid == null)
                return 1f;

            tm.CompressBounds();
            var b = tm.cellBounds;
            if (b.size.y <= 0)
                return 1f;

            return tm.layoutGrid.CellToWorld(new Vector3Int(0, b.yMax, 0)).y;
        }

        /// <summary>
        /// 활성 스프라이트 바운드 하단을 groundTop에 맞춰 Y 이동 (사이드뷰: 바닥 구조물이 지면에 정확히 앉도록).
        /// </summary>
        public static bool SnapBottomToGround(Transform t, float groundTop)
        {
            var srs = t.GetComponentsInChildren<SpriteRenderer>(false);
            if (srs.Length == 0)
                return false;

            float minY = float.MaxValue;
            foreach (var sr in srs)
            {
                if (sr.sprite == null) continue;
                minY = Mathf.Min(minY, sr.bounds.min.y);
            }

            if (minY == float.MaxValue)
                return false;

            float delta = groundTop - minY;
            if (Mathf.Abs(delta) < 0.001f)
                return false;

            t.position += new Vector3(0f, delta, 0f);
            return true;
        }

        /// <summary>갈림 X — Fork1 = Breath 중앙(회복·포토), Fork2 = Goal 구간 초입.</summary>
        public static float[] GetForkXs(StageMapSpec spec) => new[]
        {
            Mathf.Lerp(spec.PressureEnd, spec.BreathEnd, 0.5f),
            spec.SetpieceEnd + 6f,
        };

        /// <summary>S11 후반 = 축제 끝(공허). 소품·랜턴·군중이 사라지는 구간.</summary>
        public static bool IsDark(StageMapSpec spec, float x) =>
            spec.Stage == 11 && x > spec.TeachEnd;

        /// <summary>Teach 발사대(윈도우) 중심 X. S9 4 / S10 5 / S11 3(약화).</summary>
        public static float[] GetTeachWindowXs(StageMapSpec spec)
        {
            int n = spec.Stage switch { 9 => 4, 10 => 5, _ => 3 };
            return Spread(spec.IntroEnd + 7f, spec.TeachEnd - 7f, n);
        }

        /// <summary>Setpiece 데크 위 윈도우 X. S9 3 / S10 4 / S11 2.</summary>
        public static float[] GetSetpieceWindowXs(StageMapSpec spec)
        {
            int n = spec.Stage switch { 9 => 3, 10 => 4, _ => 2 };
            return Spread(spec.BreathEnd + 14f, spec.SetpieceEnd - 12f, n);
        }

        /// <summary>Pressure 지면 끊김 [x0,x1]. S9 없음 / S10 짧게 1 / S11 공허 3.</summary>
        public static List<Vector2> GetPressureGaps(StageMapSpec spec)
        {
            var gaps = new List<Vector2>();
            float p0 = spec.TeachEnd;
            float p1 = spec.PressureEnd;
            float len = p1 - p0;
            switch (spec.Stage)
            {
                case 10:
                    gaps.Add(new Vector2(p0 + len * 0.55f, p0 + len * 0.55f + 4f));
                    break;
                case 11:
                    gaps.Add(new Vector2(p0 + len * 0.22f, p0 + len * 0.22f + 7f));
                    gaps.Add(new Vector2(p0 + len * 0.50f, p0 + len * 0.50f + 9f));
                    gaps.Add(new Vector2(p0 + len * 0.78f, p0 + len * 0.78f + 8f));
                    break;
            }

            return gaps;
        }

        /// <summary>S11 Setpiece(어두운 강변) 데크 끊김 여부 — 짧은 세그먼트, 긴 간격.</summary>
        public static void GetDeckRhythm(StageMapSpec spec, out float segment, out float gap)
        {
            if (spec.Stage == 11)
            {
                segment = 8f;
                gap = 5f;
            }
            else
            {
                segment = 13f;
                gap = 3f;
            }
        }

        public static float DeckStart(StageMapSpec spec) => spec.BreathEnd + 8f;
        public static float DeckEnd(StageMapSpec spec) => spec.SetpieceEnd - 6f;

        /// <summary>지면이 있는가 (Pressure 갭 제외).</summary>
        public static bool HasGround(StageMapSpec spec, float x)
        {
            foreach (var g in GetPressureGaps(spec))
            {
                if (x >= g.x - 0.5f && x <= g.y + 0.5f)
                    return false;
            }

            return true;
        }

        /// <summary>갤 위 징검다리 X 목록.</summary>
        public static List<Vector3> GetGapStones(StageMapSpec spec)
        {
            var stones = new List<Vector3>();
            foreach (var g in GetPressureGaps(spec))
            {
                float w = g.y - g.x;
                int n = Mathf.Max(1, Mathf.RoundToInt(w / 3.2f) - 1);
                for (int i = 0; i < n; i++)
                {
                    float t = (i + 1f) / (n + 1f);
                    float x = Mathf.Lerp(g.x, g.y, t);
                    float y = 3.2f + (i % 2) * 0.9f;
                    stones.Add(new Vector3(x, y, 2.6f)); // z = width
                }
            }

            return stones;
        }

        /// <summary>Pressure 저지대 리듬 플랫폼 X (잿불 위 점프용).</summary>
        public static float[] GetPressureHopXs(StageMapSpec spec)
        {
            var list = new List<float>();
            float x = spec.TeachEnd + 9f;
            while (x < spec.PressureEnd - 6f)
            {
                if (HasGround(spec, x))
                    list.Add(x);
                x += 11f;
            }

            return list.ToArray();
        }

        /// <summary>경고→추격→회복 존. S9 1사이클 / S10·S11 2사이클. 마지막에 Breath 진입 시 안정.</summary>
        public static List<ModeZonePlacement> GetModeZones(StageMapSpec spec)
        {
            var zones = new List<ModeZonePlacement>();
            float p0 = spec.TeachEnd;
            float p1 = spec.PressureEnd;
            int cycles = spec.Monsters >= 2 ? 2 : 1;
            float span = (p1 - p0) / cycles;
            for (int c = 0; c < cycles; c++)
            {
                float s = p0 + span * c;
                zones.Add(new ModeZonePlacement(MapMode.Warning, s + 1.5f));
                zones.Add(new ModeZonePlacement(MapMode.Chase, s + Mathf.Min(10f, span * 0.35f)));
                zones.Add(new ModeZonePlacement(MapMode.Recovery, s + span - 2f));
            }

            zones.Add(new ModeZonePlacement(MapMode.Stable, spec.BreathEnd - 3f));
            return zones;
        }

        /// <summary>추격 구간 [x0,x1] (장애물 활성 범위 참고용).</summary>
        public static List<Vector2> GetChaseRanges(StageMapSpec spec)
        {
            var ranges = new List<Vector2>();
            float p0 = spec.TeachEnd;
            float p1 = spec.PressureEnd;
            int cycles = spec.Monsters >= 2 ? 2 : 1;
            float span = (p1 - p0) / cycles;
            for (int c = 0; c < cycles; c++)
            {
                float s = p0 + span * c;
                ranges.Add(new Vector2(s + Mathf.Min(10f, span * 0.35f), s + span - 2f));
            }

            return ranges;
        }

        /// <summary>기획서 사진 포인트 (Stage_All_LevelDesign.md Act3) → 프리팹·X.</summary>
        public static PhotoPlacement[] GetPhotos(StageMapSpec spec)
        {
            float teach(float t) => Mathf.Lerp(spec.IntroEnd, spec.TeachEnd, t);
            float breath(float t) => Mathf.Lerp(spec.PressureEnd, spec.BreathEnd, t);
            float setp(float t) => Mathf.Lerp(spec.BreathEnd, spec.SetpieceEnd, t);

            return spec.Stage switch
            {
                9 => new[]
                {
                    new PhotoPlacement("Photo_Act3_FireworkPeak", teach(0.86f)),
                    new PhotoPlacement("Photo_Act3_BridgeView", breath(0.32f)),
                    new PhotoPlacement("Photo_Act3_PhoneCrowd", setp(0.52f)),
                },
                10 => new[]
                {
                    new PhotoPlacement("Photo_Act3_BridgeLights", breath(0.32f)),
                    new PhotoPlacement("Photo_Act3_DroneShow", setp(0.50f)),
                },
                _ => new[]
                {
                    new PhotoPlacement("Photo_Act3_WaterReflect", setp(0.30f)),
                    new PhotoPlacement("Photo_Act3_EmptyRiver", setp(0.86f)),
                },
            };
        }

        public static float[] Spread(float x0, float x1, int n)
        {
            var arr = new float[Mathf.Max(0, n)];
            if (n <= 0) return arr;
            if (n == 1)
            {
                arr[0] = (x0 + x1) * 0.5f;
                return arr;
            }

            for (int i = 0; i < n; i++)
                arr[i] = Mathf.Lerp(x0, x1, i / (float)(n - 1));
            return arr;
        }
    }
}
#endif
