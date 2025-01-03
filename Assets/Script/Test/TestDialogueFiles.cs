using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 测试解析
/// </summary>
public class TestDialogueFiles : BaseBehaviour
{
    public TextAsset DialogueTextAsset;

    // private void Start()
    // {
    //     //发言者 对话进入这里
    //     string line = "Speaker \"Dialogue \\\"Goes In\\\" Here!\" Command(arguments here)";
    //     DialogueParser.Parse(line);
    // }
    void Start()
    {
        StartConversation();
    }

    void StartConversation()
    {
        //List<string> lines = FileManager.ReadTextAsset("GameData/testFile");
        List<string> lines = FileManager.ReadTextAsset(DialogueTextAsset);
        DialogueSystem.I.Say(lines);
        // foreach (var sLine in lines)
        // {
        //     if (sLine.IsNullOrEmpty())continue;
        //     $"说话内容：{sLine}".Log();
        //     DIALOGUE_LINE dLine = DialogueParser.Parse(sLine);
        //     int i = 0;
        //     foreach (var v in dLine.dialogue.segments)
        //     {
        //         $"片段 [{i++}] = '{v.dialogue}' [延迟={(v.signalDelay > 0 ? v.signalDelay : 0)}]".Log();
        //     }
        // }
        
        
        // foreach (string line in lines)
        // {
        //     if (line == string.Empty) continue;
        //     DIALOGUE_LINE dl = DialogueParser.Parse(line);
        // }
    }
}