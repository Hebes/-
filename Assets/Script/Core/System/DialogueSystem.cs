using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话系统
/// </summary>
public class DialogueSystem : SingletonMono<DialogueSystem>
{
    public DialogueSystemConfigurationSO Config => DB.I.DialogueSystemConfigurationSo;
    private ConversationManager ConversationManager { get; set; } = new ConversationManager();
    public TextArchitect TextArchitect { get; set; } = new TextArchitect();

    public bool IsRunningConversation => ConversationManager.isRunning;

    /// <summary>
    /// 过滤说话人的名称的列表
    /// </summary>
    private List<string> _filtrationSpeakerName = new List<string>() { "narrator" };

    /// <summary>
    /// 将说话者数据应用到对话容器中
    /// </summary>
    /// <param name="speakerName"></param>
    public void ApplySpeakerDataToDialogueContainer(string speakerName)
    {
        Character character = R.CharacterSystem.GetCharacter(speakerName);
        CharacterConfigData config = character != null ? character.Config : R.CharacterSystem.GetCharacterConfig(speakerName);
        ApplySpeakerDataToDialogueContainer(config);
    }

    public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
    {
        R.DialogueContainer.SetDialogueColor(config.dialogueColor);
        R.DialogueContainer.SetDialogueFont(config.dialogueFont);
        R.NameContainer.SetNameColor(config.nameColor);
        R.NameContainer.SetNameFont(config.nameFont);
    }

    /// <summary>
    /// 开启使用提示下一步
    /// </summary>
    public void OnUsePrompt_Next()
    {
        GameEvent.UsePrompt_Next.Trigger(null);
    }

    public Coroutine Say(string speaker, string dialogue)
    {
        List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
        return ConversationManager.StartConversation(conversation);
    }

    public Coroutine Say(List<string> conversation)
    {
        return ConversationManager.StartConversation(conversation);
    }

    public void ShowSpeakerName(string speakerName)
    {
        bool jump = false;
        foreach (string s in _filtrationSpeakerName)
        {
            if (s.ToLower().Equals(speakerName.ToLower()))
            {
                jump = true;
                break;
            }
        }

        if (jump)
            NameContainer.I.Hide();
        else
            NameContainer.I.Show(speakerName);
    }

    public void HideSpeakerName()
    {
        NameContainer.I.Hide();
    }
}