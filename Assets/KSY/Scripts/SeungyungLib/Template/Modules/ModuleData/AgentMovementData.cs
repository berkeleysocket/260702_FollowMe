using UnityEngine;

namespace SeungyungLib.Core.Template.Modules
{
    [CreateAssetMenu(fileName = "AgentMovementDataSO", menuName = "SeungyungLib/AgentMovementData")]
    public class AgentMovementData : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field:  SerializeField] public float Acceleration { get; private set; }
        [field: SerializeField] public float Deceleration { get; private set; }
        [field: SerializeField] public float JumpForce { get; private set; }
        [field: SerializeField] public float JumpDeceleration { get; private set; }
    }
}
