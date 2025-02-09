using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class EditorMethod
{
    /// <summary>
    /// 获取子物体
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="values"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static void GetAllChild<T>(this Transform transform, ref List<T> values, Action<T> action = null) where T : Component
    {
        T tValue = transform.GetComponent<T>();

        if (tValue)
        {
            values.Add(tValue);
            action?.Invoke(tValue);
        }

        for (int i = 0; i < transform.childCount; i++)
            GetAllChild<T>(transform.GetChild(i), ref values, action);
    }

    public static List<T> GetAllChild<T>(this Transform transform, Action<T> action = null) where T : Component
    {
        List<T> tList = new List<T>();
        T tValue = transform.GetComponent<T>();
        if (tValue)
        {
            tList.Add(tValue);
            action?.Invoke(tValue);
        }

        for (int i = 0; i < transform.childCount; i++)
            tList.AddRange(GetAllChild<T>(transform.GetChild(i), action));
        return tList;
    }

    public static List<Component> GetAllChild(this Transform transform, Type type, Action<Component> action = null)
    {
        List<Component> tList = new List<Component>();
        Component tValue = transform.GetComponent(type);
        if (tValue)
        {
            tList.Add(tValue);
            action?.Invoke(tValue);
        }

        for (int i = 0; i < transform.childCount; i++)
            tList.AddRange(GetAllChild(transform.GetChild(i), type, action));
        return tList;
    }

    public static void TryAddComponent<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject.GetComponent<T>() == null)
            gameObject.AddComponent<T>();
    }

    /// <summary>
    /// 获取到父物体路径
    /// </summary>
    /// <param name="t">组件</param>
    /// <param name="selectName">选中的物体的名称</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string GetParentPath<T>(this T t, string selectName) where T : Component
    {
        string path = t.name;
        Transform parent = t.transform.parent;
        while (parent != null)
        {
            if (parent.name.Equals(selectName))
                return path;
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return String.Empty;
    }
}