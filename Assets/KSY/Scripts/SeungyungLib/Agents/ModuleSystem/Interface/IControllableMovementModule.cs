namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IControllableMovementModule : IMovementModule
    {
        bool IsJumpKeyPressed { get; set; }
    }
}
