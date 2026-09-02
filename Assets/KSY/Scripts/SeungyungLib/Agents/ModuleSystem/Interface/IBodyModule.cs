using SeungyungLib.ModuleSystem.Interface;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Modules
{
    public interface IBodyModule : IModule
    {
        public delegate void OnTakeDamageHandler(int damage, int currentHealth);
        public delegate void OnDeathHandler();
        
        event OnTakeDamageHandler OnTakeDamage;
        event OnDeathHandler OnDeath;
        
        public Rigidbody2D Body { get; }
        
        void TakeDamage(int damage);
    }
}