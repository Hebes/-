#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// 生成组件
/// </summary>
public class GenerateComponent
{
    private const string CommonTitle = "GameObject/组件工具/";
    // #region 获取组件和变量
    //
    // [MenuItem("GameObject/获取组件/TweenColor", false, 1)]
    // private static void GetAll()
    // {
    //     string str1 = GetComponent<TweenColor>();
    //     string str2 = GetValue<TweenColor>();
    //     Debug.Log(str1 + "\n" + str2);
    //     $"{str1}\n{str2}".Copy();
    // }
    //
    // #endregion

    #region Other

    [MenuItem(CommonTitle + "清空PlayerPrefs", false, 1)]
    private static void Tool1() => PlayerPrefs.DeleteAll();

    #endregion

    #region 获取变量

    [MenuItem(CommonTitle + "获取变量/Text", false, 1)]
    private static void GetValue1() => GetValue<Text>();

    [MenuItem(CommonTitle + "获取变量/CanvasGroup", false, 1)]
    private static void GetValue2() => GetValue<CanvasGroup>();

    #endregion

    #region 获取组件

    [MenuItem(CommonTitle + "获取组件/Button", false, 1)]
    private static void GetComponent1() => GetComponent<Button>();

    [MenuItem(CommonTitle + "获取组件/Text", false, 1)]
    private static void GetComponent2() => GetComponent<Text>();

    [MenuItem(CommonTitle + "获取组件/CanvasGroup", false, 1)]
    private static void GetComponent3() => GetComponent<CanvasGroup>();

    [MenuItem(CommonTitle + "获取组件/EventTrigger", false, 1)]
    private static void GetComponent4() => GetComponent<EventTrigger>();

    [MenuItem(CommonTitle + "获取组件/Slider", false, 1)]
    private static void GetComponent5() => GetComponent<Slider>();

    // [MenuItem("GameObject/获取组件/UIProgressBar2", false, 1)]
    // private static void GetComponent6() => GetComponent<UIProgressBar2>();

    [MenuItem(CommonTitle + "获取组件/Image", false, 1)]
    private static void GetComponent7() => GetComponent<Image>();

    [MenuItem(CommonTitle + "获取组件/Animator", false, 1)]
    private static void GetComponent8() => GetComponent<Animator>();

    [MenuItem(CommonTitle + "获取组件/TMP_Text", false, 1)]
    private static void GetComponent9() => GetComponent<TMP_Text>();

    [MenuItem(CommonTitle + "获取组件/TextMeshProUGUI", false, 1)]
    private static void GetComponent10() => GetComponent<TextMeshProUGUI>();

    [MenuItem(CommonTitle + "获取组件/TMP_InputField", false, 1)]
    private static void GetComponent11() => GetComponent<TMP_InputField>();

    [MenuItem(CommonTitle + "获取组件/VerticalLayoutGroup", false, 1)]
    private static void GetComponent12() => GetComponent<VerticalLayoutGroup>();
    [MenuItem(CommonTitle + "获取组件/Transform", false, 1)]
    private static void GetComponent13() => GetComponent<Transform>();
    #endregion

    /// <summary>
    /// 将isValidateFunction参数设置为true
    /// 并且路径以及按钮名称需要一模一样，表示下面我们声明的方法是对应按钮的验证方法
    /// 方法的返回值一定要是bool类型的，当返回值为true时表示执行下面的方法
    /// 当每次弹出选择栏的时候便会执行一次这个方法
    /// </summary>
    private static string GetComponent<T>() where T : Component
    {
        Object obj = Selection.activeObject;
        if (!obj) throw new Exception("当前物体为获取到，可能是隐藏");
        StringBuilder sb = new StringBuilder(); //字符串
        //获取当前选择的物体组件
        T t1 = (obj as GameObject)?.GetComponent<T>();
        if (t1)
            sb.AppendLine($"{obj.name} = GetComponent<{typeof(T)}>();");
        //获取子物体组件
        List<T> values = new List<T>();
        GetAllChild((obj as GameObject)?.transform, ref values);
        for (int i = 0; i < values?.Count; i++)
        {
            string str = GetParentPath<T>(values[i], obj.name);
            sb.AppendLine($"{values[i].name} = transform.Find(\"{str}\").GetComponent<{typeof(T)}>();");
        }

        //打印
        Debug.Log(sb.ToString());
        Copy(sb.ToString());
        return sb.ToString();
    }

    /// <summary>
    /// 获取子物体
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="values"></param>
    /// <typeparam name="T"></typeparam>
    private static void GetAllChild<T>(Transform transform, ref List<T> values) where T : Component
    {
        for (int i = 0; i < transform?.childCount; i++)
        {
            T t = transform.GetChild(i).GetComponent<T>();
            if (t)
            {
                values.Add(t);
            }

            GetAllChild(transform.GetChild(i), ref values);
        }
    }

    /// <summary>
    /// 获取到父物体路径
    /// </summary>
    /// <param name="t"></param>
    /// <param name="parentName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static string GetParentPath<T>(T t, string parentName) where T : Component
    {
        List<string> path = new List<string>();
        StringBuilder sb = new StringBuilder();
        Transform flagTf = t.transform;
        while (true)
        {
            path.Add(flagTf.name);
            flagTf = flagTf.parent;
            if (flagTf.name.Equals(parentName))
                break;
        }

        for (int i = path.Count - 1; i >= 0; i--)
        {
            sb.Append(path[i]);
            if (i == 0) continue;
            sb.Append("/");
        }

        return sb.ToString();
    }

    /// <summary>
    /// [SerializeField]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static string GetValue<T>() where T : Component
    {
        Object obj = Selection.activeObject;
        if (!obj) throw new Exception("当前物体为获取到，可能是隐藏");
        StringBuilder sb = new StringBuilder(); //字符串
        T t1 = (obj as GameObject)?.GetComponent<T>();
        if (t1)
            sb.AppendLine($"public {typeof(T)} {obj.name};");
        List<T> values = new List<T>();
        GetAllChild((obj as GameObject)?.transform, ref values);
        for (int i = 0; i < values?.Count; i++)
        {
            string str = GetParentPath<T>(values[i], obj.name);
            sb.AppendLine($"public {typeof(T)} {values[i].name};");
        }

        //打印
        Debug.Log(sb.ToString());
        Copy(sb.ToString());
        return sb.ToString();
    }

    /// <summary>
    /// 复制
    /// </summary>
    /// <param name="str"></param>
    public static void Copy(string str)
    {
        TextEditor te = new TextEditor { text = str };
        te.SelectAll();
        te.Copy();
    }
}
#endif