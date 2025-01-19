using System;
using System.Collections.Generic;
using UnityEngine;

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

    public string GetLanguage(string key)
    {
        foreach (LanguageData languageData in LanguageList)
        {
            if (languageData.key.Equals(key) ||
                languageData.Engilsh.Equals(key))
            {
                switch (languageType)
                {
                    case LanguageType.Chinese: return languageData.Chinese;
                    case LanguageType.English: return languageData.Engilsh;
                }
            }
        }

        $"未配置多语言{key}".LogWarning();
        LanguageData languageDataTemp = new LanguageData() { key = key, Chinese = key, Engilsh = key, };
        DB.I.LanguageSo.LanguageList.Add(languageDataTemp);
        return key;
    }

    public static LanguageData GetLanguageData(string key)
    {
        foreach (LanguageData languageData in I.LanguageList)
        {
            if (languageData.key.Equals(key) ||
                languageData.Engilsh.Equals(key))
            {
                return languageData;
            }
        }

        $"未配置多语言{key}".LogWarning();
        LanguageData languageDataTemp = new LanguageData() { key = key, Chinese = key, Engilsh = key, };
        DB.I.LanguageSo.LanguageList.Add(languageDataTemp);
        return languageDataTemp;
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
}

[System.Serializable]
public class LanguageData
{
    public string key;
    public string Chinese;
    public string Engilsh;
}