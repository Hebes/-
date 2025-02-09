using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话系统
/// </summary>
[NoDontDestroyOnLoad]
[RequireComponent(typeof(AutoReader))]
public class DialogueSystem : SM<DialogueSystem>
{
    public DialogueSystemConfigurationSO Config => DB.I.DSCSO;
    public ConversationManager ConversationManager;
    public TextArchitect TextArchitect; //文本构建
    public AutoReader autoReader=>GetComponent<AutoReader>(); //自动阅读
    public CanvasGroup mainCanvas;
    public CanvasGroupController CgController; //画布组控制
    public DialogueContainer dialogueContainer = new DialogueContainer();

    private List<string> _filtrationSpeakerName; // 过滤说话人的名称的列表
    public bool IsRunningConversation => ConversationManager.isRunning;
    public DialogueContinuePrompt Prompt => UIDialogue.I.dialoguePrompt;
    public bool IsVisible => CgController.IsVisible;

    public delegate void DialogueSystemEvent();
    public event DialogueSystemEvent onUserPromptNext;
    public event DialogueSystemEvent onClear;

    private void Awake()
    {
        I = this;
        _filtrationSpeakerName = new List<string>() { "narrator" };
        
        mainCanvas = GameObject.Find("CanvasMain").FindComponent<CanvasGroup>();
        CgController = new CanvasGroupController(this, mainCanvas);
        ConversationManager = new ConversationManager();
        dialogueContainer.Initialize();
        
        TextArchitect = new TextArchitect(dialogueContainer.uiDialogue.dialogueText, Config.buildMethod);
        
        autoReader.Initialize();
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
        ConversationManager.AllowUserPrompts = false;

        if (autoReader.IsOn)
            autoReader.Disable();
    }

    public void OnStopViewingHistory()
    {
        Prompt.Show();
        autoReader.allowToggle = true;
        ConversationManager.AllowUserPrompts = true;
    }

    #endregion

    #region 说话

    public Coroutine Say(string speaker, string dialogue)
    {
        List<string> lines = new List<string>() { $"{speaker} \"{dialogue}\"" };
        Conversation conversation = new Conversation(lines);
        return ConversationManager.StartConversation(conversation);
    }

    public Coroutine Say(List<string> lines, string filePath = "")
    {
        Conversation conversation = new Conversation(lines, file: filePath);
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
        if (speakerName.ToLower() == "narrator")
        {
            R.UISystem.UIDialogue.Hide();
            R.UISystem.UIDialogue.NameText.text = "";
        }
        else
        {
            R.UISystem.UIDialogue.Show(speakerName);
        }
    }

    #endregion
}