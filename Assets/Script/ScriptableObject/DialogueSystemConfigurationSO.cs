using TMPro;
using UnityEngine;

/// <summary>
/// 对话系统配置SO
/// </summary>
[CreateAssetMenu(fileName = "对话系统配置", menuName = "对话系统/对话系统配置", order = 0)]
public class DialogueSystemConfigurationSO : ScriptableObject
{
    public CharacterConfigSo CharacterConfigurationAssetr;
    
    public Color defaultTextColor = Color.white;
    public TMP_FontAsset defaultFont;
}