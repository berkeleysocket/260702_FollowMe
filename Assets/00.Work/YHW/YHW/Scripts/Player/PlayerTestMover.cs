using SeungyungLib.ModuleSystem.Interface;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YHW.Player
{
    public class PlayerTestMover : MonoBehaviour
    {
        private IControllableMovementModule movementModule;

        private void Awake()
        {
            movementModule = GetComponentInChildren<IControllableMovementModule>();
        }

        private void Update()
        {
            if (movementModule == null) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            float axis = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) axis -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) axis += 1f;

            // movementModule.MoveToDirection(axis);
            movementModule.IsJumpKeyPressed = keyboard.spaceKey.isPressed;
        }
    }
}
