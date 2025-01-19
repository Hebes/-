using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 对话系统
/// </summary>
public class DialogueSystem : SM<DialogueSystem>, IBehaviour
{
    public DialogueSystemConfigurationSO Config => DB.I.DialogueSystemConfigurationSo;
    public ConversationManager ConversationManager;
    public TextArchitect TextArchitect; //文本构建
    public AutoReader autoReader; //自动阅读
    public CanvasGroup mainCanvas;
    public CanvasGroupController CgController; //画布组控制
    public DialogueContainer dialogueContainer = new DialogueContainer();

    private List<string> _filtrationSpeakerName; // 过滤说话人的名称的列表
    public bool IsRunningConversation => ConversationManager.isRunning;
    public DialogueContinuePrompt Prompt => R.UISystem.UIDialogue.dialoguePrompt;
    public bool IsVisible => CgController.IsVisible;

    public delegate void DialogueSystemEvent();

    public event DialogueSystemEvent onUserPromptNext;
    public event DialogueSystemEvent onClear;

    public void OnGetComponent()
    {
        mainCanvas = GameObject.Find("CanvasMain").FindComponent<CanvasGroup>();
        gameObject.AddComponent<AutoReader>();
        if (TryGetComponent(out autoReader))
            autoReader.Initialize();
    }

    public void OnNewClass()
    {
        _filtrationSpeakerName = new List<string>() { "narrator" };
        TextArchitect = new TextArchitect();
        ConversationManager = new ConversationManager();
        CgController = new CanvasGroupController(this, mainCanvas);
    }


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
        UIDialogue uiDialogue = R.UISystem.UIDialogue;
        float fontSize = Config.defaultDialogueFontSize * config.dialogueFontScale * Config.dialogueFontScale;
        uiDialogue.SetDialogueColor(config.dialogueColor);
        uiDialogue.SetDialogueFont(config.dialogueFont);
        uiDialogue.SetDialogueFontSize(fontSize);

        fontSize = Config.defaultNameFontSize * config.nameFontScale;
        uiDialogue.SetNameColor(config.nameColor);
        uiDialogue.SetNameFont(config.nameFont);
        uiDialogue.SetNameFontSize(fontSize);
    }

    /// <summary>
    /// 开启使用提示下一步
    /// </summary>
    public void OnUserPrompt_Next()
    {
        R.DialogueSystem.onUserPromptNext?.Invoke();
        autoReader?.Disable();
    }

    public void OnSystemPrompt_Next()
    {
        R.DialogueSystem.onUserPromptNext?.Invoke();
    }

    public void OnSystemPrompt_Clear()
    {
        R.DialogueSystem.onClear?.Invoke();
    }

    #region 历史记录

    public void OnStartViewingHistory()
    {
        Prompt.Hide();
        autoReader.allowToggle = false;
        ConversationManager.allowUserPrompts = false;

        if (autoReader.IsOn)
            autoReader.Disable();
    }

    public void OnStopViewingHistory()
    {
        Prompt.Show();
        autoReader.allowToggle = true;
        ConversationManager.allowUserPrompts = true;
    }

    #endregion

    #region 说话

    public Coroutine Say(string speaker, string dialogue)
    {
        List<string> lines = new List<string>() { $"{speaker} \"{dialogue}\"" };
        Conversation conversation = new Conversation(lines);
        return ConversationManager.StartConversation(conversation);
    }

    public Coroutine Say(List<string> lines)
    {
        Conversation conversation = new Conversation(lines);
        return ConversationManager.StartConversation(conversation);
    }

    public Coroutine Say(Conversation conversation)
    {
        return ConversationManager.StartConversation(conversation);
    }

    #endregion

    #region 说话框

    public Coroutine Show(float speed = 1f, bool immediate = false) => CgController.Show(speed, immediate);
    public Coroutine Hide(float speed = 1f, bool immediate = false) => CgController.Hide(speed, immediate);


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
            HideSpeakerName();
        else
            R.UISystem.UIDialogue.Show(speakerName);
    }

    public void HideSpeakerName()
    {
        R.UISystem.UIDialogue.Hide();
    }

    #endregion
}