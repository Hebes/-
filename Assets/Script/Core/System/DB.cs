using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// 数据管理
/// </summary>
public class DB : SM<DB>
{
    public DialogueSystemConfigurationSO DSCSO;
    public LanguageSO LanguageSo;
    public InputActionAsset inputActionAsset;
    public VisualNovelSO visualNovelSo; //视觉小说配置

    private void Awake()
    {
        CheckRepetition(this);
    }
}