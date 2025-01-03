using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 对话的容器
/// </summary>
public class DialogueContainer : SingletonMono<DialogueContainer>
{
    public GameObject root;
    public TextMeshProUGUI DialohueText;

    private void Awake()
    {
        DialohueText = transform.Find("Image/DialohueText").GetComponent<TMPro.TextMeshProUGUI>();
        root = transform.FindParentByName("Root").gameObject;
    }

    public void SetDialogueColor(Color color) => DialohueText.color = color;
    public void SetDialogueFont(TMP_FontAsset font) => DialohueText.font = font;
}