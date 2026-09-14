using SeungyungLib.ModuleSystem.Core;

using System;
using UnityEngine;

namespace SeungyungLib.Md.Body.Core
{
    public interface IBodyModule : IModule, IDamageable
    {
        public delegate void OnTakeDamageHandler(int damage, int currentHealth);
        
        event OnTakeDamageHandler OnDamaged;
        event Action OnKnockdown;
        event Action OnStandUp;
        event Action OnDeath;
        
        public Rigidbody2D PhysicalBody { get; }
        public int MaxHealth { get; }
        public int CurrentHealth { get; }
        bool IsDead { get; }
        bool IsKnockdown { get; }
        bool IsInvincible { get; }

        void Knockdown();
        void StandUp();
    }
}