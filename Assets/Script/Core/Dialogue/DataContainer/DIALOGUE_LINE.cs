public class DIALOGUE_LINE
{
    public DIALOGUE_LINE(string speaker, string dialogue, string commands)
    {
        SpeakerData = speaker.IsNullOrEmpty() ? null : new DL_SPEAKER_DATA(speaker);
        DialogueData = dialogue.IsNullOrEmpty() ? null : new DL_DIALOGUE_DATA(dialogue);
        CommandsData = commands.IsNullOrEmpty() ? null : new DL_COMMAND_DATA(commands);
    }

    public DL_SPEAKER_DATA SpeakerData; //说话者
    public DL_DIALOGUE_DATA DialogueData; //对话内容
    public DL_COMMAND_DATA CommandsData; //命令

    public bool HasDialogue => DialogueData != null;
    public bool HasCommands => CommandsData != null;
    public bool HasSpeaker => SpeakerData != null;
}