using SeungyungLib.Core.NotifyValue;

namespace SeungyungLib.ModuleSystem.Core
{
    public interface IGroundCheckModule : IModule
    {
        NotifyValue<bool> NotifyIsGrounded { get; }
    }
}