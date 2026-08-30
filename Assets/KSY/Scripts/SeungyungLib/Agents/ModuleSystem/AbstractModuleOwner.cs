using SeungyungLib.ModuleSystem.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeungyungLib.ModuleSystem
{
    public abstract class AbstractModuleOwner : MonoBehaviour, IModuleOwner
    {
        private Dictionary<Type, IModule> _moduleDict;

        private void Awake()
        {
            Initialize();
            OnInitialized();
        }

        private void Initialize()
        {
            _moduleDict = GetComponentsInChildren<IModule>()
                .ToDictionary(module => module.GetType());

            InitializeComponents();
            AfterInitComponents();
        }
        protected virtual void OnInitialized() {}

        private void OnDestroy()
        {
            OnDestroyed();
        }
        protected virtual void OnDestroyed() {}

        public T GetModule<T>() where T : IModule
        {
            if(_moduleDict.TryGetValue(typeof(T), out IModule module))
                return (T) module;
            
            IModule findModule = _moduleDict.Values.FirstOrDefault(moduleType => moduleType is T);

            if (findModule is T castedModule)
                return castedModule;

            return default;
        }

        private void InitializeComponents()
        {
            foreach (IModule module in _moduleDict.Values)
                module.Initialize(this); 
        }
        
        private void AfterInitComponents()
        {
            foreach (IAfterInitModule module in _moduleDict.Values.OfType<IAfterInitModule>())
                module.AfterInitialization(this);
        }
    }
}