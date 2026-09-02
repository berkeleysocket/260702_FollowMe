using System;
using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.FSM
{
    public abstract class AbstractConditionSo : ScriptableObject
    {
        [field: SerializeField] protected bool IsNot { get; private set; }
        [field: SerializeField] public ConditionType Type { get; private set; }

        #region Unity Events
        private void OnValidate()
        {
            Type type = this.GetType();

            if (type != null)
            {
                string typeName = type.Name.Replace("ConditionSo", "");
                if (System.Enum.TryParse<ConditionType>(typeName, true, out ConditionType conditionType))
                    Type = conditionType;
            }
        }
        #endregion
        
        public abstract ICondition Create(IModuleOwner owner);
    }
}