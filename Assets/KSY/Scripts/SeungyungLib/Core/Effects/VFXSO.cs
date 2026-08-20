using SeungyungLib.Core.ParameterSO;

using UnityEngine;

namespace SeungyungLib.Core.Effects
{
    [CreateAssetMenu(fileName = "VFXSO", menuName = "SeungyungLib/Core/Effects/VFXSO")]
    public class VFXSO : ScriptableObject
    {
        [field: SerializeField] public AssetNameSO NameHash { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
    }
}