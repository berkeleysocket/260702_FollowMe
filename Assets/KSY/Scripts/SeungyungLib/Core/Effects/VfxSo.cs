using SeungyungLib.Core.ParameterSO;

using UnityEngine;

namespace SeungyungLib.Core.Effects
{
    [CreateAssetMenu(fileName = "VfxSO", menuName = "SeungyungLib/Core/Effects/VfxSO")]
    public class VfxSo : ScriptableObject
    {
        [field: SerializeField] public AssetNameSO Name { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
    }
}