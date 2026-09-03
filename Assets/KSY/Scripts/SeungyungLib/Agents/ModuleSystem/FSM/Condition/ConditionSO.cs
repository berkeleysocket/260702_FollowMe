using SeungyungLib.Core.FlyweightService;
using SeungyungLib.Core.ReadOnlyAttribute;
using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    public abstract class ConditionSO : ScriptableObject
    {
        [field: SerializeField, ReadOnly] public ConditionType Type { get; private set; }
        [field: SerializeField] protected bool IsNot { get; private set; }

        #region Unity Events
        private void OnValidate()
        {
            string typeName = this.GetType().Name.Replace("ConditionSo", "");
            if (System.Enum.TryParse<ConditionType>(typeName, true, out ConditionType conditionType))
                Type = conditionType;
        }
        #endregion
        
        public abstract ICondition Create(IModuleOwner owner, IFlyweightFactory<ConditionType, ICondition> factory);
    }
}