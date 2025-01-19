using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class CharacterConfigData
{
    public string name;
    public string alias;
    public Character.CharacterType characterType;
    public Color nameColor;
    public Color dialogueColor;
    public TMP_FontAsset nameFont;
    public TMP_FontAsset dialogueFont;

    public float nameFontScale = 1f;
    public float dialogueFontScale = 1f;

    public List<SpriteData> spriteList = new List<SpriteData>();

    public CharacterConfigData Copy()
    {
        CharacterConfigData result = new CharacterConfigData();
        result.name = name;
        result.alias = alias;
        result.characterType = characterType;
        result.nameFont = nameFont;
        result.dialogueFont = dialogueFont;
        result.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a);
        result.dialogueColor = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a);

        result.dialogueFontScale = dialogueFontScale;
        result.nameFontScale = nameFontScale;

        return result;
    }

    /// <summary>
    /// 默认数据
    /// </summary>
    /// <returns></returns>
    public static CharacterConfigData DefaultData()
    {
        DialogueSystemConfigurationSO d = R.DialogueSystem.Config;
        CharacterConfigData result = new CharacterConfigData();
        result.name = string.Empty;
        result.alias = string.Empty;
        result.characterType = Character.CharacterType.Text;
        result.nameFont = d.defaultFont;
        result.dialogueFont = d.defaultFont;
        result.nameColor = new Color(d.defaultTextColor.r, d.defaultTextColor.g, d.defaultTextColor.b, d.defaultTextColor.a);
        result.dialogueColor = new Color(d.defaultTextColor.r, d.defaultTextColor.g, d.defaultTextColor.b, d.defaultTextColor.a);

        result.dialogueFontScale = 1f;
        result.nameFontScale = 1f;

        return result;
    }
}

/// <summary>
/// 图片数据
/// </summary>
[System.Serializable]
public class SpriteData
{
    public SpriteData(string path, Sprite sprite)
    {
        this.name = path;
        this.sprite = sprite;
    }

    public string name;
    public Sprite sprite;
}