using System;
using TMPro;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
[RequireComponent(typeof(TextMeshProUGUI))]
public class LanguageComponent : BaseBehaviour
{
    public string key;
    private TextMeshProUGUI _textMeshProUGUI;
    private LanguageData _languageData;

    private void Awake()
    {
        _languageData = LanguageSysatem.GetLanguageData(key);
        LanguageSysatem.AddLanguageComponent(this);
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        key = _textMeshProUGUI.text;
        SetText();
    }

    public void SetText()
    {
        if (string.IsNullOrEmpty(key))
            throw new Exception("没有key");
        switch (LanguageSysatem.languageType)
        {
            case LanguageSysatem.LanguageType.Chinese:
                _textMeshProUGUI.text = _languageData.Chinese;
                break;
            case LanguageSysatem.LanguageType.English:
                _textMeshProUGUI.text = _languageData.Engilsh;
                break;
            default:
                throw new Exception("未配置语言类型");
        }
    }
}