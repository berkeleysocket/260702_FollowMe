using System;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// Stage_All_LevelDesign.md 기준 스테이지 스펙.
    /// </summary>
    [Serializable]
    public struct StageMapSpec
    {
        public int Stage;
        public string ActTitle;
        public StageTemplate Template;
        public float LengthX;
        public int Checkpoints;
        public int PhotoPoints;
        public int Forks;
        public int Monsters;
        public bool TormentMode;

        /// <summary>모듈 경계 X (Intro끝, Teach끝, Pressure끝, Breath끝, Setpiece끝, Goal끝)</summary>
        public float IntroEnd;
        public float TeachEnd;
        public float PressureEnd;
        public float BreathEnd;
        public float SetpieceEnd;

        public float GoalEnd => LengthX;

        public bool HasPressure => PressureEnd > TeachEnd + 1f;
    }

    public enum StageTemplate
    {
        CityStreet = 0,   // T1 번화가
        CafeAlley = 1,    // T2 카페거리
        Fireworks = 2,    // T3 불꽃
        Subway = 3        // T4 지하철
    }

    public static class StageMapDatabase
    {
        public static readonly StageMapSpec[] All =
        {
            Spec(1,  "처음",   StageTemplate.CityStreet, 130, 1, 3, 2, 0, false, 20, 45, 45, 70, 100),
            Spec(2,  "처음",   StageTemplate.CityStreet, 140, 2, 3, 2, 0, false, 22, 50, 50, 78, 108),
            Spec(3,  "처음",   StageTemplate.CityStreet, 150, 2, 2, 2, 0, false, 24, 52, 52, 82, 112),
            Spec(4,  "달콤함", StageTemplate.CafeAlley,  140, 2, 3, 2, 0, false, 25, 50, 70, 90, 120),
            Spec(5,  "달콤함", StageTemplate.CafeAlley,  145, 2, 2, 2, 1, false, 25, 48, 72, 95, 125),
            Spec(6,  "달콤함", StageTemplate.CafeAlley,  150, 2, 2, 2, 1, false, 26, 52, 76, 98, 128),
            Spec(7,  "달콤함", StageTemplate.CafeAlley,  155, 3, 2, 2, 2, false, 26, 54, 82, 102, 132),
            Spec(8,  "달콤함", StageTemplate.CafeAlley,  160, 3, 2, 2, 2, false, 28, 56, 88, 108, 138),
            Spec(9,  "불꽃",   StageTemplate.Fireworks,  160, 2, 3, 2, 1, false, 28, 58, 82, 108, 138),
            Spec(10, "불꽃",   StageTemplate.Fireworks,  165, 2, 2, 2, 2, false, 28, 60, 88, 112, 142),
            Spec(11, "불꽃",   StageTemplate.Fireworks,  170, 3, 2, 2, 2, false, 30, 58, 95, 115, 145),
            Spec(12, "터널",   StageTemplate.Subway,     150, 2, 2, 2, 2, false, 24, 48, 78, 98, 128),
            Spec(13, "터널",   StageTemplate.Subway,     155, 3, 1, 2, 2, false, 24, 46, 82, 100, 130),
            Spec(14, "터널",   StageTemplate.Subway,     160, 3, 1, 2, 3, false, 26, 48, 90, 106, 136),
            Spec(15, "도착",   StageTemplate.CityStreet, 120, 1, 1, 1, 0, false, 20, 45, 45, 70, 95),
            Spec(16, "굴레",   StageTemplate.CityStreet, 140, 1, 0, 1, 1, true,  22, 45, 85, 85, 115),
        };

        public static StageMapSpec Get(int stage)
        {
            for (int i = 0; i < All.Length; i++)
            {
                if (All[i].Stage == stage)
                    return All[i];
            }

            throw new ArgumentOutOfRangeException(nameof(stage), stage, "Stage 1~16 only.");
        }

        private static StageMapSpec Spec(
            int stage, string actTitle, StageTemplate template, float lengthX,
            int cp, int photo, int forks, int monsters, bool torment,
            float introEnd, float teachEnd, float pressureEnd, float breathEnd, float setpieceEnd)
        {
            return new StageMapSpec
            {
                Stage = stage,
                ActTitle = actTitle,
                Template = template,
                LengthX = lengthX,
                Checkpoints = cp,
                PhotoPoints = photo,
                Forks = forks,
                Monsters = monsters,
                TormentMode = torment,
                IntroEnd = introEnd,
                TeachEnd = teachEnd,
                PressureEnd = pressureEnd,
                BreathEnd = breathEnd,
                SetpieceEnd = setpieceEnd
            };
        }

        public static float[] GetPhotoPositions(StageMapSpec spec)
        {
            return spec.Stage switch
            {
                1 => new[] { 58f, 85f, 92f },
                2 => new[] { 65f, 95f, 125f },
                3 => new[] { 70f, 130f },
                4 => new[] { 38f, 78f, 105f },
                5 => new[] { 82f, 110f },
                6 => new[] { 85f, 115f },
                7 => new[] { 88f, 118f },
                8 => new[] { 90f, 120f },
                9 => new[] { 48f, 95f, 120f },
                10 => new[] { 100f, 130f },
                11 => new[] { 105f, 135f },
                12 => new[] { 88f, 115f },
                13 => new[] { 110f },
                14 => new[] { 115f },
                15 => new[] { 55f },
                16 => Array.Empty<float>(),
                _ => BuildDefaultPhotoPositions(spec)
            };
        }

        private static float[] BuildDefaultPhotoPositions(StageMapSpec spec)
        {
            if (spec.PhotoPoints <= 0)
                return Array.Empty<float>();

            float start = spec.HasPressure ? spec.BreathEnd * 0.85f : spec.TeachEnd;
            float end = spec.SetpieceEnd;
            var result = new float[spec.PhotoPoints];
            for (int i = 0; i < spec.PhotoPoints; i++)
            {
                float t = spec.PhotoPoints == 1 ? 0.5f : i / (float)(spec.PhotoPoints - 1);
                result[i] = Mathf.Lerp(start, end, t);
            }

            return result;
        }
    }
}
