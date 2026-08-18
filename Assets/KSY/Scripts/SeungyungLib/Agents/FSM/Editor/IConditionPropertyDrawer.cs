using SeungyungLib.Agents.FSM.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SeungyungLib.Agents.FSM.Editor
{
    [CustomPropertyDrawer(typeof(ICondition))]
    public class IConditionPropertyDrawer : PropertyDrawer
    {
        private List<Type> _derivedTypes;
        private string[] _typeNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeTypeNames();

            EditorGUI.BeginProperty(position, label, property);

            Rect popupRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            string managedReferenceFullTypeName = property.managedReferenceFullTypename;
            int currentIndex = 0;

            if (!string.IsNullOrEmpty(managedReferenceFullTypeName))
            {
                string typeName = managedReferenceFullTypeName.Split(' ').Last().Split('.').Last().Split('/').Last();
                int foundIndex = _derivedTypes.FindIndex(t => t.Name == typeName);
                if (foundIndex != -1) currentIndex = foundIndex + 1;
            }

            int selectedIndex = EditorGUI.Popup(popupRect, label.text, currentIndex, _typeNames);

            if (selectedIndex != currentIndex)
            {
                if (selectedIndex == 0)
                {
                    property.managedReferenceValue = null;
                }
                else
                {
                    Type targetType = _derivedTypes[selectedIndex - 1];
                    property.managedReferenceValue = Activator.CreateInstance(targetType);
                }
                property.serializedObject.ApplyModifiedProperties();
                EditorGUI.EndProperty();
                return;
            }

            if (property.managedReferenceValue != null)
            {
                property.isExpanded = true;

                Rect childrenRect = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + 2,
                    position.width,
                    position.height - EditorGUIUtility.singleLineHeight - 2
                );

                EditorGUI.indentLevel++;

                float currentY = childrenRect.y;

                SerializedProperty isNotProp = property.FindPropertyRelative("isNot");
                if (isNotProp != null)
                {
                    float elementHeight = EditorGUI.GetPropertyHeight(isNotProp, true);
                    Rect elementRect = new Rect(childrenRect.x, currentY, childrenRect.width, elementHeight);

                    EditorGUI.PropertyField(elementRect, isNotProp, new GUIContent("Is Not"), true);

                    currentY += elementHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();

                if (iterator.NextVisible(true))
                {
                    while (!SerializedProperty.EqualContents(iterator, endProperty))
                    {
                        if (iterator.name == "isNot")
                        {
                            if (!iterator.NextVisible(false)) break;
                            continue;
                        }

                        float elementHeight = EditorGUI.GetPropertyHeight(iterator, true);
                        Rect elementRect = new Rect(childrenRect.x, currentY, childrenRect.width, elementHeight);

                        EditorGUI.PropertyField(elementRect, iterator, true);

                        currentY += elementHeight + EditorGUIUtility.standardVerticalSpacing;
                        if (!iterator.NextVisible(false)) break;
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue != null)
            {
                totalHeight += 2;

                SerializedProperty isNotProp = property.FindPropertyRelative("isNot");
                if (isNotProp != null)
                {
                    totalHeight += EditorGUI.GetPropertyHeight(isNotProp, true) + EditorGUIUtility.standardVerticalSpacing;
                }

                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();

                if (iterator.NextVisible(true))
                {
                    while (!SerializedProperty.EqualContents(iterator, endProperty))
                    {
                        if (iterator.name == "isNot")
                        {
                            if (!iterator.NextVisible(false)) break;
                            continue;
                        }

                        totalHeight += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                        if (!iterator.NextVisible(false)) break;
                    }
                }
            }

            return totalHeight;
        }

        private void InitializeTypeNames()
        {
            if (_derivedTypes != null) return;

            _derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(ICondition).IsAssignableFrom(t)
                            && !t.IsInterface
                            && !t.IsAbstract
                            && t.GetCustomAttribute<SerializableAttribute>() != null)
                .ToList();

            _typeNames = new string[_derivedTypes.Count + 1];
            _typeNames[0] = "Null (None)";
            for (int i = 0; i < _derivedTypes.Count; i++)
                _typeNames[i + 1] = _derivedTypes[i].Name;
        }
    }
}