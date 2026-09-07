namespace SeungyungLib.FSM.Interface
{
    public interface IState
    {
        public ITransition[] Transitions { get; }
        
        public void Enter();
        public void Update();
        public void Exit();
    }
}
