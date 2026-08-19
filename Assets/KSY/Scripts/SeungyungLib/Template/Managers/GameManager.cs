using SeungyungLib.Core.MonoSingleton;

using UnityEngine;

namespace SeungyungLib.Template.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private InputManager inputManager;

        protected override void Awake()
        {
            base.Awake();

            inputManager.Initialize();
        }
    }
}
