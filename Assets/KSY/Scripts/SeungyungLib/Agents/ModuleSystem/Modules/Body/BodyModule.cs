using SeungyungLib.Core.CustomDebug;
using SeungyungLib.ModuleSystem.Core;
using SeungyungLib.ModuleSystem.Enum;

using System;
using System.Collections;
using UnityEngine;

namespace SeungyungLib.ModuleSystem.Modules
{
    [RequireComponent(typeof(Collider2D))]
    public class BodyModule : MonoBehaviour, IBodyModule
    {
        [field: SerializeField] public Rigidbody2D PhsicalBody { get; private set; }
        [SerializeField] private ColliderModule bodyCollider;
        [SerializeField] private BodyModuleDataSO bodyData;

        private int _health;
        private int _maxHealth;
        private float _invincibilityDuration;

        public bool IsActive { get; private set; }
        public bool IsKnockdown { get; private set; }
        public bool IsInvincible { get; private set; }

        public event IBodyModule.OnTakeDamageHandler OnDamaged;
        public event Action OnDeath;
        public event Action OnKnockdown;
        public event Action OnRecovery;

        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            Debug.Assert(GetComponent<Collider2D>() != null, "[BodyModule]: Collider2D is null.");
            this._maxHealth = bodyData.MaxHealth; 
            this._health = _maxHealth;
            this._invincibilityDuration = bodyData.InvincibilityDuration;
                
            bodyCollider.RegisterAction(ColliderModuleOption.Trigger | ColliderModuleOption.Enter, 
                (other)=>
                {
                    DebugLogger.Log("Registering body collider");
                    Damage(1);
                });
        }
        #endregion
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void Damage(int damage)
        {
            if (_health <= 0 || IsKnockdown || IsInvincible) return;
            
            _health = Mathf.Clamp(_health - damage, 0, _maxHealth);
            OnDamaged?.Invoke(damage, _health);
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
        }
    }
}