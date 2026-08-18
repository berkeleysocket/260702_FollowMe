using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SeungyungLib.Core.Input
{
    public class KeyRebindManager : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActions;
        
        // 현재 리바인딩 중인 작업. 내부 클래스인 이유는 외부에서 new()로 남용하면 안되기 때문이다.
        //외부에서 public으로 선언할 수 는 있지만 생성자는 internal이기 때문에 외부에서 함부로 new로 만들 수 없고 
        //InputActionRebindingExtensions의 확장 메서드를 통해서만 생성할 수 있다.
        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

        public void StartRebinding(string actionName, int bindingIndex, Action<string> onComplete)
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

                    string newBindingName = action.GetBindingDisplayString(bindingIndex);
                    onComplete?.Invoke(newBindingName);

                    SaveBindings();

                    operation.Dispose();
                    _rebindOperation = null;
                })
                .OnCancel(operation =>
                {
                    action.Enable();
                    operation.Dispose();
                    _rebindOperation = null;
                });

            _rebindOperation.Start();
        }

        private const string RebindsKey = "InputRebinds";

        private void SaveBindings()
        {
            string rebinds = _inputActions.ToJson();
            PlayerPrefs.SetString(RebindsKey, rebinds);
            PlayerPrefs.Save();
        }

        public void LoadBindings()
        {
            if (PlayerPrefs.HasKey(RebindsKey))
            {
                _inputActions.LoadFromJson(PlayerPrefs.GetString(RebindsKey));
            }
        }

        public void ResetAllBindings()
        {
            foreach (var map in _inputActions.actionMaps)
            {
                map.RemoveAllBindingOverrides();
            }
            PlayerPrefs.DeleteKey(RebindsKey);
        }
    }
}