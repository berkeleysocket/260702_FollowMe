using System;
using SeungyungLib.ModuleSystem.Core;

namespace SeungyungLib.CollectSystem
{
    public interface ICollectModule : IModule, ICollector
    {
        event Action<CollectContext> OnCollected;
    }
}
