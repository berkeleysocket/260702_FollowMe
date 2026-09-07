using System;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Core
{
    public interface IBodyModule : IModule
    {
        public delegate void OnTakeDamageHandler(int damage, int currentHealth);
        
        event OnTakeDamageHandler OnDamaged;
        event Action OnKnockdown;
        event Action OnRecovery;
        event Action OnDeath;
        
        public Rigidbody2D PhysicalBody { get; }
        bool IsDead { get; }
        bool IsKnockdown { get; }
        bool IsInvincible { get; }

        void Damage(int damage);
        void Knockdown();
        void Recovery();
    }
}