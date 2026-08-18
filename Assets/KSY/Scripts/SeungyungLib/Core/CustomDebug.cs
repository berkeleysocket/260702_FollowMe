using System.Diagnostics;
using UnityEngine;

namespace SeungyungLib.Core
{
    public static class CustomDebug
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log($"[DEBUG] {message}");
        }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, Color color)
        {
            string hexColor = ColorUtility.ToHtmlStringRGB(color);
            UnityEngine.Debug.Log($"[DEBUG] <color=#{hexColor}>{message}</color>");
        }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning($"[WARNING] {message}");
        }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError($"[ERROR] {message}");
        }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogSuccess(object message)
        {
            UnityEngine.Debug.Log($"<color=green>[Success] {message}</color>");
        }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Assert(bool condition,object message)
        {
            UnityEngine.Debug.Assert(condition, message);
        }
    }
}