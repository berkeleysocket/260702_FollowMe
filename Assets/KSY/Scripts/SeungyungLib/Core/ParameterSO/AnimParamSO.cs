using UnityEngine;

namespace SeungyungLib.Core.ParameterSO
{
    [CreateAssetMenu(fileName = "AnimParamSO", menuName = "SeungyungLib/Core/AnimParamSO", order = 0)]
    public class AnimParamSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int Hash { get; private set; }

        private void OnValidate()
        {
            Hash = Animator.StringToHash(Name);
        }
    }
}