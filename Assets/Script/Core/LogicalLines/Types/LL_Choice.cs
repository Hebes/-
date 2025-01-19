using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static LogicalLineUtils.Encapsulation;

/*
    choice "Give flowers to who?"
    {
        -Raelin
            $Raelin.love += 10
        -Stella
            $Stella.love += 10
    }
 */

/// <summary>
/// 选择逻辑的选择
/// </summary>
public class LL_Choice : ILogicalLine
{
    public string Keyword => "choice";
    private const char CHOICE_IDENTIFIER = '-';

    public IEnumerator Execute(DIALOGUE_LINE line)
    {
        var currentConversation = R.DialogueSystem.ConversationManager.conversation;
        var progress = R.DialogueSystem.ConversationManager.conversationProgress;
        EncapsulatedData data = RipEncapsulationData(currentConversation, progress, ripHeaderAndEncapsualators: true, parentStartingIndex: currentConversation.fileStartIndex);
        List<Choice> choices = GetChoicesFromData(data);

        string title = line.DialogueData.RawData;
        UIChoicePanel panel = R.UISystem.UIChoicePanel;
        string[] choiceTitles = choices.Select(c => c.title).ToArray();

        panel.Show(title, choiceTitles);

        while (panel.isWaitingOnUserChoice)
            yield return null;

        Choice selectedChoice = choices[panel.lastDecision.answerIndex];

        Conversation newConversation = new Conversation(selectedChoice.resultLines, file: currentConversation.file, fileStartIndex: selectedChoice.startIndex, fileEndIndex: selectedChoice.endIndex);
        R.DialogueSystem.ConversationManager.conversation.SetProgress(data.endingIndex - currentConversation.fileStartIndex);
        R.DialogueSystem.ConversationManager.EnqueuePriority(newConversation);

        AutoReader autoReader = R.DialogueSystem.autoReader;
        // if (autoReader != null && autoReader.IsOn && autoReader.Skip)
        // {
        //     if (VN_Configuration.activeConfig != null && !VN_Configuration.activeConfig.continueSkippingAfterChoice)
        //         autoReader.Disable();
        // }
    }

    public bool Matches(DIALOGUE_LINE line)
    {
        return (line.HasSpeaker && line.SpeakerData.name.ToLower() == Keyword);
    }

    private List<Choice> GetChoicesFromData(LogicalLineUtils.Encapsulation.EncapsulatedData data)
    {
        List<Choice> choices = new List<Choice>();
        int encapsulationDepth = 0;
        bool isFirstChoice = true;

        Choice choice = new Choice
        {
            title = string.Empty,
            resultLines = new List<string>(),
        };

        int choiceIndex = 0, i = 0;
        for (i = 1; i < data.lines.Count; i++)
        {
            var line = data.lines[i];
            if (IsChoiceStart(line) && encapsulationDepth == 1)
            {
                if (!isFirstChoice)
                {
                    choice.startIndex = data.startingIndex + (choiceIndex + 1);
                    choice.endIndex = data.startingIndex + (i - 1);
                    choices.Add(choice);
                    choice = new Choice
                    {
                        title = string.Empty,
                        resultLines = new List<string>(),
                    };
                }

                choiceIndex = i;
                choice.title = line.Trim().Substring(1);
                isFirstChoice = false;
                continue;
            }

            AddLineToResults(line, ref choice, ref encapsulationDepth);
        }

        if (!choices.Contains(choice))
        {
            choice.startIndex = data.startingIndex + (choiceIndex + 1);
            choice.endIndex = data.startingIndex + (i - 2);
            choices.Add(choice);
        }

        return choices;
    }

    private void AddLineToResults(string line, ref Choice choice, ref int encapsulationDepth)
    {
        line.Trim();

        if (IsEncapsulationStart(line))
        {
            if (encapsulationDepth > 0)
                choice.resultLines.Add(line);
            encapsulationDepth++;
            return;
        }

        if (IsEncapsulationEnd(line))
        {
            encapsulationDepth--;

            if (encapsulationDepth > 0)
                choice.resultLines.Add(line);

            return;
        }

        choice.resultLines.Add(line);
    }

    private bool IsChoiceStart(string line) => line.Trim().StartsWith(CHOICE_IDENTIFIER);

    private struct Choice
    {
        public string title;
        public List<string> resultLines;
        public int startIndex;
        public int endIndex;
    }
}