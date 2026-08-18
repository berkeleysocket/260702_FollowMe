using UnityEngine;

namespace SeungyungLib.Core.ParameterSO
{
    [CreateAssetMenu(fileName = "AnimationParameterDataSO", menuName = "SeungyungLib/Utility/AnimationParameterData", order = 0)]
    public class AnimationParameterDataSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int Hash { get; private set; }

        private void OnValidate()
        {
            Hash = Animator.StringToHash(Name);
        }
    }
}