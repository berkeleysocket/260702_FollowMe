using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeungyungLib.Core.EventChannelSystem
{
    [CreateAssetMenu(fileName = "Event Channel", menuName = "SeungyungLib/Utility/EventChannel", order = 40)]
    public class EventChannelSO : ScriptableObject
    {
        private Dictionary<Type, Action<ChannelEvent>> _events = new();
        private Dictionary<Delegate, Action<ChannelEvent>> _lookUp = new(); //중복구독 방지를 위한 테이블

        // handler => Handle따라하지마
        public void AddListener<T>(Action<T> handler) where T : ChannelEvent
        {
            if (_lookUp.ContainsKey(handler)) return;
            
            Action<ChannelEvent> castHandler = (evt) => handler(evt as T);  //타입캐스트 콜 핸들러
            _lookUp[handler] = castHandler;
            Type eventType = typeof(T);
            if (_events.ContainsKey(eventType))
            {
                _events[eventType] += castHandler;
            }
            else
            {
                _events[eventType] = castHandler;
            }
        }


        public void RemoveListener<T>(Action<T> handler) where T : ChannelEvent
        {
            Type eventType = typeof(T);
            if (_lookUp.TryGetValue(handler, out Action<ChannelEvent> action))
            {
                if (_events.TryGetValue(eventType, out Action<ChannelEvent> internalAction))
                {
                    internalAction -= action;
                    if (internalAction == null) //구독을 해제하고 없었다면 딕셔너리에서 action제거
                    {
                        _events.Remove(eventType);
                    }
                    else //남아있다면 다시 넣어줌.
                    {
                        _events[eventType] = internalAction;
                    }
                }
                _lookUp.Remove(handler); //룩업테이블에서는 제거.
            }
        }

        public void RaiseEvent(ChannelEvent evt)
        {
            if (_events.TryGetValue(evt.GetType(), out Action<ChannelEvent> action))
            {
                action?.Invoke(evt);
            }
        }

        public void Clear()
        {
            _events.Clear();
            _lookUp.Clear();
        }
    }
}