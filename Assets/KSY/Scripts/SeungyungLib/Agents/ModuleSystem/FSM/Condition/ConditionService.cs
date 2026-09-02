using SeungyungLib.FSM.Enum;
using SeungyungLib.FSM.Interface;

using System.Collections.Generic;

namespace SeungyungLib.FSM
{
    public class ConditionService
    {
        private Dictionary<ConditionType, ICondition> _conditions;

        public ICondition Get(ConditionType type)
        {
            if (_conditions.TryGetValue(type, out ICondition result) && result != null)
            {
                return result;
            }
            else if (!_conditions.ContainsKey(type))
            {

            }
            
            return null;
        }

        private void Register(ConditionType type)
        {
            string typeName = type
            _conditions.Add(type);
        }
    }
}
