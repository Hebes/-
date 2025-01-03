using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 对话的容器
/// </summary>
public class DialogueContainer : BaseBehaviour
{
    public GameObject root;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DialohueText;

    public static DialogueContainer I;
    private void Awake()
    {
        I = this;
        NameText = transform.Find("Image/NameText").GetComponent<TMPro.TextMeshProUGUI>();
        DialohueText = transform.Find("Image/DialohueText").GetComponent<TMPro.TextMeshProUGUI>();
        root = transform.FindParentByName("Root").gameObject;
    }
}