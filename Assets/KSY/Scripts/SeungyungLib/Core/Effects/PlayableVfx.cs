using SeungyungLib.Core.CustomDebug;
using UnityEngine;

namespace SeungyungLib.Core.Effects
{
    public abstract class PlayableVfx : MonoBehaviour, IPlayableVfx
    {
        [field: SerializeField] private VfxSo vfxSo;

        public int NameHash => vfxSo.Name.Hash;
        protected float Duration => vfxSo.Duration;

        #region Initialization

        public virtual void Initialize() => DebugLogger.Assert(vfxSo != null, $"[{this}]: VfxSo is null");
        #endregion

        public abstract void PlayVfx(Vector3 position, Quaternion rotation);
        public abstract void PlayVfx();
        public abstract void StopVfx();
    }
}