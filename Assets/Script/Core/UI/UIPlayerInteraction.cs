using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制器
/// </summary>
[NoDontDestroyOnLoad]
public class UIPlayerInteraction : SM<UIPlayerInteraction>,IUIBehaviour
{
    [SerializeField] private Button Auto;
    [SerializeField] private Button Skip;
    [SerializeField] private Button Save;
    [SerializeField] private Button Load;
    [SerializeField] private Button config;
    [SerializeField] private Button button;

    public void OnGetComponent()
    {
        Auto = transform.Find("Panel/Auto").GetComponent<UnityEngine.UI.Button>();
        Skip = transform.Find("Panel/Skip").GetComponent<UnityEngine.UI.Button>();
        Save = transform.Find("Panel/Save").GetComponent<UnityEngine.UI.Button>();
        Load = transform.Find("Panel/Load").GetComponent<UnityEngine.UI.Button>();
        config = transform.Find("Panel/Config").GetComponent<UnityEngine.UI.Button>();
        button = transform.Find("Button").GetComponent<UnityEngine.UI.Button>();
    }

    public void OnListener()
    {
        button.onClick.AddListener(R.DialogueSystem.OnUserPrompt_Next);
        Auto.onClick.AddListener(R.DialogueSystem.autoReader.Toggle_Auto);
        Skip.onClick.AddListener(R.DialogueSystem.autoReader.Toggle_Skip);
    }
}