using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 对话系统配置SO
/// </summary>
[CreateAssetMenu(fileName = "对话系统配置", menuName = "对话系统/对话系统配置", order = 0)]
public class DialogueSystemConfigurationSO : ScriptableObject
{
    public CharacterConfigSo CharacterConfigurationAssetr;

    public Color defaultTextColor = Color.white;
    public TMP_FontAsset defaultFont;

    public float dialogueFontScale = 1f;
    public float defaultDialogueFontSize = 18;
    public float defaultNameFontSize = 22;

     public TextArchitect.BuildMethod buildMethod = TextArchitect.BuildMethod.TypeWriter;
}