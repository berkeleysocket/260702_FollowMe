using SeungyungLib.Core.ManagerSystem;
using SeungyungLib.Core.ObjectPool;

using UnityEngine;

namespace SeungyungLib.Template.Managements
{
    public class PoolManagement : MonoBehaviour, IManagement
    {
        [field: SerializeField] public PoolManagerSo PoolManagerAsset { get; private set; }

        public void Initialize() => PoolManagerAsset.InitializePool(transform);
    }
}