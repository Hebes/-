using System;
using UnityEngine;

/// <summary>
/// 玩家输入管理器
/// </summary>
public class PlayerInputManager : SingletonMono<PlayerInputManager>
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            PromptAdvance();
    }

    public void PromptAdvance()
    {
        R.DialogueSystem.OnUsePrompt_Next();
    }
}