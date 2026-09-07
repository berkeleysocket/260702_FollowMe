using UnityEngine;

namespace SeungyungLib.ModuleSystem.Modules
{
    [CreateAssetMenu(fileName = "BodyModuleDataSO", menuName = "SeungyungLib/ModuleSystem/Data/Body")]
    public class BodyModuleDataSO : ScriptableObject
    {
        [field: SerializeField] public int MaxHealth { get; private set; }
        [field: SerializeField] public float InvincibilityDuration { get; private set; }
    }
}
