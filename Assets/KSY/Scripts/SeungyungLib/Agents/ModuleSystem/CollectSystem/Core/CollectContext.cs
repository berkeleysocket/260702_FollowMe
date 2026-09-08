namespace SeungyungLib.CollectSystem
{
    public readonly struct CollectContext
    {
        public CollectContext(ICollector collector, ICollectable collectable)
        {
            this.collector = collector;
            this.collectable = collectable;
        }
        
        public readonly ICollector collector;
        public readonly ICollectable collectable;
    }
}
