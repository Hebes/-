using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class CharacterConfigData
{
    public string name;
    public string alias;
    public GameObject prefab;
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
        return new CharacterConfigData
        {
            name = name,
            alias = alias,
            prefab = prefab,
            characterType = characterType,
            nameFont = nameFont,
            dialogueFont = dialogueFont,
            nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a),
            dialogueColor = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a),
            dialogueFontScale = dialogueFontScale,
            nameFontScale = nameFontScale,
            //spriteList = spriteList,
        };
    }

    /// <summary>
    /// 默认数据
    /// </summary>
    /// <returns></returns>
    public static CharacterConfigData DefaultData()
    {
        DialogueSystemConfigurationSO d = R.DB.DSCSO;
        return new CharacterConfigData
        {
            name = string.Empty,
            alias = string.Empty,
            prefab = default,
            characterType = Character.CharacterType.Text,
            nameFont = d.defaultFont,
            dialogueFont = d.defaultFont,
            nameColor = new Color(d.defaultTextColor.r, d.defaultTextColor.g, d.defaultTextColor.b, d.defaultTextColor.a),
            dialogueColor = new Color(d.defaultTextColor.r, d.defaultTextColor.g, d.defaultTextColor.b, d.defaultTextColor.a),
            dialogueFontScale = 1f,
            nameFontScale = 1f
        };
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