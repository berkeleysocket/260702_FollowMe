using SeungyungLib.Core.CustomDebug;
using SeungyungLib.Core.EventChannelSystem;
using SeungyungLib.Core.InputSystem;
using SeungyungLib.Core.ManagerSystem;
using SeungyungLib.Template.EventChannels;

using UnityEngine;

namespace SeungyungLib.Template.Managements
{
    public class InputManagement : MonoBehaviour, IManagement
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
            inputReader.OnRunKeyPressed += (int axis)=>
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