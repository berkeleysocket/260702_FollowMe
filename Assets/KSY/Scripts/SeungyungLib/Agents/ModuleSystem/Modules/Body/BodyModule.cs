using SeungyungLib.Core.ReadOnlyAttribute;
using SeungyungLib.ModuleSystem.Core;
using SeungyungLib.ModuleSystem.Enum;

using System;
using System.Collections;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Modules
{
    public class BodyModule : MonoBehaviour, IBodyModule
    {
        [field: SerializeField] public Rigidbody2D PhysicalBody { get; private set; }
        [SerializeField] private ColliderModule bodyCollider;
        [SerializeField] private BodyModuleDataSO bodyData;
        [SerializeField, ReadOnly] private int health;

        private int _maxHealth;
        private float _invincibilityDuration;

        public bool IsActive { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsKnockdown { get; private set; }
        public bool IsInvincible { get; private set; }

        public event IBodyModule.OnTakeDamageHandler OnDamaged;
        public event Action OnDeath;
        public event Action OnKnockdown;
        public event Action OnRecovery;

        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            this._maxHealth = bodyData.MaxHealth; 
            this.health = _maxHealth;
            this._invincibilityDuration = bodyData.InvincibilityDuration;
            bodyCollider.RegisterAction(ColliderModuleOption.Trigger | ColliderModuleOption.Enter,
                (other) => Damage(1));
        }
        #endregion
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void Damage(int damage)
        {
            if (health <= 0 || IsKnockdown || IsInvincible || IsDead) return;
            
            health = Mathf.Clamp(health - damage, 0, _maxHealth);

            if (health <= 0)
            {
                OnDeath?.Invoke();
                IsDead = true;
            }
            else
                OnDamaged?.Invoke(damage, health);
        }

        public void Knockdown()
        {
            if (IsKnockdown) return;
            IsKnockdown = true;
            OnKnockdown?.Invoke();
        }

        public void Recovery()
        {
            if (!IsKnockdown && IsInvincible) return;
            IsKnockdown = false;
            OnRecovery?.Invoke();

            StartCoroutine(SetInvincible());
        }
        
        private IEnumerator SetInvincible()
        {
            IsInvincible = true;
            
            yield return new WaitForSeconds(_invincibilityDuration);
            
            IsInvincible = false;
            
            if (bodyCollider.ContactCount > 0)
                Damage(1);
        }
    }
}