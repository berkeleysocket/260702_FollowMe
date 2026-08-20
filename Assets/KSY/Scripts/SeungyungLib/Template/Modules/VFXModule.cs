using SeungyungLib.Core.Effects;
using SeungyungLib.ModuleSystem.Interface;
using SeungyungLib.Core.CustomDebug;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class VfxModule : MonoBehaviour, IVfxModule
    {
        private IModuleOwner _owner;
        private Dictionary<int, IPlayableVfx> _playableDict;
        
        public void Initialize(IModuleOwner owner)
        {
            this._owner = owner;
            this._playableDict = GetComponentsInChildren<IPlayableVfx>()
                .ToDictionary(vfx => vfx.NameHash);
            foreach (var vfx in _playableDict.Values)
                vfx.Initialize();
        }

        public void PlayVfx(int hash, Vector3 position, Quaternion rotation)
        {
            if (_playableDict.TryGetValue(hash, out var vfx))
                vfx.PlayVfx(position, rotation);
            else
                DebugLogger.LogError($"[VFXModule]: with hash : {hash} not found");
        }

        public void PlayVfx(int hash)
        {
            if (_playableDict.TryGetValue(hash, out var vfx))
                vfx.PlayVfx();
            else
                DebugLogger.LogError($"[VFXModule]: with hash : {hash} not found");
        }

        public void StopVfx(int hash)
        {
            if (_playableDict.TryGetValue(hash, out var vfx))
                vfx.StopVfx();
            else
                DebugLogger.LogError($"[VFXModule]: with hash : {hash} not found");
        }
    }
}