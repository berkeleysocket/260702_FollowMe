using UnityEngine;

namespace SeungyungLib.Core.Effects
{
    public abstract class PlayableVfx : MonoBehaviour, IPlayableVfx
    {
        [field: SerializeField] private VfxSo vfxSo;

        public int NameHash => vfxSo.Name.Hash;

        #region Initialization
        public virtual void Initialize() {}
        #endregion

        public abstract void PlayVfx(Vector3 position, Quaternion rotation);
        public abstract void PlayVfx(bool isFlip);
        public abstract void PlayVfx();
        public abstract void StopVfx();
    }
}