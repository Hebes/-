using System.Collections.Generic;
using UnityEngine;
using TMPro;


[System.Serializable]
public class DialogueData
{
    public string currentDialogue = "";
    public string currentSpeaker = "";

    public string dialogueFont;
    public Color dialogueColor;
    public float dialogueScale;

    public string speakerFont;
    public Color speakerNameColor;
    public float speakerScale;

    public static DialogueData Capture()
    {
        DialogueData data = new DialogueData();

        var ds = R. DialogueSystem;
        var dialogueText = ds.dialogueContainer.dialogueText;
        var nameText = ds.dialogueContainer.nameContainer.NameText;

        data.currentDialogue = dialogueText.text;
        data.dialogueFont = FilePaths.resources_font + dialogueText.font.name;
        data.dialogueColor = dialogueText.color;
        data.dialogueScale = dialogueText.fontSize;

        data.currentSpeaker = nameText.text;
        data.speakerFont = FilePaths.resources_font + nameText.font.name;
        data.speakerNameColor = nameText.color;
        data.speakerScale = nameText.fontSize;

        return data;
    }

    public static void Apply(DialogueData data)
    {
        var ds = R .DialogueSystem;
        var dialogueText = ds.dialogueContainer.dialogueText;
        var nameText = ds.dialogueContainer.nameContainer.NameText;
        R.DialogueSystem.TextArchitect.SetText(data.currentDialogue);
        dialogueText.color = data.dialogueColor;
        dialogueText.fontSize = data.dialogueScale;

        nameText.text = data.currentSpeaker;
        if (nameText.text != string.Empty)
            ds.dialogueContainer.nameContainer.Show();
        else
            ds.dialogueContainer.nameContainer.Hide();

        nameText.color = data.speakerNameColor;
        nameText.fontSize = data.speakerScale;

        if (data.dialogueFont != dialogueText.font.name)
        {
            TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.dialogueFont);
            if (fontAsset != null)
                dialogueText.font = fontAsset;
        }

        if (data.speakerFont != nameText.font.name)
        {
            TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.speakerFont);
            if (fontAsset != null)
                nameText.font = fontAsset;
        }
    }
}