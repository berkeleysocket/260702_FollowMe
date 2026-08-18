using SeungyungLib.Core.CustomDebug;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SeungyungLib.Agents.FSM.Editor
{
    public static class StateTypeEnumGenerator
    {
        private static readonly string FolderPath = "Assets/KSY/Scripts/SeungyungLib/Agents/FSM/Enum";
        private static readonly string ClassName = "StateType"; 
        private static readonly string FilePath = $"{FolderPath}/{ClassName}.cs";
        private static readonly string Namespace = "SeungyungLib.Agents.FSM.Enum";
        
        [MenuItem("SeungyungLib/Generate StateType Enum")]
        public static void GenerateEnum()
        {
            if (!Directory.Exists(FolderPath))
            {
                DebugLogger.LogError("[StateTypeEnumGenerator]: The folder doesn't exist!");
                return;
            }

            List<Type> stateTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(IState)) 
                            && !t.IsAbstract                              
                            && !t.IsInterface)                            
                .OrderBy(t => t.Name)                                     
                .ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append("namespace ");
            sb.AppendLine(Namespace);
            sb.AppendLine("{");
            sb.AppendLine($"    public enum {ClassName}");
            sb.AppendLine("    {");
            sb.AppendLine("        None = -1,"); 

            for (int i = 0; i < stateTypes.Count; i++)
            {
                string typeName = stateTypes[i].Name.Replace("State", "");
                string comma = (i == stateTypes.Count - 1) ? "" : ",";
                sb.AppendLine($"        {typeName}{comma}");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(FilePath, sb.ToString());
            
            AssetDatabase.Refresh();
            
            DebugLogger.Log("[StateTypeEnumGenerator]: Generated State Enum", Color.green);
        }
    }
}
