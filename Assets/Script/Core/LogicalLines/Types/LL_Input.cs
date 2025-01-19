using System.Collections;

/// <summary>
/// 选择逻辑的输入
/// </summary>
public class LL_Input : ILogicalLine
{
    public string Keyword => "input";

    public IEnumerator Execute(DIALOGUE_LINE line)
    {
        string title = line.DialogueData.RawData;
        UIPlayerInput panel = R.UISystem.UIPlayerInput;
        panel.Show(title);
        while (panel.isWaitingOnUserInput)
            yield return null;
    }

    public bool Matches(DIALOGUE_LINE line)
    {
        return (line.HasSpeaker && line.SpeakerData.name.ToLower() == Keyword);
    }
}