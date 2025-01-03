using System;
using System.Text.RegularExpressions;

/// <summary>
/// 对话解析器
/// </summary>
public class DialogueParser
{
    public static DIALOGUE_LINE Parse(string rawLine)
    {
        $"解析行-'{rawLine}".Log();
        (string speaker, string dialogue, string commands) = RipContent(rawLine);
        $" speaker = {speaker}\n dialogue = {dialogue}\n commands = {commands}".Log();
        return new DIALOGUE_LINE(speaker, dialogue, commands);
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
        bool isEscaped = false;
        int dialogueEnd = -1; //是否对话结束

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

        //识别命令模式
        Regex commandRegex = new Regex(ConfigString.CommandRegexPattern);

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

        //If we are here then we either have dialogue or a multi word argument in a command. Figure out if this is dialogue.
        if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
        {
            //we know that we have valid dialogue
            speaker = rawLine.Substring(0, dialogueStart).Trim();
            dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");
            if (commandStart != -1)
                commands = rawLine.Substring(commandStart).Trim();
        }
        else if (commandStart != -1 && dialogueStart > commandStart)
            commands = rawLine;
        else
            dialogue = rawLine;

        //rawLine.Substring(dialogueStart + 1, (dialogueEnd - dialogueStart) - 1).Log();
        return (speaker, dialogue, commands);
    }
}