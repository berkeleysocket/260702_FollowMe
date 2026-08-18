using SeungyungLib.Agents.FSM.Enum;
using SeungyungLib.Agents.FSM.Interface;

using UnityEngine;

namespace SeungyungLib.Agents.FSM
{
    [CreateAssetMenu(fileName = "StateHierarchyDataSO", menuName = "SeungyungLib/FSM/StateHierarchyData", order = 0)]
    public class StateHierarchyDataSO : ScriptableObject
    {
        [field: SerializeReference] public StateHierarchyType Type { get; private set; }
        [field: SerializeReference] public IStateHierarchy HierarchyInstance { get; private set; }
    } 
} 