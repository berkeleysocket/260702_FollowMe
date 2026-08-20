using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    public abstract class AbstractConditionSO : ScriptableObject
    {
        [field: SerializeField] protected bool IsNot { get; private set; }
        public abstract ICondition Create(IModuleOwner owner);
    }


}