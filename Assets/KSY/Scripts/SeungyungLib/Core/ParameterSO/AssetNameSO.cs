using UnityEngine;

namespace SeungyungLib.Core.ParameterSO
{
    [CreateAssetMenu(fileName = "AssetNameDataSO", menuName = "SeungyungLib/Utility/AssetNameData", order = 0)]
    public class AssetNameSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int Hash { get; private set; }

        private void OnValidate()
        {
            Hash = Animator.StringToHash(Name);
        }
    }
}