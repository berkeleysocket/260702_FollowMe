using System;
using System.Collections.Generic;

namespace SeungyungLib.Core.NotifyValue
{
    public class NotifyValue<T>
    {
        public NotifyValue(T value)
        {
            this._value = value;
        }
        
        public T Value
        {
            get => this._value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                    OnChanged?.Invoke(value);
                
                this._value = value;
            }
        }

        public event Action<T> OnChanged;

        private T _value;
    }
}