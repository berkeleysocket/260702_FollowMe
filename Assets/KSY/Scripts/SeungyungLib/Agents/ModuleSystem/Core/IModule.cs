namespace SeungyungLib.ModuleSystem.Core
{
    public interface IModule
    {
        bool IsActive { get; }
        public void Activate();
        public void Deactivate();
        
        void Initialize(IModuleOwner owner);
    }
}