using System.Reflection;
using UnityEditor;
using UnityEngine;

public static partial class EditorMethod
{
    /// <summary>
    /// 用于继承自EditorWindow
    /// 用法：private void OnEnable() => this.Load();private void OnDisable() => this.Save();
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void Save(this object obj, string value = default)
    {
        //if (!EditorWindow.HasOpenInstances<DataTools>()) return;
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo[] fieldsValue = obj.GetType().GetFields(bindingFlags);
        foreach (FieldInfo data in fieldsValue)
        {
            if (data.IsLiteral) continue; //反射判断这个字段是否是const字段
            if (data.IsInitOnly) continue; //反射判断这个字段是否是readonly字段
            if (data.FieldType == typeof(string))
                PlayerPrefs.SetString($"{Application.productName}{value}{data.Name}Save", (string)data.GetValue(obj));
            else if (data.FieldType == typeof(int))
                PlayerPrefs.SetInt($"{Application.productName}{value}{data.Name}Save", (int)data.GetValue(obj));
        }

        UnityEngine.Debug.Log("保存成功");
    }

    /// <summary>
    /// 用于继承自EditorWindow
    /// 用法：private void OnEnable() => this.Load();private void OnDisable() => this.Save();
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void Load(this object obj, string value = default)
    {
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo[] fieldsValue = obj.GetType().GetFields(bindingFlags);
        foreach (var data in fieldsValue)
        {
            if (data.IsLiteral) continue; //反射判断这个字段是否是const字段
            if (data.IsInitOnly) continue; //反射判断这个字段是否是readonly字段

            if (data.FieldType == typeof(string))
                data.SetValue(obj, PlayerPrefs.GetString($"{Application.productName}{value}{data.Name}Save"));
            if (data.FieldType == typeof(int))
                data.SetValue(obj, PlayerPrefs.GetInt($"{Application.productName}{value}{data.Name}Save"));
        }

        UnityEngine.Debug.Log("加载成功");
    }

    /// <summary>
    /// 用于继承自EditorWindow
    /// 用法： [MenuItem(CommonTitle + "生成配置文件")]
    /// </summary>
    /// <param name="title"></param>
    public static void ShowUI<T>(this string title) where T : EditorWindow
    {
        if (!EditorWindow.HasOpenInstances<T>())
            EditorWindow.GetWindow(typeof(T), false, title).Show();
        else
            EditorWindow.GetWindow(typeof(T)).Close();
    }
}