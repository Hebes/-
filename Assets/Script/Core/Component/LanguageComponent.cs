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

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        key = _textMeshProUGUI.text;
        SetData(key);
        LanguageSysatem.AddLanguageComponent(this);
    }

    public void SetData(string keyValue)
    {
        if (keyValue.IsNullOrEmpty()) return;
        this.key = keyValue;
        _textMeshProUGUI.text = LanguageSysatem.I.GetLanguage(key);
    }
}