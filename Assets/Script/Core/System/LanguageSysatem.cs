using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 多语言系统
/// </summary>
public class LanguageSysatem : SingletonMono<LanguageSysatem>
{
    public LanguageSO LanguageDataList => DB.I.LanguageSo;
    private List<LanguageData> LanguageList => LanguageDataList.LanguageList;
    public LanguageType languageType = LanguageType.Chinese;

    private List<LanguageComponent> _languageComponentList = new List<LanguageComponent>();

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

        $"未配置多语言{key}".LogError();
        return key;
    }

    public void OnChangeLanguage(LanguageType languageMode)
    {
        languageType = languageMode;
        foreach (var languageComponent in _languageComponentList)
            languageComponent.SetData(languageComponent.key);
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