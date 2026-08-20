using SeungyungLib.ModuleSystem.Interface;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IVfxModule : IModule
    {
        void PlayVfx(int hash, Vector3 position, Quaternion rotation);
        void PlayVfx(int hash);
        void StopVfx(int hash);
    }
}