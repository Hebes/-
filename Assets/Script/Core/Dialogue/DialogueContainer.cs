using System;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueContainer
{
    public UIDialogue uiDialogue;

    public void Initialize()
    {
        uiDialogue = R.UITotalRoot.FindComponent<UIDialogue>();
        uiDialogue.OnInitialize();
    }
}