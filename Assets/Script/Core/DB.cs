using System;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// 数据管理
/// </summary>
public class DB : SM<DB>
{
    public DialogueSystemConfigurationSO DialogueSystemConfigurationSo;
    public LanguageSO LanguageSo;
    public InputActionAsset inputActionAsset;

    private void Start()
    {
        PlayerInputSystem playerInputSystem = R.PlayerInputSystem;
    }
}