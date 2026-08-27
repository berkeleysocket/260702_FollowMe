using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 지정 모드일 때만 타겟 오브젝트 활성화 (괴물·장애물·연출).
    /// 이 컴포넌트가 붙은 GO는 항상 활성 — 타겟만 토글.
    /// </summary>
    public class MapModeObject : MonoBehaviour
    {
        [SerializeField] private MapMode[] _activeInModes = { MapMode.Chase };
        [SerializeField] private GameObject[] _targets;

        private void OnEnable()
        {
            if (MapModeService.Instance != null)
                MapModeService.Instance.ModeChanged += OnModeChanged;

            Refresh(MapModeService.Instance != null ? MapModeService.Instance.CurrentMode : MapMode.Stable);
        }

        private void OnDisable()
        {
            if (MapModeService.Instance != null)
                MapModeService.Instance.ModeChanged -= OnModeChanged;
        }

        private void OnModeChanged(MapMode prev, MapMode next) => Refresh(next);

        private void Refresh(MapMode mode)
        {
            bool active = IsActiveInMode(mode);
            GameObject[] targets = ResolveTargets();

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                    targets[i].SetActive(active);
            }
        }

        private GameObject[] ResolveTargets()
        {
            if (_targets != null && _targets.Length > 0)
                return _targets;

            if (transform.childCount == 0)
                return System.Array.Empty<GameObject>();

            var list = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
                list[i] = transform.GetChild(i).gameObject;
            return list;
        }

        private bool IsActiveInMode(MapMode mode)
        {
            if (_activeInModes == null || _activeInModes.Length == 0)
                return false;

            for (int i = 0; i < _activeInModes.Length; i++)
            {
                if (_activeInModes[i] == mode)
                    return true;
            }

            return false;
        }
    }
}
