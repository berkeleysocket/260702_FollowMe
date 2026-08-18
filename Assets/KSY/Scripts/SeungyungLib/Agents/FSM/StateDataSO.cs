using SeungyungLib.Agents.FSM.Interface;
using SeungyungLib.Core;
using UnityEngine;

namespace SeungyungLib.Agents.FSM
{
    [CreateAssetMenu(fileName = "StateDataSO", menuName = "SeungyungLib/FSM/StateData")]
    public class StateDataSO : ScriptableObject
    {
        [field: SerializeField] public AnimationParameterDataSO AnimationParameterDataSO { get; private set; }
        [field: SerializeField] public IState State { get; private set; }
    }
}
