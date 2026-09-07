namespace SeungyungLib.ModuleSystem.Core
{
    public interface IModuleOwner
    {
        T GetModule<T>() where T : IModule;
    }
}