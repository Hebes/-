using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话管理器
/// </summary>
public class ConversationManager
{
    public bool isRunning => _process != null;
    private Coroutine _process = null;

    /// <summary>
    /// 使用提示
    /// </summary>
    private bool _usePrompt = false;

    public ConversationManager()
    {
        GameEvent.UsePrompt_Next.Register(OnUsePrompt_Next);
    }

    private void OnUsePrompt_Next(object udata)
    {
        _usePrompt = true;
    }

    public void StartConversation(List<string> conversation)
    {
        StopConversation();
        _process = R.DialogueSystem.StartCoroutine(RunningConversation(conversation));
    }

    private void StopConversation()
    {
        if (_process == null) return;
        DialogueSystem.I.StopCoroutine(_process);
        _process = null;
    }

    /// <summary>
    /// 运行对话
    /// </summary>
    /// <param name="conversation"></param>
    /// <returns></returns>
    IEnumerator RunningConversation(List<string> conversation)
    {
        for (int i = 0; i < conversation.Count; i++)
        {
            string line = conversation[i];
            //不要显示任何空行或尝试在其上运行任何逻辑。
            if (line.IsNullOrEmpty()) continue;

            DIALOGUE_LINE dialogueLine = DialogueParser.Parse(line);

            //显示对话
            if (dialogueLine.HasDialogue)
                yield return Line_RunDialogue(dialogueLine);

            //执行指令
            if (dialogueLine.HasCommands)
                yield return Line_RunCommands(dialogueLine);

            //等待用户输入
            if (dialogueLine.HasDialogue)
                yield return WaitForUserInput();
        }
    }

    IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
    {
        //显示说话者名称
        if (line.HasSpeaker)
            DialogueSystem.I.ShowSpeakerName(line.SpeakerData.displayName);

        yield return BuildLineSegments(line.DialogueData); //建立说话片段
    }


    IEnumerator Line_RunSpeaker(DIALOGUE_LINE line)
    {
        yield return null;
    }

    IEnumerator Line_RunCommands(DIALOGUE_LINE line)
    {
        List<DL_COMMAND_DATA.Command> commands = line.CommandsData.commands;
        foreach (DL_COMMAND_DATA.Command command in commands)
        {
            if (command.WaitForCompletion)
                yield return CommandSystem.I.Extend(command.Name, command.Arguments);
            else
                CommandSystem.I.Extend(command.Name, command.Arguments);
        }

        yield return null;
    }

    /// <summary>
    /// 建立说话片段
    /// </summary>
    /// <param name="lineDialogue"></param>
    /// <returns></returns>
    IEnumerator BuildLineSegments(DL_DIALOGUE_DATA lineDialogue)
    {
        for (int i = 0; i < lineDialogue.segments.Count; i++)
        {
            DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = lineDialogue.segments[i];
            //等待对话触发
            switch (segment.startSignal)
            {
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.NONE:
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                    yield return new WaitForSeconds(segment.signalDelay);
                    break;
                default:
                    throw new Exception("文件中的指定标志出现错误，现在暂时中能用{c}{a}{wc}{wa}等");
            }

            yield return BuildDialogue(segment.dialogue, segment.AppendText);
        }
    }

    /// <summary>
    /// 等待用户输入
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForUserInput()
    {
        while (!_usePrompt)
            yield return null;
        _usePrompt = false;
    }

    /// <summary>
    /// 构建对话
    /// </summary>
    /// <param name="dialogue"></param>
    /// <returns></returns>
    private IEnumerator BuildDialogue(string dialogue, bool append = false)
    {
        TextArchitect textArchitect = R.DialogueSystem.TextArchitect;
        //构建对话
        if (append)
        {
            textArchitect.Append(dialogue);
        }
        else
        {
            textArchitect.Build(dialogue);
        }

        //等待对话完成
        while (textArchitect.isBuilding)
        {
            if (_usePrompt)
            {
                if (!textArchitect.hurryUp) //没有加快的话
                    textArchitect.hurryUp = true;
                else
                    textArchitect.ForceComplete();
                _usePrompt = false;
            }

            yield return null;
        }
    }
}