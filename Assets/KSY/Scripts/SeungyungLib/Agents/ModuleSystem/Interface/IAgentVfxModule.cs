using UnityEngine;

namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IAgentVfxModule : IModule
    {
        void PlayVfx(int hash, Vector3 position, Quaternion rotation);
        void PlayVfx(int hash, bool isFlip);
        void PlayVfx(int hash);
        void StopVfx(int hash);
    }
}