using System;
using System.Collections.Generic;

/// <summary>
/// 全局检查系统
/// </summary>
public class CheckSystem : SM<CheckSystem>
{
    private Dictionary<string, bool> _checkDic = new Dictionary<string, bool>();

    public void Change(string key, bool checkValue)
    {
        if (_checkDic.ContainsKey(key))
            _checkDic[key] = checkValue;
    }

    public bool Get(string key, bool def = false)
    {
        if (_checkDic.TryGetValue(key, out bool value))
            return value;
        _checkDic[key] = def;
        return def;
    }
}