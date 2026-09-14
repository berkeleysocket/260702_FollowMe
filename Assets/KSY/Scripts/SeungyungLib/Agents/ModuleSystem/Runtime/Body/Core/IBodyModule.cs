using SeungyungLib.ModuleSystem.Core;

using System;
using UnityEngine;

namespace SeungyungLib.Md.Body.Core
{
    public interface IBodyModule : IModule, IDamageable
    {
        event Action OnDamaged;
        event Action OnKnockdown;
        event Action OnStandUp;
        
        public Rigidbody2D PhysicalBody { get; }

        bool IsKnockdown { get; }
        bool IsInvincible { get; }

        void Knockdown();
        void StandUp();
    }
}