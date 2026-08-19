namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IModuleOwner
    {
        T GetModule<T>() where T : IModule;
    }
}