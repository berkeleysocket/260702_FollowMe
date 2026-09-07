using SeungyungLib.Core.EventChannelSystem;

namespace SeungyungLib.Template.EventChannels
{
    public static class PlayerEvents 
    {
        public static PlayerHitEvent HitEvent { get; private set; } = new PlayerHitEvent();
        public static PlayerKnockdownEvent KnockdownEvent { get; private set; } = new PlayerKnockdownEvent();
        public static PlayerRecoveryEvent RecoveryEvent { get; private set; } = new PlayerRecoveryEvent();
    }
    
    public class PlayerHitEvent : ChannelEvent
    {
        public int Damage { get; private set; }
        public int CurrentHealth { get; private set; }

        public void Initialize(int damage, int currentHealth)
        {
            this.Damage = damage;
            this.CurrentHealth = currentHealth;
        }
    }

    public class PlayerKnockdownEvent : ChannelEvent
    {
        
    }

    public class PlayerRecoveryEvent : ChannelEvent
    {
        
    }
}