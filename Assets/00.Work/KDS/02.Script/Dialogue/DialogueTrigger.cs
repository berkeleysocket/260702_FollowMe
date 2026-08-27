using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 플레이어가 Collider에 들어가면 DialoguePlayer.StartDialogue()를 호출한다.
    /// Stage1에서 빈 오브젝트에 붙이고 Sequence/Player만 드래그하면 된다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private DialoguePlayer _player;
        [SerializeField] private DialogueSequenceSO _sequence;

        [Header("Trigger")]
        [SerializeField] private bool _oneShot = true;

        private bool _consumed;

        public bool IsConsumed => _consumed;
        public bool HasValidReferences => _player != null && _sequence != null;

        public void SetReferences(DialoguePlayer player, DialogueSequenceSO sequence)
        {
            _player = player;
            _sequence = sequence;
        }

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col != null)
                col.isTrigger = true;

            if (_player == null)
                _player = FindFirstObjectByType<DialoguePlayer>();
        }

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsPlayer(other))
                return;

            TryPlay();
        }

        public bool TryPlay()
        {
            if (_oneShot && _consumed)
                return false;

            if (_player == null)
            {
                Debug.LogWarning($"[DialogueTrigger] DialoguePlayer가 비어 있습니다: {name}", this);
                return false;
            }

            if (_sequence == null)
            {
                Debug.LogWarning($"[DialogueTrigger] DialogueSequenceSO가 비어 있습니다: {name}", this);
                return false;
            }

            _player.StartDialogue(_sequence);

            if (_oneShot)
                _consumed = true;

            return true;
        }

        private static bool IsPlayer(Collider2D other)
        {
            return PlayerTriggerUtility.IsPlayer(other);
        }

        private void OnDrawGizmos()
        {
            var col = GetComponent<Collider2D>();
            if (col == null)
                return;

            Gizmos.color = _consumed
                ? new Color(0.45f, 0.45f, 0.45f, 0.2f)
                : new Color(0.35f, 0.85f, 1f, 0.22f);
            Gizmos.matrix = transform.localToWorldMatrix;

            if (col is BoxCollider2D box)
                Gizmos.DrawCube(box.offset, box.size);
            else if (col is CircleCollider2D circle)
                Gizmos.DrawSphere(circle.offset, circle.radius);
        }
    }
}
