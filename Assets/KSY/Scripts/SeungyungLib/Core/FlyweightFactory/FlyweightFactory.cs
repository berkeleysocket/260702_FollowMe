using System;
using System.Collections.Generic;

namespace SeungyungLib.Core.FlyweightService
{
    public class FlyweightFactory<TKey, TValue> : IFlyweightFactory<TKey, TValue>
        where TValue : class, IFlyweight
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        
        public TValue? GetOrAdd(TKey key, Func<TValue> add = null)
        {
            TValue value = null;
            
            if (_dictionary.TryGetValue(key, out value) && value != null)
                return value;
            else
            {
                TValue newValue = add?.Invoke();

                if (newValue != null)
                {
                    _dictionary[key] = newValue;
                    return _dictionary[key];
                }
                else
                    return null;
            }
        }

        public TValue? GetOrAdd<TArgs>(TKey key, TArgs arg, Func<TArgs, TValue> add)
        {
            return GetOrAdd(key, ()=> add?.Invoke(arg));
        }

        public TValue? GetOrAdd<TArgs1, TArgs2>(TKey key, TArgs1 arg1, TArgs2 arg2, Func<TArgs1, TArgs2, TValue> add)
        {
            return GetOrAdd(key, ()=> add?.Invoke(arg1, arg2));
        }

        public TValue? GetOrAdd<TArgs1, TArgs2, TArgs3>(TKey key, TArgs1 arg1, TArgs2 arg2, TArgs3 args3, Func<TArgs1, TArgs2, TArgs3, TValue> add)
        {
            return GetOrAdd(key, ()=> add?.Invoke(arg1, arg2, args3));
        }
    }
}