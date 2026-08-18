using UnityEditor;
using UnityEngine;

namespace SeungyungLib.Agents.FSM.Editor
{
    [CustomPropertyDrawer(typeof(StateHierarchyTransition))]
    public class StateHierarchyTransitionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.managedReferenceValue == null)
            {
                property.managedReferenceValue = new StateHierarchyTransition();
                property.serializedObject.ApplyModifiedProperties();
            }

            property.isExpanded = true;

            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            if (iterator.NextVisible(true))
            {
                float currentY = position.y;

                while (!SerializedProperty.EqualContents(iterator, endProperty))
                {
                    float elementHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    Rect elementRect = new Rect(position.x, currentY, position.width, elementHeight);

                    EditorGUI.PropertyField(elementRect, iterator, true);

                    currentY += elementHeight + EditorGUIUtility.standardVerticalSpacing;
                    if (!iterator.NextVisible(false)) break;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.managedReferenceValue == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float totalHeight = 0f;
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            if (iterator.NextVisible(true))
            {
                while (!SerializedProperty.EqualContents(iterator, endProperty))
                {
                    totalHeight += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                    if (!iterator.NextVisible(false)) break;
                }
            }

            if (totalHeight > 0)
            {
                totalHeight -= EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }
    }
}