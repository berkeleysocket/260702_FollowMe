using SeungyungLib.Core.ParameterSO;

using UnityEngine;

namespace SeungyungLib.Core.Effects
{
    public interface IPlayableVfx
    {
        public int NameHash { get; }

        public void Initialize();
        void PlayVfx(Vector3 position, Quaternion rotation);
        void PlayVfx();
        void StopVfx();
    }
}