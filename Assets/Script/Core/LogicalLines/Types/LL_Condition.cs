using System.Collections;
using static LogicalLineUtils.Encapsulation;
using static LogicalLineUtils.Conditions;

/// <summary>
/// 选择逻辑的状态
/// </summary>
public class LL_Condition : ILogicalLine
{
    public string Keyword => "if";
    private const string ELSE = "else";
    private readonly string[] CONTAINERS = new string[] { "(", ")" };

    public IEnumerator Execute(DIALOGUE_LINE line)
    {
        string rawCondition = ExtractCondition(line.RawData.Trim());
        bool conditionResult = EvaluateCondition(rawCondition);

        Conversation currentConversation = R.DialogueSystem.ConversationManager.conversation;
        int currentProgress = R.DialogueSystem.ConversationManager.conversationProgress;

        EncapsulatedData ifData = RipEncapsulationData(currentConversation, currentProgress, false, parentStartingIndex: currentConversation.fileStartIndex);
        EncapsulatedData elseData = new EncapsulatedData();

        if (ifData.endingIndex + 1 < currentConversation.Count)
        {
            string nextLine = currentConversation.GetLines()[ifData.endingIndex + 1].Trim();
            if (nextLine == ELSE)
            {
                elseData = RipEncapsulationData(currentConversation, ifData.endingIndex + 1, false, parentStartingIndex: currentConversation.fileStartIndex);
            }
        }

        currentConversation.SetProgress(elseData.isNull ? ifData.endingIndex : elseData.endingIndex);

        EncapsulatedData selData = conditionResult ? ifData : elseData;

        if (!selData.isNull && selData.lines.Count > 0)
        {
            //从会话索引中删除标头和封装行
            selData.startingIndex += 2; //拆卸封头和启动封装器
            selData.endingIndex -= 1; //拆卸末端封装器

            Conversation newConversation = new Conversation(selData.lines, file: currentConversation.file, fileStartIndex: selData.startingIndex, fileEndIndex: selData.endingIndex);
            R.DialogueSystem.ConversationManager.EnqueuePriority(newConversation);
        }

        yield return null;
    }

    public bool Matches(DIALOGUE_LINE line)
    {
        return line.RawData.Trim().StartsWith(Keyword);
    }

    private string ExtractCondition(string line)
    {
        int startIndex = line.IndexOf(CONTAINERS[0]) + 1;
        int endIndex = line.IndexOf(CONTAINERS[1]);

        return line.Substring(startIndex, endIndex - startIndex).Trim();
    }
}