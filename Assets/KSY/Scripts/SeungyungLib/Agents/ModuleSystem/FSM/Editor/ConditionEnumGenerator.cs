using SeungyungLib.Core.CustomDebug;
using SeungyungLib.FSM.Interface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SeungyungLib.FSM.Editor
{
    public static class ConditionEnumGenerator
    {
        private static readonly string FolderPath = "Assets/KSY/Scripts/SeungyungLib/Agents/ModuleSystem/FSM/Enum";
        private static readonly string ClassName = "ConditionType"; 
        private static readonly string FilePath = $"{FolderPath}/{ClassName}.cs";
        private static readonly string Namespace = "SeungyungLib.FSM.Enum";
        
        [MenuItem("SeungyungLib/Generate ConditionType Enum")]
        public static void GenerateEnum()
        {
            if (!Directory.Exists(FolderPath))
            {
                DebugLogger.LogError("[ConditionEnumGenerator]: The folder doesn't exist!");
                return;
            }
        
            List<Type> conditionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(ICondition).IsAssignableFrom(t) 
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
            
            List<string >existingEnumNames = GetExistingEnumNames();

            foreach (Type type in conditionTypes)
            {
                string typeName = type.Name.Replace("Condition", "");
                if (existingEnumNames.Contains(typeName))
                    continue;
                else existingEnumNames.Add(typeName);
            }
        
            for (int i = 0; i < existingEnumNames.Count; i++)
            {
                string typeName = existingEnumNames[i];
                string comma = (i == conditionTypes.Count - 1) ? "" : ",";
                sb.AppendLine($"        {typeName}{comma}");
            }
        
            sb.AppendLine("    }");
            sb.AppendLine("}");
        
            File.WriteAllText(FilePath, sb.ToString());
            
            AssetDatabase.Refresh();
            
            DebugLogger.Log("[ConditionEnumGenerator]: Generated Condition Enum", Color.green);
        }

        private static List<string> GetExistingEnumNames()
        {
            List<string> existingList = new List<string>();
            
            Type enumType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.FullName == Namespace + "." + ClassName
                               && type.IsEnum)
                .FirstOrDefault();

            if (enumType != null)
            {
                string[] names = Enum.GetNames(enumType);

                foreach (string name in names)
                {
                    if (name != "None")
                        existingList.Add(name);
                }
            }
            
            return existingList;
        }
    }
}