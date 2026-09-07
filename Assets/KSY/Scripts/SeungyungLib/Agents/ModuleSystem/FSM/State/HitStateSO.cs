using SeungyungLib.FSM.Interface;
using SeungyungLib.ModuleSystem.Core;

using UnityEngine;

namespace SeungyungLib.FSM
{
    [CreateAssetMenu(fileName = "HitStateSO" + nameof(StateSO), menuName = "SeungyungLib/FSM/" + nameof(StateSO) + "/Hit", order = 0)]
    public class HitStateSO : StateSO
    {
        [Header("His State Settings")]
        [SerializeField] private float exitTime;
        protected override IState Create(IModuleOwner owner, int enterAnimHash, ITransition[] transitions)
        {
            return new HitState(owner, enterAnimHash, transitions, exitTime);
        }
    }
}