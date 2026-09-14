using SeungyungLib.ModuleSystem.Core;

using System;

namespace SeungyungLib.CollectSystem
{
    public interface ICollectModule : IModule
    {
        event Action<CollectContext> OnCollected;
    }
}
