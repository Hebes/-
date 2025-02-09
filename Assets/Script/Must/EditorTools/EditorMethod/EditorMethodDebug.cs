using UnityEngine;

public static partial class EditorMethod
{
    public static void EditorLog( this object message) => Debug.unityLogger.Log(LogType.Log, message);
}