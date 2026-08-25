using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.Template.Modules
{
    public class ChaseMovementModule : IChaseMovementModule
    {
        public Transform targetTrm;
        
        public void Initialize(IModuleOwner owner)
        {
        }
    }
}
