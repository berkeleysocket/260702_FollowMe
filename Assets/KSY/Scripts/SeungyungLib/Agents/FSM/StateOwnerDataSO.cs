using SeungyungLib.Agents.FSM.Enum;

using System.Linq;
using UnityEngine;

namespace SeungyungLib.Agents.FSM
{
    [CreateAssetMenu(fileName = "StateOwnerDataSO", menuName = "SeungyungLib/FSM/StateOwnerData", order = 0)]
    public class StateOwnerDataSO : ScriptableObject    
    {
        [field: SerializeReference] public StateHierarchyType StartHierarchy { get; private set; }
        [field: SerializeReference] public StateHierarchyDataSO[] StateHierarchies { get; private set; }

        private void OnValidate()
        {
            if (StateHierarchies == null || StateHierarchies.Length <= 1) return;

            StateHierarchies = StateHierarchies
                .OrderBy(hierarchyData => hierarchyData == null) 
                .ThenBy(hierarchyData => hierarchyData != null ? hierarchyData.Type : default(StateHierarchyType)) 
                .ToArray();
        }
    }
}