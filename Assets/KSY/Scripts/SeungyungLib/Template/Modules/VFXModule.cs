using SeungyungLib.Core.Effects;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.Core.CustomDebug;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class VFXModule : MonoBehaviour, IVFXModule
    {
        private IModuleOwner _owner;
        private Dictionary<int, IPlayableVFX> _playableDict;
        
        public void Initialize(IModuleOwner owner)
        {
            this._owner = owner;
            this._playableDict = GetComponentsInChildren<IPlayableVFX>()
                .ToDictionary(vfx => vfx.VFXName.Hash);
            foreach (var vfx in _playableDict.Values)
                vfx.Initialize();
        }

        public void PlayVfx(int hash, Vector3 position, Quaternion rotation)
        {
            if (_playableDict.TryGetValue(hash, out var vfx))
                vfx.PlayVFX(position, rotation);
            else
                DebugLogger.LogError($"[VFXModule]: with hash : {hash} not found");
        }

        public void PlayVfx(int hash)
        {
            if (_playableDict.TryGetValue(hash, out var vfx))
                vfx.PlayVFX();
            else
                DebugLogger.LogError($"[VFXModule]: with hash : {hash} not found");
        }

        public void StopVfx(int hash)
        {
            if (_playableDict.TryGetValue(hash, out var vfx))
                vfx.StopVFX();
            else
                DebugLogger.LogError($"[VFXModule]: with hash : {hash} not found");
        }
    }
}