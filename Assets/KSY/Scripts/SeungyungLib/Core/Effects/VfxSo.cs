using SeungyungLib.Core.ParameterSO;

using UnityEngine;

namespace SeungyungLib.Core.Effects
{
    [CreateAssetMenu(fileName = "VFXSO", menuName = "SeungyungLib/Core/Effects/VFXSO")]
    public class VfxSo : ScriptableObject
    {
        [field: SerializeField] public AssetNameSO Name { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
    }
}