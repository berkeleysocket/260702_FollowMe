using SeungyungLib.FSM.Enum;

namespace SeungyungLib.FSM.Interface
{
    public interface ITransition
    {
        public StateType TransitionTarget { get; }
        public bool Check();
    }
}
