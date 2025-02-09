using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI确认菜单
/// </summary>
[NoDontDestroyOnLoad]
public class UIConfirmationMenu : SM<UIConfirmationMenu>
{
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private LayoutGroup choiceLayoutGroup;
    [SerializeField] private GameObject buttonPrefab;

    private GameObject[] activeOptions = new GameObject[0];

    private void Awake()
    {
        I = this;
        
        anim = GetComponent<UnityEngine.Animator>();
        title = transform.Find("Root/Title").GetComponent<TMPro.TextMeshProUGUI>();
        choiceLayoutGroup = transform.Find("Root/Choices").GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        buttonPrefab = transform.FindComponent("Choice").gameObject;
    }

    public void Show(string titleValue, params ConfirmationButton[] options)
    {
        if (options.Length == 0)
        {
            "确认菜单必须至少提供一个选项供用户选择.".LogError();
            return;
        }

        this.title.text = titleValue;

        CreateOptionButtons(options);

        anim.Play("Enter");
    }
    private void Hide()
    {
        anim.Play("Exit");
    }
    private void CreateOptionButtons(ConfirmationButton[] options)
    {
        foreach (GameObject g in activeOptions)
            DestroyImmediate(g);

        activeOptions = new GameObject[options.Length];

        for (int i = 0; i < options.Length; i++)
        {
            ConfirmationButton option = options[i];
            GameObject ob = Instantiate(buttonPrefab, choiceLayoutGroup.transform);
            ob.SetActive(true);

            Button button = ob.GetComponent<Button>();

            if (option.action != null)
                button.onClick.AddListener(() => option.action.Invoke());

            if (option.autoCloseOnQuit)
                button.onClick.AddListener(Hide);

            TextMeshProUGUI txt = ob.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = option.title;

            activeOptions[i] = ob;
        }
    }

    public struct ConfirmationButton
    {
        public string title;
        public Action action;
        public bool autoCloseOnQuit;

        public ConfirmationButton(string title, Action action, bool autoCloseOnClick = true)
        {
            this.title = title;
            this.action = action;
            this.autoCloseOnQuit = autoCloseOnClick;
        }
    }
}