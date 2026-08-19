using SeungyungLib.Core.Effects;
using SeungyungLib.ModuleSystem.Interface;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class AgentVfxModule : MonoBehaviour, IModule
    {
        private IModuleOwner _owner;
        private Dictionary<int, IPlayableVFX> _playableDict;
        
        public void Initialize(IModuleOwner owner)
        {
            _owner = owner;
            _playableDict = GetComponentsInChildren<IPlayableVFX>()
                .ToDictionary(vfx => vfx.VfxName.Hash);
        }

        public void PlayVfx(int hash, Vector3 position, Quaternion rotation)
        {
            if (_playableDict.TryGetValue(hash, out var vfx))
            {
                vfx.PlayVFX(position, rotation);
            }
            else
            {
                Debug.LogWarning($"VFX with hash : {hash} not found");
            }
        }

        public void PlayVfx(int hash)
        {
            if (_playableDict.TryGetValue(hash, out var vfx))
            {
                vfx.PlayVFX();
            }
            else
            {
                Debug.LogWarning($"VFX with hash : {hash} not found");
            }
        }

        public void StopVfx(int hash)
        {
            if (_playableDict.TryGetValue(hash, out var vfx))
            {
                vfx.StopVFX();
            }
        }
    }
}