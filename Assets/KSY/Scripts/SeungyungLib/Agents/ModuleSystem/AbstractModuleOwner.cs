using SeungyungLib.Agents.ModuleSystem.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeungyungLib.Agents.ModuleSystem
{
    public abstract class AbstractModuleOwner : MonoBehaviour, IModuleOwner
    {
        protected Dictionary<Type, IModule> _moducleDict;

        protected virtual void Awake()
        {
            _moducleDict = GetComponentsInChildren<IModule>()
                .ToDictionary(module => module.GetType());

            InitializeComponents();
            AfterInitComponents();
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }
        
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