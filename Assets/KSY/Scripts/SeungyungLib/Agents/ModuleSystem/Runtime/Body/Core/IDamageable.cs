using System;

namespace SeungyungLib.Md.Body.Core
{
    public interface IDamageable
    {
        bool IsHit { get; }

        void ApplyDamage(DamageContext damageContext);
        void Recovery(int recoveryValue);
    }
}
