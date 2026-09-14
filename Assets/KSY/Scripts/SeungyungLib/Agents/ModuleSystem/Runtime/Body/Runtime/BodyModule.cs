using SeungyungLib.Core.BaseCollider;
using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Md.Body.Core;
using SeungyungLib.ModuleSystem.Core;

using System;
using System.Collections;
using KSY.StressSystem;
using SeungyungLib.Core.ManagerSystem;
using UnityEngine;

namespace SeungyungLib.Md.Body.Runtime
{
    public class BodyModule : MonoBehaviour, IBodyModule
    {
        [field: SerializeField] public Rigidbody2D PhysicalBody { get; private set; }
        [SerializeField] private BaseCollider bodyCollider;
        [SerializeField] private BodyModuleDataSO bodyData;
        
        private IRenderModule _renderModule;
        private float _invincibilityDuration;

        public bool IsHit { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsKnockdown { get; private set; }
        public bool IsInvincible { get; private set; }

        public event Action OnDamaged;
        public event Action OnKnockdown;
        public event Action OnStandUp;

        #region Initialization
        public void Initialize(IModuleOwner owner)
        {
            this._renderModule = owner.GetModule<IRenderModule>();
            this._invincibilityDuration = bodyData.InvincibilityDuration;
            
            DebugLogger.Assert(PhysicalBody != null, "[BodyModule]: PhysicalBody is null");
            DebugLogger.Assert(bodyCollider != null, "[BodyModule]: enemyLayerCollider is null");
            DebugLogger.Assert(bodyData != null, "[BodyModule]: bodyData is null");
        }
        #endregion
        
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void ApplyDamage(DamageContext damageContext)
        {
            if (IsKnockdown || IsInvincible || IsHit) return;

            int damage = damageContext.Value;
            
            IsHit = true;
            StressManagement stressManagement = GameManager.Instance.GetManagement<StressManagement>();
            stressManagement.SetStress(damage);
        }

        public void Knockdown()
        {
            if (IsKnockdown) return;
            IsHit = false;
            IsKnockdown = true;
            OnKnockdown?.Invoke();
        }

        public void Recovery(int recoveryValue)
        {
            StressManagement stressManagement = GameManager.Instance.GetManagement<StressManagement>();
            stressManagement.SetStress(-recoveryValue);
        }

        public void StandUp()
        {
            DebugLogger.Log("StandUp");
            if (!IsKnockdown && IsInvincible) return;
            IsInvincible = true;
            IsKnockdown = false;
            OnStandUp?.Invoke();

            StartCoroutine(IsInvincibleTimer());
        }

        private IEnumerator IsInvincibleTimer()
        {
            if (!IsInvincible) yield break;
            _renderModule.PlayInvincibilityEffect(true);
            yield return new WaitForSeconds(_invincibilityDuration);
            _renderModule.PlayInvincibilityEffect(false);
            IsInvincible = false;
        }
    }
}