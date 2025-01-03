using System;
using System.Collections.Generic;

/// <summary>
/// 游戏事件
/// </summary>
// public enum GameEvent
// {
//     /// <summary>
//     /// 战斗评估
//     /// </summary>
//     Assessment = 1,
// }

public delegate void OnEventAction(object udata);

/// <summary>
/// 事件管理器
/// </summary>
public class EventManager
{
    /// <summary>
    /// 事件字典
    /// </summary>
    private static readonly Dictionary<Enum, EventData> EventDic = new Dictionary<Enum, EventData>();

    /// <summary>
    /// 注册监听
    /// </summary>
    /// <param name="enumValue"></param>
    /// <param name="action"></param>
    public static void Register(Enum enumValue, OnEventAction action)
    {
        if (!EventDic.ContainsKey(@enumValue))
            EventDic.Add(@enumValue, new EventData());
        EventDic[enumValue].Add(action);
    }

    /// <summary>
    /// 移除监听
    /// </summary>
    /// <param name="enumValue"></param>
    /// <param name="action"></param>
    public static void UnRegister(Enum enumValue, OnEventAction action)
    {
        if (!EventDic.ContainsKey(@enumValue)) return;
        EventDic[enumValue].UnAdd(action);
    }

    /// <summary>
    /// 触发监听
    /// </summary>
    /// <param name="enumValue"></param>
    /// <param name="data"></param>
    public static void Trigger(Enum enumValue, object data)
    {
        if (EventDic.TryGetValue(enumValue, out EventData actionList))
            actionList?.Trigger(data);
    }
}

/// <summary>
/// 事件数据
/// </summary>
public class EventData
{
    private readonly List<OnEventAction> _actionList = new List<OnEventAction>();

    /// <summary>
    /// 添加监听
    /// </summary>
    /// <param name="action"></param>
    /// <exception cref="Exception"></exception>
    public void Add(OnEventAction action)
    {
        if (_actionList.Contains(action))
            throw new Exception($"已经有当前方法{nameof(action)}");
        _actionList.Add(action);
    }

    /// <summary>
    /// 移除监听
    /// </summary>
    /// <param name="action"></param>
    public void UnAdd(OnEventAction action)
    {
        _actionList.Remove(action);
    }

    /// <summary>
    /// 触发
    /// </summary>
    /// <param name="data"></param>
    public void Trigger(object data)
    {
        for (var i = 0; i < _actionList.Count; i++)
            _actionList[i].Invoke(data);
    }
}

/// <summary>
/// 全称EventExpand 事件拓展
/// </summary>
public static class EventExpand
{
    /// <summary>
    /// 注册事件
    /// (GameObject,T) args = ((GameObject,T))udata;
    /// </summary>
    /// <param name="enumValue"></param>
    /// <param name="action"></param>
    public static void Register(this Enum enumValue, OnEventAction action) => EventManager.Register(enumValue, action);

    /// <summary>
    /// 取消事件注册
    /// </summary>
    /// <param name="enumValue"></param>
    /// <param name="action"></param>
    public static void UnRegister(this Enum enumValue, OnEventAction action) => EventManager.UnRegister(enumValue, action);

    /// <summary>
    /// 触发注册的事件
    /// </summary>
    /// <param name="enumValue"></param>
    /// <param name="data"></param>
    public static void Trigger(this Enum enumValue, object data) => EventManager.Trigger(enumValue, data);
}