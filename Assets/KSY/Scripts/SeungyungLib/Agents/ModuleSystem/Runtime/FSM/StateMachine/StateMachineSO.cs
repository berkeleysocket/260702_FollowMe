using SeungyungLib.FSM.Enum;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "StateMachineSO",menuName = "SeungyungLib/FSM/StateMachineSO", order = 0)]
    public class StateMachineSO : ScriptableObject
    {
        [field: SerializeField] public StateSO[] StateList { get; private set; }
        [field: SerializeField] public StateType StartState { get; private set; }
    }
}
