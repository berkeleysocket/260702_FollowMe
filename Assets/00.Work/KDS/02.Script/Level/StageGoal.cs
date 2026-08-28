using UnityEngine;

namespace FollowMe.KDS
{
    [RequireComponent(typeof(Collider2D))]
    public class StageGoal : MonoBehaviour
    {
        [SerializeField] private int _stageNumber = 1;

        private bool _cleared;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_cleared || !PlayerTriggerUtility.IsPlayer(other))
                return;

            _cleared = true;
            Debug.Log($"[StageGoal] Stage {_stageNumber} 클리어 (Goal 도달)", this);
        }
    }
}
