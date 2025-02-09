#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// 生成配置文件工具
/// </summary>
public partial class GenerateConfig : EditorWindow
{
    [NonSerialized] private const string CommonTitle = "工具/";

    [MenuItem(CommonTitle + "生成配置文件")]
    public static void ShowConfigToolUI() => "生成配置文件".ShowUI<GenerateConfig>();

    private void OnEnable() => this.Load();
    private void OnDisable() => this.Save();

    private string _content;
    private Vector2 _scrollPosition;
    private GUIStyle _centeredStyle;

    private void OnGUI()
    {
        GUI.backgroundColor = Color.yellow;
        Repaint();
        GUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // 添加弹性空间使标签居中
        Template2("Tags", 100f, Generate1);
        Template2("Layer", 100f, Generate2);
        Template2("SortingLayer", 100f, Generate3);
        Template2("prefab", 100f, Generate4);
        Template2("Scene", 100f, Generate5);
        GUILayout.FlexibleSpace(); // 添加弹性空间使标签居中
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // 添加弹性空间使标签居中
        Template2("ResourcesPrefab", 110f, Generate6);
        Template2("ResourcesImage", 110f, Generate7);
        Template2("ResourcesVideo", 110f, Generate8);
        Template2("ResourcesMusic", 110f, Generate9);
        GUILayout.FlexibleSpace(); // 添加弹性空间使标签居中
        EditorGUILayout.EndHorizontal();
        CodePreview(_content);
    }

