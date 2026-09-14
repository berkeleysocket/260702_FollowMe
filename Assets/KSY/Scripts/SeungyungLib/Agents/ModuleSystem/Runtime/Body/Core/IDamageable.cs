namespace SeungyungLib.Md.Body.Core
{
    public interface IDamageable
    {
        public void ApplyDamage(DamageContext damageContext);
        void Recovery(int recoveryValue);
    }
}
