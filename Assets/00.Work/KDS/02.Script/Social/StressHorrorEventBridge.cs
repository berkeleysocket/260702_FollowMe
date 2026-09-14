using UnityEngine;
using KSY;
using YHW.Stats;

namespace FollowMe.KDS
{
    /// <summary>
    /// 스트레스가 임계치(기본 75%) 이상이면 HorrorEventManager의 호러 이벤트(적 스폰)를 시작하고,
    /// 다시 아래로 내려가면 종료한다.
    /// </summary>
    public class StressHorrorEventBridge : MonoBehaviour
    {
        [SerializeField] private StressMeter _meter;
        [SerializeField] private HorrorEventManager _horrorEventManager;
        [SerializeField] private int _triggerPercent = 75;

        private bool _horrorActive;

        private void OnEnable()
        {
            if (_meter == null)
                _meter = FindFirstObjectByType<StressMeter>();
            if (_horrorEventManager == null)
                _horrorEventManager = FindFirstObjectByType<HorrorEventManager>();

            if (_meter == null) return;

            _meter.ValueChanged += HandleValueChanged;
            Evaluate(_meter.CurrentPercent);
        }

        private void OnDisable()
        {
            if (_meter != null)
                _meter.ValueChanged -= HandleValueChanged;
        }

        private void HandleValueChanged(float current, float max)
        {
            if (_meter != null)
                Evaluate(_meter.CurrentPercent);
        }

        private void Evaluate(int percent)
        {
            bool shouldBeActive = percent >= _triggerPercent;
            if (shouldBeActive == _horrorActive) return;
            _horrorActive = shouldBeActive;

            if (_horrorEventManager == null) return;

            if (shouldBeActive)
                _horrorEventManager.StartHorrorEvent();
            else
                _horrorEventManager.StopHorrorEvent();
        }
    }
}
