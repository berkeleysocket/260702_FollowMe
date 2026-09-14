using SeungyungLib.ModuleSystem.Core;

namespace SeungyungLib.CollectSystem
{
    public interface ICollectable
    {
        void Collect(IModuleOwner collector);
    }
}