using SeungyungLib.Core.MonoSingleton;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SeungyungLib.Core.ManagerSystem
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private Dictionary<Type, IManagement> _managements;

        protected override void OnAwake()
        {
            _managements = GetComponentsInChildren<IManagement>().ToDictionary(
                key => key.GetType(),
                value => value
            );
            
            Initialize();
            AfterInitialize();
        }

        private void Initialize()
        {
            foreach (var management in _managements)
            {
                if (management.Value is IManagement)
                    management.Value.Initialize();
            }
        }

        private void AfterInitialize()
        {
            foreach (var management in _managements)
            {
                if (management.Value is IAfterInitManagement afterInitManagement)
                    afterInitManagement.AfterInitialize();
            }
        }

        public T GetManagement<T>() where T : class, IManagement
        {
            Type key = typeof(T);
            
            if (_managements.TryGetValue(key, out IManagement obj))
                return obj as T;

            return null;
        }
    }
}