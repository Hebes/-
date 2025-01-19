using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话管理器
/// </summary>
public class ConversationManager
{
    public ConversationManager()
    {
        R.DialogueSystem.onUserPromptNext += OnUsePrompt_Next;
        conversationQueue = new ConversationQueue();
    }

    private ConversationQueue conversationQueue; //谈话队列

    public bool isRunning => _process != null;
    private Coroutine _process = null;
    private bool _userPrompt = false; //使用提示
    public bool IsWaitingOnAutoTimer { get; private set; } = false;
    public bool IsOnLogicalLine { get; private set; } = false; //运行在了逻辑行中
    public Conversation conversation => (conversationQueue.IsEmpty() ? null : conversationQueue.top);
    public int conversationProgress => (conversationQueue.IsEmpty() ? -1 : conversationQueue.top.GetProgress());
    public bool allowUserPrompts = true;

    public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);
    public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

    private void OnUsePrompt_Next() => _userPrompt = true;

    public Coroutine StartConversation(Conversation conversation)
    {
        StopConversation();
        conversationQueue.Clear();
        Enqueue(conversation);
        return _process = R.DialogueSystem.StartCoroutine(RunningConversation());
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
    private IEnumerator RunningConversation()
    {
        while (!conversationQueue.IsEmpty())
        {
            Conversation currentConversation = conversation;
            if (currentConversation.HasReachedEnd()) //是否阅读结束
            {
                conversationQueue.Dequeue();
                continue;
            }

            string rawLine = currentConversation.CurrentLine();

            //不要显示任何空行或试图在它们上运行任何逻辑。
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                TryAdvanceConversation(currentConversation);
                continue;
            }

            DIALOGUE_LINE line = DialogueParser.Parse(rawLine);

            if (R.LogicalLineSystem.TryGetLogic(line, out Coroutine logic))
            {
                IsOnLogicalLine = true;
                yield return logic;
            }
            else
            {
                //显示对话
                if (line.HasDialogue)
                    yield return Line_RunDialogue(line);

                //执行指令
                if (line.HasCommands)
                    yield return Line_RunCommands(line);

                //等待用户输入,如果对话在这一行，等待用户输入
                if (line.HasDialogue)
                {
                    yield return WaitForUserInput();
                    R.CommandSystem.StopAllProcess();
                    R.DialogueSystem.OnSystemPrompt_Clear();
                }
            }

            TryAdvanceConversation(currentConversation);
            IsOnLogicalLine = false;
        }

        _process = null;
    }

    private IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
    {
        //显示说话者名称
        if (line.HasSpeaker)
        {
            HandleSpeakerLogic(line.SpeakerData);
        }

        //如果对话框不可见 确保它自动变为可见
        if (!R.UISystem.UIDialogue.IsVisible)
            R.UISystem.UIDialogue.Show();

        yield return BuildLineSegments(line.DialogueData); //建立说话片段
    }

    /// <summary>
    /// 处理说话者逻辑
    /// </summary>
    /// <param name="speakerData"></param>
    private void HandleSpeakerLogic(DL_SPEAKER_DATA speakerData)
    {
        bool characterMustBeCreated = speakerData.MakeCharacterEnter || speakerData.isCastingPosition || speakerData.IsCastingExpressions;
        Character character = R.CharacterSystem.CreateCharacter(speakerData.name, revealAfterCreation: characterMustBeCreated);
        if (speakerData.MakeCharacterEnter && !character.isVisible && character.co_revealing.IsNull())
            character.Show();

        string displayName = TagSystem.Inject(speakerData.DisplayName);
        R.DialogueSystem.ShowSpeakerName(displayName); //向UI添加角色名称
        R.DialogueSystem.ApplySpeakerDataToDialogueContainer(speakerData.name);

        //显示位置
        if (speakerData.isCastingPosition)
            character.MoveToPosition(speakerData.castPosition);

        //设置优先级
        if (speakerData.IsCastingExpressions)
        {
            foreach ((int layer, string expression) data in speakerData.CastExpressions)
                character.OnReceiveCastingExpression(data.layer, data.expression);
        }
    }

    private IEnumerator Line_RunSpeaker(DIALOGUE_LINE line)
    {
        yield return null;
    }

    private IEnumerator Line_RunCommands(DIALOGUE_LINE line)
    {
        List<DL_COMMAND_DATA.Command> commands = line.CommandsData.commands;
        foreach (DL_COMMAND_DATA.Command command in commands)
        {
            if (command.WaitForCompletion || "wait".Equals(command.Name))
            {
                CoroutineWrapper cw = R.CommandSystem.Extend(command.Name, command.Arguments);
                while (!cw.IsDone)
                {
                    if (_userPrompt)
                    {
                        R.CommandSystem.StopCurrentProcess();
                        _userPrompt = false;
                    }

                    yield return null;
                }
            }
            else
                R.CommandSystem.Extend(command.Name, command.Arguments);
        }

        yield return null;
    }

    /// <summary>
    /// 建立说话片段
    /// </summary>
    /// <param name="lineDialogue"></param>
    /// <returns></returns>
    private IEnumerator BuildLineSegments(DL_DIALOGUE_DATA lineDialogue)
    {
        for (int i = 0; i < lineDialogue.Segments.Count; i++)
        {
            DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = lineDialogue.Segments[i];
            //等待对话触发
            switch (segment.startSignal)
            {
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.NONE:
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
                    yield return WaitForUserInput();
                    R.DialogueSystem.OnSystemPrompt_Clear();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                    IsWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    IsWaitingOnAutoTimer = false;
                    R.DialogueSystem.OnSystemPrompt_Clear();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                    IsWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    IsWaitingOnAutoTimer = false;
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
        R.DialogueSystem.Prompt.Show();
        while (!_userPrompt)
            yield return null;
        R.DialogueSystem.Prompt.Hide();
        _userPrompt = false;
    }

    /// <summary>
    /// 构建对话
    /// </summary>
    /// <param name="dialogue"></param>
    /// <returns></returns>
    private IEnumerator BuildDialogue(string dialogue, bool append = false)
    {
        dialogue = TagSystem.Inject(dialogue);
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
            if (_userPrompt)
            {
                if (!textArchitect.hurryUp) //没有加快的话
                    textArchitect.hurryUp = true;
                else
                    textArchitect.ForceComplete();
                _userPrompt = false;
            }

            yield return null;
        }
    }

    private void TryAdvanceConversation(Conversation conversation)
    {
        conversation.IncrementProgress();

        if (conversation != conversationQueue.top)
            return;

        if (conversation.HasReachedEnd())
            conversationQueue.Dequeue();
    }
}