using System;
using UnityEngine;
using YHW.UI;

namespace FollowMe.KDS
{
    /// <summary>
    /// 영속 진행 데이터. PlayerPrefs JSON (`kds.progress.v1`).
    /// 스테이지 선택 UI 퍼센트는 YHW <see cref="StageProgressStore"/>에도 최고치만 동기화한다.
    /// </summary>
    [Serializable]
    public class GameProgressData
    {
        public const int StageCount = 16;

        public int version = 1;
        public int lastPlayedStage = 1;
        public int highestClearedStage;
        public long totalLikes;
        public long totalFollows;
        public bool secondCycle;
        public int[] bestStars = new int[StageCount];
        public float[] bestFollowPercent = new float[StageCount];
        public bool[] cleared = new bool[StageCount];

        public static GameProgressData CreateNew()
        {
            return new GameProgressData
            {
                bestStars = new int[StageCount],
                bestFollowPercent = new float[StageCount],
                cleared = new bool[StageCount],
                lastPlayedStage = 1
            };
        }

        public void EnsureArrays()
        {
            if (bestStars == null || bestStars.Length != StageCount)
                bestStars = Resize(bestStars, StageCount);
            if (bestFollowPercent == null || bestFollowPercent.Length != StageCount)
                bestFollowPercent = Resize(bestFollowPercent, StageCount);
            if (cleared == null || cleared.Length != StageCount)
                cleared = Resize(cleared, StageCount);
        }

        private static T[] Resize<T>(T[] src, int len)
        {
            var dst = new T[len];
            if (src != null)
                Array.Copy(src, dst, Math.Min(src.Length, len));
            return dst;
        }
    }

    /// <summary>
    /// 게임 진행 저장/로드. KDS 전용 — YHW/KSY 폴더는 수정하지 않고 훅만 한다.
    /// </summary>
    public static class GameProgressSave
    {
        private const string PrefsKey = "kds.progress.v1";
        private static GameProgressData _cache;
        private static bool _dirty;

        public static GameProgressData Data
        {
            get
            {
                if (_cache == null)
                    Load();
                return _cache;
            }
        }

        public static event Action ProgressChanged;

        public static void Load()
        {
            if (!PlayerPrefs.HasKey(PrefsKey))
            {
                _cache = GameProgressData.CreateNew();
                _dirty = false;
                return;
            }

            try
            {
                string json = PlayerPrefs.GetString(PrefsKey, string.Empty);
                if (string.IsNullOrEmpty(json))
                {
                    _cache = GameProgressData.CreateNew();
                }
                else
                {
                    _cache = JsonUtility.FromJson<GameProgressData>(json) ?? GameProgressData.CreateNew();
                    _cache.EnsureArrays();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameProgressSave] Load failed, resetting. {e.Message}");
                _cache = GameProgressData.CreateNew();
            }

            _dirty = false;
        }

        public static void Save()
        {
            if (_cache == null)
                _cache = GameProgressData.CreateNew();
            _cache.EnsureArrays();

            string json = JsonUtility.ToJson(_cache);
            PlayerPrefs.SetString(PrefsKey, json);
            PlayerPrefs.Save();
            _dirty = false;
            ProgressChanged?.Invoke();
        }

        public static void FlushIfDirty()
        {
            if (_dirty)
                Save();
        }

        /// <summary>테스트/옵션용 — 진행 초기화.</summary>
        public static void ClearAll()
        {
            PlayerPrefs.DeleteKey(PrefsKey);
            for (int i = 0; i < GameProgressData.StageCount; i++)
                PlayerPrefs.DeleteKey("stage.bestPercent." + i);
            PlayerPrefs.Save();
            _cache = GameProgressData.CreateNew();
            _dirty = false;
            ProgressChanged?.Invoke();
            Debug.Log("[GameProgressSave] Cleared all progress.");
        }

        public static bool IsStageUnlocked(int stageNumber1Based)
        {
            int s = Mathf.Clamp(stageNumber1Based, 1, GameProgressData.StageCount);
            if (s <= 1) return true;
            return Data.highestClearedStage >= s - 1 || Data.cleared[s - 2];
        }

        public static int GetBestStars(int stageNumber1Based)
        {
            int i = StageToIndex(stageNumber1Based);
            return i < 0 ? 0 : Data.bestStars[i];
        }

        public static float GetBestFollowPercent(int stageNumber1Based)
        {
            int i = StageToIndex(stageNumber1Based);
            return i < 0 ? 0f : Data.bestFollowPercent[i];
        }

        public static bool IsStageCleared(int stageNumber1Based)
        {
            int i = StageToIndex(stageNumber1Based);
            return i >= 0 && Data.cleared[i];
        }

        /// <summary>
        /// 스테이지 클리어 기록. 별·팔로우%는 최고치만 갱신.
        /// YHW 스테이지 선택 카드용 <see cref="StageProgressStore.ReportPercent"/>도 호출.
        /// </summary>
        public static void RecordStageClear(int stageNumber1Based, int stars, float followRatio01)
        {
            int i = StageToIndex(stageNumber1Based);
            if (i < 0) return;

            var data = Data;
            data.EnsureArrays();
            data.lastPlayedStage = stageNumber1Based;
            data.cleared[i] = true;
            if (stageNumber1Based > data.highestClearedStage)
                data.highestClearedStage = stageNumber1Based;

            stars = Mathf.Clamp(stars, 1, 3);
            if (stars > data.bestStars[i])
                data.bestStars[i] = stars;

            // 팔로우 0%여도 클리어 별점(1★=33 / 2★=66 / 3★=100)은 YHW 카드에 반영
            float followPercent = Mathf.Clamp01(followRatio01) * 100f;
            float starPercent = stars >= 3 ? 100f : stars >= 2 ? 66f : 33f;
            float percent = Mathf.Max(followPercent, starPercent);
            if (percent > data.bestFollowPercent[i])
                data.bestFollowPercent[i] = percent;

            // YHW UI: 0-based index, 최고 %만 저장
            try
            {
                StageProgressStore.ReportPercent(i, percent);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameProgressSave] StageProgressStore sync failed: {e.Message}");
            }

            Save();
            Debug.Log(
                $"[GameProgressSave] Stage {stageNumber1Based} clear ★{stars} " +
                $"follow={percent:0.#}% (best★={data.bestStars[i]})");
        }

        public static void SetLastPlayedStage(int stageNumber1Based)
        {
            Data.lastPlayedStage = Mathf.Clamp(stageNumber1Based, 1, GameProgressData.StageCount);
            _dirty = true;
            Save();
        }

        /// <summary>누적 좋아요/팔로우·사이클을 런타임 서비스에서 반영.</summary>
        public static void CaptureSocial(long likes, long follows, bool secondCycle)
        {
            var data = Data;
            data.totalLikes = Math.Max(0, likes);
            data.totalFollows = Math.Max(0, follows);
            data.secondCycle = secondCycle;
            _dirty = true;
        }

        public static void ApplySocialTo(SocialScoreService service)
        {
            if (service == null) return;
            var data = Data;
            service.LoadFromSave(data.totalLikes, data.totalFollows, data.secondCycle);
        }

        private static int StageToIndex(int stageNumber1Based)
        {
            if (stageNumber1Based < 1 || stageNumber1Based > GameProgressData.StageCount)
                return -1;
            return stageNumber1Based - 1;
        }
    }
}
