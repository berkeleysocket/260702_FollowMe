using SeungyungLib.FSM.Enum;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "StateMachineSo",menuName = "SeungyungLib/FSM/StateMachineSo", order = 0)]
    public class StateMachineSo : ScriptableObject
    {
        [field: SerializeField] public AbstractStateSo[] StateList { get; private set; }
        [field: SerializeField] public StateType StartState { get; private set; }
    }
}
