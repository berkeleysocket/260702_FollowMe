using SeungyungLib.ModuleSystem.Interface;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IVFXModule : IModule
    {
        void PlayVfx(int hash, Vector3 position, Quaternion rotation);
        void PlayVfx(int hash);
        void StopVfx(int hash);
    }
}