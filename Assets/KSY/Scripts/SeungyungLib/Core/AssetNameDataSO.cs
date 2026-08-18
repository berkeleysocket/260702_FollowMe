using UnityEngine;

namespace SeungyungLib.Core
{
    [CreateAssetMenu(fileName = "AssetNameDataSO", menuName = "SeungyungLib/Utility/AssetNameData", order = 0)]
    public class AssetNameDataSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int Hash { get; private set; }

        private void OnValidate()
        {
            Hash = Animator.StringToHash(Name);
        }
    }
}