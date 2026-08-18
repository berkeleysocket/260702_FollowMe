using SeungyungLib.Agents.FSM.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SeungyungLib.Agents.FSM.Editor
{
    [CustomPropertyDrawer(typeof(IState))]
    public class IStatePropertyDrawer : PropertyDrawer
    {
        private List<Type> _derivedTypes;
        private string[] _typeNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //position.position(x, y): �ν����� â ���ο��� �ش� �׸��� ���۵Ǿ�� �ϴ� ���� ��� �ȼ� ��ǥ�Դϴ�.
            //position.size(width, height): �ν����� â�� ���� �ʺ�� �׸��� �⺻ ����(���� 1�� �з��� ������ EditorGUIUtility.singleLineHeight)�� ��� �ֽ��ϴ�.

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

            // ��Ӵٿ� �˾� ǥ��
            int selectedIndex = EditorGUI.Popup(popupRect, label.text + " (Type)", currentIndex, _typeNames);

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

            // 2. Ŭ������ �Ҵ�Ǿ� �ִٸ� �� ������ �ڽ� �ʵ���� �����ϰ� �׸���
            if (property.managedReferenceValue != null)
            {
                // �� ������Ƽ�� �ν����� �󿡼� ������ ����(Fold�� ���� ����)�� ���� ����
                property.isExpanded = true;

                // �˾� �޴� �ٷ� �Ʒ� �������� �ڽ� �ʵ���� �׸��� ������
                Rect childrenRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, position.height - EditorGUIUtility.singleLineHeight - 2);

                // ������ while (iterator.NextVisible) ���� ���, 
                // ����Ƽ ��ü�� PropertyField�� ��ü �ڽ� ����(Include Children = true)�� ������ �����ϰ� �׸����� �����մϴ�.
                // �� ����� ����ؾ� ������ ����Ʈ�� �迭�� + / - ��ư Ŭ�� �̺�Ʈ�� ���� �����˴ϴ�.
                EditorGUI.PropertyField(childrenRect, property, GUIContent.none, true);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // �⺻ ��Ӵٿ� ���̺� ����
            float totalHeight = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue != null)
            {
                // ����Ƽ ���� �ý����� ���� [SerializeReference] ���� �ʵ��(���� �迭 ����)�� ��ü ���̸� �����ϰ� ��°�� ����ؿ�
                totalHeight += 2 + EditorGUI.GetPropertyHeight(property, true);
            }

            return totalHeight;
        }

        private void InitializeTypeNames()
        {
            if (_derivedTypes != null) return;

            _derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(IStateHierarchy).IsAssignableFrom(t)
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