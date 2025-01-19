using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 逻辑行接口
/// </summary>
public interface ILogicalLine
{
    string Keyword { get; }
    bool Matches(DIALOGUE_LINE line);
    IEnumerator Execute(DIALOGUE_LINE line);
}

/// <summary>
/// 逻辑行系统
/// </summary>
public class LogicalLineSystem : SM<LogicalLineSystem>
{
    private List<ILogicalLine> logicalLines = new List<ILogicalLine>();

    private void Awake()
    {
        LoadLogicalLines();
    }

    /// <summary>
    /// 加载继承ILogicalLine接口的逻辑
    /// </summary>
    private void LoadLogicalLines()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] lineTypes = assembly.GetTypes()
            .Where(t => typeof(ILogicalLine).IsAssignableFrom(t) && !t.IsInterface)
            .ToArray();

        foreach (Type lineType in lineTypes)
        {
            ILogicalLine line = (ILogicalLine)Activator.CreateInstance(lineType);
            logicalLines.Add(line);
        }
    }

    public bool TryGetLogic(DIALOGUE_LINE line, out Coroutine logic)
    {
        foreach (var logicalLine in logicalLines)
        {
            if (logicalLine.Matches(line))
            {
                logic = R.StartCoroutine(logicalLine.Execute(line));
                return true;
            }
        }

        logic = null;
        return false;
    }
}