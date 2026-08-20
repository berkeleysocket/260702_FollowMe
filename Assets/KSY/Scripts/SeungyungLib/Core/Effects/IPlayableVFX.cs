using SeungyungLib.Core.ParameterSO;

using UnityEngine;

namespace SeungyungLib.Core.Effects
{
    public interface IPlayableVFX
    {
        public void Initialize();
        
        AssetNameSO VFXName { get; }
        float VfxDuration { get; }
        void PlayVFX(Vector3 position, Quaternion rotation);
        void PlayVFX();
        void StopVFX();
    }
}
