using System;
using UnityEngine;

public class DBug
{
    public static void Log(string message) => Debug.Log(message);
    public static void Log(object message) => Debug.unityLogger.Log(LogType.Log, message);
    public static void Log(int message) => Debug.unityLogger.Log(LogType.Log, message);
    public static void Error(string message) => Debug.LogError(message);
}

public static class LogExpand
{
    public static void Log(this string message) => DBug.Log(message);
    public static void Log(this int message) => DBug.Log(message);
    public static void Log(this object message) => DBug.Log(message);
    public static void Error(this string message) => Debug.LogError(message);
    public static void LogError(this object message) => Debug.LogError(message);
    public static void Error(this int message) => Debug.LogError(message);
    public static void LogError(this int message) => Debug.LogError(message);
    public static void Warning(this string message) => Debug.LogWarning(message);
}