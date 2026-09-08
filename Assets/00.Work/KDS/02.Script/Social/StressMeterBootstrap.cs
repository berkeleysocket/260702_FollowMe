using UnityEngine;
using YHW.Stats;

namespace FollowMe.KDS
{
    /// <summary>
    /// 스테이지 시작 시 StressMeter를 만땅으로 둔다.
    /// Act1(S1~S2)은 스트레스 미사용 — 미터·UI를 끈다.
    /// </summary>
    public class StressMeterBootstrap : MonoBehaviour
    {
        [SerializeField] private StressMeter _meter;
        [SerializeField] private bool _startFull = true;

        private void Awake()
        {
            if (_meter == null)
                _meter = GetComponent<StressMeter>() ?? FindFirstObjectByType<StressMeter>();

            if (!StageStressPolicy.UsesStressInActiveScene())
                DisableStressForAct1();
        }

        private void Start()
        {
            if (!StageStressPolicy.UsesStressInActiveScene())
                return;

            if (_meter != null && _startFull)
                _meter.SetStress(_meter.MaxValue);
        }

        private void DisableStressForAct1()
        {
            if (_meter != null)
                _meter.enabled = false;

            var canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < canvases.Length; i++)
            {
                var c = canvases[i];
                if (c != null && c.gameObject.name.Contains("StressBar"))
                    c.gameObject.SetActive(false);
            }

            Debug.Log(
                $"[StressMeterBootstrap] Stage {StageStressPolicy.ResolveActiveStageNumber()} — 스트레스 비활성",
                this);
        }
    }
}
