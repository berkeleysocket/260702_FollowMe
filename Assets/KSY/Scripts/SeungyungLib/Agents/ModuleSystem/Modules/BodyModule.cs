using SeungyungLib.ModuleSystem.Interface;

using UnityEngine;

namespace SeungyungLib.ModuleSystem.Modules
{
    [RequireComponent(typeof(Collider2D))]
    public class BodyModule : MonoBehaviour, IBodyModule
    {
        [field: SerializeField] public Rigidbody2D Body { get; private set; }
        [SerializeField] private int health = 0;
        [SerializeField] private int maxHealth = 0;

        public bool IsActive { get; private set; }
        
        public event IBodyModule.OnTakeDamageHandler OnTakeDamage;
        public event IBodyModule.OnDeathHandler OnDeath;

        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            Debug.Assert(GetComponent<Collider2D>() != null, "[BodyModule]: Collider2D is null.");
        }
        #endregion
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void TakeDamage(int damage)
        {
            if (health <= 0) return;
            
            health = Mathf.Clamp(health - damage, 0, maxHealth);
            OnTakeDamage?.Invoke(damage, health);
        }
    }
}