    private string Template1(string btnName, float btnWidth, Action btnAction, string labelText, string textFieldText)
    {
        float labelWidth = EditorStyles.miniLabel.CalcSize(new GUIContent(labelText)).x;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(btnName, GUILayout.Width(btnWidth))) btnAction?.Invoke();
        EditorGUILayout.LabelField(labelText, EditorStyles.miniLabel, GUILayout.Width(labelWidth));
        var textFieldTextTemp = EditorGUILayout.TextField(textFieldText); //GUILayout.Width(textFieldWidth)
        EditorGUILayout.EndHorizontal();
        return textFieldTextTemp;
    }

    private void Template2(string btnName, float btnWidth, Action btnAction)
    {
        float labelWidth = EditorStyles.miniLabel.CalcSize(new GUIContent(btnName)).x;
        if (GUILayout.Button(btnName, GUILayout.Width(btnWidth)))
            btnAction?.Invoke();
    }

    /// <summary>
    /// Tags
    /// </summary>
    private void Generate1()
    {
        string[] tags = InternalEditorUtility.tags;
        _content = GetConfig(tags, tags, (sb, v1, v2) => { sb.AppendLine($"\tpublic const string {Path.GetFileNameWithoutExtension(v1)} = \"{v1}\";"); });
    }

    /// <summary>
    /// Layer
    /// </summary>
    private void Generate2()
    {
        string[] layers = InternalEditorUtility.layers;
        _content = GetConfig(layers, layers, (sb, v1, v2) => { sb.AppendLine($"\tpublic const string {Path.GetFileNameWithoutExtension(v1)} = \"{v1}\";"); });
    }

    /// <summary>
    /// SortingLayer
    /// </summary>
    private void Generate3()
    {
        BindingFlags bf = BindingFlags.Static | BindingFlags.NonPublic;
        PropertyInfo sortingLayersProperty = typeof(InternalEditorUtility).GetProperty("sortingLayerNames", bf);
        string[] sortingLayers = (string[])sortingLayersProperty?.GetValue(null, new object[0]);
        _content = GetConfig(sortingLayers, sortingLayers, (sb, v1, v2) => { sb.AppendLine($"\tpublic const string {Path.GetFileNameWithoutExtension(v1)} = \"{v1}\";"); });
    }

    /// <summary>
    /// prefab
    /// </summary>
    private void Generate4()
    {
        string[] temp = GetFilesAllPath($"{Application.dataPath}/Resources", "*.prefab"); //原版

        _content = GetConfig(temp, temp, (sb, v1, v2) =>
        {
            string t1 = Path.GetFileNameWithoutExtension(v1);
            string t2 = t1.Replace("-", "_", "[", "]", " ");
            sb.AppendLine($"\tpublic const string {t2} = \"{t1}\";");
        });
    }

    /// <summary>
    /// Scene
    /// </summary>
    private void Generate5()
    {
        string[] temp = GetFilesAllPath($"{Application.dataPath}", "*.unity"); //原版
        _content = GetConfig(temp, temp, (sb, v1, v2) =>
        {
            string sceneName = Path.GetFileNameWithoutExtension(v1);
            sb.AppendLine($"\tpublic const string {sceneName} = \"{sceneName}\";");
        });
    }

    /// <summary>
    /// ResourcesPrefab
    /// </summary>
    private void Generate6()
    {
        string[] temp = GetFilesAllPath($"{Application.dataPath}/Resources", "*.prefab"); //原版
        string[] key = temp.Replace("-", "_", "[", "]", " ", $"{Application.dataPath}/Resources\\", ".prefab"); //value
        string[] value = temp.Replace($"{Application.dataPath}/Resources\\", ".prefab"); //key
        _content = GetConfig(key, value, (sb, v1, v2) =>
        {
            v2 = v2.Replace(@"\", "/");
            sb.AppendLine($"\tpublic const string {Path.GetFileNameWithoutExtension(v1)} = \"{v2}\";");
        });
    }

    /// <summary>
    /// JPG,PNG
    /// </summary>
    private void Generate7()
    {
        //jpg
        string[] temp = GetFilesAllPath($"{Application.dataPath}/Resources", "*.jpg", "*.png"); //原版
        string[] key = temp.Replace("-", "_", "[", "]", " "); //value
        _content = GetConfig(key, temp, (sb, v1, v2) =>
        {
            v2 = v2.Replace(@"\", "/");
            v2 = v2.Replace($"{Application.dataPath}/Resources/", ".jpg", ".png");
            sb.AppendLine($"\tpublic const string {Path.GetFileNameWithoutExtension(v1)} = \"{v2}\";");
        });
        _content.Copy();
    }

    /// <summary>
    /// Video
    /// </summary>
    private void Generate8()
    {
        //jpg
        string[] temp = GetFilesAllPath($"{Application.dataPath}/Resources", "*.mp4"); //原版
        string[] key = temp.Replace("-", "_", "[", "]", " ", $"{Application.dataPath}/Resources\\", ".mp4"); //value
        string[] value = temp.Replace($"{Application.dataPath}/Resources\\", ".mp4"); //key
        _content = GetConfig(key, value, (sb, v1, v2) =>
        {
            v2 = v2.Replace(@"\", "/");
            sb.AppendLine($"\tpublic const string {Path.GetFileNameWithoutExtension(v1)} = \"{v2}\";");
        });
    }

    /// <summary>
    /// Music
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void Generate9()
    {
        string[] temp = GetFilesAllPath($"{Application.dataPath}/Resources", "*.wav", "*.mp3"); //原版
        string[] key = temp.Replace("-", "_", "[", "]", " "); //value
        _content = GetConfig(key, temp, (sb, v1, v2) =>
        {
            v2 = v2.Replace(@"\", "/");
            v2 = v2.Replace($"{Application.dataPath}/Resources/", ".wav", ".mp3");
            sb.AppendLine($"\tpublic const string {Path.GetFileNameWithoutExtension(v1)} = \"{v2}\";");
        });
        _content.Copy();
    }
}

/// <summary>
/// Assets
/// </summary>
public partial class GenerateConfig
{
    [MenuItem("Assets/工具/获取Image")]
    static void AssetsTools01()
    {
        
        string[] str = Selection.assetGUIDs; //它们被导进Unity时一定都会被序列化出一个唯一ID存到.meta文件中，因此，根据唯一ID可以找到文件或文件夹
        string str1 = AssetDatabase.GUIDToAssetPath(str[0]).Replace("Assets",String.Empty);
        string[] temp = GetFilesAllPath($"{Application.dataPath}{str1}", "*.jpg", "*.png"); //原版
        string[] key = temp.Replace("-", "_", "[", "]", " "); //value
       string strTemp = GetConfig(key, temp, (sb, v1, v2) =>
        {
            v2 = v2.Replace(@"\", "/");
            v2 = v2.Replace($"{Application.dataPath}/Resources/",".jpg", ".png");
            sb.AppendLine($"\tpublic const string {Path.GetFileNameWithoutExtension(v1)} = \"{v2}\";");
        });
        strTemp.EditorLog();
        strTemp.Copy();
    }
}

public partial class GenerateConfig
{
    private static string[] GetFilesAllPath(string path, params string[] suffix)
    {
        List<string> strings = new List<string>();
        foreach (string s in suffix)
        {
            string[] string1 = Directory.GetFiles(path, s, SearchOption.AllDirectories);
            strings.AddRange(string1);
        }

        string[] valueArray = new string[strings.Count];
        for (var i = 0; i < strings.Count; i++)
            valueArray[i] = strings[i];
        return valueArray;
    }

    private static string GetConfig(string[] value1, string[] value2, Action<StringBuilder, string, string> action)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < value1.Length; i++)
            action?.Invoke(sb, value1[i], value2[i]);
        string str = sb.ToString();
        str.Copy();
        return str;
    }

    /// <summary>
    /// 代码预览
    /// </summary>
    private void CodePreview(string content)
    {
        GUILayout.Space(10f);

        _centeredStyle ??= new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
        EditorGUILayout.LabelField("预览", _centeredStyle);

        if (string.IsNullOrEmpty(content)) return;
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(300));
        EditorGUILayout.TextArea(content);
        EditorGUILayout.EndScrollView();
    }
}
#endif