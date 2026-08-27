using System.IO;
using FollowMe.KDS;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FollowMe.KDS.Editor
{
    [CustomEditor(typeof(DialogueSequenceSO))]
    public class DialogueSequenceSOEditor : UnityEditor.Editor
    {
        private SerializedProperty _sequenceIdProp;
        private SerializedProperty _jsonFileNameProp;
        private SerializedProperty _linesProp;
        private ReorderableList _linesList;

        private void OnEnable()
        {
            _sequenceIdProp = serializedObject.FindProperty("SequenceId");
            _jsonFileNameProp = serializedObject.FindProperty("JsonFileName");
            _linesProp = serializedObject.FindProperty("Lines");

            _linesList = new ReorderableList(serializedObject, _linesProp, true, true, true, true);
            _linesList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Dialogue Lines");
            _linesList.elementHeight = EditorGUIUtility.singleLineHeight * 6f + 12f;
            _linesList.drawElementCallback = DrawLineElement;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_sequenceIdProp);
            EditorGUILayout.PropertyField(_jsonFileNameProp);
            EditorGUILayout.Space(6f);

            _linesList.DoLayoutList();

            EditorGUILayout.Space(8f);
            if (GUILayout.Button("Export To KDS JSON"))
            {
                ExportToJson((DialogueSequenceSO)target);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void ExportToJson(DialogueSequenceSO source)
        {
            DialogueSequenceJson sequenceJson = source.ToRuntime();

            string json = JsonUtility.ToJson(sequenceJson, true);
            Directory.CreateDirectory(DialogueJsonPaths.GetFolderFullPath());

            string fileName = string.IsNullOrWhiteSpace(source.JsonFileName) ? source.name : source.JsonFileName;
            string fullPath = DialogueJsonPaths.GetFullPath(fileName);
            File.WriteAllText(fullPath, json);
            AssetDatabase.Refresh();

            Debug.Log($"[DialogueSequenceSOEditor] JSON Exported: {DialogueJsonPaths.GetAssetPath(fileName)}");
        }

        private void DrawLineElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _linesProp.GetArrayElementAtIndex(index);
            rect.y += 2f;

            float lineH = EditorGUIUtility.singleLineHeight;
            float gap = 2f;

            var characterId = element.FindPropertyRelative("CharacterId");
            var expressionId = element.FindPropertyRelative("ExpressionId");
            var text = element.FindPropertyRelative("Text");
            var autoAdvance = element.FindPropertyRelative("AutoAdvance");
            var autoAdvanceSeconds = element.FindPropertyRelative("AutoAdvanceSeconds");

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, lineH), characterId);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineH + gap), rect.width, lineH), expressionId);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineH + gap) * 2f, rect.width, lineH * 2f), text);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineH + gap) * 4f, rect.width * 0.48f, lineH), autoAdvance);
            EditorGUI.PropertyField(new Rect(rect.x + rect.width * 0.52f, rect.y + (lineH + gap) * 4f, rect.width * 0.48f, lineH), autoAdvanceSeconds);
        }
    }
}
