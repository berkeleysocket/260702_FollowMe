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
        public int Axis { get; private set; }
        
        public void Initialize(int axis)
        {
            this.Axis = axis;
        }
    }

    public class JumpInputEvent : ChannelEvent
    {
        public bool JumpKeyPressed { get; private set; }

        public void Initialize(bool jumpKeyPressed)
        {
            this.JumpKeyPressed = jumpKeyPressed;
        }
    }
}
