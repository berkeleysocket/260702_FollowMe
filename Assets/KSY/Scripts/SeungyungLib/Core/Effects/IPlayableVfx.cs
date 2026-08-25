using UnityEngine;

namespace SeungyungLib.Core.Effects
{
    public interface IPlayableVfx
    {
        public int NameHash { get; }

        public void Initialize();
        public void PlayVfx(Vector3 position, Quaternion rotation);
        public void PlayVfx(bool isFlip);
        public void PlayVfx();
        public void StopVfx();
    }
}