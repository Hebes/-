using System;
using System.Text.RegularExpressions;

/// <summary>
/// 对话解析器
/// </summary>
public class DialogueParser
{
    public const string CommandRegexPattern = @"[\w\[\]]*[^\s]\(";

    public static DIALOGUE_LINE Parse(string rawLine)
    {
        //$"解析行-'{rawLine}".Log();
        (string speaker, string dialogue, string commands) = RipContent(rawLine);
        commands = TagSystem.Inject(commands);
        $" 说话者 = {speaker}\n 对话内容 = {dialogue}\n 命令 = {commands}".Log();
        return new DIALOGUE_LINE(rawLine, speaker, dialogue, commands);
    }

    /// <summary>
    /// 解析内容
    /// </summary>
    /// <param name="rawLine"></param>
    /// <returns></returns>
    private static (string speaker, string dialogue, string commands) RipContent(string rawLine)
    {
        string speaker = String.Empty; //发言者
        string dialogue = String.Empty; //发言内容
        string commands = String.Empty; //命令
        int dialogueStart = -1; //是否对话开始
        int dialogueEnd = -1; //是否对话结束
        bool isEscaped = false;

        for (int i = 0; i < rawLine.Length; i++)
        {
            char current = rawLine[i];
            if (current == '\\')
                isEscaped = !isEscaped;
            else if (current == '"' & !isEscaped)
            {
                if (dialogueStart == -1)
                    dialogueStart = i;
                else if (dialogueEnd == -1)
                    dialogueEnd = i;
            }
            else
            {
                isEscaped = false;
            }
        }

        //识别命令模式  4.5 
        Regex commandRegex = new Regex(CommandRegexPattern);

        MatchCollection matches = commandRegex.Matches(rawLine);
        int commandStart = -1;
        foreach (Match match in matches)
        {
            if (match.Index < dialogueStart || match.Index > dialogueEnd)
            {
                commandStart = match.Index;
                break;
            }
        }

        if (commandStart != -1 && (dialogueStart == -1 && dialogueEnd == -1))
            return ("", "", rawLine.Trim());

        //如果我们在这里，那么我们在命令中要么有对话，要么有多词的参数。弄清楚这是不是对话。
        if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
        {
            //我们知道我们之间的对话是有效的
            speaker = rawLine.Substring(0, dialogueStart).Trim();
            dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");
            if (commandStart != -1)
                commands = rawLine.Substring(commandStart).Trim();
        }
        else if (commandStart != -1 && dialogueStart > commandStart)
            commands = rawLine;
        else
            dialogue = rawLine;

        // if (dialogueStart == -1 || dialogueEnd == -1)
        //     $"{rawLine}的标点符号有问题(单独一个说话请忽略这个问题)".LogError();
        return (speaker, dialogue, commands);
    }
}