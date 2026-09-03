using UnityEditor;
using UnityEngine;

namespace SeungyungLib.Core.ReadOnlyAttribute
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // GUI 활성화 상태를 저장 후 비활성화(Disable) 처리
            GUI.enabled = false;

            // 비활성화된 상태로 Property 그리기
            EditorGUI.PropertyField(position, property, label, true);

            // 다른 Property에 영향을 주지 않도록 GUI 상태 원복
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 배열/리스트나 복잡한 클래스 형태의 높이도 정상 계산
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}