using SeungyungLib.Core.EventChannelSystem;

namespace SeungyungLib.Template.EventChannels
{
    public static class PlayerEvents 
    {
        public static PlayerDeadEvent DeadEvent { get; private set; } = new PlayerDeadEvent();
    }
    
    public class PlayerDeadEvent : ChannelEvent
    {
        
    }
}