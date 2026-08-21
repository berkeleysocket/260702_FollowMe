using SeungyungLib.Core.NotifyValue;

namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IAgentGroundCheckModule : IModule
    {
        NotifyValue<bool> NotifyIsGround { get; }
    }
}