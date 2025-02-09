using TMPro;
using UnityEngine;

/// <summary>
/// 对话UI
/// </summary>
[NoDontDestroyOnLoad]
public class UIDialogue : SM<UIDialogue>
{
    private void Awake()
    {
        I = this;
    }

    public void OnInitialize()
    {
        //历史记录
        historyState = transform.Find("RootDialogue/HistoryState").GetComponent<TMPro.TextMeshProUGUI>();
        //对话
        dialogueText = transform.Find("RootDialogue/DialogueText").GetComponent<TMPro.TextMeshProUGUI>();
        dialoguePrompt = transform.Find("RootDialogue/DialoguePrompt").GetComponent<DialogueContinuePrompt>();
        //自动阅读
        autoReadStatus = transform.Find("RootDialogue/AutoReadStatus").GetComponent<TMPro.TextMeshProUGUI>();
        //名字
        NameText = transform.Find("RootDialogue/NameText").GetComponent<TMPro.TextMeshProUGUI>();
        root = transform.Find("RootDialogue").gameObject;
        
        _cgController = new CanvasGroupController(R.DialogueSystem, root.GetComponent<UnityEngine.CanvasGroup>());
    }


    //左下角的自动对话文字
    public TextMeshProUGUI autoReadStatus;

    //历史记录
    public TextMeshProUGUI historyState;

    //对话
    public TextMeshProUGUI dialogueText;
    public DialogueContinuePrompt dialoguePrompt; //对话继续提示
    private CanvasGroupController _cgController;
    public void SetDialogueColor(Color color) => dialogueText.color = color;
    public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;
    public void SetDialogueFontSize(float size) => dialogueText.fontSize = size;

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
        NameText.gameObject.SetActive(true);
        if (nameValue.IsNoNullOrNoEmpty())
            NameText.text = R.LanguageSystem.Get(nameValue);
    }

    public void Hide()
    {
        NameText.gameObject.SetActive(false);
    }
}