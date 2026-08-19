using SeungyungLib.ModuleSystem.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeungyungLib.ModuleSystem
{
    public abstract class AbstractModuleOwner : MonoBehaviour, IModuleOwner
    {
        protected Dictionary<Type, IModule> _moducleDict;

        private void Awake()
        {
            Initialize();
            OnInitialized();
        }

        public void Initialize()
        {
            _moducleDict = GetComponentsInChildren<IModule>()
                .ToDictionary(module => module.GetType());

            InitializeComponents();
            AfterInitComponents();
        }

        protected virtual void OnInitialized() {}

        public T GetModule<T>() where T : IModule
        {
            if(_moducleDict.TryGetValue(typeof(T), out IModule module))
                return (T) module;
            
            IModule findModule = _moducleDict.Values.FirstOrDefault(moduleType => moduleType is T);

            if (findModule is T castedModule)
                return castedModule;

            return default;
        }

        protected virtual void InitializeComponents()
        {
            foreach (IModule module in _moducleDict.Values)
            {
                module.Initialize(this); //오너를 자기로 셋팅하여 넣어준다.
            }
        }
        
        protected virtual void AfterInitComponents()
        {
            foreach (IAfterInitModule module in _moducleDict.Values.OfType<IAfterInitModule>())
            {
                module.AfterInit();
            }
        }
    }
}