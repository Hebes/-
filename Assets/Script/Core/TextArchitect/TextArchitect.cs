using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using BuilderTypes = TABuilder.BuilderTypes;

/// <summary>
/// 文本构建(添加内容等)
/// {c}         =clear
/// {a}         =append
/// {wc n}      =wait clear number
/// {wa n}      =wait append number
/// </summary>
public class TextArchitect
{
    public TextArchitect(TextMeshProUGUI uiTextObject, BuilderTypes builderType = BuilderTypes.Instant)
    {
        tmpro_ui = uiTextObject;
        AddBuilderTypes();
        SetBuilderType(builderType);
    }

    public TextArchitect(TextMeshPro worldTextObject, BuilderTypes builderType = BuilderTypes.Instant)
    {
        tmpro_world = worldTextObject;
        AddBuilderTypes();
        SetBuilderType(builderType);
    }

    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;

    public BuilderTypes BuilderType { get; set; }
    private TABuilder _builder;
    private Coroutine _buildProcess = null; //构建进度

    private float _speedMultiplier = 1; //倍速器
    private const float BaseSpeed = 1; //基础速度
    public bool HurryUp = false; // 是否加快
    public string TargetText = string.Empty;//目标文字
    public string PreText = string.Empty;//出现的文字
    private int characterMultiplier = 1;

    public TMP_Text ShowText => (TMP_Text)tmpro_ui ?? (TMP_Text)tmpro_world;
    public string fullTargetText => PreText + TargetText;
    public Color textColor
    {
        get => ShowText.color;
        set => ShowText.color = value;
    }
    public float speed
    {
        get => BaseSpeed * _speedMultiplier;
        set => _speedMultiplier = value;
    }
    public bool isBuilding => _buildProcess != null; //是否正在构建
    public int CharactersPerCycle => speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; //每帧将创建多少个字符。当与渐隐技术一起使用时，这只是增加了速度。
    private Dictionary<string, Type> _builderDic = new Dictionary<string, Type>();


    /// <summary>
    /// 添加构建类型
    /// </summary>
    private void AddBuilderTypes()
    {
        _builderDic = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TABuilder)))
            .ToDictionary(t => t.Name, t => t);
    }

    /// <summary>
    /// 将tabbuilder更新为给定构建类型的正确tabbuilder。
    /// </summary>
    public void SetBuilderType(TABuilder.BuilderTypes builderType)
    {
        string name = TABuilder.CLASS_NAME_PREFIX + builderType.ToString();
        Type classType = _builderDic[name];

        _builder = Activator.CreateInstance(classType) as TABuilder;
        _builder.architect = this;
        _builder.onComplete += OnComplete;

        BuilderType = builderType;
    }

    public Coroutine Build(string text)
    {
        PreText = "";
        TargetText = text;
        Stop();
        return _buildProcess = _builder.Build();
    }

    /// <summary>
    /// 向文本架构中已经存在的内容追加文本。
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Coroutine Append(string text)
    {
        PreText = ShowText.text;
        TargetText = text;
        Stop();
        return _buildProcess = _builder.Build();
    }

    /// <summary>
    /// 立即将文本应用到对象
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        PreText = "";
        TargetText = text;

        Stop();

        ShowText.text = TargetText;
        _builder.ForceComplete();
    }

    public void Stop()
    {
        if (isBuilding)
            R.StopCoroutine(_buildProcess);
        _buildProcess = null;
    }

    /// <summary>
    /// 强制完成
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ForceComplete()
    {
        if (isBuilding)
            _builder.ForceComplete();
        Stop();
        OnComplete();
    }

    /// <summary>
    /// 动画结束后
    /// </summary>
    private void OnComplete()
    {
        _buildProcess = null;
    }
}