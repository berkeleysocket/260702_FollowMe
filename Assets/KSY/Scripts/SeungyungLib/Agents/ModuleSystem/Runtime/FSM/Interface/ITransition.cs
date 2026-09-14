using SeungyungLib.FSM.Enum;

namespace SeungyungLib.FSM.Interface
{
    public interface ITransition
    {
        StateType TransitionTarget { get; }
        bool ConditionCheck();
    }
}
