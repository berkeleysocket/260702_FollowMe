using System;
using System.Collections.Generic;
using UnityEngine;

namespace FollowMe.KDS
{
    public enum CollectibleKind
    {
        Like,
        Follow,
        Daily
    }

    public readonly struct CollectibleSpawn
    {
        public CollectibleSpawn(CollectibleKind kind, float x, float y, DailyKind dailyKind = DailyKind.Reply)
        {
            Kind = kind;
            X = x;
            Y = y;
            DailyKind = dailyKind;
        }

        public CollectibleKind Kind { get; }
        public float X { get; }
        public float Y { get; }
        public DailyKind DailyKind { get; }
    }

    /// <summary>
    /// Stage_All_LevelDesign.md 기준 ♡/일상 배치 좌표 생성.
    /// </summary>
    public static class StageCollectibleLayout
    {
        private static readonly DailyKind[] DailyCycle = { DailyKind.Reply, DailyKind.Meal, DailyKind.Sleep };

        public static IReadOnlyList<CollectibleSpawn> Build(StageMapSpec spec)
        {
            int likes = StageMapDatabase.GetLikeCount(spec.Stage);
            int follows = StageMapDatabase.GetFollowCount(spec.Stage);
            int daily = StageMapDatabase.GetDailyCount(spec.Stage);
            var result = new List<CollectibleSpawn>(likes + follows + daily);

            if (likes <= 0 && follows <= 0 && daily <= 0)
                return result;

            float lowY = GetLowLikeY(spec);
            float highY = GetHighLikeY(spec);
            float[] forkXs = GetForkXs(spec);

            // Follow hearts — main path, easy height (S1 tutorial focus)
            int introFollow = Mathf.Max(0, Mathf.RoundToInt(follows * 0.15f));
            int teachFollow = Mathf.Max(0, Mathf.RoundToInt(follows * 0.45f));
            int setpieceFollow = Mathf.Max(0, Mathf.RoundToInt(follows * 0.25f));
            int forkFollow = follows - introFollow - teachFollow - setpieceFollow;

            DistributeInRange(result, CollectibleKind.Follow, introFollow, lowY, 3f, spec.IntroEnd - 2f);
            DistributeInRange(result, CollectibleKind.Follow, teachFollow, lowY,
                spec.IntroEnd + 2f, spec.TeachEnd - 2f);
            DistributeInRange(result, CollectibleKind.Follow, setpieceFollow, lowY,
                spec.BreathEnd + 2f, spec.SetpieceEnd - 2f);
            DistributeAlongForks(result, CollectibleKind.Follow, forkFollow, lowY + 0.3f, forkXs);

            // Like emojis — some low, some on fork upper path
            int introLikes = Mathf.Max(0, Mathf.RoundToInt(likes * 0.1f));
            int teachLikes = Mathf.Max(0, Mathf.RoundToInt(likes * 0.42f));
            int setpieceLikes = Mathf.Max(0, Mathf.RoundToInt(likes * 0.19f));
            int forkLikes = likes - introLikes - teachLikes - setpieceLikes;

            DistributeInRange(result, CollectibleKind.Like, introLikes, lowY + 0.4f, 5f, spec.IntroEnd - 1f);
            DistributeInRange(result, CollectibleKind.Like, teachLikes,
                spec.Template == StageTemplate.CityStreet ? lowY + 0.4f : highY,
                spec.IntroEnd + 3f, spec.TeachEnd - 2f);
            DistributeInRange(result, CollectibleKind.Like, setpieceLikes, lowY + 0.4f,
                spec.BreathEnd + 3f, spec.SetpieceEnd - 2f);
            DistributeAlongForks(result, CollectibleKind.Like, forkLikes, highY, forkXs);

            DistributeDaily(result, daily, lowY, spec, forkXs);
            return result;
        }

        private static void DistributeAlongForks(
            List<CollectibleSpawn> result, CollectibleKind kind, int count, float y, float[] forkXs)
        {
            if (count <= 0 || forkXs == null || forkXs.Length == 0)
                return;

            int perFork = count / forkXs.Length;
            int remainder = count - perFork * forkXs.Length;
            for (int i = 0; i < forkXs.Length; i++)
            {
                int n = perFork + (i < remainder ? 1 : 0);
                DistributeInRange(result, kind, n, y, forkXs[i] - 6f, forkXs[i] + 10f);
            }
        }

        private static void DistributeDaily(
            List<CollectibleSpawn> result, int count, float y, StageMapSpec spec, float[] forkXs)
        {
            if (count <= 0) return;

            float xStart = forkXs.Length > 0 ? forkXs[0] - 4f : spec.SetpieceEnd;
            float xEnd = spec.LengthX - 4f;
            float span = Mathf.Max(8f, xEnd - xStart);
            float step = span / (count + 1);

            for (int i = 0; i < count; i++)
            {
                float x = xStart + step * (i + 1);
                var kind = DailyCycle[i % DailyCycle.Length];
                result.Add(new CollectibleSpawn(CollectibleKind.Daily, x, y, kind));
            }
        }

        private static void DistributeInRange(
            List<CollectibleSpawn> result, CollectibleKind kind, int count, float y, float xStart, float xEnd)
        {
            if (count <= 0 || xEnd <= xStart)
                return;

            float span = xEnd - xStart;
            float step = span / (count + 1);
            for (int i = 0; i < count; i++)
            {
                float x = xStart + step * (i + 1);
                result.Add(new CollectibleSpawn(kind, x, y));
            }
        }

        public static float[] GetForkXs(StageMapSpec spec)
        {
            return spec.Forks switch
            {
                0 => Array.Empty<float>(),
                1 => new[] { spec.BreathEnd * 0.7f },
                _ => new[] { spec.BreathEnd * 0.55f, spec.SetpieceEnd * 0.85f }
            };
        }

        public static float GetLowLikeY(StageMapSpec spec) =>
            spec.Template == StageTemplate.Subway ? 1.4f : 1.2f;

        public static float GetHighLikeY(StageMapSpec spec)
        {
            return spec.Template switch
            {
                StageTemplate.CityStreet => spec.Stage <= 3 ? 3.4f : 3.8f,
                StageTemplate.CafeAlley => 3.6f,
                StageTemplate.Fireworks => 5.6f,
                StageTemplate.Subway => 2.8f,
                _ => 3.2f
            };
        }

        public static float GetPlatformY(StageMapSpec spec) => GetHighLikeY(spec) - 0.5f;
    }
}
