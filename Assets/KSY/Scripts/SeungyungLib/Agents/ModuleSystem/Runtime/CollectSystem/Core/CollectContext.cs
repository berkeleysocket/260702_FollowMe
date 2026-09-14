using SeungyungLib.ModuleSystem.Core;

namespace SeungyungLib.CollectSystem
{
    public readonly struct CollectContext
    {
        public CollectContext(IModuleOwner collector, ICollectable collectable)
        {
            this.collector = collector;
            this.collectable = collectable;
        }
        
        public readonly IModuleOwner collector;
        public readonly ICollectable collectable;
    }
}
