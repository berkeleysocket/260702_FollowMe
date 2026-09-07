namespace SeungyungLib.ModuleSystem.Core
{
    public interface IAfterInitModule
    {
        void AfterInitialization(IModuleOwner owner);
    }
}