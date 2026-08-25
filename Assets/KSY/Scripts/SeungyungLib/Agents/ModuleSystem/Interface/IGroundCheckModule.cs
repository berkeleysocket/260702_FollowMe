using SeungyungLib.Core.NotifyValue;

namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IGroundCheckModule : IModule
    {
        NotifyValue<bool> NotifyIsGround { get; }
    }
}