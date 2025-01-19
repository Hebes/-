using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 对话UI
/// </summary>
[NoDontDestroyOnLoad]
public class UIDialogue : SM<UIDialogue>,IUIBehaviour
{
    public void OnGetComponent()
    {
        //对话
        dialogueText = transform.Find("Root/DialogueText").GetComponent<TMPro.TextMeshProUGUI>();
        dialoguePrompt = transform.Find("Root/DialoguePrompt").GetComponent<DialogueContinuePrompt>();
        //自动阅读
        autoReadStatus = transform.Find("Root/AutoReadStatus").GetComponent<TMPro.TextMeshProUGUI>();
        //名字
        NameText = transform.Find("Root/NameText").GetComponent<TMPro.TextMeshProUGUI>();
        root = NameText.gameObject;
        
        Initialize();
    }

    //左下角的自动对话文字
     public TextMeshProUGUI autoReadStatus;

    //对话
    public TextMeshProUGUI dialogueText;
    public DialogueContinuePrompt dialoguePrompt; //对话继续提示
    private CanvasGroupController _cgController;
    public void SetDialogueColor(Color color) => dialogueText.color = color;
    public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;
    public void SetDialogueFontSize(float size) => dialogueText.fontSize = size;
    private bool _initialized = false;
    private void Initialize()
    {
        if (_initialized)
            return;
        var rootCg = transform.Find("Root").GetComponent<UnityEngine.CanvasGroup>();
        _cgController = new CanvasGroupController(R.DialogueSystem, rootCg);
    }
    public bool IsVisible => _cgController.IsVisible;
    public Coroutine Show(float speed = 1f, bool immediate = false) => _cgController.Show(speed, immediate);
    public Coroutine Hide(float speed = 1f, bool immediate = false) => _cgController.Hide(speed, immediate);

    //名字
    public GameObject root;
    public TextMeshProUGUI NameText;
    public void SetNameColor(Color color) => NameText.color = color;
    public void SetNameFont(TMP_FontAsset font) => NameText.font = font;
    public void SetNameFontSize(float size) => NameText.fontSize = size;
    public void Show(string nameValue)
    {
        root.SetActive(true);
        if (nameValue.IsNoNullOrNoEmpty())
            NameText.text = R.LanguageSystem.GetLanguage(nameValue);
    }
    public void Hide()
    {
        root.SetActive(false);
    }
}