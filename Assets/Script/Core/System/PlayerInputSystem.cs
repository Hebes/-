using System;
using UnityEngine;

/// <summary>
/// 玩家输入管理器
/// </summary>
public class PlayerInputSystem : SingletonMono<PlayerInputSystem>
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