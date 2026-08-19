using UnityEngine;
using UnityEngine.InputSystem;

namespace SeungyungLib.Core.InputSystem
{
    public class InputSetting : ScriptableObject
    { 
        private InputActionAsset _inputActions;
        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

        public void StartRebinding(string actionName, int bindingIndex)
        {
            //InputActionAsset안에 있는 ActionMap에 매핑된 Action속에서 같은 이름을 찾는다.
            //"Player"와 같이 이름 그대로 찾을 수 있지만 "Player/Move"와 같이 ActionMap을 앞에 붙여서 찾을 수도 있다.
            InputAction action = _inputActions.FindAction(actionName);
            if (action == null)
                return;
            
            action.Disable();
            
            _rebindOperation?.Dispose();

            _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation =>
                {
                    action.Enable();
                    
                    
                });
        }
    }
}
