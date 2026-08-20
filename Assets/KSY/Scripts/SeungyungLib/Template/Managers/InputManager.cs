using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.Core.InputSystem;
using SeungyungLib.Template.EventChannels;

using UnityEngine;

namespace SeungyungLib.Template.Managers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeReference] private InputReaderSO inputReader;
        [SerializeField] private EventChannelSO playerEventChannel;
        
        public void Initialize()
        {
            this.inputReader.Initialize();

            RegisterEvents();
        }

        public void RegisterEvents()
        {
            inputReader.OnRunKeyPressed += (float axis)=>
            {
                InputEvents.MoveInputEvent.Initialize(axis);
                playerEventChannel.RaiseEvent(InputEvents.MoveInputEvent);
            };

            inputReader.OnJumpKeyPressed += (bool isJumpKeyPressed) =>
            {
                InputEvents.JumpInputEvent.Initialize(isJumpKeyPressed);
                playerEventChannel.RaiseEvent(InputEvents.JumpInputEvent);
            };
        }
    }
}