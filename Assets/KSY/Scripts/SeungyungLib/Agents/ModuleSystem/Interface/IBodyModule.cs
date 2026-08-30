using SeungyungLib.ModuleSystem.Interface;

namespace SeungyungLib.ModuleSystem.Modules
{
    public interface IBodyModule : IModule
    {
        public delegate void OnTakeDamageHandler(int damage, int currentHealth);
        public delegate void OnDeathHandler();
        
        event OnTakeDamageHandler OnTakeDamage;
        event OnDeathHandler OnDeath;
        
        void TakeDamage(int damage);
    }
}