using SeungyungLib.Core.EventChannelSystem;

namespace SeungyungLib.Template.EventChannels
{
    public static class PlayerEvents 
    {
        public static PlayerHitEvent PlayerHitEvent { get; private set; } = new PlayerHitEvent();
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
}
