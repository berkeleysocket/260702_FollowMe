namespace SeungyungLib.CollectSystem
{
    public interface ICollectable
    {
        CollectableSO Data { get; }
        
        void Collect(ICollector collector);
    }
}