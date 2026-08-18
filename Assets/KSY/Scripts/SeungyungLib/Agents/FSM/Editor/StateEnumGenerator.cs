using SeungyungLib.Core;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SeungyungLib.Agents.FSM.Editor
{
    public static class StateEnumGenerator
    {
        private static readonly string FolderPath = "Assets/KSY/Scripts/SeungyungLib/FSM/Enum";
        private static readonly string ClassName = "StateHierarchyType"; 
        private static readonly string FilePath = $"{FolderPath}/{ClassName}.cs";
        
        [MenuItem("SeungyungLib/Generate State Enum")]
        public static void GenerateEnum()
        {
            if (!Directory.Exists(FolderPath))
            {
                CustomDebug.LogError("[StateEnumGenerator]: The folder doesn't exist!");
                return;
            }

            List<Type> stateTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(AbstractStateHierarchy)) 
                            && !t.IsAbstract                              
                            && !t.IsInterface)                            
                .OrderBy(t => t.Name)                                     
                .ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("namespace SeungyungLib.FSM.Enum");
            sb.AppendLine("{");
            sb.AppendLine($"    public enum {ClassName}");
            sb.AppendLine("    {");
            sb.AppendLine("        None = -1,"); 

            for (int i = 0; i < stateTypes.Count; i++)
            {
                string typeName = stateTypes[i].Name.Replace("StateHierarchy", "");
                string comma = (i == stateTypes.Count - 1) ? "" : ",";
                sb.AppendLine($"        {typeName}{comma}");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(FilePath, sb.ToString(), Encoding.UTF8);

            AssetDatabase.ImportAsset(FilePath);
            AssetDatabase.Refresh();

            Debug.Log($"<color=green><b>[EnumGenerator]</b></color> {stateTypes.Count}���� ���¸� ������� {ClassName}.cs ���� �Ϸ�!");
        }
    }
}