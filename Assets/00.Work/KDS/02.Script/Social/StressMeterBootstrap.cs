using UnityEngine;
using YHW.Stats;

namespace FollowMe.KDS
{
    /// <summary>
    /// 스테이지 시작 시 StressMeter를 만땅으로 둔다.
    /// 화난 이모지 픽업 → SocialItemScoreBridge가 스트레스를 깎는다.
    /// </summary>
    public class StressMeterBootstrap : MonoBehaviour
    {
        [SerializeField] private StressMeter _meter;
        [SerializeField] private bool _startFull = true;

        private void Awake()
        {
            if (_meter == null)
                _meter = GetComponent<StressMeter>() ?? FindFirstObjectByType<StressMeter>();
        }

        private void Start()
        {
            if (_meter != null && _startFull)
                _meter.SetStress(_meter.MaxValue);
        }
    }
}
