using SeungyungLib.Core.ParameterSO;

using UnityEngine;
using UnityEngine.VFX;

namespace SeungyungLib.Core.Effects
{
    [RequireComponent(typeof(VisualEffect))]
    public class PlayableVFX : MonoBehaviour, IPlayableVFX
    {
        [field: SerializeField] public AssetNameSO VFXName { get; private set; }
        [field: SerializeField] public float VfxDuration { get; private set; }
        
        private VisualEffect _vfx;

        public void Initialize()
        {
            _vfx = GetComponent<VisualEffect>();
        }
        
        public void PlayVFX(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
            _vfx.Play();
        }

        public void PlayVFX() => _vfx.Play();

        public void StopVFX() => _vfx.Stop();
    }
}
