namespace SeungyungLib.FSM.Interface
{
    public interface IState
    {
        public void Enter();
        public void Update();
        public void Exit();
    }
}
