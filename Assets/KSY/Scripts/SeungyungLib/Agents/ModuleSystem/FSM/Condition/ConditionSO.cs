using PlasticGui.Help;
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
            string typeName = this.GetType().Name.Replace(nameof(ConditionSO), "");
            if (System.Enum.TryParse<ConditionType>(typeName, true, out ConditionType conditionType))
                Type = conditionType;
        }
        #endregion

        public ICondition Create(IModuleOwner owner, IFlyweightFactory<ConditionType, ICondition> factory)
        {
            if (this is IOptionalConditionSO)
                return OnCreate(owner);

            ConditionOption option = IsNot ? ConditionOption.IsNot : ConditionOption.None;
            int upperBits = (int)option << 16;
            int lowerBits = (int)Type;
            ConditionType key = (ConditionType)(upperBits | lowerBits);

            return factory.GetOrAdd(key, owner, OnCreate);
        }
        protected abstract ICondition OnCreate(IModuleOwner owner);
    }
}