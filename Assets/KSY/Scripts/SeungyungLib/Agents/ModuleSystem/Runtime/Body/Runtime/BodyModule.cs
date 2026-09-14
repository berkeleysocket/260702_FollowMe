using SeungyungLib.Core.BaseCollider;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.ReadOnlyAttribute;
using SeungyungLib.Md.Body.Core;
using SeungyungLib.ModuleSystem.Core;

using System;
using System.Collections;
using UnityEngine;

namespace SeungyungLib.Md.Body.Runtime
{
    public class BodyModule : MonoBehaviour, IBodyModule
    {
        [field: SerializeField] public Rigidbody2D PhysicalBody { get; private set; }
        [SerializeField] private BaseCollider bodyCollider;
        [SerializeField] private BodyModuleDataSO bodyData;
        [SerializeField, ReadOnly] private int _health;

        private int _maxHealth;
        private int _asd;
        
        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _health;
        public bool IsActive { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsKnockdown { get; private set; }
        public bool IsInvincible { get; private set; }

        public event IBodyModule.OnTakeDamageHandler OnDamaged;
        public event Action OnDeath;
        public event Action OnKnockdown;
        public event Action OnStandUp;

        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            this._maxHealth = bodyData.MaxHealth; 
            this._health = _maxHealth;
            
            DebugLogger.Assert(PhysicalBody != null, "[BodyModule]: PhysicalBody is null");
            DebugLogger.Assert(bodyCollider != null, "[BodyModule]: enemyLayerCollider is null");
            DebugLogger.Assert(bodyData != null, "[BodyModule]: bodyData is null");
        }
        #endregion
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void ApplyDamage(DamageContext damageContext)
        {
            int damage = damageContext.Value;
            
            if (IsKnockdown || IsInvincible || IsDead) return;
            
            _health = Mathf.Clamp(_health - damage, 0, _maxHealth);

            if (_health <= 0)
            {
                OnDeath?.Invoke();
                IsDead = true;
            }
            else
                OnDamaged?.Invoke(damage, _health);
        }

        public void Knockdown()
        {
            if (IsKnockdown) return;
            IsKnockdown = true;
            OnKnockdown?.Invoke();
        }

        public void Recovery(int recoveryValue)
        {
            _health = Mathf.Clamp(_health + recoveryValue, 0, _maxHealth);
        }

        public void StandUp()
        {
            if (!IsKnockdown && IsInvincible) return;
            IsInvincible = true;
            IsKnockdown = false;
            OnStandUp?.Invoke();
        }

        private IEnumerator IsInvincibleTimer()
        {
            yield return new WaitForSeconds()
        }
    }
}