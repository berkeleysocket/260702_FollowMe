using System;
using System.Collections.Generic;
using UnityEngine;

namespace YHW.Stats
{
    /// <summary>
    /// Maps an exact integer percent (0-100) to one or more callbacks.
    /// When the meter's percent changes, every integer it passed through
    /// (not just the final one) is looked up and invoked, so a fast jump
    /// from 40% to 55% still fires a listener registered at 50.
    /// </summary>
    public class StressEventChannel
    {
        private readonly Dictionary<int, Action<int>> _listeners = new Dictionary<int, Action<int>>();

        public void Subscribe(int percent, Action<int> callback)
        {
            if (callback == null) return;
            percent = Mathf.Clamp(percent, 0, 100);

            _listeners.TryGetValue(percent, out Action<int> existing);
            _listeners[percent] = existing + callback;
        }

        public void Unsubscribe(int percent, Action<int> callback)
        {
            if (callback == null) return;
            percent = Mathf.Clamp(percent, 0, 100);

            if (!_listeners.TryGetValue(percent, out Action<int> existing)) return;

            existing -= callback;
            if (existing == null)
                _listeners.Remove(percent);
            else
                _listeners[percent] = existing;
        }

        public void RaiseCrossed(int fromPercent, int toPercent)
        {
            if (fromPercent == toPercent)
            {
                RaiseExact(toPercent);
                return;
            }

            int step = toPercent > fromPercent ? 1 : -1;
            for (int p = fromPercent + step; ; p += step)
            {
                RaiseExact(p);
                if (p == toPercent) break;
            }
        }

        private void RaiseExact(int percent)
        {
            if (_listeners.TryGetValue(percent, out Action<int> callback))
                callback?.Invoke(percent);
        }
    }
}
