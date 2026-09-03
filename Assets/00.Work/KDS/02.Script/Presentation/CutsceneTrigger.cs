using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 트리거 진입 시 CutscenePlayer를 호출한다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CutsceneTrigger : MonoBehaviour
    {
        [SerializeField] private CutscenePlayer _player;
        [SerializeField] private string _jsonFileName;
        [SerializeField] private bool _playOnce = true;

        private bool _played;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void Awake()
        {
            if (_player == null)
                _player = FindFirstObjectByType<CutscenePlayer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_played && _playOnce)
                return;

            if (!PlayerTriggerUtility.IsPlayer(other))
                return;

            if (_player == null)
            {
                Debug.LogWarning($"[CutsceneTrigger] CutscenePlayer 없음: {name}", this);
                return;
            }

            if (string.IsNullOrWhiteSpace(_jsonFileName))
            {
                Debug.LogWarning($"[CutsceneTrigger] JSON 파일명 없음: {name}", this);
                return;
            }

            _played = true;
            _player.PlayFromJson(_jsonFileName);
        }
    }
}
