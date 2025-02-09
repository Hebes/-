using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 多语言系统
/// </summary>
public class LanguageSysatem : SM<LanguageSysatem>
{
    private List<LanguageData> LanguageList => DB.I.LanguageSo.LanguageList;
    public static LanguageType languageType = LanguageType.Chinese;
    private readonly List<LanguageComponent> _languageComponentList = new List<LanguageComponent>();

    public enum LanguageType
    {
        Chinese,
        English,
    }

    public string Get(string key)
    {
        foreach (LanguageData languageData in LanguageList)
        {
            if (languageData.key.Equals(key) ||
                languageData.engilsh.Equals(key))
            {
                switch (languageType)
                {
                    case LanguageType.Chinese: return languageData.chinese;
                    case LanguageType.English: return languageData.engilsh;
                }
            }
        }

        return RecordUnNoLanguage(key).key;
    }

    public static LanguageData GetLanguageData(string key)
    {
        foreach (LanguageData languageData in I.LanguageList)
        {
            if (languageData.key.Equals(key) ||
                languageData.engilsh.Equals(key))
            {
                return languageData;
            }
        }

        return RecordUnNoLanguage(key);
    }

    public void OnChangeLanguage(LanguageType languageMode)
    {
        languageType = languageMode;
        foreach (var languageComponent in _languageComponentList)
            languageComponent.SetText();
    }

    public static void AddLanguageComponent(LanguageComponent languageComponent)
    {
        if (!I._languageComponentList.Contains(languageComponent))
            I._languageComponentList.Add(languageComponent);
    }

    /// <summary>
    /// 记录为止多语言
    /// </summary>
    private static LanguageData RecordUnNoLanguage(string key)
    {
        LanguageData languageDataTemp = new LanguageData() { key = key, chinese = key, engilsh = key, };
        DB.I.LanguageSo.LanguageList.Add(languageDataTemp);
        $"未配置多语言:{key},但是添加到了SO中，请配置".LogWarning();
        return languageDataTemp;
    }
}

[System.Serializable]
public class LanguageData
{
    public string key = String.Empty;
    public string chinese = String.Empty;
    public string engilsh = String.Empty;
}