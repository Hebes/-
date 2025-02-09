using System;
using TMPro;
using UnityEditor;
using UnityEngine;

// [ExecuteInEditMode]//在不运行模式下修改参数,Update
// [InitializeOnLoad]
[RequireComponent(typeof(TextMeshProUGUI))]
public class LanguageComponent : BaseBehaviour
{
    public string key;
    private TextMeshProUGUI _textMeshProUGUI;
    private LanguageData _languageData;

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        key = _textMeshProUGUI.text;
        _languageData = LanguageSysatem.GetLanguageData(key);
        LanguageSysatem.AddLanguageComponent(this);
        SetText();
    }

    public void SetText()
    {
        if (string.IsNullOrEmpty(key))
        {
            //$"没有key{key}".LogWarning(this);
            _languageData = new LanguageData();
        }
        switch (LanguageSysatem.languageType)
        {
            case LanguageSysatem.LanguageType.Chinese:
                _textMeshProUGUI.text = _languageData.chinese;
                break;
            case LanguageSysatem.LanguageType.English:
                _textMeshProUGUI.text = _languageData.engilsh;
                break;
            default:
                throw new Exception("未配置语言类型");
        }
    }
}