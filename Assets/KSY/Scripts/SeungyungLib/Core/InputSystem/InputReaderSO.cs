using KSY.GameModules.InputActions;

using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace SeungyungLib.Core.InputSystem
{
    [CreateAssetMenu(fileName = "InputReaderSO", menuName = "SeungyungLib/Core/InputSystem/InputReaderSO", order = 0)]
    public class InputReaderSO : ScriptableObject, PlayerControls.IPlayerActions
    {
        public event Action<float> OnRunKeyPressed;
        public event Action<bool> OnJumpKeyPressed;

        private PlayerControls _controls;

        public void Initialize()
        {
            if (_controls == null)
            {
                _controls = new PlayerControls();
                _controls.Player.AddCallbacks(this);
            }

            _controls.Enable();
        }

        private void OnDisable()
        {
            if (_controls != null)
            {
                _controls.Player.RemoveCallbacks(this);
                _controls.Disable();
            }
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnRunKeyPressed?.Invoke(context.ReadValue<float>());
            else if (context.canceled)
                OnRunKeyPressed?.Invoke(0f);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnJumpKeyPressed?.Invoke(true);
            else if (context.canceled)
                OnJumpKeyPressed?.Invoke(false);
        }
    }
}