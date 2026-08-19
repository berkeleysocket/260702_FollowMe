using SeungyungLib.Core.EventChannelSystem;

namespace SeungyungLib.Template.EventChannels
{
    public static class InputEvents 
    {
        public static MoveInputEvent MoveInputEvent { get; private set; } = new MoveInputEvent();
        public static JumpInputEvent JumpInputEvent { get; private set; } = new JumpInputEvent();
    }

    public class MoveInputEvent : ChannelEvent
    {
        public void Initialize(float axis)
        {
            this.Axis = axis;
        }

        public float Axis { get; private set; }
    }

    public class JumpInputEvent : ChannelEvent
    {
    }
}
