namespace SeungyungLib.ModuleSystem.Interface
{
    public interface IModule
    {
        bool IsActive { get; }
        public void Activate();
        public void Deactivate();
        
        void Initialize(IModuleOwner owner);
    }
}