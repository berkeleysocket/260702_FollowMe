using SeungyungLib.Core.CustomDebug;
using UnityEngine;

namespace SeungyungLib.Core.Effects
{
    public class PlayableParticleVfx : PlayableVfx
    {
        private ParticleSystem _particle;

        #region Initialization
        public override void Initialize()
        {
            base.Initialize();
            
            this._particle = GetComponent<ParticleSystem>();
            
            DebugLogger.Assert(_particle != null, "[PlayableParticleVfx]: ParticleSystem is null");
        }
        #endregion

        public override void PlayVfx(Vector3 position, Quaternion rotation)
        {
            this.transform.position = position;
            this.transform.rotation = rotation;
            _particle.Play();
        }

        public override void PlayVfx()
        {
            _particle.Play();
        }

        public override void StopVfx()
        {
            _particle.Stop();
        }
    }
}