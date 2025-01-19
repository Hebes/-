using TMPro;
using UnityEngine;
using UnityEngine.UI;

[NoDontDestroyOnLoad]
public class UIPlayerInput : SM<UIPlayerInput>, IUIBehaviour
{
    public void OnGetComponent()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        titleText = transform.Find("TitleText").GetComponent<TMPro.TextMeshProUGUI>();
        inputField = transform.Find("InputField").GetComponent<TMPro.TMP_InputField>();
        acceptButton = transform.Find("AcceptButton").GetComponent<UnityEngine.UI.Button>();
    }

    public void OnNewClass()
    {
        cg = new CanvasGroupController(this, canvasGroup);
    }

    public void OnListener()
    {
        inputField.onValueChanged.AddListener(OnInputChanged);
        acceptButton.onClick.AddListener(OnAcceptInput);
    }

    public virtual void OnInitialize()
    {
        cg.rootCG.alpha = 0;
        cg.SetInteractableState(active: false);
        acceptButton.gameObject.SetActive(false);
    }


    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;

    private CanvasGroupController cg;
    public string lastInput = string.Empty;
    public bool isWaitingOnUserInput;

    private bool HasValidText => !inputField.text.IsNullOrEmpty();

    private void OnAcceptInput()
    {
        if (inputField.text.IsNullOrEmpty()) return;
        lastInput = inputField.text;
        Hide();
    }

    private void OnInputChanged(string value)
    {
        acceptButton.gameObject.SetActive(HasValidText);
    }


    public void Show(string title)
    {
        titleText.text = title;
        inputField.text = string.Empty;
        cg.Show();
        cg.SetInteractableState(active: true);
        isWaitingOnUserInput = true;
    }

    public void Hide()
    {
        cg.Hide();
        cg.SetInteractableState(active: false);
        isWaitingOnUserInput = false;
    }
}