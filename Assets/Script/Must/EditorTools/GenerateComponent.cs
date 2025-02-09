#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;


// public class GenerateComponentEditor : EditorWindow
// {
//     [SerializeField] private GameObject selectedObject;
//     [NonSerialized] private const string CommonTitle = "GameObject/组件工具/";
//     private string _componentName;
//     private string _content;
//     private Vector2 _scrollPosition;
//     private GUIStyle _centeredStyle;
//
//     [MenuItem(CommonTitle + "自定义组件获取")]
//     public static void ShowConfigToolUI() => "组件工具".ShowUI<GenerateComponentEditor>();
//
//     private void OnGUI()
//     {
//         GUI.backgroundColor = Color.yellow;
//         UnityEngine.Object obj = Selection.activeObject;
//         _componentName = EditorGUILayout.TextField("组件名称:", _componentName, GUILayout.ExpandWidth(true));
//         selectedObject = (GameObject)EditorGUILayout.ObjectField("选中的物体", obj, typeof(GameObject), true);
//         if (GUILayout.Button("搜索"))
//         {
//             if (string.IsNullOrEmpty(_componentName))return;
//             Type componentType = Type.GetType("UnityEngine." + _componentName);
//             string content = GenerateComponent.GetComponent(obj, componentType); //字符串转类
//             CodePreview(_centeredStyle, _scrollPosition, content);
//             Repaint();
//         }
//     }
//
//     /// <summary>
//     /// 代码预览
//     /// </summary>
//     private Vector2 CodePreview(GUIStyle centeredStyle, Vector2 scrollPosition, string content)
//     {
//         GUILayout.Space(10f);
//
//         centeredStyle ??= new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
//         EditorGUILayout.LabelField("预览", centeredStyle);
//
//         if (string.IsNullOrEmpty(content)) return default;
//         scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
//         EditorGUILayout.TextArea(content);
//         EditorGUILayout.EndScrollView();
//         return scrollPosition;
//     }
// }

/// <summary>
/// 生成组件
/// </summary>
public partial class GenerateComponent
{
    private const string CommonTitle = "GameObject/组件工具/";

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

    [MenuItem(CommonTitle + "获取组件/Camera", false, 1)]
    private static void GetComponent14() => GetComponent<Camera>();

    [MenuItem(CommonTitle + "获取组件/RawImage", false, 1)]
    private static void GetComponent15() => GetComponent<RawImage>();

    [MenuItem(CommonTitle + "获取组件/TMP_Dropdown", false, 1)]
    private static void GetComponent16() => GetComponent<TMP_Dropdown>();
    
    [MenuItem(CommonTitle + "获取组件/HorizontalLayoutGroup", false, 1)]
    private static void GetComponent17() => GetComponent<HorizontalLayoutGroup>();

    #endregion

    #region 添加组件

    [MenuItem(CommonTitle + "添加组件/LanguageComponent", false, 1)]
    private static void AddComponent01() => AddComponent<TextMeshProUGUI, LanguageComponent>();

    #endregion

    #region 移除组件

    [MenuItem(CommonTitle + "移除组件/LanguageComponent", false, 1)]
    private static void RemoveComponent01() => RemoveComponent<LanguageComponent>();

    #endregion
}

/// <summary>
/// Assets
/// </summary>
public partial class GenerateComponent
{
    // [MenuItem("Assets/工具/模板代码")]
    // static void CustomCommand()
    // {
    //     // ("在选定资产上执行的自定义命令: " + Selection.activeObject.name).EditorLog();
    //     string[] str = Selection.assetGUIDs; //它们被导进Unity时一定都会被序列化出一个唯一ID存到.meta文件中，因此，根据唯一ID可以找到文件或文件夹
    //     string str1 = AssetDatabase.GUIDToAssetPath(str[0]);
    //     List<GameObject> temp = str1.GetFolderObjects<GameObject>();
    //     temp.GetPrefabComponents<Text>((t) => { t.text.EditorLog(); });
    // }
}


public partial class GenerateComponent
{
    private static void RemoveComponent<T>() where T : Component
    {
        Object obj = Selection.activeObject;
        if (!obj) throw new Exception("当前物体未获取到，可能是隐藏");
        List<T> values = (obj as GameObject)?.transform.GetAllChild<T>((t) => { Object.DestroyImmediate(t.gameObject.GetComponent<T>()); });
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene()); // 在编辑器模式下添加组件后保存场景
        $"移除了{values?.Count}组件".EditorLog();
    }

    private static void AddComponent<T, TK>() where T : Component where TK : Component
    {
        Object obj = Selection.activeObject;
        if (!obj) throw new Exception("当前物体未获取到，可能是隐藏");
        List<T> values = (obj as GameObject)?.transform.GetAllChild<T>((t) => { t.gameObject.TryAddComponent<TK>(); });
        AssetDatabase.SaveAssets(); // 在编辑器模式下添加组件后保存预制体
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene()); // 在编辑器模式下添加组件后保存场景
        //PrefabUtility.SavePrefabAsset(gameObject);// 在编辑器模式下添加组件后保存资产
        $"添加了{values?.Count}组件".EditorLog();
    }

    /// <summary>
    /// 将isValidateFunction参数设置为true
    /// 并且路径以及按钮名称需要一模一样，表示下面我们声明的方法是对应按钮的验证方法
    /// 方法的返回值一定要是bool类型的，当返回值为true时表示执行下面的方法
    /// 当每次弹出选择栏的时候便会执行一次这个方法
    /// </summary>
    public static string GetComponent<T>() where T : Component
    {
        Object obj = Selection.activeObject;
        if (!obj) throw new Exception("当前物体未获取到，可能是隐藏");
        StringBuilder sb = new StringBuilder(); //字符串

        List<T> values = (obj as GameObject)?.transform.GetAllChild<T>((t) =>
        {
            string str1 = $"{obj.name} = GetComponent<{typeof(T)}>();";
            string str2 = $"{t.name} = transform.Find(\"{t.GetParentPath(obj.name)}\").GetComponent<{typeof(T)}>();";
            sb.AppendLine(t.name.Equals(obj.name) ? str1 : str2);
        });
        sb.ToString().EditorLog();
        sb.ToString().Copy();
        return sb.ToString();
    }

    public static string GetComponent(Object obj, Type type)
    {
        StringBuilder sb = new StringBuilder(); //字符串

        (obj as GameObject)?.transform.GetAllChild(type, (t) =>
        {
            string str1 = $"{obj.name} = GetComponent<{type}>();";
            string str2 = $"{t.name} = transform.Find(\"{t.GetParentPath(obj.name)}\").GetComponent<{type}>();";
            sb.AppendLine(t.name.Equals(obj.name) ? str1 : str2);
        });
        sb.ToString().EditorLog();
        sb.ToString().Copy();
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
        List<T> values = (obj as GameObject)?.transform.GetAllChild<T>((t) => { sb.AppendLine($"public {typeof(T)} {t.name};"); });

        sb.ToString().EditorLog();
        sb.ToString().Copy();
        return sb.ToString();
    }
}
#endif