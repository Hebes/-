using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_DatabaseExtension_General : CMD_DatabaseExtension
{
    private static readonly string[] PARAM_SPEED = new string[] { "-s", "-spd" };
    private static readonly string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
    private static readonly string[] PARAM_FILEPATH = new string[] { "-f", "-file", "-filepath" };
    private static readonly string[] PARAM_ENQUEUE = new string[] { "-e", "-enqueue" };

    public new static void Extend(CommandDataBase database)
    {
        database.AddCommand("Wait", new Func<string, IEnumerator>(Wait));

        //Dialogue System Controls
        database.AddCommand("ShowUI", new Func<string[], IEnumerator>(ShowDialogueSystem));
        database.AddCommand("HideUI", new Func<string[], IEnumerator>(HideDialogueSystem));

        //对话控制
        database.AddCommand("ShowDB", new Func<string[], IEnumerator>(ShowDialogueBox));
        database.AddCommand("HideDB", new Func<string[], IEnumerator>(HideDialogueBox));

        database.AddCommand("load", new Action<string[]>(LoadNewDialogueFile));
    }

    private static IEnumerator HideDialogueSystem(string[] data)
    {
        var parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 1f);
        parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);

        yield return R.DialogueSystem.Hide(speed, immediate);
    }

    private static IEnumerator ShowDialogueSystem(string[] data)
    {
        var parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 1f);
        parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);

        yield return R.DialogueSystem.Show(speed, immediate);
    }

    private static IEnumerator HideDialogueBox(string[] data)
    {
        var parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 1f);
        parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);

        yield return R.UISystem.UIDialogue.Hide(speed, immediate);
    }

    private static IEnumerator ShowDialogueBox(string[] data)
    {
        var parameters = ConvertDataToParameters(data);
        parameters.TryGetValue(PARAM_SPEED, out float speed, defaultValue: 1f);
        parameters.TryGetValue(PARAM_IMMEDIATE, out bool immediate, defaultValue: false);
        yield return R.UISystem.UIDialogue.Show(speed, immediate);
    }

    private static IEnumerator Wait(string data)
    {
        if (float.TryParse(data, out float time))
            yield return new WaitForSeconds(time);
    }

    private static void LoadNewDialogueFile(string[] data)
    {
        string fileName = string.Empty;
        bool enqueue = false;

        var parameters = ConvertDataToParameters(data);

        parameters.TryGetValue(PARAM_FILEPATH, out fileName);
        parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultValue: false);

        string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, fileName);
        TextAsset file = Resources.Load<TextAsset>(filePath);

        if (file == null)
        {
            Debug.LogWarning($"文件 '{filePath}' 无法从对话文件中加载. 请确保它存在于 '{FilePaths.resources_dialogueFiles}' 资源文件夹.");
            return;
        }

        List<string> lines = FileSystem.ReadTextAsset(file, includeBlankLines: true);
        Conversation newConversation = new Conversation(lines);

        if (enqueue)
            R.DialogueSystem.ConversationManager.Enqueue(newConversation);
        else
            R.DialogueSystem.ConversationManager.StartConversation(newConversation);
    }
}