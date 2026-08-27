using System;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 현재 맵 모드를 관리. 안정→추격 직행은 차단.
    /// </summary>
    public class MapModeService : MonoBehaviour
    {
        public static MapModeService Instance { get; private set; }

        [SerializeField] private MapMode _initialMode = MapMode.Stable;
        [SerializeField] private bool _logModeChanges = true;

        public MapMode CurrentMode { get; private set; }

        public event Action<MapMode, MapMode> ModeChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            CurrentMode = _initialMode;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public bool TrySetMode(MapMode next, bool force = false)
        {
            if (CurrentMode == next)
                return true;

            if (!force && !CanTransition(CurrentMode, next))
            {
                if (_logModeChanges)
                    Debug.LogWarning($"[MapModeService] 전환 거부: {CurrentMode} → {next}", this);
                return false;
            }

            MapMode prev = CurrentMode;
            CurrentMode = next;
            ModeChanged?.Invoke(prev, next);

            if (_logModeChanges)
                Debug.Log($"[MapModeService] {prev} → {next}", this);

            return true;
        }

        public static bool CanTransition(MapMode from, MapMode to)
        {
            if (from == to) return true;

            // 안정 → 추격 직행 금지 (경고 구간 필수)
            if (from == MapMode.Stable && to == MapMode.Chase)
                return false;

            // 굴레는 명시적 force만 (일반 존에서는 Torment 진입 후 Chase/Stable 자동 복귀 금지)
            if (from == MapMode.Torment && to != MapMode.Torment)
                return false;

            return true;
        }

        public static string GetDisplayName(MapMode mode)
        {
            return mode switch
            {
                MapMode.Stable => "안정",
                MapMode.Warning => "경고",
                MapMode.Chase => "추격",
                MapMode.Recovery => "회복",
                MapMode.Torment => "굴레",
                _ => mode.ToString()
            };
        }
    }
}
