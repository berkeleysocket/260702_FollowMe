using SeungyungLib.ModuleSystem.Enum;

using System;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Core
{
    public interface IColliderModule
    {
        void RegisterAction(ColliderModuleOption moduleOption, Action<GameObject> action);
    }
}