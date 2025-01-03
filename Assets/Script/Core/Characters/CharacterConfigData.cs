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


    public CharacterConfigData Copy()
    {
        CharacterConfigData result = new CharacterConfigData();
        result.name = name;
        result.alias = alias;
        result.characterType = characterType;
        result.nameFont = nameFont;
        result.dialogueFont = dialogueFont;
        result.nameColor = new  Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a);
        result.dialogueColor = new Color(dialogueColor.r,dialogueColor.g,dialogueColor.b,dialogueColor.a);
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
        result.nameFont = d.defaultFont ;
        result.dialogueFont = d.defaultFont;
        result.nameColor = new Color(d.defaultTextColor.r, d.defaultTextColor.g, d.defaultTextColor.b, d.defaultTextColor.a);
        result.dialogueColor = new Color(d.defaultTextColor.r,d.defaultTextColor.g,d.defaultTextColor.b,d.defaultTextColor.a);
        return result;
    }
}