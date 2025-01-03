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
    /// 开启使用提示下一步
    /// </summary>
    public void OnUsePrompt_Next()
    {
        GameEvent.UsePrompt_Next.Trigger(null);
    }

    public void Say(string speaker, string dialogue)
    {
        // List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
        // Say(conversation);
    }

    public void Say(List<string> conversation)
    {
        ConversationManager.StartConversation(conversation);
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